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

using CuSharp.Decompiler;

namespace CuSharp.Decompiler.Instructions;

public sealed class DynamicBinaryOperatorInstruction : DynamicInstruction
{
    public static readonly SlotInfo LeftSlot = new("Left", canInlineInto: true);
    private ILInstruction _left = null!;
    public ILInstruction Left
    {
        get => _left;
        set
        {
            ValidateChild(value);
            SetChildInstruction(ref _left, value, 0);
        }
    }
    public static readonly SlotInfo RightSlot = new("Right", canInlineInto: true);
    private ILInstruction _right = null!;
    public ILInstruction Right
    {
        get => _right;
        set
        {
            ValidateChild(value);
            SetChildInstruction(ref _right, value, 1);
        }
    }
    protected override int GetChildCount()
    {
        return 2;
    }
    protected override ILInstruction GetChild(int index)
    {
        switch (index)
        {
            case 0:
                return _left;
            case 1:
                return _right;
            default:
                throw new IndexOutOfRangeException();
        }
    }
    protected override void SetChild(int index, ILInstruction value)
    {
        switch (index)
        {
            case 0:
                Left = value;
                break;
            case 1:
                Right = value;
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
                return LeftSlot;
            case 1:
                return RightSlot;
            default:
                throw new IndexOutOfRangeException();
        }
    }
    public override ILInstruction Clone()
    {
        var clone = (DynamicBinaryOperatorInstruction)ShallowClone();
        clone.Left = _left.Clone();
        clone.Right = _right.Clone();
        return clone;
    }
    protected override InstructionFlags ComputeFlags()
    {
        return base.ComputeFlags() | InstructionFlags.MayThrow | InstructionFlags.SideEffect | _left.Flags | _right.Flags;
    }
    public override InstructionFlags DirectFlags => base.DirectFlags | InstructionFlags.MayThrow | InstructionFlags.SideEffect;

    public override void AcceptVisitor(ILVisitor visitor)
    {
        visitor.VisitDynamicBinaryOperatorInstruction(this);
    }
    public override T AcceptVisitor<T>(ILVisitor<T> visitor)
    {
        return visitor.VisitDynamicBinaryOperatorInstruction(this);
    }

    public DynamicBinaryOperatorInstruction(ILInstruction left, ILInstruction right)
        : base(OpCode.DynamicBinaryOperatorInstruction)
    {
        Left = left;
        Right = right;
    }

    public override StackType ResultType => StackType.O;
}