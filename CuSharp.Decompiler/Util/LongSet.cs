#nullable enable
// Copyright (c) 2016 Daniel Grunwald
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using CuSharp.Decompiler.Util;
using System.Collections.Immutable;
using System.Diagnostics;

namespace CuSharp.Decompiler.Util
{
    /// <summary>
    /// An immutable set of longs, that is implemented as a list of intervals.
    /// </summary>
    public struct LongSet : IEquatable<LongSet>
    {
        /// <summary>
        /// The intervals in this set of longs.
        /// </summary>
        /// <remarks>
        /// Invariant: the intervals in this array are non-empty, non-overlapping, non-touching, and sorted.
        /// 
        /// This invariant ensures every LongSet is always in a normalized representation.
        /// </remarks>
        public readonly ImmutableArray<LongInterval> Intervals;

        private LongSet(ImmutableArray<LongInterval> intervals)
        {
            Intervals = intervals;
#if DEBUG
            // Check invariant
            long minValue = long.MinValue;
            for (int i = 0; i < intervals.Length; i++)
            {
                Debug.Assert(!intervals[i].IsEmpty);
                Debug.Assert(minValue <= intervals[i].Start);
                if (intervals[i].InclusiveEnd == long.MaxValue - 1 || intervals[i].InclusiveEnd == long.MaxValue)
                {
                    // An inclusive end of long.MaxValue-1 or long.MaxValue means (after the gap of 1 element),
                    // there isn't any room for more non-empty intervals.
                    Debug.Assert(i == intervals.Length - 1);
                }
                else
                {
                    minValue = checked(intervals[i].End + 1); // enforce least 1 gap between intervals
                }
            }
#endif
        }

        /// <summary>
        /// Create a new LongSet that contains a single value.
        /// </summary>
        public LongSet(long value)
            : this(ImmutableArray.Create(LongInterval.Inclusive(value, value)))
        {
        }

        /// <summary>
        /// Create a new LongSet that contains the values from the interval.
        /// </summary>
        public LongSet(LongInterval interval)
            : this(interval.IsEmpty ? Empty.Intervals : ImmutableArray.Create(interval))
        {
        }

        /// <summary>
        /// Creates a new LongSet the contains the values from the specified intervals.
        /// </summary>
        public LongSet(IEnumerable<LongInterval> intervals)
            : this(MergeOverlapping(intervals.Where(i => !i.IsEmpty).OrderBy(i => i.Start)).ToImmutableArray())
        {
        }

        /// <summary>
        /// The empty LongSet.
        /// </summary>
        public static readonly LongSet Empty = new(ImmutableArray.Create<LongInterval>());

        /// <summary>
        /// The LongSet that contains all possible long values.
        /// </summary>
        public static readonly LongSet Universe = new(LongInterval.Inclusive(long.MinValue, long.MaxValue));

        public bool IsEmpty => Intervals.IsEmpty;

        /// <summary>
		/// Gets the number of values in this LongSet.
		/// Note: for <c>LongSet.Universe</c>, the number of values does not fit into <c>ulong</c>.
		/// Instead, this property returns the off-by-one value <c>ulong.MaxValue</c> to avoid overflow.
		/// </summary>
		public ulong Count()
        {
            unchecked
            {
                ulong count = 0;
                foreach (var interval in Intervals)
                {
                    count += (ulong)(interval.End - interval.Start);
                }

                return count == 0 && !Intervals.IsEmpty
                    ? ulong.MaxValue
                    : count;
            }
        }


        IEnumerable<LongInterval> DoIntersectWith(LongSet other)
        {
            var enumA = Intervals.GetEnumerator();
            var enumB = other.Intervals.GetEnumerator();
            bool moreA = enumA.MoveNext();
            bool moreB = enumB.MoveNext();
            while (moreA && moreB)
            {
                LongInterval a = enumA.Current;
                LongInterval b = enumB.Current;
                LongInterval intersection = a.Intersect(b);
                if (!intersection.IsEmpty)
                {
                    yield return intersection;
                }
                if (a.InclusiveEnd < b.InclusiveEnd)
                {
                    moreA = enumA.MoveNext();
                }
                else
                {
                    moreB = enumB.MoveNext();
                }
            }
        }

        public bool Overlaps(LongSet other)
        {
            return DoIntersectWith(other).Any();
        }

        /// <summary>
		/// Given an enumerable of non-empty intervals sorted by the starting position,
		/// merges overlapping or touching intervals to create a valid interval array for LongSet.
		/// </summary>
		static IEnumerable<LongInterval> MergeOverlapping(IEnumerable<LongInterval> input)
        {
            long start = long.MinValue;
            long end = long.MinValue;
            bool empty = true;
            foreach (var element in input)
            {
                Debug.Assert(start <= element.Start);
                Debug.Assert(!element.IsEmpty);

                if (!empty && element.Start <= end)
                {
                    // element overlaps or touches [start, end), so combine the intervals:
                    // special case: element goes all the way up to long.MaxValue inclusive
                    end = element.End == long.MinValue ? long.MinValue : Math.Max(end, element.End);
                }
                else
                {
                    // flush existing interval:
                    if (!empty)
                    {
                        yield return new LongInterval(start, end);
                    }
                    else
                    {
                        empty = false;
                    }
                    start = element.Start;
                    end = element.End;
                }
                if (end == long.MinValue)
                {
                    // special case: element goes all the way up to long.MaxValue inclusive
                    // all further intervals in the input must be contained in [start, end),
                    // so ignore them (and avoid trouble due to the overflow in `end`).
                    break;
                }
            }
            if (!empty)
            {
                yield return new LongInterval(start, end);
            }
        }

        public LongSet UnionWith(LongSet other)
        {
            var mergedIntervals = Intervals.Merge(other.Intervals, (a, b) => a.Start.CompareTo(b.Start));
            return new LongSet(MergeOverlapping(mergedIntervals).ToImmutableArray());
        }

        /// <summary>
		/// Creates a new LongSet that contains all elements not contained in this LongSet.
		/// </summary>
		public LongSet Invert()
        {
            // The loop below assumes a non-empty LongSet, so handle the empty case specially.
            if (IsEmpty)
            {
                return Universe;
            }
            List<LongInterval> newIntervals = new List<LongInterval>(Intervals.Length + 1);
            long prevEnd = long.MinValue; // previous exclusive end
            foreach (var interval in Intervals)
            {
                if (interval.Start > prevEnd)
                {
                    newIntervals.Add(new LongInterval(prevEnd, interval.Start));
                }
                prevEnd = interval.End;
            }
            // create a final interval up to long.MaxValue inclusive
            if (prevEnd != long.MinValue)
            {
                newIntervals.Add(new LongInterval(prevEnd, long.MinValue));
            }
            return new LongSet(newIntervals.ToImmutableArray());
        }

        public override string ToString()
        {
            return string.Join(",", Intervals);
        }

        #region Equals and GetHashCode implementation
        public override bool Equals(object? obj)
        {
            return obj is LongSet set && SetEquals(set);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        [Obsolete("Explicitly call SetEquals() instead.")]
        public bool Equals(LongSet other)
        {
            return SetEquals(other);
        }

        public bool SetEquals(LongSet other)
        {
            if (Intervals.Length != other.Intervals.Length)
                return false;
            for (int i = 0; i < Intervals.Length; i++)
            {
                if (Intervals[i] != other.Intervals[i])
                    return false;
            }
            return true;
        }
        #endregion
    }
}
