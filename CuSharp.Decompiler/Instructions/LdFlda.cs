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
using System.Reflection;
using Dotnet4Gpu.Decompilation.Util;

namespace Dotnet4Gpu.Decompilation.Instructions
{
    public sealed class LdFlda : ILInstruction
    {
        public LdFlda(ILInstruction target, FieldInfo field) : base(OpCode.LdFlda)
        {
            Target = target;
            _field = field;
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
            var clone = (LdFlda)ShallowClone();
            clone.Target = _target.Clone();
            return clone;
        }
        public bool DelayExceptions; // NullReferenceException/IndexOutOfBoundsException only occurs when the reference is dereferenced
        private readonly FieldInfo _field;
        /// <summary>Returns the field operand.</summary>
        public FieldInfo Field => _field;

        public override StackType ResultType => TypeUtils.IsIntegerType(_target.ResultType) ? StackType.I : StackType.Ref;

        protected override InstructionFlags ComputeFlags()
        {
            return _target.Flags | (DelayExceptions ? InstructionFlags.None : InstructionFlags.MayThrow);
        }
        public override InstructionFlags DirectFlags => DelayExceptions ? InstructionFlags.None : InstructionFlags.MayThrow;

        public override void AcceptVisitor(ILVisitor visitor)
        {
            visitor.VisitLdFlda(this);
        }
        public override T AcceptVisitor<T>(ILVisitor<T> visitor)
        {
            return visitor.VisitLdFlda(this);
        }

        internal override void CheckInvariant(ILPhase phase)
        {
            base.CheckInvariant(phase);
            switch (_field.DeclaringType?.IsValueType)
            {
                case false:
                    Debug.Assert(_target.ResultType == StackType.O,
                        "Class fields can only be accessed with an object on the stack");
                    break;
                case true:
                    Debug.Assert(_target.ResultType == StackType.I || _target.ResultType == StackType.Ref,
                        "Struct fields can only be accessed with a pointer on the stack");
                    break;
                case null:
                    // field of unresolved type
                    Debug.Assert(_target.ResultType == StackType.O || _target.ResultType == StackType.I
                                                                   || _target.ResultType == StackType.Ref || _target.ResultType == StackType.Unknown,
                        "Field of unresolved type with invalid target");
                    break;
            }
        }
    }
}
