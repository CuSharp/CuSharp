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
using Dotnet4Gpu.Decompilation.Util;

namespace Dotnet4Gpu.Decompilation.Instructions
{
    public sealed class Comp : BinaryInstruction
    {
        public override void AcceptVisitor(ILVisitor visitor)
        {
            visitor.VisitComp(this);
        }
        public override T AcceptVisitor<T>(ILVisitor<T> visitor)
        {
            return visitor.VisitComp(this);
        }

        private ComparisonKind _kind;

        public ComparisonKind Kind {
            get => _kind;
            set {
                _kind = value;
                MakeDirty();
            }
        }

        public readonly ComparisonLiftingKind LiftingKind;

        /// <summary>
        /// Gets the stack type of the comparison inputs.
        /// For lifted comparisons, this is the underlying input type.
        /// </summary>
        public StackType InputType;

        /// <summary>
        /// If this is an integer comparison, specifies the sign used to interpret the integers.
        /// </summary>
        public readonly Sign Sign;

        public Comp(ComparisonKind kind, Sign sign, ILInstruction left, ILInstruction right) : base(OpCode.Comp, left, right)
        {
            _kind = kind;
            LiftingKind = ComparisonLiftingKind.None;
            InputType = left.ResultType;
            Sign = sign;
            Debug.Assert(left.ResultType == right.ResultType);
        }

        public Comp(ComparisonKind kind, ComparisonLiftingKind lifting, StackType inputType, Sign sign, ILInstruction left, ILInstruction right) : base(OpCode.Comp, left, right)
        {
            _kind = kind;
            LiftingKind = lifting;
            InputType = inputType;
            Sign = sign;
        }

        public override StackType ResultType => LiftingKind == ComparisonLiftingKind.ThreeValuedLogic ? StackType.O : StackType.I4;

        internal override void CheckInvariant(ILPhase phase)
        {
            base.CheckInvariant(phase);
            if (LiftingKind == ComparisonLiftingKind.None)
            {
                Debug.Assert(Left.ResultType == InputType);
                Debug.Assert(Right.ResultType == InputType);
            }
            else
            {
                Debug.Assert(Left.ResultType == InputType || Left.ResultType == StackType.O);
                Debug.Assert(Right.ResultType == InputType || Right.ResultType == StackType.O);
            }
        }

        public static Comp LogicNot(ILInstruction arg)
        {
            return new Comp(ComparisonKind.Equality, Util.Sign.None, arg, new LdcI4(0));
        }
    }
}
