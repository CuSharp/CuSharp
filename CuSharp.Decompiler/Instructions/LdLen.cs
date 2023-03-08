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

namespace Dotnet4Gpu.Decompilation.Instructions
{
    public sealed class LdLen : ILInstruction
    {
        public static readonly SlotInfo ArraySlot = new("Array", canInlineInto: true);
        private ILInstruction _array = null!;
        public ILInstruction Array
        {
            get => _array;
            set
            {
                ValidateChild(value);
                SetChildInstruction(ref _array, value, 0);
            }
        }
        protected override int GetChildCount()
        {
            return 1;
        }
        protected override ILInstruction GetChild(int index)
        {
            return index == 0 ? _array : throw new IndexOutOfRangeException();
        }
        protected override void SetChild(int index, ILInstruction value)
        {
            Array = index == 0 ? value : throw new IndexOutOfRangeException();
        }
        protected override SlotInfo GetChildSlot(int index)
        {
            return index == 0 ? ArraySlot : throw new IndexOutOfRangeException();
        }
        public override ILInstruction Clone()
        {
            var clone = (LdLen)ShallowClone();
            clone.Array = _array.Clone();
            return clone;
        }
        protected override InstructionFlags ComputeFlags()
        {
            return _array.Flags | InstructionFlags.MayThrow;
        }
        public override InstructionFlags DirectFlags => InstructionFlags.MayThrow;

        public override void AcceptVisitor(ILVisitor visitor)
        {
            visitor.VisitLdLen(this);
        }
        public override T AcceptVisitor<T>(ILVisitor<T> visitor)
        {
            return visitor.VisitLdLen(this);
        }

        internal override void CheckInvariant(ILPhase phase)
        {
            base.CheckInvariant(phase);
            DebugAssert(_array.ResultType == StackType.O);
        }

        public LdLen(StackType type, ILInstruction array) : base(OpCode.LdLen)
        {
            Debug.Assert(type is StackType.I or StackType.I4 or StackType.I8);
            ResultType = type;
            Array = array;
        }

        public override StackType ResultType { get; }
    }
}
