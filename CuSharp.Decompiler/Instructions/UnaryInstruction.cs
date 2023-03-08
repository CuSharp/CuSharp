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

namespace CuSharp.Decompiler.Instructions
{
    /// <summary>Instruction with a single argument</summary>
    public abstract class UnaryInstruction : ILInstruction
    {
        private ILInstruction _argument = null!;

        protected UnaryInstruction(OpCode opCode, ILInstruction argument) : base(opCode)
        {
            Argument = argument;
        }
        public static readonly SlotInfo ArgumentSlot = new("Argument", canInlineInto: true);
        public ILInstruction Argument
        {
            get => _argument;
            set
            {
                ValidateChild(value);
                SetChildInstruction(ref _argument, value, 0);
            }
        }
        protected override int GetChildCount()
        {
            return 1;
        }
        protected override ILInstruction GetChild(int index)
        {
            return index == 0 ? _argument : throw new IndexOutOfRangeException();
        }
        protected override void SetChild(int index, ILInstruction value)
        {
            Argument = index == 0 ? value : throw new IndexOutOfRangeException();
        }
        protected override SlotInfo GetChildSlot(int index)
        {
            return index == 0 ? ArgumentSlot : throw new IndexOutOfRangeException();
        }
        public override ILInstruction Clone()
        {
            var clone = (UnaryInstruction)ShallowClone();
            clone.Argument = _argument.Clone();
            return clone;
        }
        protected override InstructionFlags ComputeFlags()
        {
            return _argument.Flags | InstructionFlags.None;
        }
        public override InstructionFlags DirectFlags => InstructionFlags.None;
    }
}
