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

using System.Diagnostics;

namespace Dotnet4Gpu.Decompilation.Instructions
{
    /// <summary>
    /// The collection of variables in a <c>ILFunction</c>.
    /// </summary>
    public class ILVariableCollection : ICollection<ILVariable>, IReadOnlyList<ILVariable>
    {
        private readonly ILFunction _scope;
        private readonly List<ILVariable> _list = new();

        internal ILVariableCollection(ILFunction scope)
        {
            _scope = scope;
        }

        /// <summary>
        /// Gets a variable given its <c>IndexInFunction</c>.
        /// </summary>
        public ILVariable this[int index] => _list[index];

        public bool Add(ILVariable item)
        {
            if (item.Function != null)
            {
                return item.Function == _scope
                    ? false
                    : throw new ArgumentException("Variable already belongs to another scope");
            }
            item.Function = _scope;
            item.IndexInFunction = _list.Count;
            _list.Add(item);
            return true;
        }

        void ICollection<ILVariable>.Add(ILVariable item)
        {
            Add(item);
        }

        public void Clear()
        {
            foreach (var v in _list)
            {
                v.Function = null;
            }
            _list.Clear();
        }

        public bool Contains(ILVariable item)
        {
            Debug.Assert(item.Function != _scope || _list[item.IndexInFunction] == item);
            return item.Function == _scope;
        }

        public bool Remove(ILVariable item)
        {
            if (item.Function != _scope)
                return false;
            Debug.Assert(_list[item.IndexInFunction] == item);
            RemoveAt(item.IndexInFunction);
            return true;
        }

        void RemoveAt(int index)
        {
            _list[index].Function = null;
            // swap-remove index
            _list[index] = _list[^1];
            _list[index].IndexInFunction = index;
            _list.RemoveAt(_list.Count - 1);
        }

        public int Count => _list.Count;

        public void CopyTo(ILVariable[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        bool ICollection<ILVariable>.IsReadOnly => false;

        public List<ILVariable>.Enumerator GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator<ILVariable> IEnumerable<ILVariable>.GetEnumerator()
        {
            return GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
