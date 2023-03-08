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

using CuSharp.Decompiler;
using System.Diagnostics;

namespace CuSharp.Decompiler.Instructions
{
    public sealed class NullCoalescingInstruction : ILInstruction
    {
        public static readonly SlotInfo ValueInstSlot = new("ValueInst", canInlineInto: true);
        private ILInstruction _valueInst = null!;
        public ILInstruction ValueInst
        {
            get => _valueInst;
            set
            {
                ValidateChild(value);
                SetChildInstruction(ref _valueInst, value, 0);
            }
        }
        public static readonly SlotInfo FallbackInstSlot = new("FallbackInst");
        private ILInstruction _fallbackInst = null!;
        public ILInstruction FallbackInst
        {
            get => _fallbackInst;
            set
            {
                ValidateChild(value);
                SetChildInstruction(ref _fallbackInst, value, 1);
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
                    return _valueInst;
                case 1:
                    return _fallbackInst;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
        protected override void SetChild(int index, ILInstruction value)
        {
            switch (index)
            {
                case 0:
                    ValueInst = value;
                    break;
                case 1:
                    FallbackInst = value;
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
                    return ValueInstSlot;
                case 1:
                    return FallbackInstSlot;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
        public override ILInstruction Clone()
        {
            var clone = (NullCoalescingInstruction)ShallowClone();
            clone.ValueInst = _valueInst.Clone();
            clone.FallbackInst = _fallbackInst.Clone();
            return clone;
        }
        public override void AcceptVisitor(ILVisitor visitor)
        {
            visitor.VisitNullCoalescingInstruction(this);
        }
        public override T AcceptVisitor<T>(ILVisitor<T> visitor)
        {
            return visitor.VisitNullCoalescingInstruction(this);
        }

        public readonly NullCoalescingKind Kind;
        public StackType UnderlyingResultType = StackType.O;

        public NullCoalescingInstruction(NullCoalescingKind kind, ILInstruction valueInst, ILInstruction fallbackInst) : base(OpCode.NullCoalescingInstruction)
        {
            Kind = kind;
            ValueInst = valueInst;
            FallbackInst = fallbackInst;
        }

        internal override void CheckInvariant(ILPhase phase)
        {
            base.CheckInvariant(phase);
            Debug.Assert(_valueInst.ResultType == StackType.O); // lhs is reference type or nullable type
            Debug.Assert(_fallbackInst.ResultType == StackType.O || Kind == NullCoalescingKind.NullableWithValueFallback);
            Debug.Assert(ResultType == UnderlyingResultType || Kind == NullCoalescingKind.Nullable);
        }

        public override StackType ResultType => _fallbackInst.ResultType;

        public override InstructionFlags DirectFlags => InstructionFlags.ControlFlow;

        protected override InstructionFlags ComputeFlags()
        {
            // valueInst is always executed; fallbackInst only sometimes
            return InstructionFlags.ControlFlow | _valueInst.Flags
                                                | SemanticHelper.CombineBranches(InstructionFlags.None, _fallbackInst.Flags);
        }
    }
}
