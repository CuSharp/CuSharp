#nullable enable
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

using System.Diagnostics;
using Dotnet4Gpu.Decompilation.Util;
using Dotnet4Gpu.Decompilation.Instructions;

namespace Dotnet4Gpu.Decompilation
{
    [DebuggerDisplay("{Name} : {Type}")]
    public class ILVariable
    {
        private VariableKind _kind;
        private Type _type;
        private readonly List<LdLoc> _loadInstructions = new();
        private readonly List<IStoreInstruction> _storeInstructions = new();
        private readonly List<LdLoca> _addressInstructions = new();
        private bool _initialValueIsInitialized;
        private bool _usesInitialValue;

        public readonly StackType StackType;

        /// <summary>
        /// This variable is either a C# 7 'in' parameter or must be declared as 'ref readonly'.
        /// </summary>
        public bool IsRefReadOnly { get; internal set; }

        public VariableKind Kind
        {
            get => _kind;
            internal set
            {
                if (_kind == VariableKind.Parameter)
                    throw new InvalidOperationException("Kind=Parameter cannot be changed!");
                if (Index != null && value.IsLocal() && !_kind.IsLocal())
                {
                    // For variables, Index has different meaning than for stack slots,
                    // so we need to reset it to null.
                    // StackSlot -> ForeachLocal can happen sometimes (e.g. PST.TransformForeachOnArray)
                    Index = null;
                }
                _kind = value;
            }
        }

        public Type Type
        {
            get => _type;
            internal set
            {
                if (value.GetStackType() != StackType)
                    throw new ArgumentException($"Expected stack-type: {StackType} may not be changed. Found: {value.GetStackType()}");
                _type = value;
            }
        }

        /// <summary>
        /// The index of the local variable or parameter (depending on Kind)
        /// 
        /// For VariableKinds with "Local" in the name:
        ///  * if non-null, the Index refers to the LocalVariableSignature.
        ///  * index may be null for variables that used to be fields (captured by lambda/async)
        /// For Parameters, the Index refers to the method's list of parameters.
        ///   The special "this" parameter has index -1.
        /// For ExceptionStackSlot, the index is the IL offset of the exception handler.
        /// For other kinds, the index has no meaning, and is usually null.
        /// </summary>
        public int? Index { get; private set; }

        [Conditional("DEBUG")]
        internal void CheckInvariant()
        {
            switch (_kind)
            {
                case VariableKind.Local:
                case VariableKind.ForeachLocal:
                case VariableKind.PatternLocal:
                case VariableKind.PinnedLocal:
                case VariableKind.PinnedRegionLocal:
                case VariableKind.UsingLocal:
                case VariableKind.ExceptionLocal:
                case VariableKind.DisplayClassLocal:
                    // in range of LocalVariableSignature
                    Debug.Assert(Index == null || Index >= 0);
                    break;
                case VariableKind.Parameter:
                    // -1 for the "this" parameter
                    Debug.Assert(Index >= -1);
                    Debug.Assert(Function == null || Index < Function.Parameters.Count);
                    break;
                case VariableKind.ExceptionStackSlot:
                    Debug.Assert(Index >= 0);
                    break;
            }
        }

        public string? Name { get; set; }

        public bool HasGeneratedName { get; set; }

        /// <summary>
        /// Gets the function in which this variable is declared.
        /// </summary>
        /// <remarks>
        /// This property is set automatically when the variable is added to the <c>ILFunction.Variables</c> collection.
        /// </remarks>
        public ILFunction? Function { get; internal set; }

        /// <summary>
		/// Gets the index of this variable within the <c>Function.Variables</c> collection.
		/// </summary>
		/// <remarks>
		/// This property is set automatically when the variable is added to the <c>VariableScope.Variables</c> collection.
		/// It may change if an item with a lower index is removed from the collection.
		/// </remarks>
		public int IndexInFunction { get; internal set; }

        /// <summary>
        /// Number of ldloc instructions referencing this variable.
        /// </summary>
        /// <remarks>
        /// This variable is automatically updated when adding/removing ldloc instructions from the ILAst.
        /// </remarks>
        public int LoadCount => LoadInstructions.Count;

