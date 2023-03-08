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
using Dotnet4Gpu.Decompilation.Util;

namespace Dotnet4Gpu.Decompilation.Instructions
{
    /// <summary>
    /// A container of IL blocks.
    /// Each block is an extended basic block (branches may only jump to the beginning of blocks, not into the middle),
    /// and only branches within this container may reference the blocks in this container.
    /// That means that viewed from the outside, the block container has a single entry point (but possibly multiple exit points),
    /// and the same holds for every block within the container.
    /// 
    /// All blocks in the container must perform unconditional control flow (falling through to the block end is not allowed).
    /// To exit the block container, use the 'leave' instruction.
    /// </summary>
    public sealed class BlockContainer : ILInstruction
    {
        public override StackType ResultType => ExpectedResultType;

        public override void AcceptVisitor(ILVisitor visitor)
        {
            visitor.VisitBlockContainer(this);
        }
        public override T AcceptVisitor<T>(ILVisitor<T> visitor)
        {
            return visitor.VisitBlockContainer(this);
        }

        public static readonly SlotInfo BlockSlot = new("Block", isCollection: true);
        public readonly InstructionCollection<Block> Blocks;

        public ContainerKind Kind { get; set; }
        public StackType ExpectedResultType { get; set; }

        private int _leaveCount;

        /// <summary>
        /// Gets the number of 'leave' instructions that target this BlockContainer.
        /// </summary>
        public int LeaveCount {
            get => _leaveCount;
            internal set {
                _leaveCount = value;
                InvalidateFlags();
            }
        }

        private Block? _entryPoint;

        /// <summary>
        /// Gets the container's entry point. This is the first block in the Blocks collection.
        /// </summary>
        public Block EntryPoint {
            get {
                // HACK: While it's possible to have BlockContainers without entry point,
                // normally every container must have an entry point according to its invariant.
                // Thus it's easier on the transforms if this property returns a non-nullable EntryPoint.
                return _entryPoint!;
            }
            private set {
                if (_entryPoint != null && IsConnected)
                    _entryPoint.IncomingEdgeCount--;
                _entryPoint = value;
                if (_entryPoint != null && IsConnected)
                    _entryPoint.IncomingEdgeCount++;
            }
        }

        public BlockContainer(ContainerKind kind = ContainerKind.Normal, StackType expectedResultType = StackType.Void) : base(OpCode.BlockContainer)
        {
            Kind = kind;
            Blocks = new InstructionCollection<Block>(this, 0);
            ExpectedResultType = expectedResultType;
        }

        public override ILInstruction Clone()
        {
            BlockContainer clone = new BlockContainer();
            clone.AddILRange(this);
            clone.Blocks.AddRange(Enumerable.Select<Block, Block>(this.Blocks, block => (Block)block.Clone()));
            // Adjust branch instructions to point to the new container
            foreach (var branch in clone.Descendants.OfType<Branch>())
            {
                if (branch.TargetBlock != null && branch.TargetBlock.Parent == this)
                    branch.TargetBlock = clone.Blocks[branch.TargetBlock.ChildIndex];
            }
            foreach (var leave in clone.Descendants.OfType<Leave>())
            {
                if (leave.TargetContainer == this)
                    leave.TargetContainer = clone;
            }
            return clone;
        }

        protected internal override void InstructionCollectionUpdateComplete()
        {
            base.InstructionCollectionUpdateComplete();
            EntryPoint = Blocks.FirstOrDefault()!;
        }

        protected override void Connected()
        {
            base.Connected();
            if (_entryPoint != null)
                _entryPoint.IncomingEdgeCount++;
        }

        protected override void Disconnected()
        {
            base.Disconnected();
            if (_entryPoint != null)
                _entryPoint.IncomingEdgeCount--;
        }

        protected override int GetChildCount()
        {
            return Blocks.Count;
        }

        protected override ILInstruction GetChild(int index)
        {
            return Blocks[index];
        }

        protected override void SetChild(int index, ILInstruction? value)
        {
            if (Blocks[index] != value)
                throw new InvalidOperationException("Cannot replace blocks in BlockContainer");
        }

        protected override SlotInfo GetChildSlot(int index)
        {
            return BlockSlot;
        }

