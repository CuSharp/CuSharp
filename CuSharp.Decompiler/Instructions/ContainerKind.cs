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

public enum ContainerKind
{
    /// <summary>
    /// Normal container that contains control-flow blocks.
    /// </summary>
    Normal,
    /// <summary>
    /// A while-true loop.
    /// Continue is represented as branch to entry-point.
    /// Return/break is represented as leave.
    /// </summary>
    Loop,
    /// <summary>
    /// Container that has a switch instruction as entry-point.
    /// Goto case is represented as branch.
    /// Break is represented as leave.
    /// </summary>
    Switch,
    /// <summary>
    /// while-loop.
    /// The entry-point is a block consisting of a single if instruction
    /// that if true: jumps to the head of the loop body,
    /// if false: leaves the block.
    /// Continue is a branch to the entry-point.
    /// Break is a leave.
    /// </summary>
    While,
    /// <summary>
    /// do-while-loop.
    /// The entry-point is a block that is the head of the loop body.
    /// The last block consists of a single if instruction
    /// that if true: jumps to the head of the loop body,
    /// if false: leaves the block.
    /// Only the last block is allowed to jump to the entry-point.
    /// Continue is a branch to the last block.
    /// Break is a leave.
    /// </summary>
    DoWhile,
    /// <summary>
    /// for-loop.
    /// The entry-point is a block consisting of a single if instruction
    /// that if true: jumps to the head of the loop body,
    /// if false: leaves the block.
    /// The last block is the increment block.
    /// Only the last block is allowed to jump to the entry-point.
    /// Continue is a branch to the last block.
    /// Break is a leave.
    /// </summary>
    For
}