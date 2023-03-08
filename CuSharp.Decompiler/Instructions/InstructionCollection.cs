// Copyright (c) 2014 Daniel Grunwald
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
    public sealed class InstructionCollection<T> : IList<T>, IReadOnlyList<T> where T : ILInstruction
    {
        private readonly ILInstruction _parentInstruction;
        private readonly int _firstChildIndex;
        private readonly List<T> _list = new();

        public InstructionCollection(ILInstruction parentInstruction, int firstChildIndex)
        {
            _parentInstruction = parentInstruction ?? throw new ArgumentNullException(nameof(parentInstruction));
            _firstChildIndex = firstChildIndex;
        }

        public int Count => _list.Count;

        public T this[int index]
        {
            get => _list[index];
            set
            {
                T oldValue = _list[index];
                if (!(oldValue == value && value.Parent == _parentInstruction && value.ChildIndex == index))
                {
                    _list[index] = value;
                    value.ChildIndex = index + _firstChildIndex;
                    _parentInstruction.InstructionCollectionAdded(value);
                    _parentInstruction.InstructionCollectionRemoved(oldValue);
                    _parentInstruction.InstructionCollectionUpdateComplete();
                }
            }
        }

        #region GetEnumerator
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        /// Custom enumerator for InstructionCollection.
        /// Unlike List{T}.Enumerator, this enumerator allows replacing an item during the enumeration.
        /// Adding/removing items from the collection still is invalid (however, such
        /// invalid actions are only detected in debug builds).
        /// 
        /// Warning: even though this is a struct, it is invalid to copy:
        /// the number of constructor calls must match the number of dispose calls.
        /// </summary>
        public struct Enumerator : IEnumerator<T>
        {
#if DEBUG
            private ILInstruction? parentInstruction;
#endif
            readonly List<T> list;
            int pos;

            public Enumerator(InstructionCollection<T> col)
            {
                list = col._list;
                pos = -1;
#if DEBUG
                parentInstruction = col._parentInstruction;
                col._parentInstruction.StartEnumerator();
#endif
            }

            [DebuggerStepThrough]
            public bool MoveNext()
            {
                return ++pos < list.Count;
            }

            public T Current
            {
                [DebuggerStepThrough]
                get { return list[pos]; }
            }

            [DebuggerStepThrough]
            public void Dispose()
            {
#if DEBUG
                if (parentInstruction != null)
                {
                    parentInstruction.StopEnumerator();
                    parentInstruction = null;
                }
#endif
            }

            void System.Collections.IEnumerator.Reset()
            {
                pos = -1;
            }

            object System.Collections.IEnumerator.Current => Current;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        /// <summary>
        /// Gets the index of the instruction in this collection.
        /// Returns -1 if the instruction does not exist in the collection.
        /// </summary>
        /// <remarks>
        /// Runs in O(1) if the item can be found using the Parent/ChildIndex properties.
        /// Otherwise, runs in O(N).
        /// </remarks>
        public int IndexOf(T? item)
        {
            if (item == null)
            {
                // InstructionCollection can't contain nulls
                return -1;
            }
            // If this collection is the item's primary position, we can use ChildIndex:
            int index = item.ChildIndex - _firstChildIndex;
            if (index >= 0 && index < _list.Count && _list[index] == item)
                return index;
            // But we still need to fall back on a full search, because the ILAst might be
            // in a state where item is in multiple locations.
            return _list.IndexOf(item);
        }

        /// <summary>
        /// Gets whether the item is in this collection.
        /// </summary>
        /// <remarks>
        /// This method searches the list.
        /// Usually it's more efficient to test item.Parent instead!
        /// </remarks>
        public bool Contains(T? item)
        {
            return IndexOf(item) >= 0;
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        bool ICollection<T>.IsReadOnly => false;

        public void Add(T value)
        {
            _parentInstruction.AssertNoEnumerators();
            value.ChildIndex = _list.Count + _firstChildIndex;
            _list.Add(value);
            _parentInstruction.InstructionCollectionAdded(value);
            _parentInstruction.InstructionCollectionUpdateComplete();
        }

        public void AddRange(IEnumerable<T> values)
        {
            _parentInstruction.AssertNoEnumerators();
            foreach (T value in values)
            {
                value.ChildIndex = _list.Count + _firstChildIndex;
                _list.Add(value);
                _parentInstruction.InstructionCollectionAdded(value);
            }
            _parentInstruction.InstructionCollectionUpdateComplete();
        }

        /// <summary>
        /// Replaces all entries in the InstructionCollection with the newList.
        /// </summary>
        /// <remarks>
        /// Equivalent to Clear() followed by AddRange(newList), but slightly more efficient.
        /// </remarks>
        public void ReplaceList(IEnumerable<T> newList)
        {
            _parentInstruction.AssertNoEnumerators();
            int index = 0;
            foreach (T value in newList)
            {
                value.ChildIndex = index + _firstChildIndex;
                if (index < _list.Count)
                {
                    T oldValue = _list[index];
                    _list[index] = value;
                    _parentInstruction.InstructionCollectionAdded(value);
                    _parentInstruction.InstructionCollectionRemoved(oldValue);
                }
                else
                {
                    _list.Add(value);
                    _parentInstruction.InstructionCollectionAdded(value);
                }
                index++;
            }
            for (int i = index; i < _list.Count; i++)
            {
                _parentInstruction.InstructionCollectionRemoved(_list[i]);
            }
            _list.RemoveRange(index, _list.Count - index);
            _parentInstruction.InstructionCollectionUpdateComplete();
        }

        public void Insert(int index, T item)
        {
            _parentInstruction.AssertNoEnumerators();
            _list.Insert(index, item);
            item.ChildIndex = index;
            _parentInstruction.InstructionCollectionAdded(item);
            for (int i = index + 1; i < _list.Count; i++)
            {
                T other_item = _list[i];
                // Update ChildIndex of items after the inserted one, but only if
                // that's their 'primary position' (in case of multiple parents)
                if (other_item.Parent == _parentInstruction && other_item.ChildIndex == i + _firstChildIndex - 1)
                    other_item.ChildIndex = i + _firstChildIndex;
            }
            _parentInstruction.InstructionCollectionUpdateComplete();
        }

        public void RemoveAt(int index)
        {
            _parentInstruction.AssertNoEnumerators();
            _parentInstruction.InstructionCollectionRemoved(_list[index]);
            _list.RemoveAt(index);
            for (int i = index; i < _list.Count; i++)
            {
                var other_item = _list[i];
                if (other_item.Parent == _parentInstruction && other_item.ChildIndex == i + _firstChildIndex + 1)
                    other_item.ChildIndex = i + _firstChildIndex;
            }
            _parentInstruction.InstructionCollectionUpdateComplete();
        }

        public void Clear()
        {
            _parentInstruction.AssertNoEnumerators();
            foreach (var entry in _list)
            {
                _parentInstruction.InstructionCollectionRemoved(entry);
            }
            _list.Clear();
            _parentInstruction.InstructionCollectionUpdateComplete();
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }

        public void RemoveRange(int index, int count)
        {
            _parentInstruction.AssertNoEnumerators();
            for (int i = 0; i < count; i++)
            {
                _parentInstruction.InstructionCollectionRemoved(_list[index + i]);
            }
            _list.RemoveRange(index, count);
            for (int i = index; i < _list.Count; i++)
            {
                var other_item = _list[i];
                if (other_item.Parent == _parentInstruction && other_item.ChildIndex == i + _firstChildIndex + count)
                    other_item.ChildIndex = i + _firstChildIndex;
            }
            _parentInstruction.InstructionCollectionUpdateComplete();
        }

        // more efficient versions of some LINQ methods:
        public T First()
        {
            return _list[0];
        }

        public T? FirstOrDefault()
        {
            return _list.Count > 0 ? _list[0] : null;
        }

        public T Last()
        {
            return _list[^1];
        }

        public T? LastOrDefault()
        {
            return _list.Count > 0 ? _list[^1] : null;
        }
    }
}
