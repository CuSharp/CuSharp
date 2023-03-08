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

namespace CuSharp.Decompiler.Instructions;

public sealed class NullableRewrap : UnaryInstruction
{
    public NullableRewrap(ILInstruction argument) : base(OpCode.NullableRewrap, argument)
    {
    }

    public override void AcceptVisitor(ILVisitor visitor)
    {
        visitor.VisitNullableRewrap(this);
    }
    public override T AcceptVisitor<T>(ILVisitor<T> visitor)
    {
        return visitor.VisitNullableRewrap(this);
    }

    internal override void CheckInvariant(ILPhase phase)
    {
        base.CheckInvariant(phase);
        Debug.Assert(Argument.HasFlag(InstructionFlags.MayUnwrapNull));
    }

    public override InstructionFlags DirectFlags => InstructionFlags.ControlFlow;

    protected override InstructionFlags ComputeFlags()
    {
        // Convert MayUnwrapNull flag to ControlFlow flag.
        // Also, remove EndpointUnreachable flag, because the end-point is reachable through
        // the implicit nullable.unwrap branch.
        const InstructionFlags flagsToRemove = InstructionFlags.MayUnwrapNull | InstructionFlags.EndPointUnreachable;
        return Argument.Flags & ~flagsToRemove | InstructionFlags.ControlFlow;
    }

    public override StackType ResultType
    {
        get
        {
            if (Argument.ResultType == StackType.Void)
                return StackType.Void;
            else
                return StackType.O;
        }
    }

    internal override bool CanInlineIntoSlot(int childIndex, ILInstruction expressionBeingMoved)
    {
        // Inlining into nullable.rewrap is OK unless the expression being inlined
        // contains a nullable.wrap that isn't being re-wrapped within the expression being inlined.
        return base.CanInlineIntoSlot(childIndex, expressionBeingMoved)
               && !expressionBeingMoved.HasFlag(InstructionFlags.MayUnwrapNull);
    }
}