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

public sealed class StObj : ILInstruction, ISupportsVolatilePrefix, ISupportsUnalignedPrefix
{
    public StObj(ILInstruction target, ILInstruction value, Type type) : base(OpCode.StObj)
    {
        Target = target;
        Value = value;
        _type = type;
    }
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
        var clone = (StObj)ShallowClone();
        clone.Target = _target.Clone();
        clone.Value = _value.Clone();
        return clone;
    }

    private Type _type;
    /// <summary>Returns the type operand.</summary>
    public Type Type
    {
        get => _type;
        set { _type = value; InvalidateFlags(); }
    }
    /// <summary>Gets/Sets whether the memory access is volatile.</summary>
    public bool IsVolatile { get; set; }
    /// <summary>Returns the alignment specified by the 'unaligned' prefix; or 0 if there was no 'unaligned' prefix.</summary>
    public byte UnalignedPrefix { get; set; }
    public override StackType ResultType => UnalignedPrefix == 0 ? _type.GetStackType() : StackType.Void;

    protected override InstructionFlags ComputeFlags()
    {
        return _target.Flags | _value.Flags | InstructionFlags.SideEffect | InstructionFlags.MayThrow;
    }
    public override InstructionFlags DirectFlags => InstructionFlags.SideEffect | InstructionFlags.MayThrow;

    public override void AcceptVisitor(ILVisitor visitor)
    {
        visitor.VisitStObj(this);
    }
    public override T AcceptVisitor<T>(ILVisitor<T> visitor)
    {
        return visitor.VisitStObj(this);
    }

    internal override void CheckInvariant(ILPhase phase)
    {
        base.CheckInvariant(phase);
        DebugAssert(_target.ResultType == StackType.Ref || _target.ResultType == StackType.I);
        DebugAssert(_value.ResultType == _type.GetStackType());
        CheckTargetSlot();
    }

    /// <summary>
    /// called as part of CheckInvariant()
    /// </summary>
    void CheckTargetSlot()
    {
        switch (Target.OpCode)
        {
            case OpCode.LdElema:
            case OpCode.LdFlda:
                if (Target.HasDirectFlag(InstructionFlags.MayThrow))
                {
                    Debug.Assert(SemanticHelper.IsPure(Value.Flags));
                }
                break;
        }
    }
}