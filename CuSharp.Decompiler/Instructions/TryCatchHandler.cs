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

public sealed class TryCatchHandler : ILInstruction, IStoreInstruction
{
    public TryCatchHandler(ILInstruction filter, ILInstruction body, ILVariable variable) : base(OpCode.TryCatchHandler)
    {
        Filter = filter;
        Body = body;
        _variable = variable ?? throw new ArgumentNullException(nameof(variable));
    }
    public static readonly SlotInfo FilterSlot = new("Filter");
    private ILInstruction _filter = null!;
    public ILInstruction Filter
    {
        get => _filter;
        set
        {
            ValidateChild(value);
            SetChildInstruction(ref _filter, value, 0);
        }
    }
    public static readonly SlotInfo BodySlot = new("Body");
    private ILInstruction _body = null!;
    public ILInstruction Body
    {
        get => _body;
        set
        {
            ValidateChild(value);
            SetChildInstruction(ref _body, value, 1);
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
                return _filter;
            case 1:
                return _body;
            default:
                throw new IndexOutOfRangeException();
        }
    }
    protected override void SetChild(int index, ILInstruction value)
    {
        switch (index)
        {
            case 0:
                Filter = value;
                break;
            case 1:
                Body = value;
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
                return FilterSlot;
            case 1:
                return BodySlot;
            default:
                throw new IndexOutOfRangeException();
        }
    }
    public override ILInstruction Clone()
    {
        var clone = (TryCatchHandler)ShallowClone();
        clone.Filter = _filter.Clone();
        clone.Body = _body.Clone();
        return clone;
    }

    private ILVariable _variable;
    public ILVariable Variable
    {
        get => _variable;
        set
        {
            DebugAssert(value != null);
            if (IsConnected)
                _variable.RemoveStoreInstruction(this);
            _variable = value;
            if (IsConnected)
                _variable.AddStoreInstruction(this);
        }
    }

    public int IndexInStoreInstructionList { get; set; } = -1;

    int IInstructionWithVariableOperand.IndexInVariableInstructionMapping
    {
        get => IndexInStoreInstructionList;
        set => IndexInStoreInstructionList = value;
    }

    protected override void Connected()
    {
        base.Connected();
        _variable.AddStoreInstruction(this);
    }

    protected override void Disconnected()
    {
        _variable.RemoveStoreInstruction(this);
        base.Disconnected();
    }

    public override void AcceptVisitor(ILVisitor visitor)
    {
        visitor.VisitTryCatchHandler(this);
    }
    public override T AcceptVisitor<T>(ILVisitor<T> visitor)
    {
        return visitor.VisitTryCatchHandler(this);
    }

    internal override void CheckInvariant(ILPhase phase)
    {
        base.CheckInvariant(phase);
        Debug.Assert(Parent is TryCatch);
        Debug.Assert(_filter.ResultType == StackType.I4);
        Debug.Assert(IsDescendantOf(_variable.Function!));
    }

    public override StackType ResultType => StackType.Void;

    protected override InstructionFlags ComputeFlags()
    {
        return _filter.Flags | _body.Flags | InstructionFlags.ControlFlow | InstructionFlags.MayWriteLocals;
    }

    public override InstructionFlags DirectFlags
    {
        get
        {
            // the body is not evaluated if the filter returns 0
            return InstructionFlags.ControlFlow | InstructionFlags.MayWriteLocals;
        }
    }
}