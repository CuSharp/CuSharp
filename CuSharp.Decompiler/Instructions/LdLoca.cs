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

using CuSharp.Decompiler;

namespace CuSharp.Decompiler.Instructions
{
    /// <summary>Loads the address of a local variable. (ldarga/ldloca)</summary>
    public sealed class LdLoca : SimpleInstruction, IAddressInstruction
    {
        public LdLoca(ILVariable variable) : base(OpCode.LdLoca)
        {
            _variable = variable ?? throw new ArgumentNullException(nameof(variable));
        }
        public override StackType ResultType => StackType.Ref;
        private ILVariable _variable;
        public ILVariable Variable
        {
            get => _variable;
            set
            {
                DebugAssert(value != null);
                if (IsConnected)
                    _variable.RemoveAddressInstruction(this);
                _variable = value;
                if (IsConnected)
                    _variable.AddAddressInstruction(this);
            }
        }

        public int IndexInAddressInstructionList { get; set; } = -1;

        int IInstructionWithVariableOperand.IndexInVariableInstructionMapping
        {
            get => IndexInAddressInstructionList;
            set => IndexInAddressInstructionList = value;
        }

        protected override void Connected()
        {
            base.Connected();
            _variable.AddAddressInstruction(this);
        }

        protected override void Disconnected()
        {
            _variable.RemoveAddressInstruction(this);
            base.Disconnected();
        }

        public override void AcceptVisitor(ILVisitor visitor)
        {
            visitor.VisitLdLoca(this);
        }
        public override T AcceptVisitor<T>(ILVisitor<T> visitor)
        {
            return visitor.VisitLdLoca(this);
        }

        internal override void CheckInvariant(ILPhase phase)
        {
            base.CheckInvariant(phase);
            DebugAssert(phase <= ILPhase.InILReader || IsDescendantOf(_variable.Function!));
            DebugAssert(phase <= ILPhase.InILReader || _variable.Function!.Variables[_variable.IndexInFunction] == _variable);
        }
    }
}