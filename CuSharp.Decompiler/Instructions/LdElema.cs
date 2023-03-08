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
    /// <summary>Load address of array element.</summary>
    public sealed class LdElema : ILInstruction
    {
        public LdElema(Type type, ILInstruction array, params ILInstruction[] indices) : base(OpCode.LdElema)
        {
            _type = type;
            Array = array;
            Indices = new InstructionCollection<ILInstruction>(this, 1);
            Indices.AddRange(indices);
        }
        private Type _type;
        /// <summary>Returns the type operand.</summary>
        public Type Type
        {
            get => _type;
            set { _type = value; InvalidateFlags(); }
        }
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
        public static readonly SlotInfo IndicesSlot = new("Indices", canInlineInto: true);
        public InstructionCollection<ILInstruction> Indices { get; private set; }
        protected override int GetChildCount()
        {
            return 1 + Indices.Count;
        }
        protected override ILInstruction GetChild(int index)
        {
            switch (index)
            {
                case 0:
                    return _array;
                default:
                    return Indices[index - 1];
            }
        }
        protected override void SetChild(int index, ILInstruction value)
        {
            switch (index)
            {
                case 0:
                    Array = value;
                    break;
                default:
                    Indices[index - 1] = value;
                    break;
            }
        }
        protected override SlotInfo GetChildSlot(int index)
        {
            switch (index)
            {
                case 0:
                    return ArraySlot;
                default:
                    return IndicesSlot;
            }
        }
        public override ILInstruction Clone()
        {
            var clone = (LdElema)ShallowClone();
            clone.Array = _array.Clone();
            clone.Indices = new InstructionCollection<ILInstruction>(clone, 1);
            clone.Indices.AddRange(Indices.Select(arg => arg.Clone()));
            return clone;
        }
        public bool WithSystemIndex;
        public bool DelayExceptions; // NullReferenceException/IndexOutOfBoundsException only occurs when the reference is dereferenced
        public override StackType ResultType => StackType.Ref;

        /// <summary>Gets whether the 'readonly' prefix was applied to this instruction.</summary>
        public bool IsReadOnly { get; set; }
        protected override InstructionFlags ComputeFlags()
        {
            return _array.Flags | Indices.Aggregate(InstructionFlags.None, (f, arg) => f | arg.Flags) | (DelayExceptions ? InstructionFlags.None : InstructionFlags.MayThrow);
        }
        public override InstructionFlags DirectFlags => DelayExceptions ? InstructionFlags.None : InstructionFlags.MayThrow;

        public override void AcceptVisitor(ILVisitor visitor)
        {
            visitor.VisitLdElema(this);
        }
        public override T AcceptVisitor<T>(ILVisitor<T> visitor)
        {
            return visitor.VisitLdElema(this);
        }
    }
}