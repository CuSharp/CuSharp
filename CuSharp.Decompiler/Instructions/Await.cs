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

using System.Reflection;
using CuSharp.Decompiler;
using CuSharp.Decompiler.Util;

namespace CuSharp.Decompiler.Instructions
{
    public sealed class Await : ILInstruction
    {
        public Await(ILInstruction value) : base(OpCode.Await)
        {
            Value = value;
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
            var clone = (Await)ShallowClone();
            clone.Value = _value.Clone();
            return clone;
        }
        public override StackType ResultType => GetResultMethod?.ReturnType.GetStackType() ?? StackType.Unknown;

        protected override InstructionFlags ComputeFlags()
        {
            return InstructionFlags.SideEffect | _value.Flags;
        }
        public override InstructionFlags DirectFlags => InstructionFlags.SideEffect;

        public override void AcceptVisitor(ILVisitor visitor)
        {
            visitor.VisitAwait(this);
        }
        public override T AcceptVisitor<T>(ILVisitor<T> visitor)
        {
            return visitor.VisitAwait(this);
        }

        public MethodInfo? GetResultMethod;
    }
}
