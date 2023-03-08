// Copyright (c) 2017 Siegfried Pammer
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

namespace CuSharp.Decompiler.Instructions;

/// <summary>
/// Kind of null-coalescing operator.
/// ILAst: <c>if.notnull(valueInst, fallbackInst)</c>
/// C#: <c>value ?? fallback</c>
/// </summary>
public enum NullCoalescingKind
{
    /// <summary>
    /// Both ValueInst and FallbackInst are of reference type.
    /// 
    /// Semantics: equivalent to "valueInst != null ? valueInst : fallbackInst",
    ///            except that valueInst is evaluated only once.
    /// </summary>
    Ref,
    /// <summary>
    /// Both ValueInst and FallbackInst are of type Nullable{T}.
    /// 
    /// Semantics: equivalent to "valueInst.HasValue ? valueInst : fallbackInst",
    ///            except that valueInst is evaluated only once.
    /// </summary>
    Nullable,
    /// <summary>
    /// ValueInst is Nullable{T}, but FallbackInst is non-nullable value type.
    /// 
    /// Semantics: equivalent to "valueInst.HasValue ? valueInst.Value : fallbackInst",
    ///            except that valueInst is evaluated only once.
    /// </summary>
    NullableWithValueFallback
}