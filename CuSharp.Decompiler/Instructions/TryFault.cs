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

public sealed class TryFault : TryInstruction
{
    public override void AcceptVisitor(ILVisitor visitor)
    {
        visitor.VisitTryFault(this);
    }
    public override T AcceptVisitor<T>(ILVisitor<T> visitor)
    {
        return visitor.VisitTryFault(this);
    }

    public static readonly SlotInfo FaultBlockSlot = new("FaultBlock");

    public TryFault(ILInstruction tryBlock, ILInstruction faultBlock) : base(OpCode.TryFinally, tryBlock)
    {
        FaultBlock = faultBlock;
    }

    private ILInstruction _faultBlock = null!;
    public ILInstruction FaultBlock
    {
        get => _faultBlock;
        set
        {
            ValidateChild(value);
            SetChildInstruction(ref _faultBlock, value, 1);
        }
    }

    public override ILInstruction Clone()
    {
        return new TryFault(TryBlock.Clone(), _faultBlock.Clone()).WithILRange(this);
    }

    public override StackType ResultType => TryBlock.ResultType;

    protected override InstructionFlags ComputeFlags()
    {
        // The endpoint of the try-fault is unreachable iff the try endpoint is unreachable
        return TryBlock.Flags | _faultBlock.Flags & ~InstructionFlags.EndPointUnreachable | InstructionFlags.ControlFlow;
    }

    public override InstructionFlags DirectFlags => InstructionFlags.ControlFlow;

    protected override int GetChildCount()
    {
        return 2;
    }

    protected override ILInstruction GetChild(int index)
    {
        return index switch
        {
            0 => TryBlock,
            1 => _faultBlock,
            _ => throw new IndexOutOfRangeException()
        };
    }

    protected override void SetChild(int index, ILInstruction value)
    {
        switch (index)
        {
            case 0:
                TryBlock = value;
                break;
            case 1:
                FaultBlock = value;
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
                return TryBlockSlot;
            case 1:
                return FaultBlockSlot;
            default:
                throw new IndexOutOfRangeException();
        }
    }
}