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

namespace CuSharp.Decompiler.Instructions
{
    /// <summary>A region where a pinned variable is used (initial representation of future fixed statement).</summary>
    public sealed class PinnedRegion : ILInstruction, IStoreInstruction
    {
        public PinnedRegion(ILVariable variable, ILInstruction init, ILInstruction body) : base(OpCode.PinnedRegion)
        {
            _variable = variable ?? throw new ArgumentNullException(nameof(variable));
            Init = init;
            Body = body;
        }
        public override StackType ResultType => StackType.Void;
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

        public static readonly SlotInfo InitSlot = new("Init", canInlineInto: true);
        private ILInstruction _init;
        public ILInstruction Init
        {
            get => _init;
            set
            {
                ValidateChild(value);
                SetChildInstruction(ref _init, value, 0);
            }
        }
        public static readonly SlotInfo BodySlot = new("Body");
        private ILInstruction _body;
        public ILInstruction Body
        {
            get => _body;
            set
            {
                ValidateChild(value);
                SetChildInstruction(ref _body, value, 1);
            }
        }
        protected override int GetChildCount()
        {
            return 2;
        }
        protected override ILInstruction GetChild(int index)
        {
            switch (index)
            {
                case 0:
                    return _init;
                case 1:
                    return _body;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
        protected override void SetChild(int index, ILInstruction value)
        {
            switch (index)
            {
                case 0:
                    Init = value;
                    break;
                case 1:
                    Body = value;
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
        protected override SlotInfo GetChildSlot(int index)
        {
            switch (index)
            {
                case 0:
                    return InitSlot;
                case 1:
                    return BodySlot;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
        public override ILInstruction Clone()
        {
            var clone = (PinnedRegion)ShallowClone();
            clone.Init = _init.Clone();
            clone.Body = _body.Clone();
            return clone;
        }
        protected override InstructionFlags ComputeFlags()
        {
            return InstructionFlags.MayWriteLocals | _init.Flags | _body.Flags;
        }
        public override InstructionFlags DirectFlags => InstructionFlags.MayWriteLocals;

        public override void AcceptVisitor(ILVisitor visitor)
        {
            visitor.VisitPinnedRegion(this);
        }
        public override T AcceptVisitor<T>(ILVisitor<T> visitor)
        {
            return visitor.VisitPinnedRegion(this);
        }

        internal override void CheckInvariant(ILPhase phase)
        {
            base.CheckInvariant(phase);
            DebugAssert(phase <= ILPhase.InILReader || IsDescendantOf(_variable.Function!));
            DebugAssert(phase <= ILPhase.InILReader || _variable.Function!.Variables[_variable.IndexInFunction] == _variable);
            DebugAssert(Variable.Kind == VariableKind.PinnedRegionLocal);
        }
    }
}