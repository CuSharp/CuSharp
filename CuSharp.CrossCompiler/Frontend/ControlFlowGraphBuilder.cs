using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices.JavaScript;
using LLVMSharp;

namespace CuSharp.CudaCompiler.Frontend;

public class ControlFlowGraphBuilder
{
    public ControlFlowGraphBuilder(LLVMValueRef function, IList<LocalVariableInfo> localVariableInfos, IList<ParameterInfo> parameterInfos, LLVMBuilderRef builder, Func<string> RegisterNamer)
    {

        _entryBlockNode = new BlockNode() {BlockRef = LLVM.AppendBasicBlock(function, "entry")};
        _localVariables = localVariableInfos;
        _builder = builder;
        _registerNamer = RegisterNamer;
        _parameterInfos = parameterInfos;
        
        
        CurrentBlock = _entryBlockNode;
        LLVM.PositionBuilderAtEnd(builder, _entryBlockNode.BlockRef);
        
        for (int i = 0; i < parameterInfos.Count; i++)
        {
            _entryBlockNode.Parameters.Add(i, LLVM.GetParam(function, (uint) i));
        }

        _blockList.Add(0, new BlockNode(){BlockRef = LLVM.AppendBasicBlock(function, NextBlockName)});
    }

    private readonly Dictionary<long, BlockNode> _blockList = new(); //contains all blocks except entry
    private readonly BlockNode _entryBlockNode;
    public BlockNode CurrentBlock;
    private readonly IList<LocalVariableInfo> _localVariables;
    private readonly IList<ParameterInfo> _parameterInfos;
    private readonly LLVMBuilderRef _builder;
    private readonly Func<string> _registerNamer;
    private int BlockCounter => _blockList.Count;
    private string NextBlockName => $"block{BlockCounter}";
    
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

    public bool PositionIsBlockStart(long position)
    {
        return _blockList.ContainsKey(position);
    }
    
    public void SwitchBlock(long position)
    {
        var lastInstruction = LLVM.GetLastInstruction(CurrentBlock.BlockRef);
        if (lastInstruction.Pointer == 0x0 || lastInstruction.GetInstructionOpcode() != LLVMOpcode.LLVMBr)
        {
            LLVM.BuildBr(_builder, _blockList[position].BlockRef); //Terminator instruction is needed
            _blockList[position].Predecessors.Add(CurrentBlock);
            CurrentBlock.Successors.Add(_blockList[position]);
        }
        LLVM.PositionBuilderAtEnd(_builder, _blockList[position].BlockRef);
        CurrentBlock = _blockList[position];
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
        foreach (var pred in startNode.Predecessors)
        {
            var stackImage = new Stack<LLVMValueRef>(new Stack<LLVMValueRef>(pred.VirtualRegisterStack));
            
            foreach (var phi in startNode.RestoredStack)
            {
                var value = stackImage.Pop();
                value = BuildCastIfIncompatible(value, phi.TypeOf());
                phi.AddIncoming(new[]{value},new []{pred.BlockRef},1);
            }
        }
        
        //restore locals
        foreach (var phi in startNode.RestoredLocals) //Patch Phi Instructions
        {
            foreach (var pred in startNode.Predecessors)
            {
                if (pred.LocalVariables.ContainsKey(phi.Key))
                {
                    var value = pred.LocalVariables[phi.Key];
                    value = BuildCastIfIncompatible(value, phi.Value.TypeOf());
                    phi.Value.AddIncoming(new[] { value }, new[] { pred.BlockRef }, 1);

                }
                else // Add arbitrary value (required because of NVVM)
                {
                    phi.Value.AddIncoming(new[]{GetArbitraryValue(phi.Value)}, new[]{pred.BlockRef},1);
                }
            }
        }

        foreach (var phi in startNode.RestoredParameters) //Patch Phi Instructions
        {
            foreach (var pred in startNode.Predecessors)
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
        
        foreach (var successor in startNode.Successors)
        {
            PatchBlockGraph(successor);
        }
    }
    
    public void BuildPhisForCurrentBlock()
    {
        BuildPhis(CurrentBlock);
    }

    private void BuildPhis(BlockNode block)
    {
        //Restore stack
        if (block.Predecessors.Any()) //one predecessor must already exist to restore stack
        {
            var savedStackList = block.Predecessors.First().VirtualRegisterStack.Reverse().ToArray();
            for (int i = 0; i < block.Predecessors.First().VirtualRegisterStack.Count(); i++) //predecessors must all contain same size of saved stack
            {
                var phi = LLVM.BuildPhi(_builder, savedStackList[i].TypeOf(), _registerNamer());
                block.VirtualRegisterStack.Push(phi);
                block.RestoredStack.Push(phi);
            }
        }
        
        BuildPhisForValues(_localVariables.Count, block.LocalVariables, block.RestoredLocals, _localVariables
            .Select(v => v.LocalType.ToLLVMType())
            .ToArray());

        BuildPhisForValues(_parameterInfos.Count, block.Parameters, block.RestoredParameters, _parameterInfos
            .Select(p => p.ParameterType.ToLLVMType())
            .ToArray());
    }
    
    private void BuildPhisForValues(int amountOfValues, Dictionary<int, LLVMValueRef> values,
        Dictionary<int, LLVMValueRef> restoredValues, LLVMTypeRef[] phiTypes)
    {
        for (int i = 0; i < amountOfValues; i++)
        {
            if (!values.ContainsKey(i))
            {
                var phi = LLVM.BuildPhi(_builder, phiTypes[i], _registerNamer());
                values.Add(i, phi);
                restoredValues.Add(i, phi);
            }
        }
    }
    
    private LLVMValueRef BuildCastIfIncompatible(LLVMValueRef value, LLVMTypeRef typeToCompare)
    {
        if (!value.TypeOf().Equals(typeToCompare) && (value.TypeOf().Equals(LLVMTypeRef.FloatType()) || value.TypeOf().Equals(LLVMTypeRef.DoubleType())))
        {
            value = LLVM.BuildFPCast(_builder, value, typeToCompare, _registerNamer());
        } else if (!value.TypeOf().Equals(typeToCompare) && typeToCompare.TypeKind == LLVMTypeKind.LLVMPointerTypeKind)
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