        /// <summary>
        /// List of ldloc instructions referencing this variable.
        /// </summary>
        /// <remarks>
        /// This list is automatically updated when adding/removing ldloc instructions from the ILAst.
        /// </remarks>
        public IReadOnlyList<LdLoc> LoadInstructions => _loadInstructions;

        /// <summary>
        /// Number of store instructions referencing this variable,
        /// plus 1 if HasInitialValue.
        /// 
        /// Stores are:
        /// <list type="item">
        /// <item>stloc</item>
        /// <item>TryCatchHandler (assigning the exception variable)</item>
        /// <item>PinnedRegion (assigning the pointer variable)</item>
        /// <item>initial values (<see cref="UsesInitialValue"/>)</item>
        /// </list>
        /// </summary>
        /// <remarks>
        /// This variable is automatically updated when adding/removing stores instructions from the ILAst.
        /// </remarks>
        public int StoreCount => (_usesInitialValue ? 1 : 0) + StoreInstructions.Count;

        /// <summary>
        /// List of store instructions referencing this variable.
        /// 
        /// Stores are:
        /// <list type="item">
        /// <item>stloc</item>
        /// <item>TryCatchHandler (assigning the exception variable)</item>
        /// <item>PinnedRegion (assigning the pointer variable)</item>
        /// <item>initial values (<see cref="UsesInitialValue"/>) -- however, there is no instruction for
        ///       the initial value, so it is not contained in the store list.</item>
        /// </list>
        /// </summary>
        /// <remarks>
        /// This list is automatically updated when adding/removing stores instructions from the ILAst.
        /// </remarks>
        public IReadOnlyList<IStoreInstruction> StoreInstructions => _storeInstructions;

        /// <summary>
        /// Number of ldloca instructions referencing this variable.
        /// </summary>
        /// <remarks>
        /// This variable is automatically updated when adding/removing ldloca instructions from the ILAst.
        /// </remarks>
        public int AddressCount => AddressInstructions.Count;

        /// <summary>
        /// List of ldloca instructions referencing this variable.
        /// </summary>
        /// <remarks>
        /// This list is automatically updated when adding/removing ldloca instructions from the ILAst.
        /// </remarks>
        public IReadOnlyList<LdLoca> AddressInstructions => _addressInstructions;

        internal void AddLoadInstruction(LdLoc inst) => inst.IndexInLoadInstructionList = AddInstruction(_loadInstructions, inst);
        internal void AddStoreInstruction(IStoreInstruction inst) => inst.IndexInStoreInstructionList = AddInstruction(_storeInstructions, inst);
        internal void AddAddressInstruction(LdLoca inst) => inst.IndexInAddressInstructionList = AddInstruction(_addressInstructions, inst);

        internal void RemoveLoadInstruction(LdLoc inst) => RemoveInstruction(_loadInstructions, inst.IndexInLoadInstructionList, inst);
        internal void RemoveStoreInstruction(IStoreInstruction inst) => RemoveInstruction(_storeInstructions, inst.IndexInStoreInstructionList, inst);
        internal void RemoveAddressInstruction(LdLoca inst) => RemoveInstruction(_addressInstructions, inst.IndexInAddressInstructionList, inst);

        private int AddInstruction<T>(List<T> list, T inst) where T : class, IInstructionWithVariableOperand
        {
            list.Add(inst);
            return list.Count - 1;
        }

        private void RemoveInstruction<T>(List<T> list, int index, T? inst) where T : class, IInstructionWithVariableOperand
        {
            Debug.Assert(list[index] == inst);
            int indexToMove = list.Count - 1;
            list[index] = list[indexToMove];
            list[index].IndexInVariableInstructionMapping = index;
            list.RemoveAt(indexToMove);
        }

        /// <summary>
        /// Gets/Sets whether the variable's initial value is initialized.
        /// This is always <c>true</c> for parameters (incl. <c>this</c>).
        /// 
        /// Normal variables have an initial value if the function uses ".locals init".
        /// </summary>
        public bool InitialValueIsInitialized
        {
            get => _initialValueIsInitialized;
            set
            {
                if (Kind == VariableKind.Parameter && !value)
                    throw new InvalidOperationException("Cannot remove InitialValueIsInitialized from parameters");
                _initialValueIsInitialized = value;
            }
        }


