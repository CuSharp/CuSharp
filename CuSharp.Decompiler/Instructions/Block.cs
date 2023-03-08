// Copyright (c) 2014-2016 Daniel Grunwald
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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace CuSharp.Decompiler.Instructions
{
    /// <summary>
    /// A block consists of a list of IL instructions.
    /// 
    /// <para>
    /// Note: if execution reaches the end of the instruction list,
    /// the FinalInstruction (which is not part of the list) will be executed.
    /// The block returns returns the result value of the FinalInstruction.
    /// For blocks returning void, the FinalInstruction will usually be 'nop'.
    /// </para>
    /// 
    /// There are three different uses for blocks:
    /// 1) Blocks in block containers. Used as targets for Branch instructions.
    /// 2) Blocks to group a bunch of statements, e.g. the TrueInst of an IfInstruction.
    /// 3) Inline blocks that evaluate to a value, e.g. for array initializers.
    /// </summary>
    public class Block : ILInstruction
    {
        public override void AcceptVisitor(ILVisitor visitor)
        {
            visitor.VisitBlock(this);
        }
        public override T AcceptVisitor<T>(ILVisitor<T> visitor)
        {
            return visitor.VisitBlock(this);
        }

        public static readonly SlotInfo InstructionSlot = new("Instruction", isCollection: true);
        public static readonly SlotInfo FinalInstructionSlot = new("FinalInstruction");

        public readonly BlockKind Kind;
        public readonly InstructionCollection<ILInstruction> Instructions;
        private ILInstruction _finalInstruction = null!;

        /// <summary>
        /// For blocks in a block container, this field holds
        /// the number of incoming control flow edges to this block.
        /// </summary>
        /// <remarks>
        /// This variable is automatically updated when adding/removing branch instructions from the ILAst,
        /// or when adding the block as an entry point to a BlockContainer.
        /// </remarks>
        public int IncomingEdgeCount { get; internal set; }

        /// <summary>
        /// A 'final instruction' that gets executed after the <c>Instructions</c> collection.
        /// Provides the return value for the block.
        /// </summary>
        /// <remarks>
        /// Blocks in containers must have 'Nop' as a final instruction.
        /// 
        /// Note that the FinalInstruction is included in Block.Children,
        /// but not in Block.Instructions!
        /// </remarks>
        public ILInstruction FinalInstruction
        {
            get => _finalInstruction;
            set
            {
                ValidateChild(value);
                SetChildInstruction(ref _finalInstruction, value, Instructions.Count);
            }
        }

        protected internal override void InstructionCollectionUpdateComplete()
        {
            base.InstructionCollectionUpdateComplete();
            if (_finalInstruction.Parent == this)
                _finalInstruction.ChildIndex = Instructions.Count;
        }

        public Block(BlockKind kind = BlockKind.ControlFlow) : base(OpCode.Block)
        {
            Kind = kind;
            Instructions = new InstructionCollection<ILInstruction>(this, 0);
            FinalInstruction = new Nop();
        }

        public override ILInstruction Clone()
        {
            var clone = new Block(Kind);
            clone.AddILRange(this);
            clone.Instructions.AddRange(Instructions.Select(inst => inst.Clone()));
            clone.FinalInstruction = FinalInstruction.Clone();
            return clone;
        }

        internal override void CheckInvariant(ILPhase phase)
        {
            base.CheckInvariant(phase);
            for (int i = 0; i < Instructions.Count - 1; i++)
            {
                // only the last instruction may have an unreachable endpoint
                Debug.Assert(!Instructions[i].HasFlag(InstructionFlags.EndPointUnreachable));
            }
            switch (Kind)
            {
                case BlockKind.ControlFlow:
                    Debug.Assert(_finalInstruction.OpCode == OpCode.Nop);
                    break;
                case BlockKind.CallInlineAssign:
                    Debug.Assert(MatchInlineAssignBlock(out _, out _));
                    break;
                case BlockKind.CallWithNamedArgs:
                    Debug.Assert(_finalInstruction is CallInstruction);
                    foreach (var inst in Instructions)
                    {
                        var stloc = inst as StLoc;
                        DebugAssert(stloc != null, "Instructions in CallWithNamedArgs must be assignments");
                        DebugAssert(stloc.Variable.Kind == VariableKind.NamedArgument);
                        DebugAssert(stloc.Variable.IsSingleDefinition && stloc.Variable.LoadCount == 1);
                        DebugAssert(stloc.Variable.LoadInstructions.Single().Parent == _finalInstruction);
                    }
                    var call = (CallInstruction)_finalInstruction;
                    if (call.IsInstanceCall)
                    {
                        // special case: with instance calls, Instructions[0] must be for the this parameter
                        ILVariable v = ((StLoc)Instructions[0]).Variable;
                        Debug.Assert(call.Arguments[0].MatchLdLoc(v));
                    }
                    break;
                case BlockKind.ArrayInitializer:
                    var final = _finalInstruction as LdLoc;
                    Debug.Assert(final != null && final.Variable.IsSingleDefinition && final.Variable.Kind == VariableKind.InitializerTarget);
                    Type? type = null;
                    Debug.Assert(Instructions[0].MatchStLoc(final.Variable, out var init) && init.MatchNewArr(out type));
                    for (int i = 1; i < Instructions.Count; i++)
                    {
                        DebugAssert(Instructions[i].MatchStObj(out ILInstruction? target, out _, out var t) && type != null && type == t);
                        DebugAssert(target.MatchLdElema(out t, out ILInstruction? array) && type == t);
                        DebugAssert(array.MatchLdLoc(out ILVariable? v) && v == final.Variable);
                    }
                    break;
                case BlockKind.CollectionInitializer:
                case BlockKind.ObjectInitializer:
                    var final2 = _finalInstruction as LdLoc;
                    Debug.Assert(final2 != null);
                    var initVar2 = final2!.Variable;
                    Debug.Assert(initVar2.StoreCount == 1 && initVar2.Kind == VariableKind.InitializerTarget);
                    bool condition = Instructions[0].MatchStLoc(final2.Variable, out var init2);
                    Debug.Assert(condition);
                    Debug.Assert(init2 is NewObj
                                 || init2 is DefaultValue
                                 //|| (init2 is CallInstruction c && c.Method.FullNameIs("System.Activator", "CreateInstance") && c.Method.TypeArguments.Count == 1)
                                 || init2 is Block named && named.Kind == BlockKind.CallWithNamedArgs);
                    break;
                case BlockKind.DeconstructionConversions:
                    Debug.Assert(SlotInfo == DeconstructInstruction.ConversionsSlot);
                    break;
                case BlockKind.DeconstructionAssignments:
                    Debug.Assert(SlotInfo == DeconstructInstruction.AssignmentsSlot);
                    break;
                case BlockKind.InterpolatedString:
                    Debug.Assert(FinalInstruction is Call { Method: { Name: "ToStringAndClear" }, Arguments: { Count: 1 } });
                    var interpolInit = Instructions[0] as StLoc;
                    DebugAssert(interpolInit != null
                                && interpolInit.Variable.Kind == VariableKind.InitializerTarget
                                && interpolInit.Variable.AddressCount == Instructions.Count
                                && interpolInit.Variable.StoreCount == 1);
                    for (int i = 1; i < Instructions.Count; i++)
                    {
                        Call? inst = Instructions[i] as Call;
                        DebugAssert(inst != null);
                        DebugAssert(inst.Arguments.Count >= 1 && inst.Arguments[0].MatchLdLoca(interpolInit.Variable));
                    }
                    break;
            }
        }

        public override StackType ResultType => _finalInstruction.ResultType;

        protected override int GetChildCount()
        {
            return Instructions.Count + 1;
        }

        protected override ILInstruction GetChild(int index)
        {
            return index == Instructions.Count
                ? _finalInstruction
                : Instructions[index];
        }

        protected override void SetChild(int index, ILInstruction value)
        {
            if (index == Instructions.Count)
                FinalInstruction = value;
            else
                Instructions[index] = value;
        }

        protected override SlotInfo GetChildSlot(int index)
        {
            return index == Instructions.Count
                ? FinalInstructionSlot
                : InstructionSlot;
        }

        protected override InstructionFlags ComputeFlags()
        {
            var flags = InstructionFlags.None;
            foreach (var inst in Instructions)
            {
                flags |= inst.Flags;
            }
            flags |= FinalInstruction.Flags;
            return flags;
        }

        public override InstructionFlags DirectFlags => InstructionFlags.None;

        public bool MatchInlineAssignBlock([NotNullWhen(true)] out CallInstruction? call, [NotNullWhen(true)] out ILInstruction? value)
        {
            call = null;
            value = null;
            if (Kind != BlockKind.CallInlineAssign)
                return false;
            if (Instructions.Count != 1)
                return false;
            call = Instructions[0] as CallInstruction;
            if (call == null || call.Arguments.Count == 0)
                return false;
            if (!call.Arguments.Last().MatchStLoc(out var tmp, out value))
                return false;
            if (!(tmp.IsSingleDefinition && tmp.LoadCount == 1))
                return false;
            return FinalInstruction.MatchLdLoc(tmp);
        }
    }
}
