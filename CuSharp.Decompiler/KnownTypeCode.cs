// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
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
#nullable enable

namespace Dotnet4Gpu.Decompilation
{
    /// <summary>
    /// Represents some well-known types.
    /// </summary>
    public enum KnownTypeCode
    {
        // Note: DefaultResolvedTypeDefinition uses (KnownTypeCode)-1 as special value for "not yet calculated".
        // The order of type codes at the beginning must correspond to those in System.TypeCode.

        /// <summary>
        /// Not one of the known types.
        /// </summary>
        None,
        /// <summary><c>object</c> (System.Object)</summary>
        Object,
        /// <summary><c>bool</c> (System.Boolean)</summary>
        Boolean,
        /// <summary><c>char</c> (System.Char)</summary>
        Char,
        /// <summary><c>sbyte</c> (System.SByte)</summary>
        SByte,
        /// <summary><c>byte</c> (System.Byte)</summary>
        Byte,
        /// <summary><c>short</c> (System.Int16)</summary>
        Int16,
        /// <summary><c>ushort</c> (System.UInt16)</summary>
        UInt16,
        /// <summary><c>int</c> (System.Int32)</summary>
        Int32,
        /// <summary><c>uint</c> (System.UInt32)</summary>
        UInt32,
        /// <summary><c>long</c> (System.Int64)</summary>
        Int64,
        /// <summary><c>ulong</c> (System.UInt64)</summary>
        UInt64,
        /// <summary><c>float</c> (System.Single)</summary>
        Single,
        /// <summary><c>double</c> (System.Double)</summary>
        Double,
        /// <summary><c>decimal</c> (System.Decimal)</summary>
        Decimal,
        /// <summary><c>string</c> (System.String)</summary>
        String = 18,

        // String was the last element from System.TypeCode, now our additional known types start

        /// <summary><c>void</c> (System.Void)</summary>
        Void,

        /// <summary><c>System.ValueType</c></summary>
        ValueType,
        /// <summary><c>System.Enum</c></summary>
        Enum,
        /// <summary><c>System.MulticastDelegate</c></summary>
        MulticastDelegate,
        /// <summary><c>System.IntPtr</c></summary>
        IntPtr,
        /// <summary><c>System.UIntPtr</c></summary>
        UIntPtr,
        /// <summary><c>System.Collections.Generic.IList</c></summary>
        IList,
        /// <summary><c>System.TypedReference</c></summary>
        TypedReference,
        /// <summary><c>System.Range</c></summary>
        Range
    }

    public static class TypeExtensions
    {
        public static KnownTypeCode ToKnownTypeCode(this Type type)
        {
            if (type == typeof(object)) return KnownTypeCode.Object;
            if (type == typeof(bool)) return KnownTypeCode.Boolean;
            if (type == typeof(char)) return KnownTypeCode.Char;
            if (type == typeof(sbyte)) return KnownTypeCode.SByte;
            if (type == typeof(byte)) return KnownTypeCode.Byte;
            if (type == typeof(short)) return KnownTypeCode.Int16;
            if (type == typeof(ushort)) return KnownTypeCode.UInt16;
            if (type == typeof(int)) return KnownTypeCode.Int32;
            if (type == typeof(uint)) return KnownTypeCode.UInt32;
            if (type == typeof(long)) return KnownTypeCode.Int64;
            if (type == typeof(ulong)) return KnownTypeCode.UInt64;
            if (type == typeof(float)) return KnownTypeCode.Single;
            if (type == typeof(double)) return KnownTypeCode.Double;
            if (type == typeof(decimal)) return KnownTypeCode.Decimal;
            if (type == typeof(string)) return KnownTypeCode.String;

            if (type == typeof(void)) return KnownTypeCode.Void;

            //if (type == typeof(object)) return KnownTypeCode.ValueType;
            //if (type == typeof(object)) return KnownTypeCode.Enum;
            //if (type == typeof(object)) return KnownTypeCode.MulticastDelegate;
            //if (type == typeof(object)) return KnownTypeCode.IntPtr;
            //if (type == typeof(object)) return KnownTypeCode.UIntPtr;
            //if (type == typeof(object)) return KnownTypeCode.IList;
            //if (type == typeof(object)) return KnownTypeCode.TypedReference;
            //if (type == typeof(object)) return KnownTypeCode.Range;
            return KnownTypeCode.None;
        }
    }
}
