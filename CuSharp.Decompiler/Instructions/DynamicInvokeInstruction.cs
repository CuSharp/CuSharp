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

public sealed class DynamicInvokeInstruction : DynamicInstruction
{
    public static readonly SlotInfo ArgumentsSlot = new("Arguments", canInlineInto: true);
    public InstructionCollection<ILInstruction> Arguments { get; private set; }
    protected override int GetChildCount()
    {
        return Arguments.Count;
    }
    protected override ILInstruction GetChild(int index)
    {
        return Arguments[index - 0];
    }
    protected override void SetChild(int index, ILInstruction value)
    {
        Arguments[index - 0] = value;
    }
    protected override SlotInfo GetChildSlot(int index)
    {
        return ArgumentsSlot;
    }
    public override ILInstruction Clone()
    {
        var clone = (DynamicInvokeInstruction)ShallowClone();
        clone.Arguments = new InstructionCollection<ILInstruction>(clone, 0);
        clone.Arguments.AddRange(Arguments.Select(arg => arg.Clone()));
        return clone;
    }
    protected override InstructionFlags ComputeFlags()
    {
        return base.ComputeFlags() | InstructionFlags.MayThrow | InstructionFlags.SideEffect | Arguments.Aggregate(InstructionFlags.None, (f, arg) => f | arg.Flags);
    }
    public override InstructionFlags DirectFlags => base.DirectFlags | InstructionFlags.MayThrow | InstructionFlags.SideEffect;

    public override void AcceptVisitor(ILVisitor visitor)
    {
        visitor.VisitDynamicInvokeInstruction(this);
    }
    public override T AcceptVisitor<T>(ILVisitor<T> visitor)
    {
        return visitor.VisitDynamicInvokeInstruction(this);
    }

    public DynamicInvokeInstruction(ILInstruction[] arguments)
        : base(OpCode.DynamicInvokeInstruction)
    {
        Arguments = new InstructionCollection<ILInstruction>(this, 0);
        Arguments.AddRange(arguments);
    }

    public override StackType ResultType => StackType.O;
}