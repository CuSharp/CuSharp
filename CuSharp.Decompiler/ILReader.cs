// Copyright (c) 2014 Daniel Grunwald
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

using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;
using Dotnet4Gpu.Decompilation.Instructions;
using Dotnet4Gpu.Decompilation.Util;

// TODO: proper nullable handling

namespace Dotnet4Gpu.Decompilation
{
    /// <summary>
    /// Reads IL bytecodes and converts them into ILAst instructions.
    /// </summary>
    /// <remarks>
    /// Instances of this class are not thread-safe. Use separate instances to decompile multiple members in parallel.
    /// </remarks>
    public class ILReader
    {
        private readonly MethodInfo _method;
        private readonly Module _module;
        private readonly MethodBody _body;
        private readonly StackType _methodReturnStackType;
        private readonly BinaryReader _reader;
        private ImmutableStack<ILVariable> _currentStack;
        private readonly List<ILInstruction> _expressionStack;
        private ILVariable[] _parameterVariables;
        private readonly ILVariable[] _localVariables;
        private readonly BitSet _isBranchTarget;
        private readonly BlockContainer _mainContainer;
        private List<ILInstruction> _instructionBuilder;
        private int _currentInstructionStart;
        private Type _constrainedPrefix;

        // Dictionary that stores stacks for each IL instruction
        private readonly Dictionary<int, ImmutableStack<ILVariable>> _stackByOffset;
        private readonly Dictionary<ExceptionHandlingClause, ILVariable> _variableByExceptionHandler;
        private readonly UnionFind<ILVariable> _unionFind;
        private readonly List<(ILVariable, ILVariable)> _stackMismatchPairs;
        private IEnumerable<ILVariable> _stackVariables;

        public List<string> Warnings { get; } = new();

        // List of candidate locations for sequence points. Includes empty il stack locations, any nop instructions, and the instruction following
        // a call instruction. 
        public List<int> SequencePointCandidates { get; }

        /// <summary>
        /// Creates a new ILReader instance.
        /// </summary>
        /// <param name="module">
        /// The module used to resolve metadata tokens in the type system.
        /// </param>
        public ILReader(MethodInfo method)
        {
            _method = method;
            _module = method.Module;
            SequencePointCandidates = new List<int>();
            _body = method.GetMethodBody();
            _reader = new BinaryReader(new MemoryStream(_body.GetILAsByteArray()));
            _currentStack = ImmutableStack<ILVariable>.Empty;
            _expressionStack = new List<ILInstruction>();
            _unionFind = new UnionFind<ILVariable>();
            _stackMismatchPairs = new List<(ILVariable, ILVariable)>();
            _methodReturnStackType = _method.ReturnType.GetStackType();
            InitParameterVariables();
            _localVariables = InitLocalVariables();
            foreach (var v in _localVariables)
            {
                v.InitialValueIsInitialized = _body.InitLocals;
                v.UsesInitialValue = true;
            }
            _mainContainer = new BlockContainer(expectedResultType: _methodReturnStackType);
            _instructionBuilder = new List<ILInstruction>();
            _isBranchTarget = new BitSet((int)_reader.BaseStream.Length);
            _stackByOffset = new Dictionary<int, ImmutableStack<ILVariable>>();
            _variableByExceptionHandler = new Dictionary<ExceptionHandlingClause, ILVariable>();
        }

        int ReadAndDecodeMetadataToken()
        {
            int token = _reader.ReadInt32();
            if (token <= 0)
            {
                // SRM uses negative tokens as "virtual tokens" and can get confused
                // if we manually create them.
                // Row-IDs < 1 are always invalid.
                throw new BadImageFormatException("Invalid metadata token");
            }
            return token;
        }

        Type ReadAndDecodeTypeReference()
        {
            return _module.ResolveType(ReadAndDecodeMetadataToken());
        }

        MethodBase? ReadAndDecodeMethodReference()
        {
            return _module.ResolveMethod(ReadAndDecodeMetadataToken());
        }

        FieldInfo ReadAndDecodeFieldReference()
        {
            var fieldReference = ReadAndDecodeMetadataToken();
            var field = _module.ResolveField(fieldReference);
            return field == null ? throw new BadImageFormatException("Invalid field token") : field;
        }

        ILVariable[] InitLocalVariables()
        {
            if (_body.LocalSignatureMetadataToken == 0)
                return Empty<ILVariable>.Array;
            var locals = _body.LocalVariables; // TODO: check, if this is correct
            var localVariables = new ILVariable[locals.Count];
            for (int i = 0; i < locals.Count; i++)
            {
                localVariables[i] = CreateILVariable(i, locals[i]);
            }
            return localVariables;
        }

        void InitParameterVariables()
        {
            var parameters = _method.GetParameters();
            int popCount = parameters.Length;
            if (!_method.IsStatic)
                popCount++;
            // TODO: possibly support feature again
            //if (parameters.LastOrDefault()?.ParameterType == SpecialType.ArgList)
            //	popCount--;
            _parameterVariables = new ILVariable[popCount];
            int paramIndex = 0;
            int offset = 0;
            if (!_method.IsStatic)
            {
                offset = 1;
                // TODO: figure out, what unbound is
                Type declaringType = _method.DeclaringType;
                //if (declaringType.IsUnbound())
                //{
                //	// If method is a definition (and not specialized), the declaring type is also just a definition,
                //	// and needs to be converted into a normally usable type.
                //	declaringType = new ParameterizedType(declaringType, declaringType.TypeParameters);
                //}
                ILVariable ilVar = CreateILVariable(-1, declaringType, "this");
                ilVar.IsRefReadOnly = true; // TODO: was method.ThisIsRefReadOnly, figure out what it means
                _parameterVariables[paramIndex++] = ilVar;
            }
            while (paramIndex < _parameterVariables.Length)
            {
                ParameterInfo parameter = parameters[paramIndex - offset];
                ILVariable ilVar = CreateILVariable(paramIndex - offset, parameter.ParameterType, parameter.Name);
                ilVar.IsRefReadOnly = parameter.IsIn;
                _parameterVariables[paramIndex] = ilVar;
                paramIndex++;
            }
            Debug.Assert(paramIndex == _parameterVariables.Length);
        }

        ILVariable CreateILVariable(int index, LocalVariableInfo info)
        {
            VariableKind kind;
            //if (type.SkipModifiers() is PinnedType pinned)
            //{
            //	kind = VariableKind.PinnedLocal;
            //	type = pinned.ElementType;
            //}
            //else
            {
                kind = VariableKind.Local;
            }
            var ilVar = new ILVariable(kind, info.LocalType, index)
            {
                Name = "V_" + index,
                HasGeneratedName = true
            };
            return ilVar;
        }

        ILVariable CreateILVariable(int index, Type parameterType, string name)
        {
            //Debug.Assert(!parameterType.IsUnbound());
            // TODO: figure out what a by reference type is
            //ITypeDefinition def = parameterType.GetDefinition();
            //if (def != null && index < 0 && def.IsReferenceType == false) parameterType = new ByReferenceType(parameterType);

            var ilVar = new ILVariable(VariableKind.Parameter, parameterType, index);
            Debug.Assert(ilVar.StoreCount == 1); // count the initial store when the method is called with an argument
            if (index < 0) ilVar.Name = "this";
            else if (string.IsNullOrEmpty(name)) ilVar.Name = "P_" + index;
            else ilVar.Name = name;
            return ilVar;
        }

        /// <summary>
        /// Warn when invalid IL is detected.
        /// ILSpy should be able to handle invalid IL; but this method can be helpful for debugging the ILReader,
        /// as this method should not get called when processing valid IL.
        /// </summary>
        void Warn(string message)
        {
            Warnings.Add($"IL_{_currentInstructionStart:x4}: {message}");
        }

        ImmutableStack<ILVariable> MergeStacks(ImmutableStack<ILVariable> a, ImmutableStack<ILVariable> b)
        {
            if (CheckStackCompatibleWithoutAdjustments(a, b))
            {
                // We only need to union the input variables, but can 
                // otherwise re-use the existing stack.
                ImmutableStack<ILVariable> output = a;
                while (!a.IsEmpty && !b.IsEmpty)
                {
                    Debug.Assert(a.Peek().StackType == b.Peek().StackType);
                    _unionFind.Merge(a.Peek(), b.Peek());
                    a = a.Pop();
                    b = b.Pop();
                }
                return output;
            }
            else if (a.Count() != b.Count())
            {
                // Let's not try to merge mismatched stacks.
                Warn("Incompatible stack heights: " + a.Count() + " vs " + b.Count());
                return a;
            }
            else
            {
                // The more complex case where the stacks don't match exactly.
                var output = new List<ILVariable>();
                while (!a.IsEmpty && !b.IsEmpty)
                {
                    var varA = a.Peek();
                    var varB = b.Peek();
                    if (varA.StackType == varB.StackType)
                    {
                        _unionFind.Merge(varA, varB);
                        output.Add(varA);
                    }
                    else
                    {
                        if (!IsValidTypeStackTypeMerge(varA.StackType, varB.StackType))
                        {
                            Warn("Incompatible stack types: " + varA.StackType + " vs " + varB.StackType);
                        }
                        if (varA.StackType > varB.StackType)
                        {
                            output.Add(varA);
                            // every store to varB should also store to varA
                            _stackMismatchPairs.Add((varB, varA));
                        }
                        else
                        {
                            output.Add(varB);
                            // every store to varA should also store to varB
                            _stackMismatchPairs.Add((varA, varB));
                        }
                    }
                    a = a.Pop();
                    b = b.Pop();
                }
                // because we built up output by popping from the input stacks, we need to reverse it to get back the original order
                output.Reverse();
                return ImmutableStack.CreateRange(output);
            }
        }

