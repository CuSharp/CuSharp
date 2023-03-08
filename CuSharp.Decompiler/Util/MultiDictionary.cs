#nullable enable
// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
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

namespace Dotnet4Gpu.Decompilation.Util
{
	/// <summary>
	/// A dictionary that allows multiple pairs with the same key.
	/// </summary>
	public class MultiDictionary<TKey, TValue> : ILookup<TKey, TValue> where TKey : notnull
	{
		private readonly Dictionary<TKey, List<TValue>> _dict;

		public MultiDictionary()
		{
			_dict = new Dictionary<TKey, List<TValue>>();
		}

		public void Add(TKey key, TValue value)
		{
			if (!_dict.TryGetValue(key, out List<TValue>? valueList))
			{
				valueList = new List<TValue>();
				_dict.Add(key, valueList);
			}
			valueList.Add(value);
		}

		public IReadOnlyList<TValue> this[TKey key] => _dict.TryGetValue(key, out var list) ? list : EmptyList<TValue>.Instance;

        /// <summary>
		/// Returns the number of different keys.
		/// </summary>
		public int Count => _dict.Count;

        IEnumerable<TValue> ILookup<TKey, TValue>.this[TKey key] => this[key];

        public bool Contains(TKey key)
		{
			return _dict.ContainsKey(key);
		}

		public IEnumerator<IGrouping<TKey, TValue>> GetEnumerator()
		{
			foreach (var pair in _dict) yield return new Grouping(pair.Key, pair.Value);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		sealed class Grouping : IGrouping<TKey, TValue>
		{
            private readonly List<TValue> _values;

            public TKey Key { get; }
			
            public Grouping(TKey key, List<TValue> values)
			{
				Key = key;
				_values = values;
			}

            public IEnumerator<TValue> GetEnumerator()
			{
				return _values.GetEnumerator();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return _values.GetEnumerator();
			}
		}
	}
}
