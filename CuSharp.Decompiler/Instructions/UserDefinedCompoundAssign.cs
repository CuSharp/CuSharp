// Copyright (c) 2014-2020 Daniel Grunwald
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

using System.Diagnostics;
using System.Reflection;
using Dotnet4Gpu.Decompilation.Util;

namespace Dotnet4Gpu.Decompilation.Instructions;

public sealed class UserDefinedCompoundAssign : CompoundAssignmentInstruction
{
    protected override InstructionFlags ComputeFlags()
    {
        return base.ComputeFlags() | InstructionFlags.MayThrow | InstructionFlags.SideEffect;
    }
    public override InstructionFlags DirectFlags => base.DirectFlags | InstructionFlags.MayThrow | InstructionFlags.SideEffect;

    public override void AcceptVisitor(ILVisitor visitor)
    {
        visitor.VisitUserDefinedCompoundAssign(this);
    }
    public override T AcceptVisitor<T>(ILVisitor<T> visitor)
    {
        return visitor.VisitUserDefinedCompoundAssign(this);
    }

    public readonly MethodInfo Method;

    public UserDefinedCompoundAssign(MethodInfo method, CompoundEvalMode evalMode,
        ILInstruction target, CompoundTargetKind targetKind, ILInstruction value)
        : base(OpCode.UserDefinedCompoundAssign, evalMode, target, targetKind, value)
    {
        this.Method = method;
        //Debug.Assert(Method.IsOperator || IsStringConcat(method));
        Debug.Assert(evalMode == CompoundEvalMode.EvaluatesToNewValue || (Method.Name == "op_Increment" || Method.Name == "op_Decrement"));
    }

    public static bool IsStringConcat(MethodInfo method)
    {
        return method.Name == "Concat" && method.IsStatic && method.DeclaringType == typeof(string);
    }

    public override StackType ResultType => TypeUtils.GetStackType(Method.ReturnType);
}