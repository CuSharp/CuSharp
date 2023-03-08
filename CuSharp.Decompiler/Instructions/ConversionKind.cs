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

namespace Dotnet4Gpu.Decompilation.Instructions;

/// <summary>
/// Semantic meaning of a <c>Conv</c> instruction.
/// </summary>
public enum ConversionKind : byte
{
    /// <summary>
    /// Invalid conversion.
    /// </summary>
    Invalid,
    /// <summary>
    /// Conversion between two types of same size.
    /// Can be used to change the sign of integer types, which may involve overflow-checking.
    /// </summary>
    Nop,
    /// <summary>
    /// Integer-to-float conversion.
    /// Uses <c>InputSign</c> to decide whether the integer should be treated as signed or unsigned.
    /// </summary>
    IntToFloat,
    /// <summary>
    /// Float-to-integer conversion.
    /// Truncates toward zero; may perform overflow-checking.
    /// </summary>
    FloatToInt,
    /// <summary>
    /// Converts from the current precision available on the evaluation stack to the precision specified by
    /// the <c>TargetType</c>.
    /// Uses "round-to-nearest" mode if the precision is reduced.
    /// </summary>
    FloatPrecisionChange,
    /// <summary>
    /// Conversion of integer type to larger signed integer type.
    /// May involve overflow checking (when converting from U4 to I on 32-bit).
    /// </summary>
    SignExtend,
    /// <summary>
    /// Conversion of integer type to larger unsigned integer type.
    /// May involve overflow checking (when converting from a signed type).
    /// </summary>
    ZeroExtend,
    /// <summary>
    /// Conversion to smaller integer type.
    /// 
    /// May involve overflow checking.
    /// </summary>
    /// <remarks>
    /// If the target type is smaller than the minimum stack width of 4 bytes,
    /// then the result of the conversion is zero extended (if the target type is unsigned)
    /// or sign-extended (if the target type is signed).
    /// </remarks>
    Truncate,
    /// <summary>
    /// Used to convert managed references/objects to unmanaged pointers.
    /// </summary>
    StopGCTracking,
    /// <summary>
    /// Used to convert unmanaged pointers to managed references.
    /// </summary>
    StartGCTracking,
    /// <summary>
    /// Converts from an object reference (O) to an interior pointer (Ref) pointing to the start of the object.
    /// </summary>
    /// <remarks>
    /// C++/CLI emits "ldarg.1; stloc.0" where arg1 is a string and loc0 is "ref byte" (e.g. as part of the PtrToStringChars codegen);
    /// we represent this type conversion explicitly in the ILAst.
    /// </remarks>
    ObjectInterior
}