        static bool CheckStackCompatibleWithoutAdjustments(ImmutableStack<ILVariable> a, ImmutableStack<ILVariable> b)
        {
            while (!a.IsEmpty && !b.IsEmpty)
            {
                if (a.Peek().StackType != b.Peek().StackType)
                    return false;
                a = a.Pop();
                b = b.Pop();
            }
            return a.IsEmpty && b.IsEmpty;
        }

        private bool IsValidTypeStackTypeMerge(StackType stackType1, StackType stackType2)
        {
            if (stackType1 == StackType.I && stackType2 == StackType.I4)
                return true;
            if (stackType1 == StackType.I4 && stackType2 == StackType.I)
                return true;
            if (stackType1 == StackType.F4 && stackType2 == StackType.F8)
                return true;
            if (stackType1 == StackType.F8 && stackType2 == StackType.F4)
                return true;
            // allow merging unknown type with any other type
            return stackType1 == StackType.Unknown || stackType2 == StackType.Unknown;
        }

        /// <summary>
        /// Stores the given stack for a branch to `offset`.
        /// 
        /// The stack may be modified if stack adjustments are necessary. (e.g. implicit I4->I conversion)
        /// </summary>
        void StoreStackForOffset(int offset, ref ImmutableStack<ILVariable> stack)
        {
            if (_stackByOffset.TryGetValue(offset, out var existing))
            {
                stack = MergeStacks(existing, stack);
                if (stack != existing)
                    _stackByOffset[offset] = stack;
            }
            else
            {
                _stackByOffset.Add(offset, stack);
            }
        }

        void ReadInstructions()
        {
            // TODO: find a more efficient solution that does not require setting back the stream three times - do fixup at the end
            _reader.BaseStream.Seek(0, SeekOrigin.Begin);
            ILParser.SetBranchTargets(_reader, _isBranchTarget);
            _reader.BaseStream.Seek(0, SeekOrigin.Begin);
            PrepareBranchTargetsAndStacksForExceptionHandlers();

            bool nextInstructionBeginsNewBlock = false;

            _reader.BaseStream.Seek(0, SeekOrigin.Begin);
            while (_reader.BaseStream.Position < _reader.BaseStream.Length)
            {
                int start = (int)_reader.BaseStream.Position;
                if (_isBranchTarget[start])
                {
                    FlushExpressionStack();
                    StoreStackForOffset(start, ref _currentStack);
                }
                _currentInstructionStart = start;
                bool startedWithEmptyStack = CurrentStackIsEmpty();
                DecodedInstruction decodedInstruction;
                try
                {
                    decodedInstruction = DecodeInstruction();
                }
                catch (BadImageFormatException ex)
                {
                    decodedInstruction = new InvalidBranch(ex.Message);
                }
                var inst = decodedInstruction.Instruction;
                if (inst.ResultType == StackType.Unknown && inst.OpCode != OpCode.InvalidBranch && inst.OpCode != OpCode.InvalidExpression)
                    Warn("Unknown result type (might be due to invalid IL or missing references)");
                inst.CheckInvariant(ILPhase.InILReader);
                int end = (int)_reader.BaseStream.Position;
                inst.AddILRange(new Interval(start, end));
                if (!decodedInstruction.PushedOnExpressionStack)
                {
                    // Flush to avoid re-ordering of side-effects
                    FlushExpressionStack();
                    _instructionBuilder.Add(inst);
                }
                else if (_isBranchTarget[start] || nextInstructionBeginsNewBlock)
                {
                    // If this instruction is the first in a new block, avoid it being inlined
                    // into the next instruction.
                    // This is necessary because the BlockBuilder uses inst.StartILOffset to
                    // detect block starts, and doesn't search nested instructions.
                    FlushExpressionStack();
                }
                if (inst.HasDirectFlag(InstructionFlags.EndPointUnreachable))
                {
                    FlushExpressionStack();
                    if (!_stackByOffset.TryGetValue(end, out _currentStack))
                    {
                        _currentStack = ImmutableStack<ILVariable>.Empty;
                    }
                    nextInstructionBeginsNewBlock = true;
                }
                else
                {
                    nextInstructionBeginsNewBlock = inst.HasFlag(InstructionFlags.MayBranch);
                }

                if (!decodedInstruction.PushedOnExpressionStack && IsSequencePointInstruction(inst) || startedWithEmptyStack)
                {
                    SequencePointCandidates.Add(inst.StartILOffset);
                }
            }

            FlushExpressionStack();

            var visitor = new CollectStackVariablesVisitor(_unionFind);
            for (int i = 0; i < _instructionBuilder.Count; i++)
            {
                _instructionBuilder[i] = _instructionBuilder[i].AcceptVisitor(visitor);
            }
            _stackVariables = visitor.Variables;
            InsertStackAdjustments();
        }

        private bool CurrentStackIsEmpty()
        {
            return _currentStack.IsEmpty && _expressionStack.Count == 0;
        }

        private void PrepareBranchTargetsAndStacksForExceptionHandlers()
        {
            // Fill isBranchTarget and branchStackDict based on exception handlers
            foreach (var clause in _body.ExceptionHandlingClauses)
            {
                // Always mark the start of the try block as a "branch target".
                // We don't actually need to store the stack state here,
                // but we need to ensure that no ILInstructions are inlined
                // into the try-block.
                _isBranchTarget[clause.TryOffset] = true;

                ImmutableStack<ILVariable> ehStack;
                if (clause.Flags.HasFlag(ExceptionHandlingClauseOptions.Clause))
                {
                    var catchType = clause.CatchType;
                    var v = new ILVariable(VariableKind.ExceptionStackSlot, catchType, clause.HandlerOffset)
                    {
                        Name = "E_" + clause.HandlerOffset,
                        HasGeneratedName = true
                    };
                    _variableByExceptionHandler.Add(clause, v);
                    ehStack = ImmutableStack.Create(v);
                }
                else if (clause.Flags.HasFlag(ExceptionHandlingClauseOptions.Filter))
                {
                    var v = new ILVariable(VariableKind.ExceptionStackSlot, typeof(object), clause.HandlerOffset)
                    {
                        Name = "E_" + clause.HandlerOffset,
                        HasGeneratedName = true
                    };
                    _variableByExceptionHandler.Add(clause, v);
                    ehStack = ImmutableStack.Create(v);
                }
                else
                {
                    ehStack = ImmutableStack<ILVariable>.Empty;
                }
                if (clause.FilterOffset != -1)
                {
                    _isBranchTarget[clause.FilterOffset] = true;
                    StoreStackForOffset(clause.FilterOffset, ref ehStack);
                }
                if (clause.HandlerOffset != -1)
                {
                    _isBranchTarget[clause.HandlerOffset] = true;
                    StoreStackForOffset(clause.HandlerOffset, ref ehStack);
                }
            }
        }

        private bool IsSequencePointInstruction(ILInstruction instruction)
        {
            return instruction.OpCode == OpCode.Nop ||
                   _instructionBuilder.Count > 0
                    && _instructionBuilder[^1].OpCode is OpCode.Call
                        or OpCode.CallIndirect
                        or OpCode.CallVirt;
        }

        void InsertStackAdjustments()
        {
            if (_stackMismatchPairs.Count == 0)
                return;
            var dict = new MultiDictionary<ILVariable, ILVariable>();
            foreach (var (origA, origB) in _stackMismatchPairs)
            {
                var a = _unionFind.Find(origA);
                var b = _unionFind.Find(origB);
                Debug.Assert(a.StackType < b.StackType);
                // For every store to a, insert a converting store to b.
                if (!dict[a].Contains(b))
                    dict.Add(a, b);
            }
            var newInstructions = new List<ILInstruction>();
            foreach (var inst in _instructionBuilder)
            {
                newInstructions.Add(inst);
                if (inst is StLoc store)
                {
                    foreach (var additionalVar in dict[store.Variable])
                    {
                        ILInstruction value = new LdLoc(store.Variable);
                        value = new Conv(value, additionalVar.StackType.ToPrimitiveType(), false, Sign.Signed);
                        newInstructions.Add(new StLoc(additionalVar, value)
                        {
                            IsStackAdjustment = true,
                        }.WithILRange(inst));
                    }
                }
            }
            _instructionBuilder = newInstructions;
        }

