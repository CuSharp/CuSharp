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

namespace Dotnet4Gpu.Decompilation.Instructions
{
    /// <summary>Indirect load (ref/pointer dereference).</summary>
    public sealed class LdObj : ILInstruction, ISupportsVolatilePrefix, ISupportsUnalignedPrefix
    {
        public LdObj(ILInstruction target, Type type) : base(OpCode.LdObj)
        {
            Target = target;
            _type = type;
        }
        public static readonly SlotInfo TargetSlot = new("Target", canInlineInto: true);
        private ILInstruction _target = null!;
        public ILInstruction Target
        {
            get => _target;
            set
            {
                ValidateChild(value);
                SetChildInstruction(ref _target, value, 0);
            }
        }
        protected override int GetChildCount()
        {
            return 1;
        }
        protected override ILInstruction GetChild(int index)
        {
            return index == 0 ? _target : throw new IndexOutOfRangeException();
        }
        protected override void SetChild(int index, ILInstruction value)
        {
            Target = index == 0 ? value : throw new IndexOutOfRangeException();
        }
        protected override SlotInfo GetChildSlot(int index)
        {
            return index == 0 ? TargetSlot : throw new IndexOutOfRangeException();
        }
        public override ILInstruction Clone()
        {
            var clone = (LdObj)ShallowClone();
            clone.Target = _target.Clone();
            return clone;
        }
        private readonly Type _type;

        /// <summary>Gets/Sets whether the memory access is volatile.</summary>
        public bool IsVolatile { get; set; }
        /// <summary>Returns the alignment specified by the 'unaligned' prefix; or 0 if there was no 'unaligned' prefix.</summary>
        public byte UnalignedPrefix { get; set; }
        public override StackType ResultType => _type.GetStackType();

        protected override InstructionFlags ComputeFlags()
        {
            return _target.Flags | InstructionFlags.SideEffect | InstructionFlags.MayThrow;
        }
        public override InstructionFlags DirectFlags => InstructionFlags.SideEffect | InstructionFlags.MayThrow;

        public override void AcceptVisitor(ILVisitor visitor)
        {
            visitor.VisitLdObj(this);
        }
        public override T AcceptVisitor<T>(ILVisitor<T> visitor)
        {
            return visitor.VisitLdObj(this);
        }

        internal override void CheckInvariant(ILPhase phase)
        {
            base.CheckInvariant(phase);
            DebugAssert(_target.ResultType == StackType.Ref || _target.ResultType == StackType.I);
        }
    }
}