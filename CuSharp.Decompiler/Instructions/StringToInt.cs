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
    public sealed class StringToInt : ILInstruction
    {
        public static readonly SlotInfo ArgumentSlot = new("Argument", canInlineInto: true);
        private ILInstruction _argument = null!;
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
            var clone = (StringToInt)ShallowClone();
            clone.Argument = _argument.Clone();
            return clone;
        }
        public override StackType ResultType => StackType.I4;

        protected override InstructionFlags ComputeFlags()
        {
            return _argument.Flags;
        }
        public override InstructionFlags DirectFlags => InstructionFlags.None;

        public override void AcceptVisitor(ILVisitor visitor)
        {
            visitor.VisitStringToInt(this);
        }
        public override T AcceptVisitor<T>(ILVisitor<T> visitor)
        {
            return visitor.VisitStringToInt(this);
        }

        internal override void CheckInvariant(ILPhase phase)
        {
            base.CheckInvariant(phase);
            DebugAssert(_argument.ResultType == StackType.O);
        }

        public List<(string? Key, int Value)> Map { get; }

        public StringToInt(ILInstruction argument, List<(string? Key, int Value)> map)
            : base(OpCode.StringToInt)
        {
            Argument = argument;
            Map = map;
        }

        public StringToInt(ILInstruction argument, string?[] map)
            : this(argument, ArrayToDictionary(map))
        {
        }

        static List<(string? Key, int Value)> ArrayToDictionary(string?[] map)
        {
            var dict = new List<(string? Key, int Value)>();
            for (int i = 0; i < map.Length; i++)
            {
                dict.Add((map[i], i));
            }
            return dict;
        }
    }
}
