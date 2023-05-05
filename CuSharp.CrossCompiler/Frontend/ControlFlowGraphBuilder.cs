using System.Reflection;
using System.Runtime.InteropServices.JavaScript;
using LLVMSharp;

namespace CuSharp.CudaCompiler.Frontend;

public class ControlFlowGraphBuilder
{
    public ControlFlowGraphBuilder(BlockNode entryBlockNode, IList<LocalVariableInfo> localVariableInfos, LLVMBuilderRef builder, Func<string> RegisterNamer)
    {
        _entryBlockNode = entryBlockNode;
        CurrentBlock = entryBlockNode;
        _localVariables = localVariableInfos;
        _builder = builder;
        _registerNamer = RegisterNamer;
    }

    private readonly Dictionary<long, BlockNode> BlockList = new(); //contains all blocks except entry
    private readonly BlockNode _entryBlockNode;
    public BlockNode CurrentBlock;
    private readonly IList<LocalVariableInfo> _localVariables;
    private readonly LLVMBuilderRef _builder;
    private readonly Func<string> _registerNamer;
    private int BlockCounter => BlockList.Count;
    private string NextBlockName => $"block{BlockCounter}";
    
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
        PatchBlockGraph(_entryBlockNode);
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
                var value = pred.SavedStack.Pop();
                var phiType = phi.TypeOf();
                if (!value.TypeOf().Equals(phiType))
                {
                    value = LLVM.BuildIntCast(_builder, value, phiType, _registerNamer());
                } 
                
                phi.AddIncoming(new[]{value},new []{pred.BlockRef},1);
            }
        }
        
        //restore locals
        foreach (var phi in startNode.PhiInstructions) //Patch Phi Instructions
        {
            foreach (var pred in startNode.Predecessors)
            {
                if (pred.LocalVariables.ContainsKey(phi.Key))
                {
                    var value = pred.LocalVariables[phi.Key];
                    var phiType = phi.Value.TypeOf();
                    if (!value.TypeOf().Equals(phiType) && (value.TypeOf().Equals(LLVMTypeRef.FloatType()) || value.TypeOf().Equals(LLVMTypeRef.DoubleType())))
                    {
                        value = LLVM.BuildFPCast(_builder, value, phiType, _registerNamer());
                    } else if (!value.TypeOf().Equals(phiType) && phiType.TypeKind == LLVMTypeKind.LLVMPointerTypeKind)
                    {
                        value = LLVM.BuildPointerCast(_builder, value, phiType, _registerNamer());
                    }
                    else if (!value.TypeOf().Equals(phiType))
                    {
                        value = LLVM.BuildIntCast(_builder, value, phiType, _registerNamer());
                    }
                    phi.Value.AddIncoming(new[] { value }, new[] { pred.BlockRef }, 1);

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
    
    public void BuildPhis(Stack<LLVMValueRef> virtualRegisterStack)
    {
        //Restore stack
        if (CurrentBlock.Predecessors.Any()) //one predecessor must already exist to restore stack
        {
            var savedStackList = CurrentBlock.Predecessors.First().SavedStack.ToArray();
            for (int i = CurrentBlock.Predecessors.First().SavedStack.Count() -1; i > -1; i--) //predecessors must all contain same size of saved stack
            {
                var phi = LLVM.BuildPhi(_builder, savedStackList[i].TypeOf(), _registerNamer());
                virtualRegisterStack.Push(phi);
                CurrentBlock.RestoredStack.Push(phi);
            }
        }
        
        //Restore local variables 
        for (int i = 0; i < _localVariables.Count; i++)
        {
            if (!CurrentBlock.LocalVariables.ContainsKey(i))
            {
                var phi = LLVM.BuildPhi(_builder, _localVariables[i].LocalType.ToLLVMType(), _registerNamer());
                
                CurrentBlock.LocalVariables.Add(i, phi);
                CurrentBlock.PhiInstructions.Add(i, phi);
            }
        }
    }
}