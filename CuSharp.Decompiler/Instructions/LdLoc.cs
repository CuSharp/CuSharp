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

namespace Dotnet4Gpu.Decompilation.Instructions
{
    /// <summary>Loads the value of a local variable. (ldarg/ldloc)</summary>
    public sealed class LdLoc : SimpleInstruction, ILoadInstruction
    {
        public LdLoc(ILVariable variable) : base(OpCode.LdLoc)
        {
            _variable = variable ?? throw new ArgumentNullException(nameof(variable));
        }

        private ILVariable _variable;
        public ILVariable Variable
        {
            get => _variable;
            set
            {
                DebugAssert(value != null);
                if (IsConnected)
                    _variable.RemoveLoadInstruction(this);
                _variable = value;
                if (IsConnected)
                    _variable.AddLoadInstruction(this);
            }
        }

        public int IndexInLoadInstructionList { get; set; } = -1;

        int IInstructionWithVariableOperand.IndexInVariableInstructionMapping
        {
            get => IndexInLoadInstructionList;
            set => IndexInLoadInstructionList = value;
        }

        protected override void Connected()
        {
            base.Connected();
            _variable.AddLoadInstruction(this);
        }

        protected override void Disconnected()
        {
            _variable.RemoveLoadInstruction(this);
            base.Disconnected();
        }

        public override StackType ResultType => _variable.StackType;

        protected override InstructionFlags ComputeFlags()
        {
            return InstructionFlags.MayReadLocals;
        }
        public override InstructionFlags DirectFlags => InstructionFlags.MayReadLocals;

        public override void AcceptVisitor(ILVisitor visitor)
        {
            visitor.VisitLdLoc(this);
        }
        public override T AcceptVisitor<T>(ILVisitor<T> visitor)
        {
            return visitor.VisitLdLoc(this);
        }

        internal override void CheckInvariant(ILPhase phase)
        {
            base.CheckInvariant(phase);
            DebugAssert(phase <= ILPhase.InILReader || IsDescendantOf(_variable.Function!));
            DebugAssert(phase <= ILPhase.InILReader || _variable.Function!.Variables[_variable.IndexInFunction] == _variable);
        }
    }
}