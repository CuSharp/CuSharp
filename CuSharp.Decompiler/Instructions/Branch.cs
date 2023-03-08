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

namespace Dotnet4Gpu.Decompilation.Instructions
{
    /// <summary>
    /// Unconditional branch. <c>goto target;</c>
    /// </summary>
    /// <remarks>
    /// When jumping to the entrypoint of the current block container, the branch represents a <c>continue</c> statement.
    /// </remarks>
    public sealed class Branch : SimpleInstruction, IBranchOrLeaveInstruction
    {
        public override StackType ResultType => StackType.Void;

        protected override InstructionFlags ComputeFlags()
        {
            return InstructionFlags.EndPointUnreachable | InstructionFlags.MayBranch;
        }
        public override InstructionFlags DirectFlags => InstructionFlags.EndPointUnreachable | InstructionFlags.MayBranch;

        public override void AcceptVisitor(ILVisitor visitor)
        {
            visitor.VisitBranch(this);
        }
        public override T AcceptVisitor<T>(ILVisitor<T> visitor)
        {
            return visitor.VisitBranch(this);
        }

        private readonly int _targetIlOffset;
        private Block? _targetBlock;

        public Branch(int targetILOffset) : base(OpCode.Branch)
        {
            _targetIlOffset = targetILOffset;
        }

        public Branch(Block targetBlock) : base(OpCode.Branch)
        {
            _targetBlock = targetBlock ?? throw new ArgumentNullException(nameof(targetBlock));
            _targetIlOffset = targetBlock.StartILOffset;
        }

        public int TargetILOffset => _targetBlock?.StartILOffset ?? _targetIlOffset;

        public Block TargetBlock {
            get {
                // HACK: We treat TargetBlock as non-nullable publicly, because it's only null inside
                // the ILReader, and becomes non-null once the BlockBuilder has run.
                return _targetBlock!;
            }
            set {
                if (_targetBlock != null && IsConnected)
                    _targetBlock.IncomingEdgeCount--;
                _targetBlock = value;
                if (_targetBlock != null && IsConnected)
                    _targetBlock.IncomingEdgeCount++;
            }
        }

        protected override void Connected()
        {
            base.Connected();
            if (_targetBlock != null)
                _targetBlock.IncomingEdgeCount++;
        }

        protected override void Disconnected()
        {
            base.Disconnected();
            if (_targetBlock != null)
                _targetBlock.IncomingEdgeCount--;
        }

        internal override void CheckInvariant(ILPhase phase)
        {
            base.CheckInvariant(phase);
            if (phase > ILPhase.InILReader)
            {
                Debug.Assert(_targetBlock?.Parent is BlockContainer);
                Debug.Assert(this.IsDescendantOf(_targetBlock!.Parent!));
                Debug.Assert(_targetBlock!.Parent!.Children[_targetBlock.ChildIndex] == _targetBlock);
            }
        }
    }
}
