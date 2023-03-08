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

namespace CuSharp.Decompiler.Instructions;

public enum ComparisonLiftingKind
{
    /// <summary>
    /// Not a lifted comparison.
    /// </summary>
    None,
    /// <summary>
    /// C#-style lifted comparison:
    /// * operands that have a ResultType != this.InputType are expected to return a value of
    ///   type Nullable{T}, where T.GetStackType() == this.InputType.
    /// * if both operands are <c>null</c>, equality comparisons evaluate to 1, all other comparisons to 0.
    /// * if one operand is <c>null</c>, inequality comparisons evaluate to 1, all other comparisons to 0.
    /// * if neither operand is <c>null</c>, the underlying comparison is performed.
    /// 
    /// Note that even though C#-style lifted comparisons set IsLifted=true,
    /// the ResultType remains I4 as with normal comparisons.
    /// </summary>
    CSharp,
    /// <summary>
    /// SQL-style lifted comparison: works like a lifted binary numeric instruction,
    /// that is, if any input operand is <c>null</c>, the comparison evaluates to <c>null</c>.
    /// </summary>
    /// <remarks>
    /// This lifting kind is currently only used for operator! on bool?.
    /// </remarks>
    ThreeValuedLogic
}