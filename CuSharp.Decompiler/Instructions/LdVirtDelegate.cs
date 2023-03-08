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

using System.Reflection;

namespace CuSharp.Decompiler.Instructions
{
    /// <summary>Virtual delegate construction</summary>
    public sealed class LdVirtDelegate : UnaryInstruction
    {
        public LdVirtDelegate(ILInstruction argument, Type type, MethodInfo method) : base(OpCode.LdVirtDelegate, argument)
        {
            _type = type;
            _method = method;
        }

        private readonly Type _type;
        private readonly MethodInfo _method;
        /// <summary>Returns the method operand.</summary>
        public override StackType ResultType => StackType.O;

        protected override InstructionFlags ComputeFlags()
        {
            return base.ComputeFlags() | InstructionFlags.MayThrow;
        }
        public override InstructionFlags DirectFlags => base.DirectFlags | InstructionFlags.MayThrow;

        public override void AcceptVisitor(ILVisitor visitor)
        {
            visitor.VisitLdVirtDelegate(this);
        }
        public override T AcceptVisitor<T>(ILVisitor<T> visitor)
        {
            return visitor.VisitLdVirtDelegate(this);
        }
    }
}