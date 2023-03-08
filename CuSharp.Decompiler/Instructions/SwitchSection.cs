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

using Dotnet4Gpu.Decompilation.Util;

namespace Dotnet4Gpu.Decompilation.Instructions;

public sealed class SwitchSection : ILInstruction
{
    public static readonly SlotInfo BodySlot = new("Body");
    private ILInstruction _body = null!;
    public ILInstruction Body
    {
        get => _body;
        set
        {
            ValidateChild(value);
            SetChildInstruction(ref _body, value, 0);
        }
    }
    protected override int GetChildCount()
    {
        return 1;
    }
    protected override ILInstruction GetChild(int index)
    {
        return index == 0 ? _body : throw new IndexOutOfRangeException();
    }
    protected override void SetChild(int index, ILInstruction value)
    {
        Body = index == 0 ? value : throw new IndexOutOfRangeException();
    }
    protected override SlotInfo GetChildSlot(int index)
    {
        return index == 0 ? BodySlot : throw new IndexOutOfRangeException();
    }
    public override ILInstruction Clone()
    {
        var clone = (SwitchSection)ShallowClone();
        clone.Body = _body.Clone();
        return clone;
    }
    public override StackType ResultType => StackType.Void;

    public override void AcceptVisitor(ILVisitor visitor)
    {
        visitor.VisitSwitchSection(this);
    }
    public override T AcceptVisitor<T>(ILVisitor<T> visitor)
    {
        return visitor.VisitSwitchSection(this);
    }

    public SwitchSection()
        : base(OpCode.SwitchSection)
    {
        Labels = LongSet.Empty;
    }

    /// <summary>
    /// If true, serves as 'case null' in a lifted switch.
    /// </summary>
    public bool HasNullLabel { get; set; }

    /// <summary>
    /// The set of labels that cause execution to jump to this switch section.
    /// </summary>
    public LongSet Labels { get; set; }

    protected override InstructionFlags ComputeFlags()
    {
        return _body.Flags;
    }

    public override InstructionFlags DirectFlags => InstructionFlags.None;
}