        /// <summary>
        /// Decodes the specified method body and returns an ILFunction.
        /// </summary>
        public ILFunction ReadIL()
        {
            ReadInstructions();
            var blockBuilder = new BlockBuilder(_body, _variableByExceptionHandler);
            blockBuilder.CreateBlocks(_mainContainer, _instructionBuilder, _isBranchTarget);
            var function = new ILFunction(_method, _mainContainer);
            function.Variables.AddRange(_parameterVariables);
            function.Variables.AddRange(_localVariables);
            function.Variables.AddRange(_stackVariables);
            function.Variables.AddRange(_variableByExceptionHandler.Values);
            function.Variables.AddRange(blockBuilder.OnErrorDispatcherVariables);
            function.AddRef(); // mark the root node
            var removedBlocks = new List<Block>();
            foreach (var c in function.Descendants.OfType<BlockContainer>())
            {
                var newOrder = c.TopologicalSort(deleteUnreachableBlocks: true);
                if (newOrder.Count < c.Blocks.Count)
                {
                    removedBlocks.AddRange(c.Blocks.Except(newOrder));
                }
                c.Blocks.ReplaceList(newOrder);
            }
            if (removedBlocks.Count > 0)
            {
                removedBlocks.SortBy(b => b.StartILOffset);
                function.Warnings.Add("Discarded unreachable code: "
                            + string.Join(", ", removedBlocks.Select(b => $"IL_{b.StartILOffset:x4}")));
            }

            SequencePointCandidates.Sort();
            function.SequencePointCandidates = SequencePointCandidates;

            function.Warnings.AddRange(Warnings);
            return function;
        }

        DecodedInstruction Neg()
        {
            switch (PeekStackType())
            {
                case StackType.I4:
                    return Push(new BinaryNumericInstruction(BinaryNumericOperator.Sub, new LdcI4(0), Pop(), checkForOverflow: false, sign: Sign.None));
                case StackType.I:
                    return Push(new BinaryNumericInstruction(BinaryNumericOperator.Sub, new Conv(new LdcI4(0), PrimitiveType.I, false, Sign.None), Pop(), checkForOverflow: false, sign: Sign.None));
                case StackType.I8:
                    return Push(new BinaryNumericInstruction(BinaryNumericOperator.Sub, new LdcI8(0), Pop(), checkForOverflow: false, sign: Sign.None));
                case StackType.F4:
                    return Push(new BinaryNumericInstruction(BinaryNumericOperator.Sub, new LdcF4(0), Pop(), checkForOverflow: false, sign: Sign.None));
                case StackType.F8:
                    return Push(new BinaryNumericInstruction(BinaryNumericOperator.Sub, new LdcF8(0), Pop(), checkForOverflow: false, sign: Sign.None));
                default:
                    Warn("Unsupported input type for neg.");
                    goto case StackType.I4;
            }
        }

        struct DecodedInstruction
        {
            public ILInstruction Instruction;
            public bool PushedOnExpressionStack;

            public static implicit operator DecodedInstruction(ILInstruction instruction)
            {
                return new DecodedInstruction { Instruction = instruction };
            }
        }

