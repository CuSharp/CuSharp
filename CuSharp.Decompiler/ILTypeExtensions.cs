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

using CuSharp.Decompiler;

namespace CuSharp.Decompiler
{
    static class ILTypeExtensions
    {
        public static StackType GetStackType(this PrimitiveType primitiveType)
        {
            switch (primitiveType)
            {
                case PrimitiveType.I1:
                case PrimitiveType.U1:
                case PrimitiveType.I2:
                case PrimitiveType.U2:
                case PrimitiveType.I4:
                case PrimitiveType.U4:
                    return StackType.I4;
                case PrimitiveType.I8:
                case PrimitiveType.U8:
                    return StackType.I8;
                case PrimitiveType.I:
                case PrimitiveType.U:
                    return StackType.I;
                case PrimitiveType.R4:
                    return StackType.F4;
                case PrimitiveType.R8:
                case PrimitiveType.R:
                    return StackType.F8;
                case PrimitiveType.Ref: // ByRef
                    return StackType.Ref;
                case PrimitiveType.Unknown:
                    return StackType.Unknown;
                default:
                    return StackType.O;
            }
        }

        public static bool IsFloatType(this PrimitiveType type)
        {
            switch (type)
            {
                case PrimitiveType.R4:
                case PrimitiveType.R8:
                case PrimitiveType.R:
                    return true;
                default:
                    return false;
            }
        }
    }
}
