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

namespace CuSharp.Decompiler.Instructions;

public sealed class TryCatch : TryInstruction
{
    public override void AcceptVisitor(ILVisitor visitor)
    {
        visitor.VisitTryCatch(this);
    }
    public override T AcceptVisitor<T>(ILVisitor<T> visitor)
    {
        return visitor.VisitTryCatch(this);
    }

    public static readonly SlotInfo HandlerSlot = new("Handler", isCollection: true);
    public readonly InstructionCollection<TryCatchHandler> Handlers;

    public TryCatch(ILInstruction tryBlock) : base(OpCode.TryCatch, tryBlock)
    {
        Handlers = new InstructionCollection<TryCatchHandler>(this, 1);
    }

    public override ILInstruction Clone()
    {
        var clone = new TryCatch(TryBlock.Clone());
        clone.AddILRange(this);
        clone.Handlers.AddRange(Handlers.Select(h => (TryCatchHandler)h.Clone()));
        return clone;
    }

    public override StackType ResultType => StackType.Void;

    protected override InstructionFlags ComputeFlags()
    {
        var flags = TryBlock.Flags;
        foreach (var handler in Handlers)
            flags = SemanticHelper.CombineBranches(flags, handler.Flags);
        return flags | InstructionFlags.ControlFlow;
    }

    public override InstructionFlags DirectFlags => InstructionFlags.ControlFlow;

    protected override int GetChildCount()
    {
        return 1 + Handlers.Count;
    }

    protected override ILInstruction GetChild(int index)
    {
        return index == 0
            ? TryBlock
            : Handlers[index - 1];
    }

    protected override void SetChild(int index, ILInstruction value)
    {
        if (index == 0)
            TryBlock = value;
        else
            Handlers[index - 1] = (TryCatchHandler)value;
    }

    protected override SlotInfo GetChildSlot(int index)
    {
        return index == 0
            ? TryBlockSlot
            : HandlerSlot;
    }
}