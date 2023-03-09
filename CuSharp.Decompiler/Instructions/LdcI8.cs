﻿// Copyright (c) 2014-2020 Daniel Grunwald
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

namespace CuSharp.Decompiler.Instructions
{
    /// <summary>Loads a constant 64-bit integer.</summary>
    public sealed class LdcI8 : SimpleInstruction
    {
        public LdcI8(long value) : base(OpCode.LdcI8)
        {
            Value = value;
        }
        public readonly long Value;
        public override StackType ResultType => StackType.I8;

        public override void AcceptVisitor(ILVisitor visitor)
        {
            visitor.VisitLdcI8(this);
        }
        public override T AcceptVisitor<T>(ILVisitor<T> visitor)
        {
            return visitor.VisitLdcI8(this);
        }
    }
}