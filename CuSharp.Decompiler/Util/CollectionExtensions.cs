#nullable enable
using CuSharp.Decompiler.Util;

namespace CuSharp.Decompiler.Util
{
    static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> input)
        {
            foreach (T item in input) collection.Add(item);
        }

        /// <summary>
        /// The merge step of merge sort.
        /// </summary>
        public static IEnumerable<T> Merge<T>(this IEnumerable<T> input1, IEnumerable<T> input2, Comparison<T> comparison)
        {
            using var enumA = input1.GetEnumerator();
            using var enumB = input2.GetEnumerator();
            bool moreA = enumA.MoveNext();
            bool moreB = enumB.MoveNext();
            while (moreA && moreB)
            {
                if (comparison(enumA.Current, enumB.Current) <= 0)
                {
                    yield return enumA.Current;
                    moreA = enumA.MoveNext();
                }
                else
                {
                    yield return enumB.Current;
                    moreB = enumB.MoveNext();
                }
            }
            while (moreA)
            {
                yield return enumA.Current;
                moreA = enumA.MoveNext();
            }
            while (moreB)
            {
                yield return enumB.Current;
                moreB = enumB.MoveNext();
            }
        }
    }
}
