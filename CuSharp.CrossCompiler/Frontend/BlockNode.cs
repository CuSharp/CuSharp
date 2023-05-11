using System.Reflection;
using LLVMSharp;

namespace CuSharp.CudaCompiler.Frontend;

public class BlockNode
{
    public LLVMBasicBlockRef BlockRef { get; set; }
    public Dictionary<int, LLVMValueRef> LocalVariables { get; set; } = new();//Per Local-Index

    public Dictionary<int, LLVMValueRef> Parameters { get; set; } = new();
    public Dictionary<int, LLVMValueRef> RestoredParameters { get; set; } = new();
    public List<BlockNode> Predecessors { get; set; } = new();
    public List<BlockNode> Successors { get; set; } = new();
    public Dictionary<int, LLVMValueRef> RestoredLocals { get; set; } = new(); //Per Local-Index
    public bool Visited { get; set; } = false;
    
    public Stack<LLVMValueRef> VirtualRegisterStack { get; set; } = new();
    public Stack<LLVMValueRef> RestoredStack { get; set; } = new();

    public void AddSuccessors(params BlockNode[] nodes)
    {
        foreach (var node in nodes)
        {
            node.Predecessors.Add(this);
            Successors.Add(node);
        }
    }

}