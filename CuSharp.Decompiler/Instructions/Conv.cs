﻿// Copyright (c) 2014-2020 Daniel Grunwald
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

namespace Dotnet4Gpu.Decompilation.Instructions
{
    public sealed class Conv : UnaryInstruction
    {
        public override void AcceptVisitor(ILVisitor visitor)
        {
            visitor.VisitConv(this);
        }
        public override T AcceptVisitor<T>(ILVisitor<T> visitor)
        {
            return visitor.VisitConv(this);
        }

        /// <summary>
        /// Gets the conversion kind.
        /// </summary>
        public readonly ConversionKind Kind;

        /// <summary>
        /// Gets whether the conversion performs overflow-checking.
        /// </summary>
        public readonly bool CheckForOverflow;

        /// <summary>
        /// Gets whether this conversion is a lifted nullable conversion.
        /// </summary>
        /// <remarks>
        /// A lifted conversion expects its argument to be a value of type Nullable{T}, where
        /// T.GetStackType() == conv.InputType.
        /// If the value is non-null:
        ///  * it is sign/zero-extended to InputType (based on T's sign)
        ///  * the underlying conversion is performed
        ///  * the result is wrapped in a Nullable{TargetType}.
        /// If the value is null, the conversion evaluates to default(TargetType?).
        /// (this result type is underspecified, since there may be multiple C# types for the TargetType)
        /// </remarks>
        public bool IsLifted { get; }

        /// <summary>
        /// Gets the stack type of the input type.
        /// </summary>
        /// <remarks>
        /// For non-lifted conversions, this is equal to <c>Argument.ResultType</c>.
        /// For lifted conversions, corresponds to the underlying type of the argument.
        /// </remarks>
        public readonly StackType InputType;

        /// <summary>
        /// Gets the sign of the input type.
        /// 
        /// For conversions to integer types, the input Sign is set iff overflow-checking is enabled.
        /// For conversions to floating-point types, the input sign is always set.
        /// </summary>
        /// <remarks>
        /// The input sign does not have any effect on whether the conversion zero-extends or sign-extends;
        /// that is purely determined by the <c>TargetType</c>.
        /// </remarks>
        public readonly Sign InputSign;

        /// <summary>
        /// The target type of the conversion.
        /// </summary>
        /// <remarks>
        /// For lifted conversions, corresponds to the underlying target type.
        /// 
        /// Target type == PrimitiveType.None can happen for implicit conversions to O in invalid IL.
        /// </remarks>
        public readonly PrimitiveType TargetType;

        public Conv(ILInstruction argument, PrimitiveType targetType, bool checkForOverflow, Sign inputSign)
            : this(argument, argument.ResultType, inputSign, targetType, checkForOverflow)
        {
        }

        public Conv(ILInstruction argument, StackType inputType, Sign inputSign, PrimitiveType targetType, bool checkForOverflow, bool isLifted = false)
            : base(OpCode.Conv, argument)
        {
            bool needsSign = checkForOverflow || (!inputType.IsFloatType() && targetType.IsFloatType());
            Debug.Assert(!(needsSign && inputSign == Sign.None));
            InputSign = needsSign ? inputSign : Sign.None;
            InputType = inputType;
            TargetType = targetType;
            CheckForOverflow = checkForOverflow;
            Kind = GetConversionKind(targetType, this.InputType, this.InputSign);
            // Debug.Assert(Kind != ConversionKind.Invalid); // invalid conversion can happen with invalid IL/missing references
            IsLifted = isLifted;
        }

        internal override void CheckInvariant(ILPhase phase)
        {
            base.CheckInvariant(phase);
            // Debug.Assert(Kind != ConversionKind.Invalid); // invalid conversion can happen with invalid IL/missing references
            Debug.Assert(Argument.ResultType == (IsLifted ? StackType.O : InputType));
            Debug.Assert(!(IsLifted && Kind == ConversionKind.StopGCTracking));
        }

