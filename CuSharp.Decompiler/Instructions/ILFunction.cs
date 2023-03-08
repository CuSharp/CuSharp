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
using CuSharp.Decompiler;
using CuSharp.Decompiler.Util;

namespace CuSharp.Decompiler.Instructions
{
    public sealed class ILFunction : ILInstruction
    {
        public static readonly SlotInfo BodySlot = new("Body");
        private ILInstruction _body = null!;
        public ILInstruction Body
        {
            get => _body;
            set
            {
                ValidateChild(value);
                SetChildInstruction(ref _body, value, 0);
            }
        }
        public static readonly SlotInfo LocalFunctionsSlot = new("LocalFunctions");
        public InstructionCollection<ILFunction> LocalFunctions { get; private set; }
        protected override int GetChildCount()
        {
            return 1 + LocalFunctions.Count;
        }
        protected override ILInstruction GetChild(int index)
        {
            return index == 0 ? _body : LocalFunctions[index - 1];
        }
        protected override void SetChild(int index, ILInstruction value)
        {
            if (index == 0)
                Body = value;
            else
                LocalFunctions[index - 1] = (ILFunction)value;
        }
        protected override SlotInfo GetChildSlot(int index)
        {
            return index == 0 ? BodySlot : LocalFunctionsSlot;
        }
        public override ILInstruction Clone()
        {
            var clone = (ILFunction)ShallowClone();
            clone.Body = _body.Clone();
            clone.LocalFunctions = new InstructionCollection<ILFunction>(clone, 1);
            clone.LocalFunctions.AddRange(LocalFunctions.Select(arg => (ILFunction)arg.Clone()));
            clone.CloneVariables();
            return clone;
        }
        public override StackType ResultType => DelegateType?.GetStackType() ?? StackType.O;

        public override void AcceptVisitor(ILVisitor visitor)
        {
            visitor.VisitILFunction(this);
        }
        public override T AcceptVisitor<T>(ILVisitor<T> visitor)
        {
            return visitor.VisitILFunction(this);
        }

        /// <summary>
        /// Gets the method definition from metadata.
        /// May be null for functions that were not constructed from metadata,
        /// e.g., expression trees.
        /// </summary>
        public readonly MethodInfo? Method;

        /// <summary>
        /// Gets the name of this function, usually this returns the name from metadata.
        /// <para>
        /// For local functions:
        /// This is the name that is used to declare and use the function.
        /// It may not conflict with the names of local variables of ancestor functions
        /// and may be overwritten by the AssignVariableNames step.
        /// 
        /// For top-level functions, delegates and expressions trees modifying this usually
        /// has no effect, as the name should not be used in the final AST construction.
        /// </para>
        /// </summary>
        public string? Name;

        /// <summary>
        /// List of ILVariables used in this function.
        /// </summary>
        public readonly ILVariableCollection Variables;

        /// <summary>
        /// Gets the scope in which the local function is declared.
        /// Returns null, if this is not a local function.
        /// </summary>
        public BlockContainer? DeclarationScope { get; internal set; }

        /// <summary>
        /// List of warnings of ILReader.
        /// </summary>
        public List<string> Warnings { get; } = new();


        /// <summary>
        /// If this is an expression tree or delegate, returns the expression tree type Expression{T} or T.
        /// T is the delegate type that matches the signature of this method.
        /// Otherwise this must be null.
        /// </summary>
        public Type? DelegateType;

        private readonly ILFunctionKind _kind;

        /// <summary>
        /// Return type of this function.
        /// </summary>
        public readonly Type ReturnType;

        /// <summary>
        /// List of parameters of this function.
        /// </summary>
        public readonly IReadOnlyList<ParameterInfo> Parameters;

        /// <summary>
        /// List of candidate locations for sequence points. Includes any offset
        /// where the stack is empty, nop instructions, and the instruction following
        /// a call instruction
        /// </summary>
        public List<int>? SequencePointCandidates { get; set; }

        /// <summary>
        /// Constructs a new ILFunction from the given metadata and with the given ILAst body.
        /// </summary>
        /// <remarks>
        /// Use <see cref="ILReader"/> to create ILAst.
        /// </remarks>
        public ILFunction(MethodInfo method, ILInstruction body, ILFunctionKind kind = ILFunctionKind.TopLevelFunction) : base(OpCode.ILFunction)
        {
            Method = method;
            Name = method.Name;
            Body = body;
            ReturnType = method.ReturnType;
            Parameters = method.GetParameters();
            Variables = new ILVariableCollection(this);
            LocalFunctions = new InstructionCollection<ILFunction>(this, 1);
            _kind = kind;
        }

        internal override void CheckInvariant(ILPhase phase)
        {
            switch (_kind)
            {
                case ILFunctionKind.TopLevelFunction:
                    Debug.Assert(Parent == null);
                    Debug.Assert(DelegateType == null);
                    Debug.Assert(DeclarationScope == null);
                    Debug.Assert(Method != null);
                    break;
                case ILFunctionKind.Delegate:
                    Debug.Assert(DelegateType != null);
                    Debug.Assert(DeclarationScope == null);
                    //Debug.Assert(!(DelegateType?.FullName == "System.Linq.Expressions.Expression" && DelegateType.TypeParameterCount == 1));
                    break;
                case ILFunctionKind.ExpressionTree:
                    Debug.Assert(DelegateType != null);
                    Debug.Assert(DeclarationScope == null);
                    //Debug.Assert(DelegateType?.FullName == "System.Linq.Expressions.Expression" && DelegateType.TypeParameterCount == 1);
                    break;
                case ILFunctionKind.LocalFunction:
                    Debug.Assert(Parent is ILFunction && SlotInfo == LocalFunctionsSlot);
                    Debug.Assert(DeclarationScope != null);
                    Debug.Assert(DelegateType == null);
                    Debug.Assert(Method != null);
                    break;
            }
            for (int i = 0; i < Variables.Count; i++)
            {
                Debug.Assert(Variables[i].Function == this);
                Debug.Assert(Variables[i].IndexInFunction == i);
                Variables[i].CheckInvariant();
            }
            base.CheckInvariant(phase);
        }

        void CloneVariables()
        {
            throw new NotSupportedException("ILFunction.CloneVariables is currently not supported!");
        }

        protected override InstructionFlags ComputeFlags()
        {
            // Creating a lambda may throw OutOfMemoryException
            // We intentionally don't propagate any flags from the lambda body!
            return InstructionFlags.MayThrow | InstructionFlags.ControlFlow;
        }

        public override InstructionFlags DirectFlags => InstructionFlags.MayThrow | InstructionFlags.ControlFlow;

        internal override bool CanInlineIntoSlot(int childIndex, ILInstruction expressionBeingMoved)
        {
            // With expression trees, we occasionally need to inline constants into an existing expression tree.
            // Only allow this for completely pure constants; a MayReadLocals effect would already be problematic
            // because we're essentially delaying evaluation of the expression until the ILFunction is called.
            Debug.Assert(childIndex == 0);
            return _kind == ILFunctionKind.ExpressionTree && expressionBeingMoved.Flags == InstructionFlags.None;
        }
    }
}
