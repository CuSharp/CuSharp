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

namespace CuSharp.Decompiler.Instructions
{
    public sealed class DeconstructInstruction : ILInstruction
    {
        public override StackType ResultType => StackType.Void;

        public override void AcceptVisitor(ILVisitor visitor)
        {
            visitor.VisitDeconstructInstruction(this);
        }
        public override T AcceptVisitor<T>(ILVisitor<T> visitor)
        {
            return visitor.VisitDeconstructInstruction(this);
        }

        public static readonly SlotInfo InitSlot = new("Init", canInlineInto: true, isCollection: true);
        public static readonly SlotInfo PatternSlot = new("Pattern", canInlineInto: true);
        public static readonly SlotInfo ConversionsSlot = new("Conversions");
        public static readonly SlotInfo AssignmentsSlot = new("Assignments");

        public DeconstructInstruction()
            : base(OpCode.DeconstructInstruction)
        {
            Init = new InstructionCollection<StLoc>(this, 0);
        }

        public readonly InstructionCollection<StLoc> Init;

        private MatchInstruction _pattern;

        public MatchInstruction Pattern
        {
            get => _pattern;
            set
            {
                ValidateChild(value);
                SetChildInstruction(ref _pattern, value, Init.Count);
            }
        }

        private Block _conversions;
        public Block Conversions
        {
            get => _conversions;
            set
            {
                ValidateChild(value);
                SetChildInstruction(ref _conversions, value, Init.Count + 1);
            }
        }

        private Block _assignments;
        public Block Assignments
        {
            get => _assignments;
            set
            {
                ValidateChild(value);
                SetChildInstruction(ref _assignments, value, Init.Count + 2);
            }
        }

        protected override int GetChildCount()
        {
            return Init.Count + 3;
        }

        protected override ILInstruction GetChild(int index)
        {
            return (index - Init.Count) switch
            {
                0 => _pattern,
                1 => _conversions,
                2 => _assignments,
                _ => Init[index]
            };
        }

        protected override void SetChild(int index, ILInstruction value)
        {
            switch (index - Init.Count)
            {
                case 0:
                    Pattern = (MatchInstruction)value;
                    break;
                case 1:
                    Conversions = (Block)value;
                    break;
                case 2:
                    Assignments = (Block)value;
                    break;
                default:
                    Init[index] = (StLoc)value;
                    break;
            }
        }

        protected override SlotInfo GetChildSlot(int index)
        {
            switch (index - Init.Count)
            {
                case 0:
                    return PatternSlot;
                case 1:
                    return ConversionsSlot;
                case 2:
                    return AssignmentsSlot;
                default:
                    return InitSlot;
            }
        }

        public override ILInstruction Clone()
        {
            var clone = new DeconstructInstruction();
            clone.Init.AddRange(Init.Select(inst => (StLoc)inst.Clone()));
            clone.Pattern = (MatchInstruction)_pattern.Clone();
            clone.Conversions = (Block)_conversions.Clone();
            clone.Assignments = (Block)_assignments.Clone();
            return clone;
        }

        protected override InstructionFlags ComputeFlags()
        {
            var flags = InstructionFlags.None;
            foreach (var inst in Init)
            {
                flags |= inst.Flags;
            }
            flags |= _pattern.Flags | _conversions.Flags | _assignments.Flags;
            return flags;
        }

        public override InstructionFlags DirectFlags => InstructionFlags.None;

        protected internal override void InstructionCollectionUpdateComplete()
        {
            base.InstructionCollectionUpdateComplete();
            if (_pattern.Parent == this)
                _pattern.ChildIndex = Init.Count;
            if (_conversions.Parent == this)
                _conversions.ChildIndex = Init.Count + 1;
            if (_assignments.Parent == this)
                _assignments.ChildIndex = Init.Count + 2;
        }

        internal static bool IsConversionStLoc(ILInstruction inst, out ILVariable variable, out ILVariable inputVariable)
        {
            inputVariable = null;
            if (!inst.MatchStLoc(out variable, out var value))
                return false;
            ILInstruction input;
            switch (value)
            {
                case Conv conv:
                    input = conv.Argument;
                    break;
                //case Call { Method: { IsOperator: true, Name: "op_Implicit" }, Arguments: { Count: 1 } } call:
                //	input = call.Arguments[0];
                //	break;
                default:
                    return false;
            }
            return input.MatchLdLoc(out inputVariable) || input.MatchLdLoca(out inputVariable);
        }

