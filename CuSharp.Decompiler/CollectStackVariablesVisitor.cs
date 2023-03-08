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

using System.Diagnostics;
using CuSharp.Decompiler.Instructions;
using CuSharp.Decompiler.Util;

namespace CuSharp.Decompiler;

class CollectStackVariablesVisitor : ILVisitor<ILInstruction>
{
    private readonly UnionFind<ILVariable> _unionFind;
    internal readonly HashSet<ILVariable> Variables = new();

    public CollectStackVariablesVisitor(UnionFind<ILVariable> unionFind)
    {
        Debug.Assert(unionFind != null);
        _unionFind = unionFind;
    }

    protected override ILInstruction Default(ILInstruction inst)
    {
        foreach (var child in inst.Children)
        {
            var newChild = child.AcceptVisitor(this);
            if (newChild != child)
                child.ReplaceWith(newChild);
        }
        return inst;
    }

    protected internal override ILInstruction VisitLdLoc(LdLoc inst)
    {
        base.VisitLdLoc(inst);
        if (inst.Variable.Kind == VariableKind.StackSlot)
        {
            var variable = _unionFind.Find(inst.Variable);
            if (Variables.Add(variable))
                variable.Name = "S_" + (Variables.Count - 1);
            return new LdLoc(variable).WithILRange(inst);
        }
        return inst;
    }

    protected internal override ILInstruction VisitStLoc(StLoc inst)
    {
        base.VisitStLoc(inst);
        if (inst.Variable.Kind == VariableKind.StackSlot)
        {
            var variable = _unionFind.Find(inst.Variable);
            if (Variables.Add(variable))
                variable.Name = "S_" + (Variables.Count - 1);
            return new StLoc(variable, inst.Value).WithILRange(inst);
        }
        return inst;
    }
}