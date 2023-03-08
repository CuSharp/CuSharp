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

using System.Reflection;

namespace Dotnet4Gpu.Decompilation.Instructions
{
    public sealed class CallIndirect : ILInstruction
    {
        public override void AcceptVisitor(ILVisitor visitor)
        {
            visitor.VisitCallIndirect(this);
        }
        public override T AcceptVisitor<T>(ILVisitor<T> visitor)
        {
            return visitor.VisitCallIndirect(this);
        }

        // Note: while in IL the arguments come first and the function pointer last;
        // in the ILAst we're handling it as in C#: the function pointer is evaluated first, the arguments later.
        public static readonly SlotInfo FunctionPointerSlot = new("FunctionPointer", canInlineInto: true);
        public static readonly SlotInfo ArgumentSlot = new("Argument", canInlineInto: true, isCollection: true);

        private ILInstruction _functionPointer = null!;
        public readonly InstructionCollection<ILInstruction> Arguments;
        public bool IsInstance { get; }
        public bool HasExplicitThis { get; }
        public TypeInfo FunctionPointerType { get; }

        public ILInstruction FunctionPointer {
            get => _functionPointer;
            set {
                ValidateChild(value);
                SetChildInstruction(ref _functionPointer, value, 0);
            }
        }

        public CallIndirect(bool isInstance, bool hasExplicitThis, TypeInfo functionPointerType,
            ILInstruction functionPointer, IEnumerable<ILInstruction> arguments) : base(OpCode.CallIndirect)
        {
            IsInstance = isInstance;
            HasExplicitThis = hasExplicitThis;
            FunctionPointerType = functionPointerType;
            FunctionPointer = functionPointer;
            Arguments = new InstructionCollection<ILInstruction>(this, 1);
            Arguments.AddRange(arguments);
        }

        public override ILInstruction Clone()
        {
            return new CallIndirect(IsInstance, HasExplicitThis, FunctionPointerType,
                _functionPointer.Clone(), Arguments.Select(inst => inst.Clone())
            ).WithILRange(this);
        }

        public override StackType ResultType => throw new NotImplementedException();
        //FunctionPointerType.ReturnType.GetStackType();

        internal override void CheckInvariant(ILPhase phase)
        {
            base.CheckInvariant(phase);
            //Debug.Assert(Arguments.Count == FunctionPointerType.ParameterTypes.Length + (IsInstance ? 1 : 0));
        }

        protected override int GetChildCount()
        {
            return Arguments.Count + 1;
        }

        protected override ILInstruction GetChild(int index)
        {
            return index == 0 
                ? _functionPointer 
                : Arguments[index - 1];
        }

        protected override void SetChild(int index, ILInstruction value)
        {
            if (index == 0)
                FunctionPointer = value;
            else
                Arguments[index - 1] = value;
        }

        protected override SlotInfo GetChildSlot(int index)
        {
            return index == 0 
                ? FunctionPointerSlot 
                : ArgumentSlot;
        }

        protected override InstructionFlags ComputeFlags()
        {
            var flags = DirectFlags;
            flags |= _functionPointer.Flags;
            foreach (var inst in Arguments)
            {
                flags |= inst.Flags;
            }
            return flags;
        }

        public override InstructionFlags DirectFlags => InstructionFlags.MayThrow | InstructionFlags.SideEffect;

        bool EqualSignature(CallIndirect other)
        {
            if (IsInstance != other.IsInstance)
                return false;
            if (HasExplicitThis != other.HasExplicitThis)
                return false;
            return FunctionPointerType.Equals(other.FunctionPointerType);
        }
    }
}
