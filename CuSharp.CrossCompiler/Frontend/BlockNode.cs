using LLVMSharp;

namespace CuSharp.CudaCompiler.Frontend;

public class BlockNode
{
    public LLVMBasicBlockRef BlockRef { get; set; }
    public Dictionary<int, LLVMValueRef> LocalVariables { get; set; } = new();//Per Local-Index
    public List<BlockNode> Predecessors { get; set; } = new();
    public List<BlockNode> Successors { get; set; } = new();
    public Dictionary<int, LLVMValueRef> PhiInstructions { get; set; } = new(); //Per Local-Index
    public bool Visited { get; set; } = false;
}