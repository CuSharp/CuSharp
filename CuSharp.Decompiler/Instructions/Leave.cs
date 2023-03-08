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

namespace CuSharp.Decompiler.Instructions
{
    /// <summary>
    /// Unconditional branch. <c>goto target;</c>
    /// </summary>
    /// <remarks>
    /// When jumping to the entrypoint of the current block container, the branch represents a <c>continue</c> statement.
    /// 
    /// Phase-1 execution of a branch is a no-op.
    /// Phase-2 execution removes PopCount elements from the evaluation stack
    /// and jumps to the target block.
    /// </remarks>
    public sealed class Leave : ILInstruction, IBranchOrLeaveInstruction
    {
        public static readonly SlotInfo ValueSlot = new("Value", canInlineInto: true);
        private ILInstruction _value = null!;
        public ILInstruction Value
        {
            get => _value;
            set
            {
                ValidateChild(value);
                SetChildInstruction(ref _value, value, 0);
            }
        }
        protected override int GetChildCount()
        {
            return 1;
        }
        protected override ILInstruction GetChild(int index)
        {
            return index == 0 ? _value : throw new IndexOutOfRangeException();
        }
        protected override void SetChild(int index, ILInstruction value)
        {
            Value = index == 0 ? value : throw new IndexOutOfRangeException();
        }
        protected override SlotInfo GetChildSlot(int index)
        {
            return index == 0 ? ValueSlot : throw new IndexOutOfRangeException();
        }
        public override ILInstruction Clone()
        {
            var clone = (Leave)ShallowClone();
            clone.Value = _value.Clone();
            return clone;
        }
        public override StackType ResultType => StackType.Void;

        public override void AcceptVisitor(ILVisitor visitor)
        {
            visitor.VisitLeave(this);
        }
        public override T AcceptVisitor<T>(ILVisitor<T> visitor)
        {
            return visitor.VisitLeave(this);
        }

        private BlockContainer? _targetContainer;

        public Leave(BlockContainer? targetContainer, ILInstruction? value = null) : base(OpCode.Leave)
        {
            // Note: ILReader will create Leave instructions with targetContainer==null to represent 'endfinally',
            // the targetContainer will then be filled in by BlockBuilder
            _targetContainer = targetContainer;
            Value = value ?? new Nop();
        }

        protected override InstructionFlags ComputeFlags()
        {
            return _value.Flags | InstructionFlags.MayBranch | InstructionFlags.EndPointUnreachable;
        }

        public override InstructionFlags DirectFlags => InstructionFlags.MayBranch | InstructionFlags.EndPointUnreachable;

        public BlockContainer TargetContainer
        {
            get => _targetContainer!;
            set
            {
                if (_targetContainer != null && IsConnected)
                    _targetContainer.LeaveCount--;
                _targetContainer = value;
                if (_targetContainer != null && IsConnected)
                    _targetContainer.LeaveCount++;
            }
        }

        protected override void Connected()
        {
            base.Connected();
            if (_targetContainer != null)
                _targetContainer.LeaveCount++;
        }

        protected override void Disconnected()
        {
            base.Disconnected();
            if (_targetContainer != null)
                _targetContainer.LeaveCount--;
        }

        internal override void CheckInvariant(ILPhase phase)
        {
            base.CheckInvariant(phase);
            Debug.Assert(phase <= ILPhase.InILReader || IsDescendantOf(_targetContainer!));
            Debug.Assert(phase is <= ILPhase.InILReader or ILPhase.InAsyncAwait || _value.ResultType == _targetContainer!.ResultType);
        }
    }
}
