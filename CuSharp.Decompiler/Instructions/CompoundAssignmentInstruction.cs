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

namespace CuSharp.Decompiler.Instructions
{
    public abstract class CompoundAssignmentInstruction : ILInstruction
    {
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
        public static readonly SlotInfo ValueSlot = new("Value", canInlineInto: true);
        private ILInstruction _value = null!;
        public ILInstruction Value
        {
            get => _value;
            set
            {
                ValidateChild(value);
                SetChildInstruction(ref _value, value, 1);
            }
        }
        protected override int GetChildCount()
        {
            return 2;
        }
        protected override ILInstruction GetChild(int index)
        {
            switch (index)
            {
                case 0:
                    return _target;
                case 1:
                    return _value;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
        protected override void SetChild(int index, ILInstruction value)
        {
            switch (index)
            {
                case 0:
                    Target = value;
                    break;
                case 1:
                    Value = value;
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
                    return TargetSlot;
                case 1:
                    return ValueSlot;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
        public override ILInstruction Clone()
        {
            var clone = (CompoundAssignmentInstruction)ShallowClone();
            clone.Target = _target.Clone();
            clone.Value = _value.Clone();
            return clone;
        }
        protected override InstructionFlags ComputeFlags()
        {
            return _target.Flags | _value.Flags;
        }
        public override InstructionFlags DirectFlags => InstructionFlags.None;
        public readonly CompoundEvalMode EvalMode;

        /// <summary>
        /// If TargetIsProperty is true, the Target must be a call to a property getter,
        /// and the compound.assign will implicitly call the corresponding property setter.
        /// Otherwise, the Target can be any instruction that evaluates to an address,
        /// and the compound.assign will implicit load and store from/to that address.
        /// </summary>
        public readonly CompoundTargetKind TargetKind;

        protected CompoundAssignmentInstruction(OpCode opCode, CompoundEvalMode evalMode, ILInstruction target, CompoundTargetKind targetKind, ILInstruction value)
            : base(opCode)
        {
            EvalMode = evalMode;
            Target = target;
            TargetKind = targetKind;
            Value = value;
            CheckValidTarget();
        }

        internal override void CheckInvariant(ILPhase phase)
        {
            base.CheckInvariant(phase);
            CheckValidTarget();
        }

        [Conditional("DEBUG")]
        void CheckValidTarget()
        {
            switch (TargetKind)
            {
                case CompoundTargetKind.Address:
                    Debug.Assert(_target.ResultType == StackType.Ref || _target.ResultType == StackType.I);
                    break;
                case CompoundTargetKind.Property:
                    Debug.Assert(_target.OpCode == OpCode.Call || _target.OpCode == OpCode.CallVirt);
                    //var owner = ((CallInstruction)target).Method.AccessorOwner as IProperty;
                    //Debug.Assert(owner != null && owner.CanSet);
                    break;
                case CompoundTargetKind.Dynamic:
                    Debug.Assert(_target.OpCode == OpCode.DynamicGetMemberInstruction || _target.OpCode == OpCode.DynamicGetIndexInstruction);
                    break;
            }
        }
    }
}


