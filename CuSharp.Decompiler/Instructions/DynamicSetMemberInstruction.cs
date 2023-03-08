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

public sealed class DynamicSetMemberInstruction : DynamicInstruction
{
    public static readonly SlotInfo TargetSlot = new("Target", canInlineInto: true);
    private ILInstruction _target = null!;
    public ILInstruction Target
    {
        get => _target;
        set
        {
            ValidateChild(value);
            SetChildInstruction(ref _target, value, 0);
        }
    }
    public static readonly SlotInfo ValueSlot = new("Value", canInlineInto: true);
    private ILInstruction _value = null!;
    public ILInstruction Value
    {
        get => _value;
        set
        {
            ValidateChild(value);
            SetChildInstruction(ref _value, value, 1);
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
                return _target;
            case 1:
                return _value;
            default:
                throw new IndexOutOfRangeException();
        }
    }
    protected override void SetChild(int index, ILInstruction value)
    {
        switch (index)
        {
            case 0:
                Target = value;
                break;
            case 1:
                Value = value;
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
                return TargetSlot;
            case 1:
                return ValueSlot;
            default:
                throw new IndexOutOfRangeException();
        }
    }
    public override ILInstruction Clone()
    {
        var clone = (DynamicSetMemberInstruction)ShallowClone();
        clone.Target = _target.Clone();
        clone.Value = _value.Clone();
        return clone;
    }
    protected override InstructionFlags ComputeFlags()
    {
        return base.ComputeFlags() | InstructionFlags.MayThrow | InstructionFlags.SideEffect | _target.Flags | _value.Flags;
    }
    public override InstructionFlags DirectFlags => base.DirectFlags | InstructionFlags.MayThrow | InstructionFlags.SideEffect;

    public override void AcceptVisitor(ILVisitor visitor)
    {
        visitor.VisitDynamicSetMemberInstruction(this);
    }
    public override T AcceptVisitor<T>(ILVisitor<T> visitor)
    {
        return visitor.VisitDynamicSetMemberInstruction(this);
    }

    internal override void CheckInvariant(ILPhase phase)
    {
        base.CheckInvariant(phase);
        DebugAssert(_target.ResultType == StackType.O);
    }

    public DynamicSetMemberInstruction(ILInstruction target, ILInstruction value)
        : base(OpCode.DynamicSetMemberInstruction)
    {
        Target = target;
        Value = value;
    }

    public override StackType ResultType => StackType.O;
}