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

using System.Linq.Expressions;

namespace CuSharp.Decompiler.Instructions;

public sealed class DynamicCompoundAssign : CompoundAssignmentInstruction
{
    public override StackType ResultType => StackType.O;

    protected override InstructionFlags ComputeFlags()
    {
        return base.ComputeFlags() | InstructionFlags.MayThrow | InstructionFlags.SideEffect;
    }
    public override InstructionFlags DirectFlags => base.DirectFlags | InstructionFlags.MayThrow | InstructionFlags.SideEffect;

    public override void AcceptVisitor(ILVisitor visitor)
    {
        visitor.VisitDynamicCompoundAssign(this);
    }
    public override T AcceptVisitor<T>(ILVisitor<T> visitor)
    {
        return visitor.VisitDynamicCompoundAssign(this);
    }

    public ExpressionType Operation { get; }

    public DynamicCompoundAssign(
        ExpressionType op,
        ILInstruction target,
        ILInstruction value,
        CompoundTargetKind targetKind = CompoundTargetKind.Dynamic)
        : base(OpCode.DynamicCompoundAssign, CompoundEvalModeFromOperation(op), target, targetKind, value)
    {
        if (!IsExpressionTypeSupported(op))
            throw new ArgumentOutOfRangeException(nameof(op));
        Operation = op;
    }

    internal static bool IsExpressionTypeSupported(ExpressionType type)
    {
        return type
            is ExpressionType.AddAssign
            or ExpressionType.AddAssignChecked
            or ExpressionType.AndAssign
            or ExpressionType.DivideAssign
            or ExpressionType.ExclusiveOrAssign
            or ExpressionType.LeftShiftAssign
            or ExpressionType.ModuloAssign
            or ExpressionType.MultiplyAssign
            or ExpressionType.MultiplyAssignChecked
            or ExpressionType.OrAssign
            or ExpressionType.PostDecrementAssign
            or ExpressionType.PostIncrementAssign
            or ExpressionType.PreDecrementAssign
            or ExpressionType.PreIncrementAssign
            or ExpressionType.RightShiftAssign
            or ExpressionType.SubtractAssign
            or ExpressionType.SubtractAssignChecked;
    }

    static CompoundEvalMode CompoundEvalModeFromOperation(ExpressionType op)
    {
        return op switch
        {
            ExpressionType.PostIncrementAssign => CompoundEvalMode.EvaluatesToOldValue,
            ExpressionType.PostDecrementAssign => CompoundEvalMode.EvaluatesToOldValue,
            _ => CompoundEvalMode.EvaluatesToNewValue
        };
    }
}