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
    /// <summary>Rethrows the current exception.</summary>
    public sealed class Rethrow : SimpleInstruction
    {
        public Rethrow() : base(OpCode.Rethrow)
        {
        }
        public override StackType ResultType => StackType.Void;

        protected override InstructionFlags ComputeFlags()
        {
            return InstructionFlags.MayThrow | InstructionFlags.EndPointUnreachable;
        }
        public override InstructionFlags DirectFlags => InstructionFlags.MayThrow | InstructionFlags.EndPointUnreachable;

        public override void AcceptVisitor(ILVisitor visitor)
        {
            visitor.VisitRethrow(this);
        }
        public override T AcceptVisitor<T>(ILVisitor<T> visitor)
        {
            return visitor.VisitRethrow(this);
        }
    }
}