        internal override void CheckInvariant(ILPhase phase)
        {
            base.CheckInvariant(phase);
            Debug.Assert(Blocks.Count > 0 && EntryPoint == Blocks[0]);
            Debug.Assert(!IsConnected || EntryPoint.IncomingEdgeCount >= 1);
            Debug.Assert(Parent is ILFunction || !ILRangeIsEmpty);
            Debug.Assert(Enumerable.All<Block>(Blocks, b => b.HasFlag(InstructionFlags.EndPointUnreachable)));
            Debug.Assert(Enumerable.All<Block>(Blocks, b => b.Kind == BlockKind.ControlFlow)); // this also implies that the blocks don't use FinalInstruction
            Debug.Assert(TopologicalSort(deleteUnreachableBlocks: true).Count == Blocks.Count, "Container should not have any unreachable blocks");
            Block? bodyStartBlock;
            switch (Kind)
            {
                case ContainerKind.Normal:
                    break;
                case ContainerKind.Loop:
                    Debug.Assert(EntryPoint.IncomingEdgeCount > 1);
                    break;
                case ContainerKind.Switch:
                    Debug.Assert(EntryPoint.Instructions.Count == 1);
                    Debug.Assert(EntryPoint.Instructions[0] is SwitchInstruction);
                    Debug.Assert(EntryPoint.IncomingEdgeCount == 1);
                    break;
                case ContainerKind.While:
                    Debug.Assert(EntryPoint.IncomingEdgeCount > 1);
                    Debug.Assert(Blocks.Count >= 2);
                    Debug.Assert(MatchConditionBlock(EntryPoint, out _, out bodyStartBlock));
                    Debug.Assert(bodyStartBlock == Blocks[1]);
                    break;
                case ContainerKind.DoWhile:
                    Debug.Assert(EntryPoint.IncomingEdgeCount > 1);
                    Debug.Assert(Blocks.Count >= 2);
                    Debug.Assert(MatchConditionBlock(Blocks.Last(), out _, out bodyStartBlock));
                    Debug.Assert(bodyStartBlock == EntryPoint);
                    break;
                case ContainerKind.For:
                    Debug.Assert(EntryPoint.IncomingEdgeCount == 2);
                    Debug.Assert(Blocks.Count >= 3);
                    Debug.Assert(MatchConditionBlock(EntryPoint, out _, out bodyStartBlock));
                    Debug.Assert(MatchIncrementBlock(Blocks.Last()));
                    Debug.Assert(bodyStartBlock == Blocks[1]);
                    break;
            }
        }

        protected override InstructionFlags ComputeFlags()
        {
            InstructionFlags flags = InstructionFlags.ControlFlow;
            foreach (var block in Blocks)
            {
                flags |= block.Flags;
            }
            // The end point of the BlockContainer is only reachable if there's a leave instruction
            if (LeaveCount == 0)
                flags |= InstructionFlags.EndPointUnreachable;
            else
                flags &= ~InstructionFlags.EndPointUnreachable;
            return flags;
        }

        public override InstructionFlags DirectFlags => InstructionFlags.ControlFlow;

        internal override bool CanInlineIntoSlot(int childIndex, ILInstruction expressionBeingMoved)
        {
            // Inlining into the entry-point is allowed as long as we're not moving code into a loop.
            // This is used to inline into the switch expression.
            return childIndex == 0 && this.EntryPoint.IncomingEdgeCount == 1;
        }

        /// <summary>
        /// Topologically sort the blocks.
        /// The new order is returned without modifying the BlockContainer.
        /// </summary>
        /// <param name="deleteUnreachableBlocks">If true, unreachable blocks are not included in the new order.</param>
        public List<Block> TopologicalSort(bool deleteUnreachableBlocks = false)
        {
            // Visit blocks in post-order
            BitSet visited = new BitSet(Blocks.Count);
            List<Block> postOrder = new List<Block>();
            Visit(EntryPoint);
            postOrder.Reverse();
            if (!deleteUnreachableBlocks)
            {
                for (int i = 0; i < Blocks.Count; i++)
                {
                    if (!visited[i])
                        postOrder.Add(Blocks[i]);
                }
            }
            return postOrder;

            void Visit(Block block)
            {
                Debug.Assert(block.Parent == this);
                if (!visited[block.ChildIndex])
                {
                    visited[block.ChildIndex] = true;

                    foreach (var branch in block.Descendants.OfType<Branch>())
                    {
                        if (branch.TargetBlock.Parent == this)
                        {
                            Visit(branch.TargetBlock);
                        }
                    }

                    postOrder.Add(block);
                }
            };
        }

        public bool MatchConditionBlock(Block block, [NotNullWhen(true)] out ILInstruction? condition, [NotNullWhen(true)] out Block? bodyStartBlock)
        {
            condition = null;
            bodyStartBlock = null;
            if (block.Instructions.Count != 1)
                return false;
            if (!block.Instructions[0].MatchIfInstruction(out condition, out var trueInst, out var falseInst))
                return false;
            return falseInst.MatchLeave(this) && trueInst.MatchBranch(out bodyStartBlock);
        }

        public bool MatchIncrementBlock(Block block)
        {
            if (block.Instructions.Count == 0)
                return false;
            if (!block.Instructions.Last().MatchBranch(EntryPoint))
                return false;
            return true;
        }
    }
}
