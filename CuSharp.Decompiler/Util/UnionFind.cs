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
#nullable enable

using CuSharp.Decompiler.Util;

namespace CuSharp.Decompiler.Util
{
    /// <summary>
    /// Union-Find data structure.
    /// </summary>
    public class UnionFind<T> where T : notnull
    {
        private readonly Dictionary<T, Node> _mapping;

        class Node
        {
            public int Rank;
            public Node Parent;
            public readonly T Value;

            internal Node(T value)
            {
                Value = value;
                Parent = this;
            }
        }

        public UnionFind()
        {
            _mapping = new Dictionary<T, Node>();
        }

        Node GetNode(T element)
        {
            if (!_mapping.TryGetValue(element, out Node? node))
            {
                node = new Node(element);
                node.Parent = node;
                _mapping.Add(element, node);
            }
            return node;
        }

        public T Find(T element)
        {
            return FindRoot(GetNode(element)).Value;
        }

        Node FindRoot(Node node)
        {
            if (node.Parent != node)
                node.Parent = FindRoot(node.Parent);
            return node.Parent;
        }

        public void Merge(T a, T b)
        {
            var rootA = FindRoot(GetNode(a));
            var rootB = FindRoot(GetNode(b));
            if (rootA == rootB)
                return;
            if (rootA.Rank < rootB.Rank)
                rootA.Parent = rootB;
            else if (rootA.Rank > rootB.Rank)
                rootB.Parent = rootA;
            else
            {
                rootB.Parent = rootA;
                rootA.Rank++;
            }
        }
    }
}
