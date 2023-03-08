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
    public sealed class NullableUnwrap : UnaryInstruction
    {
        protected override InstructionFlags ComputeFlags()
        {
            return base.ComputeFlags() | InstructionFlags.MayUnwrapNull;
        }
        public override InstructionFlags DirectFlags => base.DirectFlags | InstructionFlags.MayUnwrapNull;

        public override void AcceptVisitor(ILVisitor visitor)
        {
            visitor.VisitNullableUnwrap(this);
        }
        public override T AcceptVisitor<T>(ILVisitor<T> visitor)
        {
            return visitor.VisitNullableUnwrap(this);
        }

        /// <summary>
        /// Whether the argument is dereferenced before checking for a null input.
        /// If true, the argument must be a managed reference to a valid input type.
        /// </summary>
        /// <remarks>
        /// This mode exists because the C# compiler sometimes avoids copying the whole Nullable{T} struct
        /// before the null-check.
        /// The underlying struct T is still copied by the GetValueOrDefault() call, but only in the non-null case.
        /// </remarks>
        public readonly bool RefInput;

        public NullableUnwrap(StackType unwrappedType, ILInstruction argument, bool refInput = false)
            : base(OpCode.NullableUnwrap, argument)
        {
            ResultType = unwrappedType;
            RefInput = refInput;
            if (unwrappedType == StackType.Ref)
            {
                Debug.Assert(refInput);
            }
        }

        internal override void CheckInvariant(ILPhase phase)
        {
            base.CheckInvariant(phase);
            if (this.RefInput)
            {
                Debug.Assert(Argument.ResultType == StackType.Ref, "nullable.unwrap expects reference to nullable type as input");
            }
            else
            {
                Debug.Assert(Argument.ResultType == StackType.O, "nullable.unwrap expects nullable type as input");
            }
            Debug.Assert(Ancestors.Any(a => a is NullableRewrap));
        }

        public override StackType ResultType { get; }
    }
}
