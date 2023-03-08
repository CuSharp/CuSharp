// Copyright (c) 2014-2020 Daniel Grunwald
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
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Dotnet4Gpu.Decompilation.Util;

namespace Dotnet4Gpu.Decompilation.Instructions
{
    public abstract class ILInstruction
    {
        public bool MatchNop()
        {
            return this is Nop;
        }

        public bool MatchLdLoc([NotNullWhen(true)] out ILVariable? variable)
        {
            if (this is LdLoc inst)
            {
                variable = inst.Variable;
                return true;
            }
            variable = default;
            return false;
        }
        public bool MatchLdLoca([NotNullWhen(true)] out ILVariable? variable)
        {
            if (this is LdLoca inst)
            {
                variable = inst.Variable;
                return true;
            }
            variable = default;
            return false;
        }
        public bool MatchStLoc([NotNullWhen(true)] out ILVariable? variable, [NotNullWhen(true)] out ILInstruction? value)
        {
            if (this is StLoc inst)
            {
                variable = inst.Variable;
                value = inst.Value;
                return true;
            }
            variable = default;
            value = default;
            return false;
        }
        public bool MatchAddressOf([NotNullWhen(true)] out ILInstruction? value, [NotNullWhen(true)] out Type? type)
        {
            if (this is AddressOf inst)
            {
                value = inst.Value;
                type = inst.Type;
                return true;
            }
            value = default;
            type = default;
            return false;
        }

        public bool MatchLdcI4(out int value)
        {
            if (this is LdcI4 inst)
            {
                value = inst.Value;
                return true;
            }
            value = default;
            return false;
        }
        public bool MatchLdcI8(out long value)
        {
            if (this is LdcI8 inst)
            {
                value = inst.Value;
                return true;
            }
            value = default;
            return false;
        }
        public bool MatchLdFlda([NotNullWhen(true)] out ILInstruction? target, [NotNullWhen(true)] out FieldInfo? field)
        {
            if (this is LdFlda inst)
            {
                target = inst.Target;
                field = inst.Field;
                return true;
            }
            target = default;
            field = default;
            return false;
        }
        public bool MatchStObj([NotNullWhen(true)] out ILInstruction? target, [NotNullWhen(true)] out ILInstruction? value, [NotNullWhen(true)] out Type? type)
        {
            if (this is StObj inst)
            {
                target = inst.Target;
                value = inst.Value;
                type = inst.Type;
                return true;
            }
            target = default;
            value = default;
            type = default;
            return false;
        }
        public bool MatchNewArr([NotNullWhen(true)] out Type? type)
        {
            if (this is NewArr inst)
            {
                type = inst.Type;
                return true;
            }
            type = default;
            return false;
        }
        public bool MatchLdElema([NotNullWhen(true)] out Type? type, [NotNullWhen(true)] out ILInstruction? array)
        {
            if (this is LdElema inst)
            {
                type = inst.Type;
                array = inst.Array;
                return true;
            }
            type = default;
            array = default;
            return false;
        }

        public readonly OpCode OpCode;

        protected ILInstruction(OpCode opCode)
        {
            OpCode = opCode;
        }

        protected void ValidateChild(ILInstruction? inst)
        {
            if (inst == null)
                throw new ArgumentNullException(nameof(inst));
            Debug.Assert(!this.IsDescendantOf(inst), "ILAst must form a tree");
            // If a call to ReplaceWith() triggers the "ILAst must form a tree" assertion,
            // make sure to read the remarks on the ReplaceWith() method.
        }

        internal static void DebugAssert([DoesNotReturnIf(false)] bool b)
        {
            Debug.Assert(b);
        }

        internal static void DebugAssert([DoesNotReturnIf(false)] bool b, string msg)
        {
            Debug.Assert(b, msg);
        }

