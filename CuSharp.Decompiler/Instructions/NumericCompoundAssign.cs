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
using CuSharp.Decompiler;
using CuSharp.Decompiler.Util;

namespace CuSharp.Decompiler.Instructions;

public sealed class NumericCompoundAssign : CompoundAssignmentInstruction
{
    private readonly Type _type;
    public override StackType ResultType => _type.GetStackType();

    public override void AcceptVisitor(ILVisitor visitor)
    {
        visitor.VisitNumericCompoundAssign(this);
    }
    public override T AcceptVisitor<T>(ILVisitor<T> visitor)
    {
        return visitor.VisitNumericCompoundAssign(this);
    }

    /// <summary>
    /// Gets whether the instruction checks for overflow.
    /// </summary>
    public readonly bool CheckForOverflow;

    /// <summary>
    /// For integer operations that depend on the sign, specifies whether the operation
    /// is signed or unsigned.
    /// For instructions that produce the same result for either sign, returns Sign.None.
    /// </summary>
    public readonly Sign Sign;

    public readonly StackType LeftInputType;
    public readonly StackType RightInputType;
    public StackType UnderlyingResultType { get; }

    /// <summary>
    /// The operator used by this assignment operator instruction.
    /// </summary>
    public readonly BinaryNumericOperator Operator;

    public bool IsLifted { get; }

    public NumericCompoundAssign(BinaryNumericInstruction binary, ILInstruction target,
        CompoundTargetKind targetKind, ILInstruction value, Type type, CompoundEvalMode evalMode)
        : base(OpCode.NumericCompoundAssign, evalMode, target, targetKind, value)
    {
        CheckForOverflow = binary.CheckForOverflow;
        Sign = binary.Sign;
        LeftInputType = binary.LeftInputType;
        RightInputType = binary.RightInputType;
        UnderlyingResultType = binary.UnderlyingResultType;
        Operator = binary.Operator;
        IsLifted = binary.IsLifted;
        _type = type;
        AddILRange(binary);
        Debug.Assert(evalMode == CompoundEvalMode.EvaluatesToNewValue ||
                     Operator == BinaryNumericOperator.Add || Operator == BinaryNumericOperator.Sub);
        Debug.Assert(ResultType == (IsLifted ? StackType.O : UnderlyingResultType));
    }

    protected override InstructionFlags ComputeFlags()
    {
        var flags = Target.Flags | Value.Flags | InstructionFlags.SideEffect;
        if (CheckForOverflow || Operator == BinaryNumericOperator.Div || Operator == BinaryNumericOperator.Rem)
            flags |= InstructionFlags.MayThrow;
        return flags;
    }

    public override InstructionFlags DirectFlags
    {
        get
        {
            var flags = InstructionFlags.SideEffect;
            if (CheckForOverflow ||
                Operator == BinaryNumericOperator.Div || Operator == BinaryNumericOperator.Rem)
                flags |= InstructionFlags.MayThrow;
            return flags;
        }
    }
}