        /// <summary>
        /// Gets/Sets whether the initial value of the variable is used.
        /// This is always <c>true</c> for parameters (incl. <c>this</c>).
        /// 
        /// Normal variables use the initial value, if no explicit initialization is done.
        /// </summary>
        /// <remarks>
        /// The following table shows the relationship between <see cref="InitialValueIsInitialized"/>
        /// and <see cref="UsesInitialValue"/>.
        /// <list type="table">
        /// <listheader>
        /// <term><see cref="InitialValueIsInitialized"/></term>
        /// <term><see cref="UsesInitialValue"/></term>
        /// <term>Meaning</term>
        /// </listheader>
        /// <item>
        /// <term><see langword="true" /></term>
        /// <term><see langword="true" /></term>
        /// <term>This variable's initial value is zero-initialized (<c>.locals init</c>) and the initial value is used.
        /// From C#'s point of view a the value <c>default(T)</c> is assigned at the site of declaration.</term>
        /// </item>
        /// <item>
        /// <term><see langword="true" /></term>
        /// <term><see langword="false" /></term>
        /// <term>This variable's initial value is zero-initialized (<c>.locals init</c>) and the initial value is not used.
        /// From C#'s point of view no implicit initialization occurs, because the code assigns a value
        /// explicitly, before the variable is first read.</term>
        /// </item>
        /// <item>
        /// <term><see langword="false" /></term>
        /// <term><see langword="true" /></term>
        /// <term>This variable's initial value is uninitialized (<c>.locals</c> without <c>init</c>) and the
        /// initial value is used.
        /// From C#'s point of view a call to <code>System.Runtime.CompilerServices.Unsafe.SkipInit(out T)</code>
        /// is generated after the declaration.</term>
        /// </item>
        /// <item>
        /// <term><see langword="false" /></term>
        /// <term><see langword="false" /></term>
        /// <term>This variable's initial value is uninitialized (<c>.locals</c> without <c>init</c>) and the
        /// initial value is not used.
        /// From C#'s point of view no implicit initialization occurs, because the code assigns a value
        /// explicitly, before the variable is first read.</term>
        /// </item>
        /// </list>
        /// </remarks>
        public bool UsesInitialValue
        {
            get => _usesInitialValue;
            set
            {
                if (Kind == VariableKind.Parameter && !value)
                    throw new InvalidOperationException("Cannot remove UsesInitialValue from parameters");
                _usesInitialValue = value;
            }
        }

        /// <summary>
        /// Gets whether the variable is in SSA form:
        /// There is exactly 1 store, and every load sees the value from that store.
        /// </summary>
        /// <remarks>
        /// Note: the single store is not necessary a store instruction, it might also
        /// be the use of the implicit initial value.
        /// For example: for parameters, IsSingleDefinition will only return true if
        /// the parameter is never assigned to within the function.
        /// </remarks>
        public bool IsSingleDefinition => StoreCount == 1 && AddressCount == 0;

        public ILVariable(VariableKind kind, Type type, int? index = null)
        {
            Kind = kind;
            _type = type ?? throw new ArgumentNullException(nameof(type));
            StackType = type.GetStackType();
            Index = index;
            if (kind == VariableKind.Parameter)
            {
                InitialValueIsInitialized = true;
                UsesInitialValue = true;
            }
            CheckInvariant();
        }

        public ILVariable(VariableKind kind, Type type, StackType stackType, int? index = null)
        {
            Kind = kind;
            _type = type ?? throw new ArgumentNullException(nameof(type));
            StackType = stackType;
            Index = index;
            if (kind == VariableKind.Parameter)
            {
                InitialValueIsInitialized = true;
                UsesInitialValue = true;
            }
            CheckInvariant();
        }

        public override string? ToString()
        {
            return Name;
        }
    }

    public interface IInstructionWithVariableOperand
    {
        int IndexInVariableInstructionMapping { get; set; }
    }

    public interface IStoreInstruction : IInstructionWithVariableOperand
    {
        int IndexInStoreInstructionList { get; set; }
    }

    interface ILoadInstruction : IInstructionWithVariableOperand
    {
        int IndexInLoadInstructionList { get; set; }
    }

    interface IAddressInstruction : IInstructionWithVariableOperand
    {
        int IndexInAddressInstructionList { get; set; }
    }
}
