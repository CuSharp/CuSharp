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
    /// <summary>memcpy(destAddress, sourceAddress, size);</summary>
    public sealed class Cpblk : ILInstruction, ISupportsVolatilePrefix, ISupportsUnalignedPrefix
    {
        public Cpblk(ILInstruction destAddress, ILInstruction sourceAddress, ILInstruction size) : base(OpCode.Cpblk)
        {
            DestAddress = destAddress;
            SourceAddress = sourceAddress;
            Size = size;
        }
        public static readonly SlotInfo DestAddressSlot = new("DestAddress", canInlineInto: true);
        private ILInstruction _destAddress = null!;
        public ILInstruction DestAddress
        {
            get => _destAddress;
            set
            {
                ValidateChild(value);
                SetChildInstruction(ref _destAddress, value, 0);
            }
        }
        public static readonly SlotInfo SourceAddressSlot = new("SourceAddress", canInlineInto: true);
        private ILInstruction _sourceAddress = null!;
        public ILInstruction SourceAddress
        {
            get => _sourceAddress;
            set
            {
                ValidateChild(value);
                SetChildInstruction(ref _sourceAddress, value, 1);
            }
        }
        public static readonly SlotInfo SizeSlot = new("Size", canInlineInto: true);
        private ILInstruction _size = null!;
        public ILInstruction Size
        {
            get => _size;
            set
            {
                ValidateChild(value);
                SetChildInstruction(ref _size, value, 2);
            }
        }
        protected override int GetChildCount()
        {
            return 3;
        }
        protected override ILInstruction GetChild(int index)
        {
            switch (index)
            {
                case 0:
                    return _destAddress;
                case 1:
                    return _sourceAddress;
                case 2:
                    return _size;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
        protected override void SetChild(int index, ILInstruction value)
        {
            switch (index)
            {
                case 0:
                    DestAddress = value;
                    break;
                case 1:
                    SourceAddress = value;
                    break;
                case 2:
                    Size = value;
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
                    return DestAddressSlot;
                case 1:
                    return SourceAddressSlot;
                case 2:
                    return SizeSlot;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
        public override ILInstruction Clone()
        {
            var clone = (Cpblk)ShallowClone();
            clone.DestAddress = _destAddress.Clone();
            clone.SourceAddress = _sourceAddress.Clone();
            clone.Size = _size.Clone();
            return clone;
        }
        /// <summary>Gets/Sets whether the memory access is volatile.</summary>
        public bool IsVolatile { get; set; }
        /// <summary>Returns the alignment specified by the 'unaligned' prefix; or 0 if there was no 'unaligned' prefix.</summary>
        public byte UnalignedPrefix { get; set; }
        public override StackType ResultType => StackType.Void;

        protected override InstructionFlags ComputeFlags()
        {
            return _destAddress.Flags | _sourceAddress.Flags | _size.Flags | InstructionFlags.MayThrow | InstructionFlags.SideEffect;
        }
        public override InstructionFlags DirectFlags => InstructionFlags.MayThrow | InstructionFlags.SideEffect;

        public override void AcceptVisitor(ILVisitor visitor)
        {
            visitor.VisitCpblk(this);
        }
        public override T AcceptVisitor<T>(ILVisitor<T> visitor)
        {
            return visitor.VisitCpblk(this);
        }

        internal override void CheckInvariant(ILPhase phase)
        {
            base.CheckInvariant(phase);
            DebugAssert(_destAddress.ResultType == StackType.I || _destAddress.ResultType == StackType.Ref);
            DebugAssert(_sourceAddress.ResultType == StackType.I || _sourceAddress.ResultType == StackType.Ref);
            DebugAssert(_size.ResultType == StackType.I4);
        }
    }
}