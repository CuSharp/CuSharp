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

namespace CuSharp.Decompiler;

public enum VariableKind
{
    /// <summary>
    /// A local variable.
    /// </summary>
    Local,
    /// <summary>
    /// A pinned local variable (not associated with a pinned region)
    /// </summary>
    PinnedLocal,
    /// <summary>
    /// A pinned local variable (associated with a pinned region)
    /// </summary>
    PinnedRegionLocal,
    /// <summary>
    /// A local variable used as using-resource variable.
    /// </summary>
    UsingLocal,
    /// <summary>
    /// A local variable used as foreach variable.
    /// </summary>
    ForeachLocal,
    /// <summary>
    /// A local variable used inside an array, collection or
    /// object initializer block to denote the object being initialized.
    /// </summary>
    InitializerTarget,
    /// <summary>
    /// A parameter.
    /// </summary>
    Parameter,
    /// <summary>
    /// Variable created for exception handler.
    /// </summary>
    ExceptionStackSlot,
    /// <summary>
    /// Local variable used in a catch block.
    /// </summary>
    ExceptionLocal,
    /// <summary>
    /// Variable created from stack slot.
    /// </summary>
    StackSlot,
    /// <summary>
    /// Variable in BlockKind.CallWithNamedArgs
    /// </summary>
    NamedArgument,
    /// <summary>
    /// Local variable that holds the display class used for lambdas within this function.
    /// </summary>
    DisplayClassLocal,
    /// <summary>
    /// Local variable declared within a pattern match.
    /// </summary>
    PatternLocal,
    /// <summary>
    /// Temporary variable declared in a deconstruction init section.
    /// </summary>
    DeconstructionInitTemporary,
}

static class VariableKindExtensions
{
    public static bool IsLocal(this VariableKind kind)
    {
        return kind switch
        {
            VariableKind.Local => true,
            VariableKind.ExceptionLocal => true,
            VariableKind.ForeachLocal => true,
            VariableKind.UsingLocal => true,
            VariableKind.PatternLocal => true,
            VariableKind.PinnedLocal => true,
            VariableKind.PinnedRegionLocal => true,
            VariableKind.DisplayClassLocal => true,
            _ => false
        };
    }
}
