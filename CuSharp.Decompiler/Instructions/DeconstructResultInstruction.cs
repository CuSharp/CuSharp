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
    public sealed class DeconstructResultInstruction : UnaryInstruction
    {
        public override void AcceptVisitor(ILVisitor visitor)
        {
            visitor.VisitDeconstructResultInstruction(this);
        }
        public override T AcceptVisitor<T>(ILVisitor<T> visitor)
        {
            return visitor.VisitDeconstructResultInstruction(this);
        }

        internal override void CheckInvariant(ILPhase phase)
        {
            base.CheckInvariant(phase);
            AdditionalInvariants();
        }

        public int Index { get; }

        public override StackType ResultType { get; }

        public DeconstructResultInstruction(int index, StackType resultType, ILInstruction argument)
            : base(OpCode.DeconstructResultInstruction, argument)
        {
            Debug.Assert(index >= 0);
            Index = index;
            ResultType = resultType;
        }

        MatchInstruction? FindMatch()
        {
            for (ILInstruction? inst = this; inst != null; inst = inst.Parent)
            {
                if (inst.Parent is MatchInstruction match && inst != match.TestedOperand)
                    return match;
            }
            return null;
        }

        void AdditionalInvariants()
        {
            var matchInst = FindMatch();
            DebugAssert(matchInst != null && (matchInst.IsDeconstructCall || matchInst.IsDeconstructTuple));
            DebugAssert(Argument.MatchLdLoc(matchInst.Variable));
            var outParamType = matchInst.GetDeconstructResultType(this.Index);
            DebugAssert(outParamType.GetStackType() == ResultType);
        }
    }
}