        DecodedInstruction DecodeInstruction()
        {
            if (_reader.BaseStream.Position == _reader.BaseStream.Length)
                return new InvalidBranch("Unexpected end of body");
            var opCode = _reader.DecodeOpCode();
            switch (opCode)
            {
                case ILOpCode.Constrained:
                    return DecodeConstrainedCall();
                case ILOpCode.Readonly:
                    return DecodeReadonly();
                case ILOpCode.Tail:
                    return DecodeTailCall();
                case ILOpCode.Unaligned:
                    return DecodeUnaligned();
                case ILOpCode.Volatile:
                    return DecodeVolatile();
                case ILOpCode.Add:
                    return BinaryNumeric(BinaryNumericOperator.Add);
                case ILOpCode.Add_ovf:
                    return BinaryNumeric(BinaryNumericOperator.Add, true, Sign.Signed);
                case ILOpCode.Add_ovf_un:
                    return BinaryNumeric(BinaryNumericOperator.Add, true, Sign.Unsigned);
                case ILOpCode.And:
                    return BinaryNumeric(BinaryNumericOperator.BitAnd);
                case ILOpCode.Arglist:
                    return Push(new Arglist());
                case ILOpCode.Beq:
                    return DecodeComparisonBranch(opCode, ComparisonKind.Equality);
                case ILOpCode.Beq_s:
                    return DecodeComparisonBranch(opCode, ComparisonKind.Equality);
                case ILOpCode.Bge:
                    return DecodeComparisonBranch(opCode, ComparisonKind.GreaterThanOrEqual);
                case ILOpCode.Bge_s:
                    return DecodeComparisonBranch(opCode, ComparisonKind.GreaterThanOrEqual);
                case ILOpCode.Bge_un:
                    return DecodeComparisonBranch(opCode, ComparisonKind.GreaterThanOrEqual, un: true);
                case ILOpCode.Bge_un_s:
                    return DecodeComparisonBranch(opCode, ComparisonKind.GreaterThanOrEqual, un: true);
                case ILOpCode.Bgt:
                    return DecodeComparisonBranch(opCode, ComparisonKind.GreaterThan);
                case ILOpCode.Bgt_s:
                    return DecodeComparisonBranch(opCode, ComparisonKind.GreaterThan);
                case ILOpCode.Bgt_un:
                    return DecodeComparisonBranch(opCode, ComparisonKind.GreaterThan, un: true);
                case ILOpCode.Bgt_un_s:
                    return DecodeComparisonBranch(opCode, ComparisonKind.GreaterThan, un: true);
                case ILOpCode.Ble:
                    return DecodeComparisonBranch(opCode, ComparisonKind.LessThanOrEqual);
                case ILOpCode.Ble_s:
                    return DecodeComparisonBranch(opCode, ComparisonKind.LessThanOrEqual);
                case ILOpCode.Ble_un:
                    return DecodeComparisonBranch(opCode, ComparisonKind.LessThanOrEqual, un: true);
                case ILOpCode.Ble_un_s:
                    return DecodeComparisonBranch(opCode, ComparisonKind.LessThanOrEqual, un: true);
                case ILOpCode.Blt:
                    return DecodeComparisonBranch(opCode, ComparisonKind.LessThan);
                case ILOpCode.Blt_s:
                    return DecodeComparisonBranch(opCode, ComparisonKind.LessThan);
                case ILOpCode.Blt_un:
                    return DecodeComparisonBranch(opCode, ComparisonKind.LessThan, un: true);
                case ILOpCode.Blt_un_s:
                    return DecodeComparisonBranch(opCode, ComparisonKind.LessThan, un: true);
                case ILOpCode.Bne_un:
                    return DecodeComparisonBranch(opCode, ComparisonKind.Inequality, un: true);
                case ILOpCode.Bne_un_s:
                    return DecodeComparisonBranch(opCode, ComparisonKind.Inequality, un: true);
                case ILOpCode.Br:
                    return DecodeUnconditionalBranch(opCode);
                case ILOpCode.Br_s:
                    return DecodeUnconditionalBranch(opCode);
                case ILOpCode.Break:
                    return new DebugBreak();
                case ILOpCode.Brfalse:
                    return DecodeConditionalBranch(opCode, true);
                case ILOpCode.Brfalse_s:
                    return DecodeConditionalBranch(opCode, true);
                case ILOpCode.Brtrue:
                    return DecodeConditionalBranch(opCode, false);
                case ILOpCode.Brtrue_s:
                    return DecodeConditionalBranch(opCode, false);
                case ILOpCode.Call:
                    return DecodeCall(OpCode.Call);
                case ILOpCode.Callvirt:
                    return DecodeCall(OpCode.CallVirt);
                case ILOpCode.Calli:
                    return DecodeCallIndirect();
                case ILOpCode.Ceq:
                    return Push(Comparison(ComparisonKind.Equality));
                case ILOpCode.Cgt:
                    return Push(Comparison(ComparisonKind.GreaterThan));
                case ILOpCode.Cgt_un:
                    return Push(Comparison(ComparisonKind.GreaterThan, un: true));
                case ILOpCode.Clt:
                    return Push(Comparison(ComparisonKind.LessThan));
                case ILOpCode.Clt_un:
                    return Push(Comparison(ComparisonKind.LessThan, un: true));
                case ILOpCode.Ckfinite:
                    return new Ckfinite(Peek());
                case ILOpCode.Conv_i1:
                    return Push(new Conv(Pop(), PrimitiveType.I1, false, Sign.None));
                case ILOpCode.Conv_i2:
                    return Push(new Conv(Pop(), PrimitiveType.I2, false, Sign.None));
                case ILOpCode.Conv_i4:
                    return Push(new Conv(Pop(), PrimitiveType.I4, false, Sign.None));
                case ILOpCode.Conv_i8:
                    return Push(new Conv(Pop(), PrimitiveType.I8, false, Sign.None));
                case ILOpCode.Conv_r4:
                    return Push(new Conv(Pop(), PrimitiveType.R4, false, Sign.Signed));
                case ILOpCode.Conv_r8:
                    return Push(new Conv(Pop(), PrimitiveType.R8, false, Sign.Signed));
                case ILOpCode.Conv_u1:
                    return Push(new Conv(Pop(), PrimitiveType.U1, false, Sign.None));
                case ILOpCode.Conv_u2:
                    return Push(new Conv(Pop(), PrimitiveType.U2, false, Sign.None));
                case ILOpCode.Conv_u4:
                    return Push(new Conv(Pop(), PrimitiveType.U4, false, Sign.None));
                case ILOpCode.Conv_u8:
                    return Push(new Conv(Pop(), PrimitiveType.U8, false, Sign.None));
                case ILOpCode.Conv_i:
                    return Push(new Conv(Pop(), PrimitiveType.I, false, Sign.None));
                case ILOpCode.Conv_u:
                    return Push(new Conv(Pop(), PrimitiveType.U, false, Sign.None));
                case ILOpCode.Conv_r_un:
                    return Push(new Conv(Pop(), PrimitiveType.R, false, Sign.Unsigned));
                case ILOpCode.Conv_ovf_i1:
                    return Push(new Conv(Pop(), PrimitiveType.I1, true, Sign.Signed));
                case ILOpCode.Conv_ovf_i2:
                    return Push(new Conv(Pop(), PrimitiveType.I2, true, Sign.Signed));
                case ILOpCode.Conv_ovf_i4:
                    return Push(new Conv(Pop(), PrimitiveType.I4, true, Sign.Signed));
                case ILOpCode.Conv_ovf_i8:
                    return Push(new Conv(Pop(), PrimitiveType.I8, true, Sign.Signed));
                case ILOpCode.Conv_ovf_u1:
                    return Push(new Conv(Pop(), PrimitiveType.U1, true, Sign.Signed));
                case ILOpCode.Conv_ovf_u2:
                    return Push(new Conv(Pop(), PrimitiveType.U2, true, Sign.Signed));
                case ILOpCode.Conv_ovf_u4:
                    return Push(new Conv(Pop(), PrimitiveType.U4, true, Sign.Signed));
                case ILOpCode.Conv_ovf_u8:
                    return Push(new Conv(Pop(), PrimitiveType.U8, true, Sign.Signed));
                case ILOpCode.Conv_ovf_i:
                    return Push(new Conv(Pop(), PrimitiveType.I, true, Sign.Signed));
                case ILOpCode.Conv_ovf_u:
                    return Push(new Conv(Pop(), PrimitiveType.U, true, Sign.Signed));
                case ILOpCode.Conv_ovf_i1_un:
                    return Push(new Conv(Pop(), PrimitiveType.I1, true, Sign.Unsigned));
                case ILOpCode.Conv_ovf_i2_un:
                    return Push(new Conv(Pop(), PrimitiveType.I2, true, Sign.Unsigned));
                case ILOpCode.Conv_ovf_i4_un:
                    return Push(new Conv(Pop(), PrimitiveType.I4, true, Sign.Unsigned));
                case ILOpCode.Conv_ovf_i8_un:
                    return Push(new Conv(Pop(), PrimitiveType.I8, true, Sign.Unsigned));
                case ILOpCode.Conv_ovf_u1_un:
                    return Push(new Conv(Pop(), PrimitiveType.U1, true, Sign.Unsigned));
                case ILOpCode.Conv_ovf_u2_un:
                    return Push(new Conv(Pop(), PrimitiveType.U2, true, Sign.Unsigned));
                case ILOpCode.Conv_ovf_u4_un:
                    return Push(new Conv(Pop(), PrimitiveType.U4, true, Sign.Unsigned));
                case ILOpCode.Conv_ovf_u8_un:
                    return Push(new Conv(Pop(), PrimitiveType.U8, true, Sign.Unsigned));
                case ILOpCode.Conv_ovf_i_un:
                    return Push(new Conv(Pop(), PrimitiveType.I, true, Sign.Unsigned));
                case ILOpCode.Conv_ovf_u_un:
                    return Push(new Conv(Pop(), PrimitiveType.U, true, Sign.Unsigned));
                case ILOpCode.Cpblk:
                    // This preserves the evaluation order because the ILAst will run
                    // destAddress; sourceAddress; size.
                    return new Cpblk(size: Pop(StackType.I4), sourceAddress: PopPointer(), destAddress: PopPointer());
                case ILOpCode.Div:
                    return BinaryNumeric(BinaryNumericOperator.Div, false, Sign.Signed);
                case ILOpCode.Div_un:
                    return BinaryNumeric(BinaryNumericOperator.Div, false, Sign.Unsigned);
                case ILOpCode.Dup:
                    return Push(Peek());
                case ILOpCode.Endfilter:
                    return new Leave(null, Pop());
                case ILOpCode.Endfinally:
                    return new Leave(null);
                case ILOpCode.Initblk:
                    // This preserves the evaluation order because the ILAst will run
                    // address; value; size.
                    return new Initblk(size: Pop(StackType.I4), value: Pop(StackType.I4), address: PopPointer());
                case ILOpCode.Jmp:
                    return DecodeJmp();
                case ILOpCode.Ldarg:
                case ILOpCode.Ldarg_s:
                    return Push(Ldarg(_reader.DecodeIndex(opCode)));
                case ILOpCode.Ldarg_0:
                    return Push(Ldarg(0));
                case ILOpCode.Ldarg_1:
                    return Push(Ldarg(1));
                case ILOpCode.Ldarg_2:
                    return Push(Ldarg(2));
                case ILOpCode.Ldarg_3:
                    return Push(Ldarg(3));
                case ILOpCode.Ldarga:
                case ILOpCode.Ldarga_s:
                    return Push(Ldarga(_reader.DecodeIndex(opCode)));
                case ILOpCode.Ldc_i4:
                    return Push(new LdcI4(_reader.ReadInt32()));
                case ILOpCode.Ldc_i8:
                    return Push(new LdcI8(_reader.ReadInt64()));
                case ILOpCode.Ldc_r4:
                    return Push(new LdcF4(_reader.ReadSingle()));
                case ILOpCode.Ldc_r8:
                    return Push(new LdcF8(_reader.ReadDouble()));
                case ILOpCode.Ldc_i4_m1:
                    return Push(new LdcI4(-1));
                case ILOpCode.Ldc_i4_0:
                    return Push(new LdcI4(0));
                case ILOpCode.Ldc_i4_1:
                    return Push(new LdcI4(1));
                case ILOpCode.Ldc_i4_2:
                    return Push(new LdcI4(2));
                case ILOpCode.Ldc_i4_3:
                    return Push(new LdcI4(3));
                case ILOpCode.Ldc_i4_4:
                    return Push(new LdcI4(4));
                case ILOpCode.Ldc_i4_5:
                    return Push(new LdcI4(5));
                case ILOpCode.Ldc_i4_6:
                    return Push(new LdcI4(6));
                case ILOpCode.Ldc_i4_7:
                    return Push(new LdcI4(7));
                case ILOpCode.Ldc_i4_8:
                    return Push(new LdcI4(8));
                case ILOpCode.Ldc_i4_s:
                    return Push(new LdcI4(_reader.ReadSByte()));
                case ILOpCode.Ldnull:
                    return Push(new LdNull());
                case ILOpCode.Ldstr:
                    return Push(DecodeLdstr());
                case ILOpCode.Ldftn:
                    return Push(new LdFtn(ReadAndDecodeMethodReference()));
                case ILOpCode.Ldind_i1:
                    return Push(new LdObj(PopPointer(), typeof(sbyte)));
                case ILOpCode.Ldind_i2:
                    return Push(new LdObj(PopPointer(), typeof(short)));
                case ILOpCode.Ldind_i4:
                    return Push(new LdObj(PopPointer(), typeof(int)));
                case ILOpCode.Ldind_i8:
                    return Push(new LdObj(PopPointer(), typeof(long)));
                case ILOpCode.Ldind_u1:
                    return Push(new LdObj(PopPointer(), typeof(byte)));
                case ILOpCode.Ldind_u2:
                    return Push(new LdObj(PopPointer(), typeof(ushort)));
                case ILOpCode.Ldind_u4:
                    return Push(new LdObj(PopPointer(), typeof(uint)));
                case ILOpCode.Ldind_r4:
                    return Push(new LdObj(PopPointer(), typeof(float)));
                case ILOpCode.Ldind_r8:
                    return Push(new LdObj(PopPointer(), typeof(double)));
                case ILOpCode.Ldind_i:
                    return Push(new LdObj(PopPointer(), typeof(IntPtr)));
                case ILOpCode.Ldind_ref:
                    return Push(new LdObj(PopPointer(), typeof(object)));
                case ILOpCode.Ldloc:
                case ILOpCode.Ldloc_s:
                    return Push(Ldloc(_reader.DecodeIndex(opCode)));
                case ILOpCode.Ldloc_0:
                    return Push(Ldloc(0));
                case ILOpCode.Ldloc_1:
                    return Push(Ldloc(1));
                case ILOpCode.Ldloc_2:
                    return Push(Ldloc(2));
                case ILOpCode.Ldloc_3:
                    return Push(Ldloc(3));
                case ILOpCode.Ldloca:
                case ILOpCode.Ldloca_s:
                    return Push(Ldloca(_reader.DecodeIndex(opCode)));
                case ILOpCode.Leave:
                    return DecodeUnconditionalBranch(opCode, isLeave: true);
                case ILOpCode.Leave_s:
                    return DecodeUnconditionalBranch(opCode, isLeave: true);
                case ILOpCode.Localloc:
                    return Push(new LocAlloc(Pop()));
                case ILOpCode.Mul:
                    return BinaryNumeric(BinaryNumericOperator.Mul, false, Sign.None);
                case ILOpCode.Mul_ovf:
                    return BinaryNumeric(BinaryNumericOperator.Mul, true, Sign.Signed);
                case ILOpCode.Mul_ovf_un:
                    return BinaryNumeric(BinaryNumericOperator.Mul, true, Sign.Unsigned);
                case ILOpCode.Neg:
                    return Neg();
                case ILOpCode.Newobj:
                    return DecodeCall(OpCode.NewObj);
                case ILOpCode.Nop:
                    return new Nop();
                case ILOpCode.Not:
                    return Push(new BitNot(Pop()));
                case ILOpCode.Or:
                    return BinaryNumeric(BinaryNumericOperator.BitOr);
                case ILOpCode.Pop:
                    FlushExpressionStack(); // discard only the value, not the side-effects
                    Pop();
                    return new Nop() { Kind = NopKind.Pop };
                case ILOpCode.Rem:
                    return BinaryNumeric(BinaryNumericOperator.Rem, false, Sign.Signed);
                case ILOpCode.Rem_un:
                    return BinaryNumeric(BinaryNumericOperator.Rem, false, Sign.Unsigned);
                case ILOpCode.Ret:
                    return Return();
                case ILOpCode.Shl:
                    return BinaryNumeric(BinaryNumericOperator.ShiftLeft, false, Sign.None);
                case ILOpCode.Shr:
                    return BinaryNumeric(BinaryNumericOperator.ShiftRight, false, Sign.Signed);
                case ILOpCode.Shr_un:
                    return BinaryNumeric(BinaryNumericOperator.ShiftRight, false, Sign.Unsigned);
                case ILOpCode.Starg:
                case ILOpCode.Starg_s:
                    return Starg(_reader.DecodeIndex(opCode));
                case ILOpCode.Stind_i1:
                    // target will run before value, thus preserving the evaluation order
                    return new StObj(value: Pop(StackType.I4), target: PopStObjTarget(), type: typeof(sbyte));
                case ILOpCode.Stind_i2:
                    return new StObj(value: Pop(StackType.I4), target: PopStObjTarget(), type: typeof(short));
                case ILOpCode.Stind_i4:
                    return new StObj(value: Pop(StackType.I4), target: PopStObjTarget(), type: typeof(int));
                case ILOpCode.Stind_i8:
                    return new StObj(value: Pop(StackType.I8), target: PopStObjTarget(), type: typeof(long));
                case ILOpCode.Stind_r4:
                    return new StObj(value: Pop(StackType.F4), target: PopStObjTarget(), type: typeof(float));
                case ILOpCode.Stind_r8:
                    return new StObj(value: Pop(StackType.F8), target: PopStObjTarget(), type: typeof(double));
                case ILOpCode.Stind_i:
                    return new StObj(value: Pop(StackType.I), target: PopStObjTarget(), type: typeof(IntPtr));
                case ILOpCode.Stind_ref:
                    return new StObj(value: Pop(StackType.O), target: PopStObjTarget(), type: typeof(object));
                case ILOpCode.Stloc:
                case ILOpCode.Stloc_s:
                    return Stloc(_reader.DecodeIndex(opCode));
                case ILOpCode.Stloc_0:
                    return Stloc(0);
                case ILOpCode.Stloc_1:
                    return Stloc(1);
                case ILOpCode.Stloc_2:
                    return Stloc(2);
                case ILOpCode.Stloc_3:
                    return Stloc(3);
                case ILOpCode.Sub:
                    return BinaryNumeric(BinaryNumericOperator.Sub, false, Sign.None);
                case ILOpCode.Sub_ovf:
                    return BinaryNumeric(BinaryNumericOperator.Sub, true, Sign.Signed);
                case ILOpCode.Sub_ovf_un:
                    return BinaryNumeric(BinaryNumericOperator.Sub, true, Sign.Unsigned);
                case ILOpCode.Switch:
                    return DecodeSwitch();
                case ILOpCode.Xor:
                    return BinaryNumeric(BinaryNumericOperator.BitXor);
                case ILOpCode.Box:
                    {
                        var type = ReadAndDecodeTypeReference();
                        return Push(new Box(Pop(type.GetStackType()), type));
                    }
                case ILOpCode.Castclass:
                    return Push(new CastClass(Pop(StackType.O), ReadAndDecodeTypeReference()));
                case ILOpCode.Cpobj:
                    {
                        var type = ReadAndDecodeTypeReference();
                        // OK, 'target' runs before 'value: ld'
                        var ld = new LdObj(PopPointer(), type);
                        return new StObj(PopStObjTarget(), ld, type);
                    }
                case ILOpCode.Initobj:
                    return InitObj(PopStObjTarget(), ReadAndDecodeTypeReference());
                case ILOpCode.Isinst:
                    return Push(new IsInst(Pop(StackType.O), ReadAndDecodeTypeReference()));
                case ILOpCode.Ldelem:
                    return LdElem(ReadAndDecodeTypeReference());
                case ILOpCode.Ldelem_i1:
                    return LdElem(typeof(sbyte));
                case ILOpCode.Ldelem_i2:
                    return LdElem(typeof(short));
                case ILOpCode.Ldelem_i4:
                    return LdElem(typeof(int));
                case ILOpCode.Ldelem_i8:
                    return LdElem(typeof(long));
                case ILOpCode.Ldelem_u1:
                    return LdElem(typeof(byte));
                case ILOpCode.Ldelem_u2:
                    return LdElem(typeof(ushort));
                case ILOpCode.Ldelem_u4:
                    return LdElem(typeof(uint));
                case ILOpCode.Ldelem_r4:
                    return LdElem(typeof(float));
                case ILOpCode.Ldelem_r8:
                    return LdElem(typeof(double));
                case ILOpCode.Ldelem_i:
                    return LdElem(typeof(IntPtr));
                case ILOpCode.Ldelem_ref:
                    return LdElem(typeof(object));
                case ILOpCode.Ldelema:
                    // LdElema will evalute the array before the indices, so we're preserving the evaluation order
                    return Push(new LdElema(indices: Pop(), array: Pop(), type: ReadAndDecodeTypeReference()));
                case ILOpCode.Ldfld:
                    {
                        var field = ReadAndDecodeFieldReference();
                        return Push(new LdObj(new LdFlda(PopLdFldTarget(field), field) { DelayExceptions = true }, field.FieldType));
                    }
                case ILOpCode.Ldflda:
                    {
                        var field = ReadAndDecodeFieldReference();
                        return Push(new LdFlda(PopFieldTarget(field), field));
                    }
                case ILOpCode.Stfld:
                    {
                        var field = ReadAndDecodeFieldReference();
                        return new StObj(value: Pop(field.FieldType.GetStackType()), target: new LdFlda(PopFieldTarget(field), field) { DelayExceptions = true }, type: field.FieldType);
                    }
                case ILOpCode.Ldlen:
                    return Push(new LdLen(StackType.I, Pop(StackType.O)));
                case ILOpCode.Ldobj:
                    return Push(new LdObj(PopPointer(), ReadAndDecodeTypeReference()));
                case ILOpCode.Ldsfld:
                    {
                        var field = ReadAndDecodeFieldReference();
                        return Push(new LdObj(new LdsFlda(field), field.FieldType));
                    }
                case ILOpCode.Ldsflda:
                    return Push(new LdsFlda(ReadAndDecodeFieldReference()));
                case ILOpCode.Stsfld:
                    {
                        var field = ReadAndDecodeFieldReference();
                        return new StObj(value: Pop(field.FieldType.GetStackType()), target: new LdsFlda(field), type: field.FieldType);
                    }
                case ILOpCode.Ldtoken:
                    return Push(LdToken(ReadAndDecodeMetadataToken()));
                case ILOpCode.Ldvirtftn:
                    return Push(new LdVirtFtn(Pop(), ReadAndDecodeMethodReference()));
                case ILOpCode.Mkrefany:
                    return Push(new MakeRefAny(PopPointer(), ReadAndDecodeTypeReference()));
                case ILOpCode.Newarr:
                    return Push(new NewArr(ReadAndDecodeTypeReference(), Pop()));
                case ILOpCode.Refanytype:
                    return Push(new RefAnyType(Pop()));
                case ILOpCode.Refanyval:
                    return Push(new RefAnyValue(Pop(), ReadAndDecodeTypeReference()));
                case ILOpCode.Rethrow:
                    return new Rethrow();
                case ILOpCode.Sizeof:
                    return Push(new SizeOf(ReadAndDecodeTypeReference()));
                case ILOpCode.Stelem:
                    return StElem(ReadAndDecodeTypeReference());
                case ILOpCode.Stelem_i1:
                    return StElem(typeof(sbyte));
                case ILOpCode.Stelem_i2:
                    return StElem(typeof(short));
                case ILOpCode.Stelem_i4:
                    return StElem(typeof(int));
                case ILOpCode.Stelem_i8:
                    return StElem(typeof(long));
                case ILOpCode.Stelem_r4:
                    return StElem(typeof(float));
                case ILOpCode.Stelem_r8:
                    return StElem(typeof(double));
                case ILOpCode.Stelem_i:
                    return StElem(typeof(IntPtr));
                case ILOpCode.Stelem_ref:
                    return StElem(typeof(object));
                case ILOpCode.Stobj:
                    {
                        var type = ReadAndDecodeTypeReference();
                        // OK, target runs before value
                        return new StObj(value: Pop(type.GetStackType()), target: PopStObjTarget(), type: type);
                    }
                case ILOpCode.Throw:
                    return new Throw(Pop());
                case ILOpCode.Unbox:
                    return Push(new Unbox(Pop(), ReadAndDecodeTypeReference()));
                case ILOpCode.Unbox_any:
                    return Push(new UnboxAny(Pop(), ReadAndDecodeTypeReference()));
                default:
                    return new InvalidBranch($"Unknown opcode: 0x{(int)opCode:X2}");
            }
        }

