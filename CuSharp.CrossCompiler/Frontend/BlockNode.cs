using System.Reflection;
using LLVMSharp;

namespace CuSharp.CudaCompiler.Frontend;

public class BlockNode
{
    public LLVMBasicBlockRef BlockRef { get; set; }
    public Dictionary<int, LLVMValueRef> LocalVariables { get; set; } = new();//Per Local-Index
    public Dictionary<int, LLVMValueRef> RestoredLocals { get; set; } = new(); //Per Local-Index, local where instruction type == phi
    public Dictionary<int, LLVMValueRef> Parameters { get; set; } = new();
    public Dictionary<int, LLVMValueRef> RestoredParameters { get; set; } = new(); //parameter where instruction type == phi
    public List<BlockNode> Predecessors { get; set; } = new();
    public List<BlockNode> Successors { get; set; } = new();
    public bool Visited { get; set; } = false;
    
    public Stack<LLVMValueRef> VirtualRegisterStack { get; set; } = new();
    public Stack<LLVMValueRef> RestoredStack { get; set; } = new(); //stack where type == phi

    public void AddSuccessors(params BlockNode[] nodes)
    {
        foreach (var node in nodes)
        {
            node.Predecessors.Add(this);
            Successors.Add(node);
        }
    }
    
    public void BuildPhis(MSILKernel _inputKernel, LLVMBuilderRef _builder, Func<string> registerNamer)
    {
        BuildPhisForValues(Parameters, RestoredParameters, _inputKernel.ParameterInfos
                    .Select(p => p.ParameterType.ToLLVMType())
                    .ToArray(), _builder, registerNamer);
        
        BuildPhisForValues(LocalVariables, RestoredLocals, _inputKernel.LocalVariables
            .Select(v => v.LocalType.ToLLVMType())
            .ToArray(), _builder, registerNamer);
        
        //Restore stack
        if (Predecessors.Any()) //one predecessor must already exist to restore stack
        {
            var savedStackList = Predecessors.First().VirtualRegisterStack.Reverse().ToArray();
            for (int i = 0; i < Predecessors.First().VirtualRegisterStack.Count(); i++) //predecessors must all contain same size of saved stack
            {
                var phi = LLVM.BuildPhi(_builder, savedStackList[i].TypeOf(), registerNamer());
                VirtualRegisterStack.Push(phi);
                RestoredStack.Push(phi);
            }
        }
        
    }
    
    private void BuildPhisForValues(Dictionary<int, LLVMValueRef> values,
        Dictionary<int, LLVMValueRef> restoredValues, LLVMTypeRef[] phiTypes, LLVMBuilderRef builder, Func<string> registerNamer)
    {
        for (int i = 0; i < phiTypes.Length; i++)
        {
            var phi = LLVM.BuildPhi(builder, phiTypes[i], registerNamer());
            values.Add(i, phi);
            restoredValues.Add(i, phi);
        }
    }

}