        /// <summary>
        /// Implements Ecma-335 Table 8: Conversion Operators.
        /// </summary>
        static ConversionKind GetConversionKind(PrimitiveType targetType, StackType inputType, Sign inputSign)
        {
            switch (targetType)
            {
                case PrimitiveType.I1:
                case PrimitiveType.I2:
                case PrimitiveType.U1:
                case PrimitiveType.U2:
                    switch (inputType)
                    {
                        case StackType.I4:
                        case StackType.I8:
                        case StackType.I:
                            return ConversionKind.Truncate;
                        case StackType.F4:
                        case StackType.F8:
                            return ConversionKind.FloatToInt;
                        default:
                            return ConversionKind.Invalid;
                    }
                case PrimitiveType.I4:
                case PrimitiveType.U4:
                    switch (inputType)
                    {
                        case StackType.I4:
                            return ConversionKind.Nop;
                        case StackType.I:
                        case StackType.I8:
                            return ConversionKind.Truncate;
                        case StackType.F4:
                        case StackType.F8:
                            return ConversionKind.FloatToInt;
                        default:
                            return ConversionKind.Invalid;
                    }
                case PrimitiveType.I8:
                case PrimitiveType.U8:
                    switch (inputType)
                    {
                        case StackType.I4:
                        case StackType.I:
                            if (inputSign == Sign.None)
                                return targetType == PrimitiveType.I8 ? ConversionKind.SignExtend : ConversionKind.ZeroExtend;
                            else
                                return inputSign == Sign.Signed ? ConversionKind.SignExtend : ConversionKind.ZeroExtend;
                        case StackType.I8:
                            return ConversionKind.Nop;
                        case StackType.F4:
                        case StackType.F8:
                            return ConversionKind.FloatToInt;
                        case StackType.Ref:
                        case StackType.O:
                            return ConversionKind.StopGCTracking;
                        default:
                            return ConversionKind.Invalid;
                    }
                case PrimitiveType.I:
                case PrimitiveType.U:
                    switch (inputType)
                    {
                        case StackType.I4:
                            if (inputSign == Sign.None)
                                return targetType == PrimitiveType.I ? ConversionKind.SignExtend : ConversionKind.ZeroExtend;
                            else
                                return inputSign == Sign.Signed ? ConversionKind.SignExtend : ConversionKind.ZeroExtend;
                        case StackType.I:
                            return ConversionKind.Nop;
                        case StackType.I8:
                            return ConversionKind.Truncate;
                        case StackType.F4:
                        case StackType.F8:
                            return ConversionKind.FloatToInt;
                        case StackType.Ref:
                        case StackType.O:
                            return ConversionKind.StopGCTracking;
                        default:
                            return ConversionKind.Invalid;
                    }
                case PrimitiveType.R4:
                    switch (inputType)
                    {
                        case StackType.I4:
                        case StackType.I:
                        case StackType.I8:
                            return ConversionKind.IntToFloat;
                        case StackType.F4:
                            return ConversionKind.Nop;
                        case StackType.F8:
                            return ConversionKind.FloatPrecisionChange;
                        default:
                            return ConversionKind.Invalid;
                    }
                case PrimitiveType.R:
                case PrimitiveType.R8:
                    switch (inputType)
                    {
                        case StackType.I4:
                        case StackType.I:
                        case StackType.I8:
                            return ConversionKind.IntToFloat;
                        case StackType.F4:
                            return ConversionKind.FloatPrecisionChange;
                        case StackType.F8:
                            return ConversionKind.Nop;
                        default:
                            return ConversionKind.Invalid;
                    }
                case PrimitiveType.Ref:
                    // There's no "conv.ref" in IL, but IL allows these conversions implicitly,
                    // whereas we represent them explicitly in the ILAst.
                    switch (inputType)
                    {
                        case StackType.I4:
                        case StackType.I:
                        case StackType.I8:
                            return ConversionKind.StartGCTracking;
                        case StackType.O:
                            return ConversionKind.ObjectInterior;
                        default:
                            return ConversionKind.Invalid;
                    }
                default:
                    return ConversionKind.Invalid;
            }
        }

        public override StackType ResultType => IsLifted ? StackType.O : ILTypeExtensions.GetStackType(TargetType);

        protected override InstructionFlags ComputeFlags()
        {
            var flags = base.ComputeFlags();
            if (CheckForOverflow)
                flags |= InstructionFlags.MayThrow;
            return flags;
        }

        public override ILInstruction UnwrapConv(ConversionKind kind)
        {
            if (this.Kind == kind && !IsLifted)
                return Argument.UnwrapConv(kind);
            else
                return this;
        }
    }
}
