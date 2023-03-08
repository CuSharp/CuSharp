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
	public static class KeyComparer
	{
		public static KeyComparer<TElement, TKey> Create<TElement, TKey>(Func<TElement, TKey> keySelector)
		{
			return new KeyComparer<TElement, TKey>(keySelector, Comparer<TKey>.Default, EqualityComparer<TKey>.Default);
		}

		public static void SortBy<TElement, TKey>(this List<TElement> list, Func<TElement, TKey> keySelector)
		{
			list.Sort(Create(keySelector));
		}
	}

	public class KeyComparer<TElement, TKey> : IComparer<TElement>, IEqualityComparer<TElement>
	{
		private readonly Func<TElement, TKey> _keySelector;
        private readonly IComparer<TKey> _keyComparer;
        private readonly IEqualityComparer<TKey> _keyEqualityComparer;

		public KeyComparer(Func<TElement, TKey> keySelector, IComparer<TKey> keyComparer, IEqualityComparer<TKey> keyEqualityComparer)
		{
            _keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
			_keyComparer = keyComparer ?? throw new ArgumentNullException(nameof(keyComparer));
			_keyEqualityComparer = keyEqualityComparer ?? throw new ArgumentNullException(nameof(keyEqualityComparer));
		}

		public int Compare(TElement? x, TElement? y)
		{
			return _keyComparer.Compare(_keySelector(x!), _keySelector(y!));
		}

		public bool Equals(TElement? x, TElement? y)
		{
			return _keyEqualityComparer.Equals(_keySelector(x!), _keySelector(y!));
		}

		public int GetHashCode(TElement obj)
		{
			return _keyEqualityComparer.GetHashCode(_keySelector(obj));
		}
	}
}
