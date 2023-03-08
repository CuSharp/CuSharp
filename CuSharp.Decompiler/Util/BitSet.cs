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
#nullable enable

using System.Diagnostics;
using System.Text;

namespace Dotnet4Gpu.Decompilation.Util
{
	/// <summary>
	/// Improved version of BitArray
	/// </summary>
	public class BitSet
	{
		const int BitsPerWord = 64;
		const int Log2BitsPerWord = 6;

        private readonly ulong[] _words;

		static int WordIndex(int bitIndex)
		{
			Debug.Assert(bitIndex >= 0);
			return bitIndex >> Log2BitsPerWord;
		}

		/// <summary>
		/// Creates a new bitset, where initially all bits are zero.
		/// </summary>
		public BitSet(int capacity)
		{
			_words = new ulong[Math.Max(1, WordIndex(capacity + BitsPerWord - 1))];
		}

        public bool this[int index] {
			get => (_words[WordIndex(index)] & (1UL << index)) != 0;
            set {
				if (value)
					Set(index);
				else
					Clear(index);
			}
		}

        public void Set(int index)
		{
			_words[WordIndex(index)] |= (1UL << index);
		}

		public void Clear(int index)
		{
			_words[WordIndex(index)] &= ~(1UL << index);
		}

        public override string ToString()
		{
			var b = new StringBuilder();
			b.Append('{');
			for (int i = 0; i < _words.Length * BitsPerWord; i++)
			{
				if (this[i])
				{
					if (b.Length > 1)
						b.Append(", ");
					if (b.Length > 500)
					{
						b.Append("...");
						break;
					}
					b.Append(i);
				}
			}
			b.Append('}');
			return b.ToString();
		}
	}
}
