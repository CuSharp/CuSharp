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

namespace Dotnet4Gpu.Decompilation.Instructions;

public sealed class DynamicUnaryOperatorInstruction : DynamicInstruction
{
    public static readonly SlotInfo OperandSlot = new("Operand", canInlineInto: true);
    private ILInstruction _operand = null!;
    public ILInstruction Operand
    {
        get => _operand;
        set
        {
            ValidateChild(value);
            SetChildInstruction(ref _operand, value, 0);
        }
    }
    protected override int GetChildCount()
    {
        return 1;
    }
    protected override ILInstruction GetChild(int index)
    {
        return index == 0 ? _operand : throw new IndexOutOfRangeException();
    }
    protected override void SetChild(int index, ILInstruction value)
    {
        Operand = index == 0 ? value : throw new IndexOutOfRangeException();
    }
    protected override SlotInfo GetChildSlot(int index)
    {
        return index == 0 ? OperandSlot : throw new IndexOutOfRangeException();
    }
    public override ILInstruction Clone()
    {
        var clone = (DynamicUnaryOperatorInstruction)ShallowClone();
        clone.Operand = _operand.Clone();
        return clone;
    }
    protected override InstructionFlags ComputeFlags()
    {
        return base.ComputeFlags() | InstructionFlags.MayThrow | InstructionFlags.SideEffect | _operand.Flags;
    }
    public override InstructionFlags DirectFlags => base.DirectFlags | InstructionFlags.MayThrow | InstructionFlags.SideEffect;

    public override void AcceptVisitor(ILVisitor visitor)
    {
        visitor.VisitDynamicUnaryOperatorInstruction(this);
    }
    public override T AcceptVisitor<T>(ILVisitor<T> visitor)
    {
        return visitor.VisitDynamicUnaryOperatorInstruction(this);
    }

    public ExpressionType Operation { get; }

    public DynamicUnaryOperatorInstruction(ExpressionType operation, ILInstruction operand)
        : base(OpCode.DynamicUnaryOperatorInstruction)
    {
        Operation = operation;
        Operand = operand;
    }

    public override StackType ResultType {
        get {
            switch (Operation)
            {
                case ExpressionType.IsFalse:
                case ExpressionType.IsTrue:
                    return StackType.I4; // bool
                default:
                    return StackType.O;
            }
        }
    }
}