        StackType PeekStackType()
        {
            if (_expressionStack.Count > 0)
                return _expressionStack[^1].ResultType;
            if (_currentStack.IsEmpty)
                return StackType.Unknown;
            else
                return _currentStack.Peek().StackType;
        }

        DecodedInstruction Push(ILInstruction inst)
        {
            _expressionStack.Add(inst);
            return new DecodedInstruction
            {
                Instruction = inst,
                PushedOnExpressionStack = true
            };
        }

        ILInstruction Peek()
        {
            FlushExpressionStack();
            if (_currentStack.IsEmpty)
            {
                int streamPosition = (int)_reader.BaseStream.Position;
                return new InvalidExpression("Stack underflow").WithILRange(new Interval(streamPosition, streamPosition));
            }
            return new LdLoc(_currentStack.Peek());
        }

        /// <summary>
        /// Pops a value/instruction from the evaluation stack.
        /// Note that instructions popped from the stack must be evaluated in the order they
        /// were pushed (so in reverse order of the pop calls!).
        /// 
        /// For instructions like 'conv' that pop a single element and then push their result,
        /// it's fine to pop just one element as the instruction itself will end up on the stack,
        /// thus maintaining the evaluation order.
        /// For instructions like 'call' that pop multiple arguments, it's critical that
        /// the evaluation order of the resulting ILAst will be reverse from the order of the push
        /// calls.
        /// For instructions like 'brtrue', it's fine to pop only a part of the stack because
        /// ReadInstructions() will flush the evaluation stack before outputting the brtrue instruction.
        /// 
        /// Use FlushExpressionStack() to ensure that following Pop() calls do not return
        /// instructions that involve side-effects. This way evaluation order is preserved
        /// no matter which order the ILAst will execute the popped instructions in.
        /// </summary>
        ILInstruction Pop()
        {
            if (_expressionStack.Count > 0)
            {
                var inst = _expressionStack[^1];
                _expressionStack.RemoveAt(_expressionStack.Count - 1);
                return inst;
            }
            if (_currentStack.IsEmpty)
            {
                int position = (int)_reader.BaseStream.Position;
                return new InvalidExpression("Stack underflow").WithILRange(new Interval(position, position));
            }

            _currentStack = _currentStack.Pop(out var v);
            return new LdLoc(v);
        }

