using System.Reflection;
using LLVMSharp;

namespace CuSharp.CudaCompiler.Frontend;

public class ControlFlowGraphBuilder
{
    public ControlFlowGraphBuilder(BlockNode entryBlockNode, IList<LocalVariableInfo> localVariableInfos)
    {
        EntryBlockNode = entryBlockNode;
        CurrentBlock = entryBlockNode;
        _localVariables = localVariableInfos;
    }

    public readonly Dictionary<long, BlockNode> BlockList = new(); //contains all blocks except entry
    public BlockNode EntryBlockNode;
    public BlockNode CurrentBlock;
    private IList<LocalVariableInfo> _localVariables;
    private int BlockCounter => BlockList.Count;
    public string NextBlockName => $"block{BlockCounter}";
    
    public BlockNode GetBlock(long index, LLVMValueRef function)
    {
        if (BlockList.ContainsKey(index))
        {
            return BlockList[index];
        }

        var block = LLVM.AppendBasicBlock(function, NextBlockName);
        BlockList.Add(index, new BlockNode() { BlockRef = block });
        return BlockList[index];
    }

    public bool PositionIsBlockStart(long position)
    {
        return BlockList.ContainsKey(position);
    }
    
    public void SwitchBlock(long position, LLVMBuilderRef builder)
    {
        var lastInstruction = LLVM.GetLastInstruction(CurrentBlock.BlockRef);
        if (lastInstruction.GetInstructionOpcode() != LLVMOpcode.LLVMBr)
        {
            LLVM.BuildBr(builder, BlockList[position].BlockRef); //Terminator instruction is needed!

            BlockList[position].Predecessors.Add(CurrentBlock);
            CurrentBlock.Successors.Add(BlockList[position]);
        }
        LLVM.PositionBuilderAtEnd(builder, BlockList[position].BlockRef);
        CurrentBlock = BlockList[position];
    }

    public void PatchBlockGraph()
    {
        PatchBlockGraph(EntryBlockNode);
    }
    
    private void PatchBlockGraph(BlockNode startNode)
    {
        if (startNode.Visited) return;
        startNode.Visited = true;

        //restore stack
        foreach (var phi in startNode.RestoredStack)
        {
            foreach (var pred in startNode.Predecessors)
            {
                phi.AddIncoming(new[]{pred.SavedStack.Pop()},new []{pred.BlockRef},1);
            }
        }
        
        //restore locals
        foreach (var phi in startNode.PhiInstructions) //Patch Phi Instructions
        {
            foreach (var pred in startNode.Predecessors)
            {
                if (pred.LocalVariables.ContainsKey(phi.Key))
                {
                    phi.Value.AddIncoming(new[] { pred.LocalVariables[phi.Key] }, new[] { pred.BlockRef }, 1);

                }
                else // Add arbitrary value (required because of NVVM)
                {
                    if (_localVariables[phi.Key].LocalType == typeof(float) ||
                        _localVariables[phi.Key].LocalType == typeof(double))
                    {
                        phi.Value.AddIncoming(new[] { LLVM.ConstReal(phi.Value.TypeOf(), 1.0) }, new[] { pred.BlockRef }, 1);
                    }
                    else if (_localVariables[phi.Key].LocalType.IsArray)
                    {
                        phi.Value.AddIncoming(new [] {LLVM.ConstPointerNull(phi.Value.TypeOf())}, new[]{pred.BlockRef}, 1);
                    }
                    else
                    {
                        phi.Value.AddIncoming(new[] { LLVM.ConstInt(phi.Value.TypeOf(), 1, false) }, new[] { pred.BlockRef }, 1);
                    }
                }
            }
        }

        foreach (var successor in startNode.Successors)
        {
            PatchBlockGraph(successor);
        }
    }
}