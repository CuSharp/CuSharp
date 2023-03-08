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
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Dotnet4Gpu.Decompilation.Instructions
{
    public sealed class MatchInstruction : ILInstruction, IStoreInstruction
    {
        public MatchInstruction(ILVariable variable, MethodInfo? method, ILInstruction testedOperand, params ILInstruction[] subPatterns) : base(OpCode.MatchInstruction)
        {
            _variable = variable ?? throw new ArgumentNullException(nameof(variable));
            _method = method;
            TestedOperand = testedOperand;
            SubPatterns = new InstructionCollection<ILInstruction>(this, 1);
            SubPatterns.AddRange(subPatterns);
        }

        private ILVariable _variable;
        public ILVariable Variable
        {
            get => _variable;
            set
            {
                DebugAssert(value != null);
                if (IsConnected)
                    _variable.RemoveStoreInstruction(this);
                _variable = value;
                if (IsConnected)
                    _variable.AddStoreInstruction(this);
            }
        }

        public int IndexInStoreInstructionList { get; set; } = -1;

        int IInstructionWithVariableOperand.IndexInVariableInstructionMapping
        {
            get => IndexInStoreInstructionList;
            set => IndexInStoreInstructionList = value;
        }

        protected override void Connected()
        {
            base.Connected();
            _variable.AddStoreInstruction(this);
        }

        protected override void Disconnected()
        {
            _variable.RemoveStoreInstruction(this);
            base.Disconnected();
        }

        private readonly MethodInfo _method;
        /// <summary>Returns the method operand.</summary>
        public bool IsDeconstructCall;
        public bool IsDeconstructTuple;
        public bool CheckType;
        public bool CheckNotNull;
        public static readonly SlotInfo TestedOperandSlot = new("TestedOperand", canInlineInto: true);
        private ILInstruction _testedOperand = null!;
        public ILInstruction TestedOperand
        {
            get => _testedOperand;
            set
            {
                ValidateChild(value);
                SetChildInstruction(ref _testedOperand, value, 0);
            }
        }
        public static readonly SlotInfo SubPatternsSlot = new("SubPatterns");
        public InstructionCollection<ILInstruction> SubPatterns { get; private set; }
        protected override int GetChildCount()
        {
            return 1 + SubPatterns.Count;
        }
        protected override ILInstruction GetChild(int index)
        {
            return index == 0 ? _testedOperand : SubPatterns[index - 1];
        }
        protected override void SetChild(int index, ILInstruction value)
        {
            if (index == 0)
                TestedOperand = value;
            else
                SubPatterns[index - 1] = value;
        }
        protected override SlotInfo GetChildSlot(int index)
        {
            return index == 0 ? TestedOperandSlot : SubPatternsSlot;
        }
        public override ILInstruction Clone()
        {
            var clone = (MatchInstruction)ShallowClone();
            clone.TestedOperand = _testedOperand.Clone();
            clone.SubPatterns = new InstructionCollection<ILInstruction>(clone, 1);
            clone.SubPatterns.AddRange(Enumerable.Select<ILInstruction, ILInstruction>(SubPatterns, arg => arg.Clone()));
            return clone;
        }
        public override StackType ResultType => StackType.I4;

        protected override InstructionFlags ComputeFlags()
        {
            return InstructionFlags.MayWriteLocals | _testedOperand.Flags | Enumerable.Aggregate<ILInstruction, InstructionFlags>(SubPatterns, InstructionFlags.None, (f, arg) => f | arg.Flags) | InstructionFlags.SideEffect | InstructionFlags.MayThrow | InstructionFlags.ControlFlow;
        }
        public override InstructionFlags DirectFlags => InstructionFlags.MayWriteLocals | InstructionFlags.SideEffect | InstructionFlags.MayThrow | InstructionFlags.ControlFlow;

        public override void AcceptVisitor(ILVisitor visitor)
        {
            visitor.VisitMatchInstruction(this);
        }
        public override T AcceptVisitor<T>(ILVisitor<T> visitor)
        {
            return visitor.VisitMatchInstruction(this);
        }

        internal override void CheckInvariant(ILPhase phase)
        {
            base.CheckInvariant(phase);
            DebugAssert(phase <= ILPhase.InILReader || IsDescendantOf(_variable.Function!));
            DebugAssert(phase <= ILPhase.InILReader || _variable.Function!.Variables[_variable.IndexInFunction] == _variable);
            AdditionalInvariants();
        }
        /* Pseudo-Code for interpreting a MatchInstruction:
			bool Eval()
			{
				var value = this.TestedOperand.Eval();
				if (this.CheckNotNull && value == null)
					return false;
				if (this.CheckType && !(value is this.Variable.Type))
					return false;
				if (this.IsDeconstructCall) {
					deconstructResult = new[numArgs];
					EvalCall(this.Method, value, out deconstructResult[0], .., out deconstructResult[numArgs-1]);
					// any occurrences of 'deconstruct.result' in the subPatterns will refer
					// to the values provided by evaluating the call.
				}
				Variable.Value = value;
				foreach (var subPattern in this.SubPatterns) {
					if (!subPattern.Eval())
						return false;
				}
				return true;
			}
		*/
        /* Examples of MatchInstructions:
			expr is var x:
				match(x = expr)

			expr is {} x:
				match.notnull(x = expr)

			expr is T x:
				match.type[T](x = expr)

			expr is C { A: var x } z:
				match.type[C](z = expr) {
				   match(x = z.A)
				}

			expr is C { A: var x, B: 42, C: { A: 4 } } z:
				match.type[C](z = expr) {
					match(x = z.A),
					comp (z.B == 42),
					match.notnull(temp2 = z.C) {
						comp (temp2.A == 4)
					}
				}

			expr is C(var x, var y, <4):
				match.type[C].deconstruct[C.Deconstruct](tmp1 = expr) {
					match(x = deconstruct.result1(tmp1)),
					match(y = deconstruct.result2(tmp1)),
					comp(deconstruct.result3(tmp1) < 4),
				}
			
			expr is C(1, D(2, 3)):
				match.type[C].deconstruct(c = expr) {
					comp(deconstruct.result1(c) == 1),
					match.type[D].deconstruct(d = deconstruct.result2(c)) {
						comp(deconstruct.result1(d) == 2),
						comp(deconstruct.result2(d) == 3),
					}
				}
		 */

        public bool IsVar => !CheckType && !CheckNotNull && !IsDeconstructCall && !IsDeconstructTuple && SubPatterns.Count == 0;

        public bool HasDesignator => Variable.LoadCount + Variable.AddressCount > SubPatterns.Count;

        public int NumPositionalPatterns {
            get {
                if (IsDeconstructCall)
                    return _method!.GetParameters().Length - (_method.IsStatic ? 1 : 0);
                else if (IsDeconstructTuple)
                    throw new NotImplementedException();
                //return TupleType.GetTupleElementTypes(variable.Type).Length;
                else
                    return 0;
            }
        }

        public MatchInstruction(ILVariable variable, ILInstruction testedOperand)
            : this(variable, method: null, testedOperand)
        {
        }

        /// <summary>
        /// Checks whether the input instruction can represent a pattern matching operation.
        /// 
        /// Any pattern matching instruction will first evaluate the `testedOperand` (a descendant of `inst`),
        /// and then match the value of that operand against the pattern encoded in the instruction.
        /// The matching may have side-effects on the newly-initialized pattern variables
        /// (even if the pattern fails to match!).
        /// The pattern matching instruction evaluates to 1 (as I4) if the pattern matches, or 0 otherwise.
        /// </summary>
        public static bool IsPatternMatch(ILInstruction? inst, [NotNullWhen(true)] out ILInstruction? testedOperand)
        {
            switch (inst)
            {
                case MatchInstruction m:
                    testedOperand = m._testedOperand;
                    return true;
                case Comp comp:
                    if (comp.MatchLogicNot(out var operand))
                    {
                        return IsPatternMatch(operand, out testedOperand);
                    }
                    else
                    {
                        testedOperand = comp.Left;
                        return IsConstant(comp.Right);
                    }
                default:
                    testedOperand = null;
                    return false;
            }
        }

        private static bool IsConstant(ILInstruction inst)
        {
            return inst.OpCode switch {
                OpCode.LdcDecimal => true,
                OpCode.LdcF4 => true,
                OpCode.LdcF8 => true,
                OpCode.LdcI4 => true,
                OpCode.LdcI8 => true,
                OpCode.LdNull => true,
                OpCode.LdStr => true,
                _ => false
            };
        }

        internal Type GetDeconstructResultType(int index)
        {
            if (this.IsDeconstructCall)
            {
                int firstOutParam = (_method!.IsStatic ? 1 : 0);
                var outParamType = _method.GetParameters()[firstOutParam + index].ParameterType;
                if (!outParamType.IsByRef)
                    throw new InvalidOperationException("deconstruct out param must be by reference");
                return outParamType.GetElementType();
            }
            if (this.IsDeconstructTuple)
            {
                throw new NotImplementedException();
                //var elementTypes = TupleType.GetTupleElementTypes(this.variable.Type);
                //return elementTypes[index];
            }
            throw new InvalidOperationException("GetDeconstructResultType requires a deconstruct pattern");
        }

        void AdditionalInvariants()
        {
            Debug.Assert(_variable.Kind == VariableKind.PatternLocal);
            if (this.IsDeconstructCall)
            {
                Debug.Assert(IsDeconstructMethod(_method));
            }
            else
            {
                Debug.Assert(_method == null);
            }
            if (this.IsDeconstructTuple)
            {
                //Debug.Assert(variable.Type.Kind == TypeKind.Tuple);
            }
            Debug.Assert(SubPatterns.Count >= NumPositionalPatterns);
            foreach (var subPattern in SubPatterns)
            {
                if (!IsPatternMatch(subPattern, out ILInstruction? operand))
                    throw new InvalidOperationException("Sub-Pattern must be a valid pattern");
                // the first child is TestedOperand
                int subPatternIndex = subPattern.ChildIndex - 1;
                if (subPatternIndex < NumPositionalPatterns)
                {
                    // positional pattern
                    Debug.Assert(operand is DeconstructResultInstruction result && result.Index == subPatternIndex);
                }
                else if (operand.MatchLdFld(out var target, out _))
                {
                    Debug.Assert(target.MatchLdLoc(_variable));
                }
                else if (operand is CallInstruction call)
                {
                    //Debug.Assert(call.Method.AccessorKind == System.Reflection.MethodSemanticsAttributes.Getter);
                    Debug.Assert(call.Arguments[0].MatchLdLoc(_variable));
                }
                else
                {
                    Debug.Fail("Tested operand of sub-pattern is invalid.");
                }
            }
        }

        internal static bool IsDeconstructMethod(MethodInfo? method)
        {
            if (method == null)
                return false;
            if (method.Name != "Deconstruct")
                return false;
            if (method.ReturnType != typeof(void))
                return false;
            int firstOutParam = (method.IsStatic ? 1 : 0);
            if (method.IsStatic)
            {
                throw new NotImplementedException();
                //if (!method.IsExtensionMethod)
                //	return false;
                // TODO : check whether all type arguments can be inferred from the first argument
            }
            else
            {
                if (method.GetGenericArguments().Length != 0)
                    return false;
            }

            // TODO : check whether the method is ambiguous

            var parameters = method.GetParameters();
            if (parameters.Length < firstOutParam)
                return false;

            for (int i = firstOutParam; i < parameters.Length; i++)
            {
                if (!parameters[i].IsOut)
                    return false;
            }

            return true;
        }
    }
}