        ILInstruction Pop(StackType expectedType)
        {
            ILInstruction inst = Pop();
            return Cast(inst, expectedType, Warnings, (int)_reader.BaseStream.Position);
        }

        internal static ILInstruction Cast(ILInstruction inst, StackType expectedType, List<string> warnings, int ilOffset)
        {
            if (expectedType != inst.ResultType)
            {
                if (inst is InvalidExpression)
                {
                    ((InvalidExpression)inst).ExpectedResultType = expectedType;
                }
                else if (expectedType == StackType.I && inst.ResultType == StackType.I4)
                {
                    // IL allows implicit I4->I conversions
                    inst = new Conv(inst, PrimitiveType.I, false, Sign.None);
                }
                else if (expectedType == StackType.I4 && inst.ResultType == StackType.I)
                {
                    // C++/CLI also sometimes implicitly converts in the other direction:
                    inst = new Conv(inst, PrimitiveType.I4, false, Sign.None);
                }
                else if (expectedType == StackType.Unknown)
                {
                    inst = new Conv(inst, PrimitiveType.Unknown, false, Sign.None);
                }
                else if (inst.ResultType == StackType.Ref)
                {
                    // Implicitly stop GC tracking; this occurs when passing the result of 'ldloca' or 'ldsflda'
                    // to a method expecting a native pointer.
                    inst = new Conv(inst, PrimitiveType.I, false, Sign.None);
                    switch (expectedType)
                    {
                        case StackType.I4:
                            inst = new Conv(inst, PrimitiveType.I4, false, Sign.None);
                            break;
                        case StackType.I:
                            break;
                        case StackType.I8:
                            inst = new Conv(inst, PrimitiveType.I8, false, Sign.None);
                            break;
                        default:
                            Warn($"Expected {expectedType}, but got {StackType.Ref}");
                            inst = new Conv(inst, expectedType.ToPrimitiveType(), false, Sign.None);
                            break;
                    }
                }
                else if (expectedType == StackType.Ref)
                {
                    // implicitly start GC tracking / object to interior
                    if (!inst.ResultType.IsIntegerType() && inst.ResultType != StackType.O)
                    {
                        // We also handle the invalid to-ref cases here because the else case
                        // below uses expectedType.ToKnownTypeCode(), which doesn't work for Ref.
                        Warn($"Expected {expectedType}, but got {inst.ResultType}");
                    }
                    inst = new Conv(inst, PrimitiveType.Ref, false, Sign.None);
                }
                else if (expectedType == StackType.F8 && inst.ResultType == StackType.F4)
                {
                    // IL allows implicit F4->F8 conversions, because in IL F4 and F8 are the same.
                    inst = new Conv(inst, PrimitiveType.R8, false, Sign.Signed);
                }
                else if (expectedType == StackType.F4 && inst.ResultType == StackType.F8)
                {
                    // IL allows implicit F8->F4 conversions, because in IL F4 and F8 are the same.
                    inst = new Conv(inst, PrimitiveType.R4, false, Sign.Signed);
                }
                else
                {
                    Warn($"Expected {expectedType}, but got {inst.ResultType}");
                    inst = new Conv(inst, expectedType.ToPrimitiveType(), false, Sign.Signed);
                }
            }
            return inst;

            void Warn(string message)
            {
                if (warnings != null)
                {
                    warnings.Add($"IL_{ilOffset:x4}: {message}");
                }
            }
        }

        ILInstruction PopPointer()
        {
            ILInstruction inst = Pop();
            switch (inst.ResultType)
            {
                case StackType.I4:
                case StackType.I8:
                case StackType.Unknown:
                    return new Conv(inst, PrimitiveType.I, false, Sign.None);
                case StackType.I:
                case StackType.Ref:
                    return inst;
                default:
                    Warn("Expected native int or pointer, but got " + inst.ResultType);
                    return new Conv(inst, PrimitiveType.I, false, Sign.None);
            }
        }

        ILInstruction PopStObjTarget()
        {
            // stobj has a special invariant (StObj.CheckTargetSlot)
            // that prohibits inlining LdElema/LdFlda.
            if (_expressionStack.LastOrDefault() is LdElema or LdFlda)
            {
                FlushExpressionStack();
            }
            return PopPointer();
        }

        ILInstruction PopFieldTarget(FieldInfo field)
        {
            switch (field.DeclaringType?.IsValueType)
            {
                case false:
                    return Pop(StackType.O);
                case true:
                    return PopPointer();
                default:
                    // field in unresolved type
                    var stackType = PeekStackType();
                    if (stackType == StackType.O || stackType == StackType.Unknown)
                        return Pop();
                    else
                        return PopPointer();
            }
        }

        /// <summary>
        /// Like PopFieldTarget, but supports ldfld's special behavior for fields of temporary value types.
        /// </summary>
        ILInstruction PopLdFldTarget(FieldInfo field)
        {
            switch (field.DeclaringType?.IsValueType)
            {
                case false:
                    return Pop(StackType.O);
                case true:
                    // field of value type: ldfld can handle temporaries
                    if (PeekStackType() == StackType.O || PeekStackType() == StackType.Unknown)
                        return new AddressOf(Pop(), field.DeclaringType);
                    else
                        return PopPointer();
                default:
                    // field in unresolved type
                    if (PeekStackType() == StackType.O || PeekStackType() == StackType.Unknown)
                        return Pop();
                    else
                        return PopPointer();
            }
        }

        private ILInstruction Return()
        {
            if (_methodReturnStackType == StackType.Void)
                return new Leave(_mainContainer);
            else
                return new Leave(_mainContainer, Pop(_methodReturnStackType));
        }

        private ILInstruction DecodeLdstr()
        {
            throw new NotImplementedException();
            //return new LdStr(reader.DecodeUserString(metadata));
        }

        private ILInstruction Ldarg(int v)
        {
            if (v >= 0 && v < _parameterVariables.Length)
            {
                return new LdLoc(_parameterVariables[v]);
            }
            else
            {
                return new InvalidExpression($"ldarg {v} (out-of-bounds)");
            }
        }

        private ILInstruction Ldarga(int v)
        {
            if (v >= 0 && v < _parameterVariables.Length)
            {
                return new LdLoca(_parameterVariables[v]);
            }
            else
            {
                return new InvalidExpression($"ldarga {v} (out-of-bounds)");
            }
        }

        private ILInstruction Starg(int v)
        {
            if (v >= 0 && v < _parameterVariables.Length)
            {
                return new StLoc(_parameterVariables[v], Pop(_parameterVariables[v].StackType));
            }
            else
            {
                FlushExpressionStack();
                Pop();
                return new InvalidExpression($"starg {v} (out-of-bounds)");
            }
        }

        private ILInstruction Ldloc(int v)
        {
            if (v >= 0 && v < _localVariables.Length)
            {
                return new LdLoc(_localVariables[v]);
            }
            else
            {
                return new InvalidExpression($"ldloc {v} (out-of-bounds)");
            }
        }

        private ILInstruction Ldloca(int v)
        {
            if (v >= 0 && v < _localVariables.Length)
            {
                return new LdLoca(_localVariables[v]);
            }
            else
            {
                return new InvalidExpression($"ldloca {v} (out-of-bounds)");
            }
        }