        internal static bool IsAssignment(ILInstruction inst, out Type expectedType, out ILInstruction value)
        {
            expectedType = null;
            value = null;
            switch (inst)
            {
                case CallInstruction call:
                    //if (call.Method.AccessorKind != System.Reflection.MethodSemanticsAttributes.Setter)
                    if (call.Method.MemberType != MemberTypes.Property)
                        return false;
                    for (int i = 0; i < call.Arguments.Count - 1; i++)
                    {
                        ILInstruction arg = call.Arguments[i];
                        if (arg.Flags == InstructionFlags.None)
                        {
                            // OK - we accept integer literals, etc.
                        }
                        else if (arg.MatchLdLoc(out var v))
                        {
                        }
                        else
                        {
                            return false;
                        }
                    }
                    expectedType = call.Method.GetParameters().Last().ParameterType;
                    value = call.Arguments.Last();
                    return true;
                case StLoc stloc:
                    expectedType = stloc.Variable.Type;
                    value = stloc.Value;
                    return true;
                case StObj stobj:
                    var target = stobj.Target;
                    while (target.MatchLdFlda(out var nestedTarget, out _))
                        target = nestedTarget;
                    if (target.Flags == InstructionFlags.None)
                    {
                        // OK - we accept integer literals, etc.
                    }
                    else if (target.MatchLdLoc(out var v))
                    {
                    }
                    else
                    {
                        return false;
                    }

                    throw new NotImplementedException();
                //if (stobj.Target.InferType(typeSystem) is ByReferenceType brt)
                //	expectedType = brt.ElementType;
                //else
                //	expectedType = SpecialType.UnknownType;
                //value = stobj.Value;
                //return true;
                default:
                    return false;
            }
        }

        internal override void CheckInvariant(ILPhase phase)
        {
            base.CheckInvariant(phase);
            var patternVariables = new HashSet<ILVariable>();
            var conversionVariables = new HashSet<ILVariable>();

            foreach (StLoc init in Init)
            {
                Debug.Assert(init.Variable.IsSingleDefinition && init.Variable.LoadCount == 1);
                Debug.Assert(init.Variable.LoadInstructions[0].IsDescendantOf(_assignments));
            }

            ValidatePattern(_pattern);

            foreach (var inst in _conversions.Instructions)
            {
                if (!IsConversionStLoc(inst, out var variable, out var inputVariable))
                    Debug.Fail("inst is not a conversion stloc!");
                Debug.Assert(variable.IsSingleDefinition && variable.LoadCount == 1);
                Debug.Assert(variable.LoadInstructions[0].IsDescendantOf(_assignments));
                Debug.Assert(patternVariables.Contains(inputVariable));
                conversionVariables.Add(variable);
            }
            Debug.Assert(_conversions.FinalInstruction is Nop);

            foreach (var inst in _assignments.Instructions)
            {
                if (!(IsAssignment(inst, out _, out var value) && value.MatchLdLoc(out var inputVariable)))
                    throw new InvalidOperationException("inst is not an assignment!");
                Debug.Assert(patternVariables.Contains(inputVariable) || conversionVariables.Contains(inputVariable));
            }
            Debug.Assert(_assignments.FinalInstruction is Nop);

            void ValidatePattern(MatchInstruction inst)
            {
                Debug.Assert(inst.IsDeconstructCall || inst.IsDeconstructTuple);
                Debug.Assert(!inst.CheckNotNull && !inst.CheckType);
                Debug.Assert(!inst.HasDesignator);
                foreach (var subPattern in inst.SubPatterns.Cast<MatchInstruction>())
                {
                    if (subPattern.IsVar)
                    {
                        Debug.Assert(subPattern.Variable.IsSingleDefinition && subPattern.Variable.LoadCount <= 1);
                        if (subPattern.Variable.LoadCount == 1)
                            Debug.Assert(subPattern.Variable.LoadInstructions[0].IsDescendantOf(this));
                        patternVariables.Add(subPattern.Variable);
                    }
                    else
                    {
                        ValidatePattern(subPattern);
                    }
                }
            }
        }
    }
}
