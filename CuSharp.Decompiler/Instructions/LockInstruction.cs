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

namespace Dotnet4Gpu.Decompilation.Instructions
{
    /// <summary>Lock statement</summary>
    public sealed class LockInstruction : ILInstruction
    {
        public LockInstruction(ILInstruction onExpression, ILInstruction body) : base(OpCode.LockInstruction)
        {
            OnExpression = onExpression;
            Body = body;
        }
        public static readonly SlotInfo OnExpressionSlot = new("OnExpression", canInlineInto: true);
        private ILInstruction _onExpression = null!;
        public ILInstruction OnExpression
        {
            get => _onExpression;
            set
            {
                ValidateChild(value);
                SetChildInstruction(ref _onExpression, value, 0);
            }
        }
        public static readonly SlotInfo BodySlot = new("Body");
        private ILInstruction _body = null!;
        public ILInstruction Body
        {
            get => _body;
            set
            {
                ValidateChild(value);
                SetChildInstruction(ref _body, value, 1);
            }
        }
        protected override int GetChildCount()
        {
            return 2;
        }
        protected override ILInstruction GetChild(int index)
        {
            switch (index)
            {
                case 0:
                    return _onExpression;
                case 1:
                    return _body;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
        protected override void SetChild(int index, ILInstruction value)
        {
            switch (index)
            {
                case 0:
                    OnExpression = value;
                    break;
                case 1:
                    Body = value;
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
        protected override SlotInfo GetChildSlot(int index)
        {
            switch (index)
            {
                case 0:
                    return OnExpressionSlot;
                case 1:
                    return BodySlot;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
        public override ILInstruction Clone()
        {
            var clone = (LockInstruction)ShallowClone();
            clone.OnExpression = _onExpression.Clone();
            clone.Body = _body.Clone();
            return clone;
        }
        public override StackType ResultType => StackType.Void;

        protected override InstructionFlags ComputeFlags()
        {
            return _onExpression.Flags | _body.Flags | InstructionFlags.ControlFlow | InstructionFlags.SideEffect;
        }
        public override InstructionFlags DirectFlags => InstructionFlags.ControlFlow | InstructionFlags.SideEffect;

        public override void AcceptVisitor(ILVisitor visitor)
        {
            visitor.VisitLockInstruction(this);
        }
        public override T AcceptVisitor<T>(ILVisitor<T> visitor)
        {
            return visitor.VisitLockInstruction(this);
        }

        internal override void CheckInvariant(ILPhase phase)
        {
            base.CheckInvariant(phase);
            DebugAssert(_onExpression.ResultType == StackType.O);
        }
    }
}