        private ILInstruction Stloc(int v)
        {
            if (v >= 0 && v < _localVariables.Length)
            {
                return new StLoc(_localVariables[v], Pop(_localVariables[v].StackType))
                {
                    ILStackWasEmpty = CurrentStackIsEmpty()
                };
            }
            else
            {
                FlushExpressionStack();
                Pop();
                return new InvalidExpression($"stloc {v} (out-of-bounds)");
            }
        }

        private DecodedInstruction LdElem(Type type)
        {
            return Push(new LdObj(new LdElema(indices: Pop(), array: Pop(), type: type) { DelayExceptions = true }, type));
        }

        private ILInstruction StElem(Type type)
        {
            var value = Pop(type.GetStackType());
            var index = Pop();
            var array = Pop();
            // OK, evaluation order is array, index, value
            return new StObj(new LdElema(type, array, index) { DelayExceptions = true }, value, type);
        }

        ILInstruction InitObj(ILInstruction target, Type type)
        {
            var value = new DefaultValue(type)
            {
                ILStackWasEmpty = CurrentStackIsEmpty()
            };
            return new StObj(target, value, type);
        }

        private DecodedInstruction DecodeConstrainedCall()
        {
            _constrainedPrefix = ReadAndDecodeTypeReference();
            var inst = DecodeInstruction();
            if (inst.Instruction is CallInstruction call)
                Debug.Assert(call.ConstrainedTo == _constrainedPrefix);
            else
                Warn("Ignored invalid 'constrained' prefix");
            _constrainedPrefix = null;
            return inst;
        }

        private DecodedInstruction DecodeTailCall()
        {
            var inst = DecodeInstruction();
            if (inst.Instruction is CallInstruction call)
                call.IsTail = true;
            else
                Warn("Ignored invalid 'tail' prefix");
            return inst;
        }

        private DecodedInstruction DecodeUnaligned()
        {
            byte alignment = _reader.ReadByte();
            var inst = DecodeInstruction();
            if (inst.Instruction is ISupportsUnalignedPrefix sup)
                sup.UnalignedPrefix = alignment;
            else
                Warn("Ignored invalid 'unaligned' prefix");
            return inst;
        }

        private DecodedInstruction DecodeVolatile()
        {
            var inst = DecodeInstruction();
            if (inst.Instruction is ISupportsVolatilePrefix svp)
                svp.IsVolatile = true;
            else
                Warn("Ignored invalid 'volatile' prefix");
            return inst;
        }

        private DecodedInstruction DecodeReadonly()
        {
            var inst = DecodeInstruction();
            if (inst.Instruction is LdElema ldelema)
                ldelema.IsReadOnly = true;
            else
                Warn("Ignored invalid 'readonly' prefix");
            return inst;
        }

        DecodedInstruction DecodeCall(OpCode opCode)
        {
            var method = ReadAndDecodeMethodReference();
            ILInstruction[] arguments;
            if (method.DeclaringType.IsArray)
            {
                arguments = PrepareArguments(firstArgumentIsStObjTarget: false);
                var elementType = method.DeclaringType.GetElementType();
                if (opCode == OpCode.NewObj)
                    return Push(new NewArr(elementType, arguments));
                if (method.Name == "Set")
                {
                    var target = arguments[0];
                    var indices = arguments.Skip(1).Take(arguments.Length - 2).ToArray();
                    var value = arguments[^1];
                    // preserves evaluation order target,indices,value
                    return new StObj(new LdElema(elementType, target, indices) { DelayExceptions = true }, value, elementType);
                }
                if (method.Name == "Get")
                {
                    var target = arguments[0];
                    var indices = arguments.Skip(1).ToArray();
                    // preserves evaluation order target,indices
                    return Push(new LdObj(new LdElema(elementType, target, indices) { DelayExceptions = true }, elementType));
                }
                if (method.Name == "Address")
                {
                    var target = arguments[0];
                    var indices = arguments.Skip(1).ToArray();
                    // preserves evaluation order target,indices
                    return Push(new LdElema(elementType, target, indices));
                }
                Warn("Unknown method called on array type: " + method.Name);
            }

            if (method.DeclaringType.IsValueType &&
                method.IsConstructor &&
                !method.IsStatic &&
                opCode == OpCode.Call &&
                method is MethodInfo info &&
                info.ReturnType == typeof(void))
            {
                // "call Struct.ctor(target, ...)" doesn't exist in C#,
                // the next best equivalent is an assignment `*target = new Struct(...);`.
                // So we represent this call as "stobj Struct(target, newobj Struct.ctor(...))".
                // This needs to happen early (not as a transform) because the StObj.TargetSlot has
                // restricted inlining (doesn't accept ldflda when exceptions aren't delayed).
                arguments = PrepareArguments(firstArgumentIsStObjTarget: true);
                var newobj = new NewObj(method)
                {
                    ILStackWasEmpty = CurrentStackIsEmpty(),
                    ConstrainedTo = _constrainedPrefix
                };
                newobj.Arguments.AddRange(arguments.Skip(1));
                return new StObj(arguments[0], newobj, method.DeclaringType);
            }

            arguments = PrepareArguments(firstArgumentIsStObjTarget: false);
            var call = CallInstruction.Create(opCode, method);
            call.ILStackWasEmpty = CurrentStackIsEmpty();
            call.ConstrainedTo = _constrainedPrefix;
            call.Arguments.AddRange(arguments);
            return call.ResultType != StackType.Void ? Push(call) : call;

            ILInstruction[] PrepareArguments(bool firstArgumentIsStObjTarget)
            {
                int firstArgument = opCode != OpCode.NewObj && !method.IsStatic ? 1 : 0;
                var parameterInfos = method.GetParameters();
                var arguments = new ILInstruction[firstArgument + parameterInfos.Length];
                for (int i = parameterInfos.Length - 1; i >= 0; i--)
                {
                    arguments[firstArgument + i] = Pop(parameterInfos[i].ParameterType.GetStackType());
                }
                if (firstArgument == 1)
                {
                    arguments[0] = firstArgumentIsStObjTarget
                        ? PopStObjTarget()
                        : Pop(CallInstruction.ExpectedTypeForThisPointer(_constrainedPrefix));
                }
                // arguments is in reverse order of the Pop calls, thus
                // arguments is now in the correct evaluation order.
                return arguments;
            }
        }

        DecodedInstruction DecodeCallIndirect()
        {
            throw new NotImplementedException();
            //var signatureHandle = (StandaloneSignatureHandle)ReadAndDecodeMetadataToken();
            //var (header, fpt) = module.DecodeMethodSignature(signatureHandle, genericContext);
            //var functionPointer = Pop(StackType.I);
            //int firstArgument = header.IsInstance ? 1 : 0;
            //var arguments = new ILInstruction[firstArgument + fpt.ParameterTypes.Length];
            //for (int i = fpt.ParameterTypes.Length - 1; i >= 0; i--)
            //{
            //	arguments[firstArgument + i] = Pop(fpt.ParameterTypes[i].GetStackType());
            //}
            //if (firstArgument == 1)
            //{
            //	arguments[0] = Pop();
            //}
            //// arguments is in reverse order of the Pop calls, thus
            //// arguments is now in the correct evaluation order.
            //var call = new CallIndirect(
            //	header.IsInstance,
            //	header.HasExplicitThis,
            //	fpt,
            //	functionPointer,
            //	arguments
            //);
            //if (call.ResultType != StackType.Void)
            //	return Push(call);
            //else
            //	return call;
        }

        ILInstruction Comparison(ComparisonKind kind, bool un = false)
        {
            var right = Pop();
            var left = Pop();
            // left will run before right, thus preserving the evaluation order

            if ((left.ResultType == StackType.O || left.ResultType == StackType.Ref) && right.ResultType.IsIntegerType())
            {
                // C++/CLI sometimes compares object references with integers.
                // Also happens with Ref==I in Unsafe.IsNullRef().
                if (right.ResultType == StackType.I4)
                {
                    // ensure we compare at least native integer size
                    right = new Conv(right, PrimitiveType.I, false, Sign.None);
                }
                left = new Conv(left, right.ResultType.ToPrimitiveType(), false, Sign.None);
            }
            else if ((right.ResultType == StackType.O || right.ResultType == StackType.Ref) && left.ResultType.IsIntegerType())
            {
                if (left.ResultType == StackType.I4)
                {
                    left = new Conv(left, PrimitiveType.I, false, Sign.None);
                }
                right = new Conv(right, left.ResultType.ToPrimitiveType(), false, Sign.None);
            }

            // make implicit integer conversions explicit:
            MakeExplicitConversion(sourceType: StackType.I4, targetType: StackType.I, conversionType: PrimitiveType.I);
            MakeExplicitConversion(sourceType: StackType.I4, targetType: StackType.I8, conversionType: PrimitiveType.I8);
            MakeExplicitConversion(sourceType: StackType.I, targetType: StackType.I8, conversionType: PrimitiveType.I8);

            // Based on Table 4: Binary Comparison or Branch Operation
            if (left.ResultType.IsFloatType() && right.ResultType.IsFloatType())
            {
                if (left.ResultType != right.ResultType)
                {
                    // make the implicit F4->F8 conversion explicit:
                    MakeExplicitConversion(StackType.F4, StackType.F8, PrimitiveType.R8);
                }
                if (un)
                {
                    // for floats, 'un' means 'unordered'
                    return Comp.LogicNot(new Comp(kind.Negate(), Sign.None, left, right));
                }
                else
                {
                    return new Comp(kind, Sign.None, left, right);
                }
            }
            else if (left.ResultType.IsIntegerType() && right.ResultType.IsIntegerType() && !kind.IsEqualityOrInequality())
            {
                // integer comparison where the sign matters
                Debug.Assert(right.ResultType.IsIntegerType());
                return new Comp(kind, un ? Sign.Unsigned : Sign.Signed, left, right);
            }
            else if (left.ResultType == right.ResultType)
            {
                // integer equality, object reference or managed reference comparison
                return new Comp(kind, Sign.None, left, right);
            }
            else
            {
                Warn($"Invalid comparison between {left.ResultType} and {right.ResultType}");
                if (left.ResultType < right.ResultType)
                {
                    left = new Conv(left, right.ResultType.ToPrimitiveType(), false, Sign.Signed);
                }
                else
                {
                    right = new Conv(right, left.ResultType.ToPrimitiveType(), false, Sign.Signed);
                }
                return new Comp(kind, Sign.None, left, right);
            }

            void MakeExplicitConversion(StackType sourceType, StackType targetType, PrimitiveType conversionType)
            {
                if (left.ResultType == sourceType && right.ResultType == targetType)
                {
                    left = new Conv(left, conversionType, false, Sign.None);
                }
                else if (left.ResultType == targetType && right.ResultType == sourceType)
                {
                    right = new Conv(right, conversionType, false, Sign.None);
                }
            }
        }

