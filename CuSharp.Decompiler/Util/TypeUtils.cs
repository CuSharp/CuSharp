// Copyright (c) 2015 Siegfried Pammer
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

namespace CuSharp.Decompiler.Util
{
    public static class TypeUtils
    {
        /// <summary>
        /// Gets whether the type is an IL integer type.
        /// Returns true for I4, I, or I8.
        /// </summary>
        public static bool IsIntegerType(this StackType type)
        {
            switch (type)
            {
                case StackType.I4:
                case StackType.I:
                case StackType.I8:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets whether the type is an IL floating point type.
        /// Returns true for F4 or F8.
        /// </summary>
        public static bool IsFloatType(this StackType type)
        {
            switch (type)
            {
                case StackType.F4:
                case StackType.F8:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets the stack type corresponding to this type.
        /// </summary>
        public static StackType GetStackType(this Type type)
        {
            if (type.IsByRef) return StackType.Ref;
            if (type.IsPointer) return StackType.I;

            // TODO: figure out how to handle the following
            //switch (type.Kind)
            //{
            //    case TypeKind.Unknown:
            //        return !type.IsValueType 
            //            ? StackType.O 
            //            : StackType.Unknown;
            //    case TypeKind.NInt:
            //    case TypeKind.NUInt:
            //    case TypeKind.FunctionPointer:
            //        return StackType.I;
            //    case TypeKind.TypeParameter:
            //        // Type parameters are always considered StackType.O, even
            //        // though they might be instantiated with primitive types.
            //        return StackType.O;
            //    case TypeKind.ModOpt:
            //    case TypeKind.ModReq:
            //        return type.SkipModifiers().GetStackType();
            //}
            //ITypeDefinition typeDef = type.GetEnumUnderlyingType().GetDefinition();
            //if (typeDef == null)
            //    return StackType.O;
            switch (type.ToKnownTypeCode())
            {
                case KnownTypeCode.Boolean:
                case KnownTypeCode.Char:
                case KnownTypeCode.SByte:
                case KnownTypeCode.Byte:
                case KnownTypeCode.Int16:
                case KnownTypeCode.UInt16:
                case KnownTypeCode.Int32:
                case KnownTypeCode.UInt32:
                    return StackType.I4;
                case KnownTypeCode.Int64:
                case KnownTypeCode.UInt64:
                    return StackType.I8;
                case KnownTypeCode.Single:
                    return StackType.F4;
                case KnownTypeCode.Double:
                    return StackType.F8;
                case KnownTypeCode.Void:
                    return StackType.Void;
                case KnownTypeCode.IntPtr:
                case KnownTypeCode.UIntPtr:
                    return StackType.I;
                default:
                    return StackType.O;
            }
        }

        public static PrimitiveType ToPrimitiveType(this StackType stackType, Sign sign = Sign.None)
        {
            switch (stackType)
            {
                case StackType.I4:
                    return sign == Sign.Unsigned ? PrimitiveType.U4 : PrimitiveType.I4;
                case StackType.I8:
                    return sign == Sign.Unsigned ? PrimitiveType.U8 : PrimitiveType.I8;
                case StackType.I:
                    return sign == Sign.Unsigned ? PrimitiveType.U : PrimitiveType.I;
                case StackType.F4:
                    return PrimitiveType.R4;
                case StackType.F8:
                    return PrimitiveType.R8;
                case StackType.Ref:
                    return PrimitiveType.Ref;
                case StackType.Unknown:
                    return PrimitiveType.Unknown;
                default:
                    return PrimitiveType.None;
            }
        }
    }

    public enum Sign : byte
    {
        None,
        Signed,
        Unsigned
    }
}
