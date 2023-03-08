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

namespace CuSharp.Decompiler.Instructions
{
    /// <summary>If statement / conditional expression. <c>if (condition) trueExpr else falseExpr</c></summary>
    /// <remarks>
    /// The condition must return StackType.I4, use comparison instructions like Ceq to check if other types are non-zero.
    ///
    /// If the condition evaluates to non-zero, the TrueInst is executed.
    /// If the condition evaluates to zero, the FalseInst is executed.
    /// The return value of the IfInstruction is the return value of the TrueInst or FalseInst.
    /// 
    /// IfInstruction is also used to represent logical operators:
    ///   "a || b" ==> if (a) (ldc.i4 1) else (b)
    ///   "a &amp;&amp; b" ==> if (a) (b) else (ldc.i4 0)
    ///   "a ? b : c" ==> if (a) (b) else (c)
    /// </remarks>
    public sealed class IfInstruction : ILInstruction
    {
        public static readonly SlotInfo ConditionSlot = new("Condition", canInlineInto: true);
        private ILInstruction _condition = null!;
        public ILInstruction Condition
        {
            get => _condition;
            set
            {
                ValidateChild(value);
                SetChildInstruction(ref _condition, value, 0);
            }
        }
        public static readonly SlotInfo TrueInstSlot = new("TrueInst");
        private ILInstruction _trueInst = null!;
        public ILInstruction TrueInst
        {
            get => _trueInst;
            set
            {
                ValidateChild(value);
                SetChildInstruction(ref _trueInst, value, 1);
            }
        }
        public static readonly SlotInfo FalseInstSlot = new("FalseInst");
        private ILInstruction _falseInst = null!;
        public ILInstruction FalseInst
        {
            get => _falseInst;
            set
            {
                ValidateChild(value);
                SetChildInstruction(ref _falseInst, value, 2);
            }
        }
        protected override int GetChildCount()
        {
            return 3;
        }
        protected override ILInstruction GetChild(int index)
        {
            switch (index)
            {
                case 0:
                    return _condition;
                case 1:
                    return _trueInst;
                case 2:
                    return _falseInst;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
        protected override void SetChild(int index, ILInstruction value)
        {
            switch (index)
            {
                case 0:
                    Condition = value;
                    break;
                case 1:
                    TrueInst = value;
                    break;
                case 2:
                    FalseInst = value;
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
                    return ConditionSlot;
                case 1:
                    return TrueInstSlot;
                case 2:
                    return FalseInstSlot;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
        public override ILInstruction Clone()
        {
            var clone = (IfInstruction)ShallowClone();
            clone.Condition = _condition.Clone();
            clone.TrueInst = _trueInst.Clone();
            clone.FalseInst = _falseInst.Clone();
            return clone;
        }
        public override void AcceptVisitor(ILVisitor visitor)
        {
            visitor.VisitIfInstruction(this);
        }
        public override T AcceptVisitor<T>(ILVisitor<T> visitor)
        {
            return visitor.VisitIfInstruction(this);
        }

        public IfInstruction(ILInstruction condition, ILInstruction trueInst, ILInstruction? falseInst = null) : base(OpCode.IfInstruction)
        {
            Condition = condition;
            TrueInst = trueInst;
            FalseInst = falseInst ?? new Nop();
        }

        internal override void CheckInvariant(ILPhase phase)
        {
            base.CheckInvariant(phase);
            Debug.Assert(_condition.ResultType == StackType.I4);
            Debug.Assert(_trueInst.ResultType == _falseInst.ResultType
                         || _trueInst.HasDirectFlag(InstructionFlags.EndPointUnreachable)
                         || _falseInst.HasDirectFlag(InstructionFlags.EndPointUnreachable));
        }

        public override StackType ResultType => _trueInst.HasDirectFlag(InstructionFlags.EndPointUnreachable)
            ? _falseInst.ResultType
            : _trueInst.ResultType;

        public override InstructionFlags DirectFlags => InstructionFlags.ControlFlow;

        protected override InstructionFlags ComputeFlags()
        {
            return InstructionFlags.ControlFlow | _condition.Flags | SemanticHelper.CombineBranches(_trueInst.Flags, _falseInst.Flags);
        }

        /// <summary>
        /// Gets whether the input instruction occurs in a context where it is being compared with 0.
        /// </summary>
        internal static bool IsInConditionSlot(ILInstruction inst)
        {
            var slot = inst.SlotInfo;
            if (slot == ConditionSlot)
                return true;
            if (slot == TrueInstSlot || slot == FalseInstSlot || slot == NullCoalescingInstruction.FallbackInstSlot)
                return IsInConditionSlot(inst.Parent!);
            if (inst.Parent is Comp comp)
            {
                if (comp.Left == inst && comp.Right.MatchLdcI4(0))
                    return true;
                if (comp.Right == inst && comp.Left.MatchLdcI4(0))
                    return true;
            }
            return false;
        }
    }
}