        bool IsInvalidBranch(int target) => target < 0 || target >= _reader.BaseStream.Length;

        ILInstruction DecodeComparisonBranch(ILOpCode opCode, ComparisonKind kind, bool un = false)
        {
            int start = (int)_reader.BaseStream.Position - 1; // opCode is always one byte in this case
            int target = _reader.DecodeBranchTarget(opCode);
            var condition = Comparison(kind, un);
            condition.AddILRange(new Interval(start, (int)_reader.BaseStream.Position));
            if (!IsInvalidBranch(target))
            {
                MarkBranchTarget(target);
                return new IfInstruction(condition, new Branch(target));
            }
            else
            {
                return new IfInstruction(condition, new InvalidBranch("Invalid branch target"));
            }
        }

        ILInstruction DecodeConditionalBranch(ILOpCode opCode, bool negate)
        {
            int target = _reader.DecodeBranchTarget(opCode);
            ILInstruction condition = Pop();
            switch (condition.ResultType)
            {
                case StackType.O:
                    // introduce explicit comparison with null
                    condition = new Comp(
                        negate ? ComparisonKind.Equality : ComparisonKind.Inequality,
                        Sign.None, condition, new LdNull());
                    break;
                case StackType.I:
                    // introduce explicit comparison with 0
                    condition = new Comp(
                        negate ? ComparisonKind.Equality : ComparisonKind.Inequality,
                        Sign.None, condition, new Conv(new LdcI4(0), PrimitiveType.I, false, Sign.None));
                    break;
                case StackType.I8:
                    // introduce explicit comparison with 0
                    condition = new Comp(
                        negate ? ComparisonKind.Equality : ComparisonKind.Inequality,
                        Sign.None, condition, new LdcI8(0));
                    break;
                case StackType.Ref:
                    // introduce explicit comparison with null ref
                    condition = new Comp(
                        negate ? ComparisonKind.Equality : ComparisonKind.Inequality,
                        Sign.None, new Conv(condition, PrimitiveType.I, false, Sign.None), new Conv(new LdcI4(0), PrimitiveType.I, false, Sign.None));
                    break;
                case StackType.I4:
                    if (negate)
                    {
                        condition = Comp.LogicNot(condition);
                    }
                    break;
                default:
                    condition = new Conv(condition, PrimitiveType.I4, false, Sign.None);
                    if (negate)
                    {
                        condition = Comp.LogicNot(condition);
                    }
                    break;
            }
            if (!IsInvalidBranch(target))
            {
                MarkBranchTarget(target);
                return new IfInstruction(condition, new Branch(target));
            }
            else
            {
                return new IfInstruction(condition, new InvalidBranch("Invalid branch target"));
            }
        }

        ILInstruction DecodeUnconditionalBranch(ILOpCode opCode, bool isLeave = false)
        {
            int target = _reader.DecodeBranchTarget(opCode);
            if (isLeave)
            {
                FlushExpressionStack();
                _currentStack = _currentStack.Clear();
            }
            if (!IsInvalidBranch(target))
            {
                MarkBranchTarget(target);
                return new Branch(target);
            }
            else
            {
                return new InvalidBranch("Invalid branch target");
            }
        }

        void MarkBranchTarget(int targetILOffset)
        {
            FlushExpressionStack();
            Debug.Assert(_isBranchTarget[targetILOffset]);
            StoreStackForOffset(targetILOffset, ref _currentStack);
        }

        /// <summary>
        /// The expression stack holds ILInstructions that might have side-effects
        /// that should have already happened (in the order of the pushes).
        /// This method forces these instructions to be added to the instructionBuilder.
        /// This is used e.g. to avoid moving side-effects past branches.
        /// </summary>
        private void FlushExpressionStack()
        {
            foreach (var inst in _expressionStack)
            {
                Debug.Assert(inst.ResultType != StackType.Void);
                var type = inst.ResultType.ToType();
                var v = new ILVariable(VariableKind.StackSlot, type, inst.ResultType)
                {
                    HasGeneratedName = true
                };
                _currentStack = _currentStack.Push(v);
                _instructionBuilder.Add(new StLoc(v, inst).WithILRange(inst));
            }
            _expressionStack.Clear();
        }

        ILInstruction DecodeSwitch()
        {
            var targets = _reader.DecodeSwitchTargets();
            var instr = new SwitchInstruction(Pop(StackType.I4));

            for (int i = 0; i < targets.Length; i++)
            {
                var section = new SwitchSection
                {
                    Labels = new LongSet(i)
                };
                int target = targets[i];
                if (!IsInvalidBranch(target))
                {
                    MarkBranchTarget(target);
                    section.Body = new Branch(target);
                }
                else
                {
                    section.Body = new InvalidBranch("Invalid branch target");
                }
                instr.Sections.Add(section);
            }
            var defaultSection = new SwitchSection
            {
                Labels = new LongSet(new LongInterval(0, targets.Length)).Invert(),
                Body = new Nop()
            };
            instr.Sections.Add(defaultSection);
            return instr;
        }

        DecodedInstruction BinaryNumeric(BinaryNumericOperator @operator, bool checkForOverflow = false, Sign sign = Sign.None)
        {
            var right = Pop();
            var left = Pop();
            // left will run before right, thus preserving the evaluation order
            if (@operator != BinaryNumericOperator.Add && @operator != BinaryNumericOperator.Sub)
            {
                // we are treating all Refs as I, make the conversion explicit
                if (left.ResultType == StackType.Ref)
                {
                    left = new Conv(left, PrimitiveType.I, false, Sign.None);
                }
                if (right.ResultType == StackType.Ref)
                {
                    right = new Conv(right, PrimitiveType.I, false, Sign.None);
                }
            }
            if (@operator != BinaryNumericOperator.ShiftLeft && @operator != BinaryNumericOperator.ShiftRight)
            {
                // make the implicit I4->I conversion explicit:
                MakeExplicitConversion(sourceType: StackType.I4, targetType: StackType.I, conversionType: PrimitiveType.I);
                // I4->I8 conversion:
                MakeExplicitConversion(sourceType: StackType.I4, targetType: StackType.I8, conversionType: PrimitiveType.I8);
                // I->I8 conversion:
                MakeExplicitConversion(sourceType: StackType.I, targetType: StackType.I8, conversionType: PrimitiveType.I8);
                // F4->F8 conversion:
                MakeExplicitConversion(sourceType: StackType.F4, targetType: StackType.F8, conversionType: PrimitiveType.R8);
            }
            return Push(new BinaryNumericInstruction(@operator, left, right, checkForOverflow, sign));

            void MakeExplicitConversion(StackType sourceType, StackType targetType, PrimitiveType conversionType)
            {
                if (left.ResultType == sourceType && right.ResultType == targetType)
                {
                    left = new Conv(left, conversionType, false, Sign.None);
                }
                else if (left.ResultType == targetType && right.ResultType == sourceType)
                {
                    right = new Conv(right, conversionType, false, Sign.None);
                }
            }
        }

        ILInstruction DecodeJmp()
        {
            var method = ReadAndDecodeMethodReference();
            // Translate jmp into tail call:
            var call = new Call(method)
            {
                IsTail = true,
                ILStackWasEmpty = true
            };
            if (!method.IsStatic)
            {
                call.Arguments.Add(Ldarg(0));
            }
            for (int i = 0; i < method.GetParameters().Length; i++)
            {
                call.Arguments.Add(Ldarg(i));
            }
            return new Leave(_mainContainer, call);
        }

        ILInstruction LdToken(int token)
        {
            throw new NotImplementedException();
            //if (token.Kind.IsTypeKind()) return new LdTypeToken(module.ResolveType(token));
            //if (token.Kind.IsMemberKind()) return new LdMemberToken(module.ResolveMember(token));
            //         throw new BadImageFormatException("Invalid metadata token for ldtoken instruction.");
        }
    }
}
