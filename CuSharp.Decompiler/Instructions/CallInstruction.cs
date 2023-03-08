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
    public abstract class CallInstruction : ILInstruction
    {
        public static readonly SlotInfo ArgumentsSlot = new("Arguments", canInlineInto: true);
        public InstructionCollection<ILInstruction> Arguments { get; private set; }
        protected override int GetChildCount()
        {
            return Arguments.Count;
        }
        protected override ILInstruction GetChild(int index)
        {
            switch (index)
            {
                default:
                    return Arguments[index - 0];
            }
        }
        protected override void SetChild(int index, ILInstruction value)
        {
            switch (index)
            {
                default:
                    Arguments[index - 0] = value;
                    break;
            }
        }
        protected override SlotInfo GetChildSlot(int index)
        {
            switch (index)
            {
                default:
                    return ArgumentsSlot;
            }
        }
        public override ILInstruction Clone()
        {
            var clone = (CallInstruction)ShallowClone();
            clone.Arguments = new InstructionCollection<ILInstruction>(clone, 0);
            clone.Arguments.AddRange(Arguments.Select(arg => arg.Clone()));
            return clone;
        }
        protected override InstructionFlags ComputeFlags()
        {
            return Arguments.Aggregate(InstructionFlags.None, (f, arg) => f | arg.Flags) | InstructionFlags.MayThrow | InstructionFlags.SideEffect;
        }
        public override InstructionFlags DirectFlags => InstructionFlags.MayThrow | InstructionFlags.SideEffect;

        public static CallInstruction Create(OpCode opCode, MethodBase method)
        {
            return opCode switch
            {
                OpCode.Call => new Call(method),
                OpCode.CallVirt => new CallVirt(method),
                OpCode.NewObj => new NewObj(method),
                _ => throw new ArgumentException("Not a valid call opcode")
            };
        }

        public readonly MethodBase Method;

        /// <summary>
        /// Gets/Sets whether the call has the 'tail.' prefix.
        /// </summary>
        public bool IsTail;

        /// <summary>
        /// Gets/Sets the type specified in the 'constrained.' prefix.
        /// Returns null if no 'constrained.' prefix exists for this call.
        /// </summary>
        public Type? ConstrainedTo;

        /// <summary>
        /// Gets whether the IL stack was empty at the point of this call.
        /// (not counting the arguments/return value of the call itself)
        /// </summary>
        public bool ILStackWasEmpty;

        protected CallInstruction(OpCode opCode, MethodBase method) : base(opCode)
        {
            Method = method ?? throw new ArgumentNullException(nameof(method));
            Arguments = new InstructionCollection<ILInstruction>(this, 0);
        }

        /// <summary>
        /// Gets whether this is an instance call (i.e. whether the first argument is the 'this' pointer).
        /// </summary>
        public bool IsInstanceCall => !(Method.IsStatic || OpCode == OpCode.NewObj);

        public override StackType ResultType =>
            OpCode == OpCode.NewObj
                ? Method.DeclaringType.GetStackType()
                : Method is MethodInfo info
                    ? info.ReturnType.GetStackType()
                    : default;

        /// <summary>
        /// Gets the expected stack type for passing the this pointer in a method call.
        /// Returns StackType.O for reference types (this pointer passed as object reference),
        /// and StackType.Ref for type parameters and value types (this pointer passed as managed reference).
        /// 
        /// Returns StackType.Unknown if the input type is unknown.
        /// </summary>
        internal static StackType ExpectedTypeForThisPointer(Type? type)
        {
            if (type == null) return StackType.Unknown;
            if (type.IsGenericTypeParameter) return StackType.Ref;
            return type.IsValueType
                ? StackType.Ref
                : StackType.O;
        }

        internal override void CheckInvariant(ILPhase phase)
        {
            base.CheckInvariant(phase);
            int firstArgument = (OpCode != OpCode.NewObj && !Method.IsStatic) ? 1 : 0;
            var parameters = Method.GetParameters();
            Debug.Assert(parameters.Length + firstArgument == Arguments.Count);
            if (firstArgument == 1)
            {
                if (Arguments[0].ResultType != ExpectedTypeForThisPointer(ConstrainedTo ?? Method.DeclaringType))
                    Debug.Fail($"Stack type mismatch in 'this' argument in call to {Method.Name}()");
            }
            for (int i = 0; i < parameters.Length; ++i)
            {
                if (Arguments[firstArgument + i].ResultType != TypeUtils.GetStackType(parameters[i].ParameterType))
                    Debug.Fail($"Stack type mismatch in parameter {i} in call to {Method.Name}()");
            }
        }
    }

    //partial class Call : ILiftableInstruction
	//{
	//	/// <summary>
	//	/// Calls can only be lifted when calling a lifted operator.
	//	/// Note that the semantics of such a lifted call depend on the type of operator:
	//	/// we follow C# semantics here.
	//	/// </summary>
	//	public bool IsLifted => Method is CSharp.Resolver.ILiftedOperator;

	//	public StackType UnderlyingResultType {
	//		get {
	//			if (Method is CSharp.Resolver.ILiftedOperator liftedOp)
	//				return liftedOp.NonLiftedReturnType.GetStackType();
	//			else
	//				return Method.ReturnType.GetStackType();
	//		}
	//	}
	//}
}