        [Conditional("DEBUG")]
        internal virtual void CheckInvariant(ILPhase phase)
        {
            foreach (var child in Children)
            {
                Debug.Assert(child.Parent == this);
                Debug.Assert(this.GetChild(child.ChildIndex) == child);
                // if child flags are invalid, parent flags must be too
                // exception: nested ILFunctions (lambdas)
                Debug.Assert(this is ILFunction || child._flags != InvalidFlags || this._flags == InvalidFlags);
                Debug.Assert(child.IsConnected == this.IsConnected);
                child.CheckInvariant(phase);
            }
            Debug.Assert((this.DirectFlags & ~this.Flags) == 0, "All DirectFlags must also appear in this.Flags");
        }

        /// <summary>
        /// Gets whether this node is a descendant of <paramref name="possibleAncestor"/>.
        /// Also returns true if <c>this</c>==<paramref name="possibleAncestor"/>.
        /// </summary>
        /// <remarks>
        /// This method uses the <c>Parent</c> property, so it may produce surprising results
        /// when called on orphaned nodes or with a possibleAncestor that contains stale positions
        /// (see remarks on Parent property).
        /// </remarks>
        public bool IsDescendantOf(ILInstruction possibleAncestor)
        {
            for (ILInstruction? ancestor = this; ancestor != null; ancestor = ancestor.Parent)
            {
                if (ancestor == possibleAncestor)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the stack type of the value produced by this instruction.
        /// </summary>
        public abstract StackType ResultType { get; }

        /* Not sure if it's a good idea to offer this on all instructions --
		 *   e.g. ldloc for a local of type `int?` would return StackType.O (because it's not a lifted operation),
		 *   even though the underlying type is int = StackType.I4.
		/// <summary>
		/// Gets the underlying result type of the value produced by this instruction.
		/// 
		/// If this is a lifted operation, the ResultType will be `StackType.O` (because Nullable{T} is a struct),
		/// and UnderlyingResultType will be result type of the corresponding non-lifted operation.
		/// 
		/// If this is not a lifted operation, the underlying result type is equal to the result type.
		/// </summary>
		public virtual StackType UnderlyingResultType { get => ResultType; }
		*/

#if DEBUG
        /// <summary>
        /// Gets whether this node (or any subnode) was modified since the last <c>ResetDirty()</c> call.
        /// </summary>
        /// <remarks>
        /// IsDirty is used by the StatementTransform, and must not be used by individual transforms within the loop.
        /// </remarks>
        internal bool IsDirty { get; private set; }

#endif

        [Conditional("DEBUG")]
        protected void MakeDirty()
        {
#if DEBUG
            for (ILInstruction? inst = this; inst != null && !inst.IsDirty; inst = inst._parent)
            {
                inst.IsDirty = true;
            }
#endif
        }

        private const InstructionFlags InvalidFlags = (InstructionFlags)(-1);

        private InstructionFlags _flags = InvalidFlags;

        /// <summary>
        /// Gets the flags describing the behavior of this instruction.
        /// This property computes the flags on-demand and caches them
        /// until some change to the ILAst invalidates the cache.
        /// </summary>
        /// <remarks>
        /// Flag cache invalidation makes use of the <c>Parent</c> property,
        /// so it is possible for this property to return a stale value
        /// if the instruction contains "stale positions" (see remarks on Parent property).
        /// </remarks>
        public InstructionFlags Flags {
            get {
                if (_flags == InvalidFlags)
                {
                    _flags = ComputeFlags();
                }
                return _flags;
            }
        }

        /// <summary>
        /// Returns whether the instruction (or one of its child instructions) has at least one of the specified flags.
        /// </summary>
        public bool HasFlag(InstructionFlags flags)
        {
            return (this.Flags & flags) != 0;
        }

        /// <summary>
        /// Returns whether the instruction (without considering child instructions) has at least one of the specified flags.
        /// </summary>
        public bool HasDirectFlag(InstructionFlags flags)
        {
            return (DirectFlags & flags) != 0;
        }

        protected void InvalidateFlags()
        {
            for (ILInstruction? inst = this; inst != null && inst._flags != InvalidFlags; inst = inst._parent)
                inst._flags = InvalidFlags;
        }

        protected abstract InstructionFlags ComputeFlags();

        /// <summary>
        /// Gets the flags for this instruction only, without considering the child instructions.
        /// </summary>
        public abstract InstructionFlags DirectFlags { get; }

        /// <summary>
        /// Gets the ILRange for this instruction alone, ignoring the operands.
        /// </summary>
        private Interval _ilRange;

        public void AddILRange(Interval newRange)
        {
            _ilRange = CombineILRange(_ilRange, newRange);
        }

        protected static Interval CombineILRange(Interval oldRange, Interval newRange)
        {
            if (oldRange.IsEmpty)
            {
                return newRange;
            }
            if (newRange.IsEmpty)
            {
                return oldRange;
            }
            if (newRange.Start <= oldRange.Start)
            {
                if (newRange.End < oldRange.Start)
                {
                    return newRange; // use the earlier range
                }
                else
                {
                    // join overlapping ranges
                    return new Interval(newRange.Start, Math.Max(newRange.End, oldRange.End));
                }
            }
            else if (newRange.Start <= oldRange.End)
            {
                // join overlapping ranges
                return new Interval(oldRange.Start, Math.Max(newRange.End, oldRange.End));
            }
            return oldRange;
        }

        public void AddILRange(ILInstruction sourceInstruction)
        {
            AddILRange(sourceInstruction._ilRange);
        }

        public void SetILRange(Interval range)
        {
            _ilRange = range;
        }

        public int StartILOffset => _ilRange.Start;

        public int EndILOffset => _ilRange.End;

        public bool ILRangeIsEmpty => _ilRange.IsEmpty;

        public IEnumerable<Interval> ILRanges => new[] { _ilRange };


        /// <summary>
        /// Calls the Visit*-method on the visitor corresponding to the concrete type of this instruction.
        /// </summary>
        public abstract void AcceptVisitor(ILVisitor visitor);

        /// <summary>
        /// Calls the Visit*-method on the visitor corresponding to the concrete type of this instruction.
        /// </summary>
        public abstract T AcceptVisitor<T>(ILVisitor<T> visitor);

        /// <summary>
        /// Gets the child nodes of this instruction.
        /// </summary>
        /// <remarks>
        /// The ChildrenCollection does not actually store the list of children,
        /// it merely allows accessing the children stored in the various slots.
        /// </remarks>
        public ChildrenCollection Children => new(this);

        protected abstract int GetChildCount();
        protected abstract ILInstruction GetChild(int index);
        protected abstract void SetChild(int index, ILInstruction value);
        protected abstract SlotInfo GetChildSlot(int index);

        #region ChildrenCollection + ChildrenEnumerator
        public readonly struct ChildrenCollection : IReadOnlyList<ILInstruction>
        {
            private readonly ILInstruction _inst;

            internal ChildrenCollection(ILInstruction inst)
            {
                Debug.Assert(inst != null);
                _inst = inst!;
            }

            public int Count => _inst.GetChildCount();

            public ILInstruction this[int index] {
                get => _inst.GetChild(index);
                set => _inst.SetChild(index, value);
            }

            public ChildrenEnumerator GetEnumerator()
            {
                return new ChildrenEnumerator(_inst);
            }

            IEnumerator<ILInstruction> IEnumerable<ILInstruction>.GetEnumerator()
            {
                return GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

#if DEBUG
        int activeEnumerators;

        [Conditional("DEBUG")]
        internal void StartEnumerator()
        {
            activeEnumerators++;
        }

        [Conditional("DEBUG")]
        internal void StopEnumerator()
        {
            Debug.Assert(activeEnumerators > 0);
            activeEnumerators--;
        }
#endif

        [Conditional("DEBUG")]
        internal void AssertNoEnumerators()
        {
#if DEBUG
            Debug.Assert(activeEnumerators == 0);
#endif
        }

        /// <summary>
        /// Enumerator over the children of an ILInstruction.
        /// Warning: even though this is a struct, it is invalid to copy:
        /// the number of constructor calls must match the number of dispose calls.
        /// </summary>
        public struct ChildrenEnumerator : IEnumerator<ILInstruction>
        {
            private ILInstruction? _inst;
            private readonly int _end;
            private int _pos;

            internal ChildrenEnumerator(ILInstruction inst)
            {
                DebugAssert(inst != null);
                _inst = inst;
                _pos = -1;
                _end = inst!.GetChildCount();
#if DEBUG
                inst.StartEnumerator();
#endif
            }

            public ILInstruction Current => _inst!.GetChild(_pos);

            public bool MoveNext()
            {
                return ++_pos < _end;
            }

            public void Dispose()
            {
#if DEBUG
                if (_inst != null)
                {
                    _inst.StopEnumerator();
                    _inst = null;
                }
#endif
            }

            object System.Collections.IEnumerator.Current => this.Current;

            void System.Collections.IEnumerator.Reset()
            {
                _pos = -1;
            }
        }
        #endregion

        /// <summary>
        /// Replaces this ILInstruction with the given replacement instruction.
        /// </summary>
        /// <remarks>
        /// It is temporarily possible for a node to be used in multiple places in the ILAst,
        /// this method only replaces this node at its primary position (see remarks on <see cref="Parent"/>).
        /// 
        /// This means you cannot use ReplaceWith() to wrap an instruction in another node.
        /// For example, <c>node.ReplaceWith(new BitNot(node))</c> will first call the BitNot constructor,
        /// which sets <c>node.Parent</c> to the BitNot instance.
        /// The ReplaceWith() call then attempts to set <c>BitNot.Argument</c> to the BitNot instance,
        /// which creates a cyclic ILAst. Meanwhile, node's original parent remains unmodified.
        /// 
        /// The solution in this case is to avoid using <c>ReplaceWith</c>.
        /// If the parent node is unknown, the following trick can be used:
        /// <code>
        /// node.Parent.Children[node.ChildIndex] = new BitNot(node);
        /// </code>
        /// Unlike the <c>ReplaceWith()</c> call, this will evaluate <c>node.Parent</c> and <c>node.ChildIndex</c>
        /// before the <c>BitNot</c> constructor is called, thus modifying the expected position in the ILAst.
        /// </remarks>
        public void ReplaceWith(ILInstruction replacement)
        {
            Debug.Assert(_parent!.GetChild(ChildIndex) == this);
            if (replacement == this)
                return;
            _parent.SetChild(ChildIndex, replacement);
        }

        /// <summary>
        /// Returns all descendants of the ILInstruction in post-order.
        /// (including the ILInstruction itself)
        /// </summary>
        /// <remarks>
        /// Within a loop 'foreach (var node in inst.Descendants)', it is illegal to
        /// add or remove from the child collections of node's ancestors, as those are
        /// currently being enumerated.
        /// Note that it is valid to modify node's children as those were already previously visited.
        /// As a special case, it is also allowed to replace node itself with another node.
        /// </remarks>
        public IEnumerable<ILInstruction> Descendants {
            get {
                // Copy of TreeTraversal.PostOrder() specialized for ChildrenEnumerator
                // We could potentially eliminate the stack by using Parent/ChildIndex,
                // but that makes it difficult to reason about the behavior in the cases
                // where Parent/ChildIndex is not accurate (stale positions), especially
                // if the ILAst is modified during enumeration.
                Stack<ChildrenEnumerator> stack = new Stack<ChildrenEnumerator>();
                ChildrenEnumerator enumerator = new ChildrenEnumerator(this);
                try
                {
                    while (true)
                    {
                        while (enumerator.MoveNext())
                        {
                            var element = enumerator.Current;
                            stack.Push(enumerator);
                            enumerator = new ChildrenEnumerator(element);
                        }
                        enumerator.Dispose();
                        if (stack.Count > 0)
                        {
                            enumerator = stack.Pop();
                            yield return enumerator.Current;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                finally
                {
                    enumerator.Dispose();
                    while (stack.Count > 0)
                    {
                        stack.Pop().Dispose();
                    }
                }
                yield return this;
            }
        }

        /// <summary>
        /// Gets the ancestors of this node (including the node itself as first element).
        /// </summary>
        public IEnumerable<ILInstruction> Ancestors {
            get {
                for (ILInstruction? node = this; node != null; node = node.Parent)
                {
                    yield return node;
                }
            }
        }

        /// <summary>
        /// Number of parents that refer to this instruction and are connected to the root.
        /// Usually is 0 for unconnected nodes and 1 for connected nodes, but may temporarily increase to 2
        /// when the ILAst is re-arranged (e.g. within SetChildInstruction),
        /// or possibly even more (re-arrangement with stale positions).
        /// </summary>
        private byte _refCount;

        internal void AddRef()
        {
            if (_refCount++ == 0)
            {
                Connected();
            }
        }

        internal void ReleaseRef()
        {
            Debug.Assert(_refCount > 0);
            if (--_refCount == 0)
            {
                Disconnected();
            }
        }

        /// <summary>
        /// Gets whether this ILInstruction is connected to the root node of the ILAst.
        /// </summary>
        /// <remarks>
        /// This property returns true if the ILInstruction is reachable from the root node
        /// of the ILAst; it does not make use of the <c>Parent</c> field so the considerations
        /// about orphaned nodes and stale positions don't apply.
        /// </remarks>
        protected internal bool IsConnected => _refCount > 0;

        /// <summary>
        /// Called after the ILInstruction was connected to the root node of the ILAst.
        /// </summary>
        protected virtual void Connected()
        {
            foreach (var child in Children)
                child.AddRef();
        }

        /// <summary>
        /// Called after the ILInstruction was disconnected from the root node of the ILAst.
        /// </summary>
        protected virtual void Disconnected()
        {
            foreach (var child in Children)
                child.ReleaseRef();
        }

        private ILInstruction? _parent;

        /// <summary>
        /// Gets the parent of this ILInstruction.
        /// </summary>
        /// <remarks>
        /// It is temporarily possible for a node to be used in multiple places in the ILAst
        /// (making the ILAst a DAG instead of a tree).
        /// The <c>Parent</c> and <c>ChildIndex</c> properties are written whenever
        /// a node is stored in a slot.
        /// The node's occurrence in that slot is termed the "primary position" of the node,
        /// and all other (older) uses of the nodes are termed "stale positions".
        /// 
        /// A consistent ILAst must not contain any stale positions.
        /// Debug builds of ILSpy check the ILAst for consistency after every IL transform.
        /// 
        /// If a slot containing a node is overwritten with another node, the <c>Parent</c>
        /// and <c>ChildIndex</c> of the old node are not modified.
        /// This allows overwriting stale positions to restore consistency of the ILAst.
        /// 
        /// If a "primary position" is overwritten, the <c>Parent</c> of the old node also remains unmodified.
        /// This makes the old node an "orphaned node".
        /// Orphaned nodes may later be added back to the ILAst (or can just be garbage-collected).
        /// 
        /// Note that is it is possible (though unusual) for a stale position to reference an orphaned node.
        /// </remarks>
        public ILInstruction? Parent => _parent;

        /// <summary>
        /// Gets the index of this node in the <c>Parent.Children</c> collection.
        /// </summary>
        /// <remarks>
        /// It is temporarily possible for a node to be used in multiple places in the ILAst,
        /// this property returns the index of the primary position of this node (see remarks on <see cref="Parent"/>).
        /// </remarks>
        public int ChildIndex { get; internal set; } = -1;

        /// <summary>
        /// Gets information about the slot in which this instruction is stored.
        /// (i.e., the relation of this instruction to its parent instruction)
        /// </summary>
        /// <remarks>
        /// It is temporarily possible for a node to be used in multiple places in the ILAst,
        /// this property returns the slot of the primary position of this node (see remarks on <see cref="Parent"/>).
        /// 
        /// Precondition: this node must not be orphaned.
        /// </remarks>
        public SlotInfo? SlotInfo {
            get {
                if (_parent == null)
                    return null;
                Debug.Assert(_parent.GetChild(this.ChildIndex) == this);
                return _parent.GetChildSlot(this.ChildIndex);
            }
        }

        /// <summary>
        /// Replaces a child of this ILInstruction.
        /// </summary>
        /// <param name="childPointer">Reference to the field holding the child</param>
        /// <param name="newValue">New child</param>
        /// <param name="index">Index of the field in the Children collection</param>
        protected internal void SetChildInstruction<T>(ref T childPointer, T newValue, int index)
            where T : ILInstruction?
        {
            T oldValue = childPointer;
            Debug.Assert(oldValue == GetChild(index));
            if (oldValue == newValue && newValue?._parent == this && newValue.ChildIndex == index)
                return;
            childPointer = newValue;
            if (newValue != null)
            {
                newValue._parent = this;
                newValue.ChildIndex = index;
            }
            InvalidateFlags();
            MakeDirty();
            if (_refCount > 0)
            {
                // The new value may be a subtree of the old value.
                // We first call AddRef(), then ReleaseRef() to prevent the subtree
                // that stays connected from receiving a Disconnected() notification followed by a Connected() notification.
                if (newValue != null)
                    newValue.AddRef();
                if (oldValue != null)
                    oldValue.ReleaseRef();
            }
        }

        /// <summary>
        /// Called when a new child is added to a InstructionCollection.
        /// </summary>
        protected internal void InstructionCollectionAdded(ILInstruction newChild)
        {
            Debug.Assert(GetChild(newChild.ChildIndex) == newChild);
            Debug.Assert(!this.IsDescendantOf(newChild), "ILAst must form a tree");
            // If a call to ReplaceWith() triggers the "ILAst must form a tree" assertion,
            // make sure to read the remarks on the ReplaceWith() method.
            newChild._parent = this;
            if (_refCount > 0)
                newChild.AddRef();
        }

        /// <summary>
        /// Called when a child is removed from a InstructionCollection.
        /// </summary>
        protected internal void InstructionCollectionRemoved(ILInstruction oldChild)
        {
            if (_refCount > 0)
                oldChild.ReleaseRef();
        }

        /// <summary>
        /// Called when a series of add/remove operations on the InstructionCollection is complete.
        /// </summary>
        protected internal virtual void InstructionCollectionUpdateComplete()
        {
            InvalidateFlags();
            MakeDirty();
        }

        /// <summary>
        /// Creates a deep clone of the ILInstruction.
        /// </summary>
        /// <remarks>
        /// It is valid to clone nodes with stale positions (see remarks on <c>Parent</c>);
        /// the result of such a clone will not contain any stale positions (nodes at
        /// multiple positions will be cloned once per position).
        /// </remarks>
        public abstract ILInstruction Clone();

        /// <summary>
        /// Creates a shallow clone of the ILInstruction.
        /// </summary>
        /// <remarks>
        /// Like MemberwiseClone(), except that the new instruction starts as disconnected.
        /// </remarks>
        protected ILInstruction ShallowClone()
        {
            ILInstruction inst = (ILInstruction)MemberwiseClone();
            // reset refCount and parent so that the cloned instruction starts as disconnected
            inst._refCount = 0;
            inst._parent = null;
            inst._flags = InvalidFlags;
#if DEBUG
            inst.activeEnumerators = 0;
#endif
            return inst;
        }

        /// <summary>
        /// Gets whether the specified instruction may be inlined into the specified slot.
        /// Note: this does not check whether reordering with the previous slots is valid; only wheter the target slot supports inlining at all!
        /// </summary>
        internal virtual bool CanInlineIntoSlot(int childIndex, ILInstruction expressionBeingMoved)
        {
            return GetChildSlot(childIndex).CanInlineInto;
        }

        public bool MatchLdcI4(int val)
        {
            return OpCode == OpCode.LdcI4 && ((LdcI4)this).Value == val;
        }

        /// <summary>
        /// Matches ldc.i4, ldc.i8, and extending conversions.
        /// </summary>
        public bool MatchLdcI(out long val)
        {
            if (MatchLdcI8(out val))
                return true;
            if (MatchLdcI4(out int intVal))
            {
                val = intVal;
                return true;
            }
            if (this is Conv conv)
            {
                if (conv.Kind == ConversionKind.SignExtend)
                {
                    return conv.Argument.MatchLdcI(out val);
                }
                else if (conv.Kind == ConversionKind.ZeroExtend && conv.InputType == StackType.I4)
                {
                    if (conv.Argument.MatchLdcI(out val))
                    {
                        // clear top 32 bits
                        val &= uint.MaxValue;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool MatchLdLoc(ILVariable? variable)
        {
            var inst = this as LdLoc;
            return inst != null && inst.Variable == variable;
        }

        public bool MatchLdLoca(ILVariable? variable)
        {
            var inst = this as LdLoca;
            return inst != null && inst.Variable == variable;
        }

        public bool MatchStLoc(ILVariable? variable, [NotNullWhen(true)] out ILInstruction? value)
        {
            if (this is StLoc inst && inst.Variable == variable)
            {
                value = inst.Value;
                return true;
            }
            value = null;
            return false;
        }

        public bool MatchBranch([NotNullWhen(true)] out Block? targetBlock)
        {
            if (this is Branch inst)
            {
                targetBlock = inst.TargetBlock;
                return true;
            }
            targetBlock = null;
            return false;
        }

        public bool MatchBranch(Block? targetBlock)
        {
            var inst = this as Branch;
            return inst != null && inst.TargetBlock == targetBlock;
        }

        public bool MatchLeave(BlockContainer? targetContainer)
        {
            var inst = this as Leave;
            return inst != null && inst.TargetContainer == targetContainer && inst.Value.MatchNop();
        }

        public bool MatchIfInstruction([NotNullWhen(true)] out ILInstruction? condition, [NotNullWhen(true)] out ILInstruction? trueInst, [NotNullWhen(true)] out ILInstruction? falseInst)
        {
            if (this is IfInstruction inst)
            {
                condition = inst.Condition;
                trueInst = inst.TrueInst;
                falseInst = inst.FalseInst;
                return true;
            }
            condition = null;
            trueInst = null;
            falseInst = null;
            return false;
        }

        /// <summary>
        /// Matches an logical negation.
        /// </summary>
        public bool MatchLogicNot([NotNullWhen(true)] out ILInstruction? arg)
        {
            if (this is Comp comp && comp.Kind == ComparisonKind.Equality
                                  && comp.LiftingKind == ComparisonLiftingKind.None
                                  && comp.Right.MatchLdcI4(0))
            {
                arg = comp.Left;
                return true;
            }
            arg = null;
            return false;
        }

        public bool MatchLdFld([NotNullWhen(true)] out ILInstruction? target, [NotNullWhen(true)] out FieldInfo? field)
        {
            if (this is LdObj ldobj && ldobj.Target is LdFlda ldflda && ldobj.UnalignedPrefix == 0 && !ldobj.IsVolatile)
            {
                field = ldflda.Field;
                if (!field.DeclaringType.IsValueType || !ldflda.Target.MatchAddressOf(out target, out _))
                {
                    target = ldflda.Target;
                }
                return true;
            }
            target = null;
            field = null;
            return false;
        }

        /// <summary>
        /// If this instruction is a conversion of the specified kind, return its argument.
        /// Otherwise, return the instruction itself.
        /// </summary>
        /// <remarks>
        /// Does not unwrap lifted conversions.
        /// </remarks>
        public virtual ILInstruction UnwrapConv(ConversionKind kind)
        {
            return this;
        }
    }
}
