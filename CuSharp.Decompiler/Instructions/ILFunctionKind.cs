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

public enum ILFunctionKind
{
    /// <summary>
    /// ILFunction is a "top-level" function, i.e., method, accessor, constructor, destructor or operator.
    /// </summary>
    TopLevelFunction,
    /// <summary>
    /// ILFunction is a delegate or lambda expression.
    /// </summary>
    /// <remarks>
    /// This kind is introduced by the DelegateConstruction and TransformExpressionTrees steps in the decompiler pipeline.
    /// </remarks>
    Delegate,
    /// <summary>
    /// ILFunction is an expression tree lambda.
    /// </summary>
    /// <remarks>
    /// This kind is introduced by the TransformExpressionTrees step in the decompiler pipeline.
    /// </remarks>
    ExpressionTree,
    /// <summary>
    /// ILFunction is a C# 7.0 local function.
    /// </summary>
    /// <remarks>
    /// This kind is introduced by the LocalFunctionDecompiler step in the decompiler pipeline.
    /// </remarks>
    LocalFunction
}