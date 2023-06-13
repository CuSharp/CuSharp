using CuSharp.CudaCompiler.Kernels;
using CuSharp.CudaCompiler.LLVMConfiguration;
using LLVMSharp;

namespace CuSharp.CudaCompiler.ControlFlowGraph;

public class ControlFlowGraphBuilder
{
    private readonly Dictionary<long, BlockNode> _blockList = new(); //contains all blocks except entry
    private readonly BlockNode _entryBlockNode;
    public BlockNode CurrentBlock { get; private set; }
    private readonly MSILKernel _inputKernel;
    private readonly LLVMBuilderRef _builder;
    private readonly Func<string> _registerNamer;
    private int BlockCounter => _blockList.Count;
    private string NextBlockName => $"block{BlockCounter}";

    public ControlFlowGraphBuilder(LLVMValueRef function, MSILKernel inputKernel, LLVMBuilderRef builder,
        Func<string> registerNamer)
    {

        _entryBlockNode = new BlockNode() { BlockRef = LLVM.AppendBasicBlock(function, "entry") };
        _builder = builder;
        _registerNamer = registerNamer;
        _inputKernel = inputKernel;
        CurrentBlock = _entryBlockNode;

        LLVM.PositionBuilderAtEnd(builder, _entryBlockNode.BlockRef);

        for (int i = 0; i < _inputKernel.ParameterInfos.Length; i++)
        {
            _entryBlockNode.Parameters.Add(i, LLVM.GetParam(function, (uint)i));
        }

        _blockList.Add(0, new BlockNode() { BlockRef = LLVM.AppendBasicBlock(function, NextBlockName) });
    }

    public BlockNode GetBlock(long index, LLVMValueRef function)
    {

        if (index == 1) //debug mode: skip nop
        {
            index = 0;
        }

        if (_blockList.ContainsKey(index))
        {
            return _blockList[index];
        }

        var block = LLVM.AppendBasicBlock(function, NextBlockName);
        _blockList.Add(index, new BlockNode() { BlockRef = block });

        return _blockList[index];
    }

    public void UpdateBlocks(long currentPosition)
    {
        if (_blockList.ContainsKey(currentPosition))
        {
            SwitchBlock(currentPosition);
        }
    }

    public void PatchBlockGraph()
    {
        PatchBlockGraph(_entryBlockNode);
    }

    private void PatchBlockGraph(BlockNode startNode)
    {
        if (startNode.Visited) return;
        startNode.Visited = true;

        RestoreStack(startNode);
        RestoreLocals(startNode);
        RestoreParams(startNode);

        foreach (var successor in startNode.Successors)
        {
            PatchBlockGraph(successor);
        }
    }

    private void SwitchBlock(long position)
    {
        var lastInstruction = LLVM.GetLastInstruction(CurrentBlock.BlockRef);
        if (lastInstruction.Pointer == 0x0 || (lastInstruction.GetInstructionOpcode() != LLVMOpcode.LLVMBr &&
                                               lastInstruction.GetInstructionOpcode() != LLVMOpcode.LLVMRet))
        {
            LLVM.BuildBr(_builder, _blockList[position].BlockRef); //Terminator instruction is needed
            _blockList[position].Predecessors.Add(CurrentBlock);
            CurrentBlock.Successors.Add(_blockList[position]);
        }

        LLVM.PositionBuilderAtEnd(_builder, _blockList[position].BlockRef);
        CurrentBlock = _blockList[position];
        CurrentBlock.BuildPhis(_inputKernel, _builder, _registerNamer);
    }

    private void RestoreStack(BlockNode node)
    {

        foreach (var pred in node.Predecessors)
        {
            var stackImage = new Stack<LLVMValueRef>(new Stack<LLVMValueRef>(pred.VirtualRegisterStack));
            foreach (var phi in node.RestoredStack)
            {
                var value = stackImage.Pop();
                value = BuildCastIfIncompatible(value, phi.TypeOf());
                phi.AddIncoming(new[] { value }, new[] { pred.BlockRef }, 1);
            }
        }
    }

    private void RestoreLocals(BlockNode node)
    {
        foreach (var phi in node.RestoredLocals) //Patch Phi Instructions
        {
            foreach (var pred in node.Predecessors)
            {

                if (pred.LocalVariables.ContainsKey(phi.Key))
                {
                    var value = pred.LocalVariables[phi.Key];
                    value = BuildCastIfIncompatible(value, phi.Value.TypeOf());
                    phi.Value.AddIncoming(new[] { value }, new[] { pred.BlockRef }, 1);
                }
                else // Add arbitrary value (required because of NVVM)
                {
                    phi.Value.AddIncoming(new[] { GetArbitraryValue(phi.Value) }, new[] { pred.BlockRef }, 1);
                }
            }
        }
    }

    private void RestoreParams(BlockNode node)
    {
        foreach (var phi in node.RestoredParameters) //Patch Phi Instructions
        {
            foreach (var pred in node.Predecessors)
            {
                if (pred.Parameters.ContainsKey(phi.Key))
                {
                    var value = pred.Parameters[phi.Key];
                    value = BuildCastIfIncompatible(value, phi.Value.TypeOf());
                    phi.Value.AddIncoming(new[] { value }, new[] { pred.BlockRef }, 1);
                }
                else
                {
                    throw new Exception("Parameter did not exist in all predecessors.");
                }
            }
        }
    }

    private LLVMValueRef BuildCastIfIncompatible(LLVMValueRef value, LLVMTypeRef typeToCompare)
    {
        if (!value.TypeOf().Equals(typeToCompare) && (value.TypeOf().Equals(LLVMTypeRef.FloatType()) ||
                                                      value.TypeOf().Equals(LLVMTypeRef.DoubleType())))
        {
            value = LLVM.BuildFPCast(_builder, value, typeToCompare, _registerNamer());
        }
        else if (!value.TypeOf().Equals(typeToCompare) && typeToCompare.TypeKind == LLVMTypeKind.LLVMPointerTypeKind)
        {
            value = LLVM.BuildPointerCast(_builder, value, typeToCompare, _registerNamer());
        }
        else if (!value.TypeOf().Equals(typeToCompare))
        {
            value = LLVM.BuildIntCast(_builder, value, typeToCompare, _registerNamer());
        }

        return value;
    }

    private LLVMValueRef GetArbitraryValue(LLVMValueRef value)
    {
        if (value.TypeOf().ToNativeType() == typeof(float) ||
            value.TypeOf().ToNativeType() == typeof(double))
            return LLVM.ConstReal(value.TypeOf(), 1.0);

        if (value.TypeOf().ToNativeType().IsArray)
            return LLVM.ConstPointerNull(value.TypeOf());

        return LLVM.ConstInt(value.TypeOf(), 1, false);
    }
}