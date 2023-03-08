// Copyright (c) 2014-2016 Daniel Grunwald
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

public enum BlockKind
{
    /// <summary>
    /// Block is used for control flow.
    /// All blocks in block containers must have this type.
    /// Control flow blocks cannot evaluate to a value (FinalInstruction must be Nop).
    /// </summary>
    ControlFlow,
    /// <summary>
    /// Block is used for array initializers, e.g. `new int[] { expr1, expr2 }`.
    /// </summary>
    ArrayInitializer,
    CollectionInitializer,
    ObjectInitializer,
    StackAllocInitializer,
    /// <summary>
    /// Block is used for using the result of a property setter inline.
    /// Example: <code>Use(this.Property = value);</code>
    /// This is only for inline assignments to property or indexers; other inline assignments work
    /// by using the result value of the stloc/stobj instructions.
    /// 
    /// Constructed by TransformAssignment.
    /// Can be deconstructed using Block.MatchInlineAssignBlock().
    /// </summary>
    /// <example>
    /// Block {
    ///   call setter(..., stloc s(...))
    ///   final: ldloc s
    /// }
    /// </example>
    CallInlineAssign,
    /// <summary>
    /// Call using named arguments.
    /// </summary>
    /// <remarks>
    /// Each instruction is assigning to a new local.
    /// The final instruction is a call.
    /// The locals for this block have exactly one store and
    /// exactly one load, which must be an immediate argument to the call.
    /// </remarks>
    /// <example>
    /// Block {
    ///   stloc arg0 = ...
    ///   stloc arg1 = ...
    ///   final: call M(..., arg1, arg0, ...)
    /// }
    /// </example>
    CallWithNamedArgs,
    /// <summary>
    /// <see cref="DeconstructInstruction"/>
    /// </summary>
    DeconstructionConversions,
    /// <summary>
    /// <see cref="DeconstructInstruction"/>
    /// </summary>
    DeconstructionAssignments,
    WithInitializer,
    /// <summary>
    /// String interpolation using DefaultInterpolatedStringHandler.
    /// </summary>
    /// <example>
    /// Block {
    ///		stloc I_0 = newobj DefaultInterpolatedStringHandler(...)
    ///		call AppendXXX(I_0, ...)
    ///		...
    ///		final: call ToStringAndClear(ldloc I_0)
    /// }
    /// </example>
    InterpolatedString,
}