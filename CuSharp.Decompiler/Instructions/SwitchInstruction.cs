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
using CuSharp.Decompiler.Util;

namespace CuSharp.Decompiler.Instructions
{
    /// <summary>
    /// Generalization of IL switch-case: like a VB switch over integers, this instruction
    /// supports integer value ranges as labels.
    /// 
    /// The section labels are using 'long' as integer type.
    /// If the Value instruction produces StackType.I4 or I, the value is implicitly sign-extended to I8.
    /// </summary>
    public sealed class SwitchInstruction : ILInstruction
    {
        public override void AcceptVisitor(ILVisitor visitor)
        {
            visitor.VisitSwitchInstruction(this);
        }
        public override T AcceptVisitor<T>(ILVisitor<T> visitor)
        {
            return visitor.VisitSwitchInstruction(this);
        }

        public static readonly SlotInfo ValueSlot = new("Value", canInlineInto: true);
        public static readonly SlotInfo SectionSlot = new("Section", isCollection: true);

        /// <summary>
        /// If the switch instruction is lifted, the value instruction produces a value of type <c>Nullable{T}</c> for some
        /// integral type T. The section with <c>SwitchSection.HasNullLabel</c> is called if the value is null.
        /// </summary>
        public bool IsLifted;

        private ILInstruction _value = null!;


        public SwitchInstruction(ILInstruction value)
            : base(OpCode.SwitchInstruction)
        {
            Value = value;
            Sections = new InstructionCollection<SwitchSection>(this, 1);
        }

        public override StackType ResultType => StackType.Void;

        public ILInstruction Value
        {
            get => _value;
            set
            {
                ValidateChild(value);
                SetChildInstruction(ref _value, value, 0);
            }
        }

        public readonly InstructionCollection<SwitchSection> Sections;

        protected override InstructionFlags ComputeFlags()
        {
            var sectionFlags = InstructionFlags.EndPointUnreachable; // neutral element for CombineBranches()
            foreach (var section in Sections)
            {
                sectionFlags = SemanticHelper.CombineBranches(sectionFlags, section.Flags);
            }
            return _value.Flags | InstructionFlags.ControlFlow | sectionFlags;
        }

        public override InstructionFlags DirectFlags => InstructionFlags.ControlFlow;

        protected override int GetChildCount()
        {
            return 1 + Sections.Count;
        }

        protected override ILInstruction GetChild(int index)
        {
            return index == 0
                ? _value
                : Sections[index - 1];
        }

        protected override void SetChild(int index, ILInstruction value)
        {
            if (index == 0)
                Value = value;
            else
                Sections[index - 1] = (SwitchSection)value;
        }

        protected override SlotInfo GetChildSlot(int index)
        {
            return index == 0 ? ValueSlot : SectionSlot;
        }

        public override ILInstruction Clone()
        {
            var clone = new SwitchInstruction(_value.Clone());
            clone.AddILRange(this);
            clone.Value = _value.Clone();
            clone.Sections.AddRange(Sections.Select(h => (SwitchSection)h.Clone()));
            return clone;
        }

        internal override void CheckInvariant(ILPhase phase)
        {
            base.CheckInvariant(phase);
            bool expectNullSection = IsLifted;
            LongSet sets = LongSet.Empty;
            foreach (var section in Sections)
            {
                if (section.HasNullLabel)
                {
                    Debug.Assert(expectNullSection, "Duplicate 'case null' or 'case null' in non-lifted switch.");
                    expectNullSection = false;
                }
                Debug.Assert(!section.Labels.IsEmpty || section.HasNullLabel);
                Debug.Assert(!section.Labels.Overlaps(sets));
                Debug.Assert(section.Body.ResultType == ResultType);
                sets = sets.UnionWith(section.Labels);
            }
            Debug.Assert(sets.SetEquals(LongSet.Universe), "switch does not handle all possible cases");
            Debug.Assert(!expectNullSection, "Lifted switch is missing 'case null'");
            Debug.Assert(IsLifted ? _value.ResultType == StackType.O : _value.ResultType == StackType.I4 || _value.ResultType == StackType.I8);
        }
    }
}
