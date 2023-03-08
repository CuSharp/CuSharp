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

namespace CuSharp.Decompiler.Instructions
{
    /// <summary>Stores the value into an anonymous temporary variable, and returns the address of that variable.</summary>
    public sealed class AddressOf : ILInstruction
    {
        public AddressOf(ILInstruction value, Type type) : base(OpCode.AddressOf)
        {
            Value = value;
            _type = type;
        }
        public static readonly SlotInfo ValueSlot = new("Value", canInlineInto: true);
        private ILInstruction _value = null!;
        public ILInstruction Value
        {
            get => _value;
            set
            {
                ValidateChild(value);
                SetChildInstruction(ref _value, value, 0);
            }
        }
        protected override int GetChildCount()
        {
            return 1;
        }
        protected override ILInstruction GetChild(int index)
        {
            return index == 0 ? _value : throw new IndexOutOfRangeException();
        }
        protected override void SetChild(int index, ILInstruction value)
        {
            Value = index == 0 ? value : throw new IndexOutOfRangeException();
        }
        protected override SlotInfo GetChildSlot(int index)
        {
            return index == 0 ? ValueSlot : throw new IndexOutOfRangeException();
        }
        public override ILInstruction Clone()
        {
            var clone = (AddressOf)ShallowClone();
            clone.Value = _value.Clone();
            return clone;
        }
        public override StackType ResultType => StackType.Ref;
        private Type _type;
        /// <summary>Returns the type operand.</summary>
        public Type Type
        {
            get => _type;
            set { _type = value; InvalidateFlags(); }
        }
        protected override InstructionFlags ComputeFlags()
        {
            return _value.Flags;
        }
        public override InstructionFlags DirectFlags => InstructionFlags.None;

        public override void AcceptVisitor(ILVisitor visitor)
        {
            visitor.VisitAddressOf(this);
        }
        public override T AcceptVisitor<T>(ILVisitor<T> visitor)
        {
            return visitor.VisitAddressOf(this);
        }
    }
}