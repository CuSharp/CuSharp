﻿
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
#nullable enable

using CuSharp.Decompiler.Util;
using System.Collections;

namespace CuSharp.Decompiler.Util
{
    [Serializable]
    public sealed class EmptyList<T> : IList<T>, IEnumerator<T>, IReadOnlyList<T>
    {
        public static readonly EmptyList<T> Instance = new();

        private EmptyList() { }

        public T this[int index]
        {
            get => throw new ArgumentOutOfRangeException(nameof(index));
            set => throw new ArgumentOutOfRangeException(nameof(index));
        }

        public int Count => 0;

        bool ICollection<T>.IsReadOnly => true;

        int IList<T>.IndexOf(T item)
        {
            return -1;
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
        }

        bool ICollection<T>.Contains(T item)
        {
            return false;
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
        }

        bool ICollection<T>.Remove(T item)
        {
            return false;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        T IEnumerator<T>.Current => throw new NotSupportedException();

        object IEnumerator.Current => throw new NotSupportedException();

        void IDisposable.Dispose()
        {
        }

        bool IEnumerator.MoveNext()
        {
            return false;
        }

        void IEnumerator.Reset()
        {
        }
    }

    public static class Empty<T>
    {
        public static readonly T[] Array = System.Array.Empty<T>();
    }

    public struct Unit { }
}
