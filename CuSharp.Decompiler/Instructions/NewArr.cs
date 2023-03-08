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

namespace Dotnet4Gpu.Decompilation.Instructions
{
    /// <summary>Creates an array instance.</summary>
    public sealed class NewArr : ILInstruction
    {
        public NewArr(Type type, params ILInstruction[] indices) : base(OpCode.NewArr)
        {
            _type = type;
            Indices = new InstructionCollection<ILInstruction>(this, 0);
            Indices.AddRange(indices);
        }

        private Type _type;
        /// <summary>Returns the type operand.</summary>
        public Type Type
        {
            get => _type;
            set { _type = value; InvalidateFlags(); }
        }
        public static readonly SlotInfo IndicesSlot = new("Indices", canInlineInto: true);
        public InstructionCollection<ILInstruction> Indices { get; private set; }
        protected override int GetChildCount()
        {
            return Indices.Count;
        }
        protected override ILInstruction GetChild(int index)
        {
            return Indices[index - 0];
        }
        protected override void SetChild(int index, ILInstruction value)
        {
            Indices[index - 0] = value;
        }
        protected override SlotInfo GetChildSlot(int index)
        {
            return IndicesSlot;
        }
        public override ILInstruction Clone()
        {
            var clone = (NewArr)ShallowClone();
            clone.Indices = new InstructionCollection<ILInstruction>(clone, 0);
            clone.Indices.AddRange(Indices.Select(arg => arg.Clone()));
            return clone;
        }
        public override StackType ResultType => StackType.O;

        protected override InstructionFlags ComputeFlags()
        {
            return Indices.Aggregate(InstructionFlags.None, (f, arg) => f | arg.Flags) | InstructionFlags.MayThrow;
        }
        public override InstructionFlags DirectFlags => InstructionFlags.MayThrow;

        public override void AcceptVisitor(ILVisitor visitor)
        {
            visitor.VisitNewArr(this);
        }
        public override T AcceptVisitor<T>(ILVisitor<T> visitor)
        {
            return visitor.VisitNewArr(this);
        }
    }
}