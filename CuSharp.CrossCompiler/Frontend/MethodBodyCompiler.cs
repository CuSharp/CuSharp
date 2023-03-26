﻿using System.Reflection.Metadata;
using LLVMSharp;

namespace CuSharp.CudaCompiler.Frontend;

public class MethodBodyCompiler
{
    private readonly BinaryReader _reader;
    private readonly MSILKernel _inputKernel;
    private readonly LLVMBuilderRef _builder;
    private readonly FunctionsDto _functionsDto;
    private readonly Stack<LLVMValueRef> _virtualRegisterStack = new();
    private readonly Dictionary<long, BlockNode> _blockList = new(); //contains all blocks except entry
    private readonly MemoryStream _stream;

    private BlockNode _entryBlockNode;
    private BlockNode _currentBlock;
    private long _virtualRegisterCounter;
    private long _blockCounter;
    private string? _nameOfMethodToCall;

    public MethodBodyCompiler(MSILKernel inputKernel, LLVMBuilderRef builder, FunctionsDto functionsDto)
    {
        _inputKernel = inputKernel;
        _builder = builder;
        _functionsDto = functionsDto;
        _stream = new MemoryStream(inputKernel.KernelBuffer);
        _reader = new BinaryReader(_stream);
    }

    public IEnumerable<(ILOpCode, object?)> CompileMethodBody()
    {

        _entryBlockNode = new BlockNode() { BlockRef = LLVM.GetEntryBasicBlock(_functionsDto.Function) };
        _currentBlock = _entryBlockNode;
        IList<(ILOpCode, object?)> opCodes = new List<(ILOpCode, object?)>();
        while (_reader.BaseStream.Position < _reader.BaseStream.Length)
        {

            if (_reader.BaseStream.Position == _reader.BaseStream.Length)
            {
                throw new ArgumentOutOfRangeException("Unexpected end of method body.");
            }

            if (_blockList.ContainsKey(_stream.Position))
            {


                var lastInstruction = LLVM.GetLastInstruction(_currentBlock.BlockRef);
                if (lastInstruction.GetInstructionOpcode() != LLVMOpcode.LLVMBr)
                {
                    LLVM.BuildBr(_builder, _blockList[_stream.Position].BlockRef); //Terminator instruction is needed!

                    _blockList[_stream.Position].Predecessors.Add(_currentBlock);
                    _currentBlock.Successors.Add(_blockList[_stream.Position]);
                }


                LLVM.PositionBuilderAtEnd(_builder, _blockList[_stream.Position].BlockRef);
                _currentBlock = _blockList[_stream.Position];

                BuildPhis();
            }

            opCodes.Add(CompileNextOpCode());
        }

        PatchBlockGraph(_entryBlockNode);

        return opCodes;
    }

    private (ILOpCode, object?) CompileNextOpCode()
    {
        var opCode = ReadOpCode();
        object? operand = null;
        switch (opCode)
        {
            case ILOpCode.Nop:
                break;

            //case ILOpCode.Constrained: throw new NotSupportedException();
            //case ILOpCode.Readonly: throw new NotSupportedException();
            //case ILOpCode.Tail: throw new NotSupportedException();
            //case ILOpCode.Unaligned: throw new NotSupportedException();
            //case ILOpCode.Volatile: throw new NotSupportedException();
            case ILOpCode.Add:
                CompileAdd();
                break;
            //case ILOpCode.Add_ovf: throw new NotSupportedException();
            //case ILOpCode.Add_ovf_un: throw new NotSupportedException();
            //case ILOpCode.And: throw new NotSupportedException();
            //case ILOpCode.Arglist: throw new NotSupportedException();
            //case ILOpCode.Beq: throw new NotSupportedException();
            //case ILOpCode.Beq_s: throw new NotSupportedException();
            //case ILOpCode.Bge: throw new NotSupportedException();
            //case ILOpCode.Bge_s: throw new NotSupportedException();
            //case ILOpCode.Bge_un: throw new NotSupportedException();
            //case ILOpCode.Bge_un_s: throw new NotSupportedException();
            //case ILOpCode.Bgt: throw new NotSupportedException();
            //case ILOpCode.Bgt_s: throw new NotSupportedException();
            //case ILOpCode.Bgt_un: throw new NotSupportedException();
            //case ILOpCode.Bgt_un_s: throw new NotSupportedException();
            //case ILOpCode.Ble: throw new NotSupportedException();
            //case ILOpCode.Ble_s: throw new NotSupportedException();
            //case ILOpCode.Ble_un: throw new NotSupportedException();
            //case ILOpCode.Ble_un_s: throw new NotSupportedException();
            //case ILOpCode.Blt: throw new NotSupportedException();
            //case ILOpCode.Blt_s: throw new NotSupportedException();
            //case ILOpCode.Blt_un: throw new NotSupportedException();
            //case ILOpCode.Blt_un_s: throw new NotSupportedException();
            //case ILOpCode.Bne_un: throw new NotSupportedException();
            //case ILOpCode.Bne_un_s: throw new NotSupportedException();
            case ILOpCode.Br:
                operand = _reader.ReadInt32();
                CompileBr((int)operand);
                break;
            case ILOpCode.Br_s:
                operand = _reader.ReadSByte();
                CompileBr((sbyte)operand);
                break;
            //case ILOpCode.Break: throw new NotSupportedException();
            case ILOpCode.Brfalse:
                operand = _reader.ReadInt32();
                CompileBrFalse((int)operand);
                break;
            case ILOpCode.Brfalse_s:
                operand = _reader.ReadSByte();
                CompileBrFalse((sbyte)operand);
                break;
            case ILOpCode.Brtrue:
                operand = _reader.ReadInt32();
                CompileBrTrue((int)operand);
                break;
            case ILOpCode.Brtrue_s:
                operand = _reader.ReadSByte();
                CompileBrTrue((sbyte)operand);
                break;
            case ILOpCode.Call:
                operand = _reader.ReadInt32();
                CompileCall((int)operand);
                break;
            //case ILOpCode.Callvirt: throw new NotSupportedException();
            //case ILOpCode.Calli: throw new NotSupportedException();
            case ILOpCode.Ceq:
                CompileCeq();
                break;
            case ILOpCode.Cgt:
                CompileCgt();
                break;
            //case ILOpCode.Cgt_un: throw new NotSupportedException();
            case ILOpCode.Clt:
                CompileClt();
                break;
            //case ILOpCode.Clt_un: throw new NotSupportedException();
            //case ILOpCode.Ckfinite: throw new NotSupportedException();
            //case ILOpCode.Conv_i1: throw new NotSupportedException();
            //case ILOpCode.Conv_i2: throw new NotSupportedException();
            //case ILOpCode.Conv_i4: throw new NotSupportedException();
            case ILOpCode.Conv_i8: break; //TODO
            //case ILOpCode.Conv_r4: throw new NotSupportedException();
            //case ILOpCode.Conv_r8: throw new NotSupportedException();
            //case ILOpCode.Conv_u1: throw new NotSupportedException();
            //case ILOpCode.Conv_u2: throw new NotSupportedException();
            //case ILOpCode.Conv_u4: throw new NotSupportedException();
            case ILOpCode.Conv_u8: break; //TODO
            //case ILOpCode.Conv_i: throw new NotSupportedException();
            //case ILOpCode.Conv_u: throw new NotSupportedException();
            //case ILOpCode.Conv_r_un: throw new NotSupportedException();
            //case ILOpCode.Conv_ovf_i1: throw new NotSupportedException();
            //case ILOpCode.Conv_ovf_i2: throw new NotSupportedException();
            //case ILOpCode.Conv_ovf_i4: throw new NotSupportedException();
            //case ILOpCode.Conv_ovf_i8: throw new NotSupportedException();
            //case ILOpCode.Conv_ovf_u1: throw new NotSupportedException();
            //case ILOpCode.Conv_ovf_u2: throw new NotSupportedException();
            //case ILOpCode.Conv_ovf_u4: throw new NotSupportedException();
            //case ILOpCode.Conv_ovf_u8: throw new NotSupportedException();
            case ILOpCode.Conv_ovf_i: break; //TODO 
            //case ILOpCode.Conv_ovf_u: throw new NotSupportedException();
            //case ILOpCode.Conv_ovf_i1_un: throw new NotSupportedException();
            //case ILOpCode.Conv_ovf_i2_un: throw new NotSupportedException();
            //case ILOpCode.Conv_ovf_i4_un: throw new NotSupportedException();
            //case ILOpCode.Conv_ovf_i8_un: throw new NotSupportedException();
            //case ILOpCode.Conv_ovf_u1_un: throw new NotSupportedException();
            //case ILOpCode.Conv_ovf_u2_un: throw new NotSupportedException();
            //case ILOpCode.Conv_ovf_u4_un: throw new NotSupportedException();
            //case ILOpCode.Conv_ovf_u8_un: throw new NotSupportedException();
            //case ILOpCode.Conv_ovf_i_un: throw new NotSupportedException();
            //case ILOpCode.Conv_ovf_u_un: throw new NotSupportedException();
            //case ILOpCode.Cpblk: throw new NotSupportedException();
            case ILOpCode.Div:
                CompileDiv();
                break;
            //case ILOpCode.Div_un: throw new NotSupportedException();
            case ILOpCode.Dup:
                CompileDup();
                break;
            //case ILOpCode.Endfilter: throw new NotSupportedException();
            //case ILOpCode.Endfinally: throw new NotSupportedException();
            //case ILOpCode.Initblk: throw new NotSupportedException();
            //case ILOpCode.Jmp: throw new NotSupportedException();
            case ILOpCode.Ldarg:
                operand = _reader.ReadUInt16();
                CompileLdarg((ushort)operand);
                break;
            case ILOpCode.Ldarg_s:
                operand = _reader.ReadByte();
                CompileLdarg((byte)operand);
                break;
            case ILOpCode.Ldarg_0:
                CompileLdarg(0);
                break;
            case ILOpCode.Ldarg_1:
                CompileLdarg(1);
                break;
            case ILOpCode.Ldarg_2:
                CompileLdarg(2);
                break;
            case ILOpCode.Ldarg_3:
                CompileLdarg(3);
                break;
            //case ILOpCode.Ldarga: throw new NotSupportedException();
            //case ILOpCode.Ldarga_s: throw new NotSupportedException();
            case ILOpCode.Ldc_i4:
                operand = _reader.ReadInt32();
                CompileLdcInt((int)operand);
                break;
            case ILOpCode.Ldc_i8:
                operand = _reader.ReadInt64();
                CompileLdcLong((long)operand);
                break;
            case ILOpCode.Ldc_r4:
                operand = _reader.ReadSingle();
                CompileLdcFloat((float)operand);
                break;
            case ILOpCode.Ldc_r8:
                operand = _reader.ReadDouble();
                CompileLdcDouble((double)operand);
                break;
            case ILOpCode.Ldc_i4_m1:
                CompileLdcInt(-1);
                break;
            case ILOpCode.Ldc_i4_0:
                CompileLdcInt(0);
                break;
            case ILOpCode.Ldc_i4_1:
                CompileLdcInt(1);
                break;
            case ILOpCode.Ldc_i4_2:
                CompileLdcInt(2);
                break;
            case ILOpCode.Ldc_i4_3:
                CompileLdcInt(3);
                break;
            case ILOpCode.Ldc_i4_4:
                CompileLdcInt(4);
                break;
            case ILOpCode.Ldc_i4_5:
                CompileLdcInt(5);
                break;
            case ILOpCode.Ldc_i4_6:
                CompileLdcInt(6);
                break;
            case ILOpCode.Ldc_i4_7:
                CompileLdcInt(7);
                break;
            case ILOpCode.Ldc_i4_8:
                CompileLdcInt(8);
                break;
            case ILOpCode.Ldc_i4_s:
                operand = _reader.ReadSByte();
                CompileLdcInt((sbyte)operand);
                break;
            //case ILOpCode.Ldnull: throw new NotSupportedException();
            //case ILOpCode.Ldstr: throw new NotSupportedException();
            //case ILOpCode.Ldftn: throw new NotSupportedException();
            case ILOpCode.Ldind_i1:
            case ILOpCode.Ldind_i2:
            case ILOpCode.Ldind_i4:
            case ILOpCode.Ldind_i8:
            case ILOpCode.Ldind_u1:
            case ILOpCode.Ldind_u2:
            case ILOpCode.Ldind_u4:
            case ILOpCode.Ldind_r4:
            case ILOpCode.Ldind_r8:
                CompileLdind();
                break;
            //case ILOpCode.Ldind_i: throw new NotSupportedException();
            //case ILOpCode.Ldind_ref: throw new NotSupportedException();
            case ILOpCode.Ldloc:
                operand = _reader.ReadUInt16();
                CompileLdloc((ushort)operand);
                break;
            case ILOpCode.Ldloc_s:
                operand = _reader.ReadByte();
                CompileLdloc((byte)operand);
                break;
            case ILOpCode.Ldloc_0:
                CompileLdloc(0);
                break;
            case ILOpCode.Ldloc_1:
                CompileLdloc(1);
                break;
            case ILOpCode.Ldloc_2:
                CompileLdloc(2);
                break;
            case ILOpCode.Ldloc_3:
                CompileLdloc(3);
                break;
            //case ILOpCode.Ldloca: throw new NotSupportedException();
            //case ILOpCode.Ldloca_s: throw new NotSupportedException();
            //case ILOpCode.Leave: throw new NotSupportedException();
            //case ILOpCode.Leave_s: throw new NotSupportedException();
            //case ILOpCode.Localloc: throw new NotSupportedException();
            case ILOpCode.Mul:
                CompileMul();
                break;
            //case ILOpCode.Mul_ovf: throw new NotSupportedException();
            //case ILOpCode.Mul_ovf_un: throw new NotSupportedException();
            //case ILOpCode.Neg: throw new NotSupportedException();
            //case ILOpCode.Newobj: throw new NotSupportedException();
            //case ILOpCode.Not: throw new NotSupportedException();
            //case ILOpCode.Or: throw new NotSupportedException();
            //case ILOpCode.Pop: throw new NotSupportedException();
            case ILOpCode.Rem:
                CompileRem();
                break;
            //case ILOpCode.Rem_un: throw new NotSupportedException();
            case ILOpCode.Ret:
                LLVM.BuildRetVoid(_builder);
                break;
            //case ILOpCode.Shl: throw new NotSupportedException();
            //case ILOpCode.Shr: throw new NotSupportedException();
            //case ILOpCode.Shr_un: throw new NotSupportedException();
            case ILOpCode.Starg:
                operand = _reader.ReadInt16();
                CompileStarg((ushort)operand);
                break;
            case ILOpCode.Starg_s:
                operand = _reader.ReadByte();
                CompileStarg((byte)operand);
                break;
            case ILOpCode.Stind_i1:
            case ILOpCode.Stind_i2:
            case ILOpCode.Stind_i4:
            case ILOpCode.Stind_i8:
            case ILOpCode.Stind_r4:
            case ILOpCode.Stind_r8:
                CompileStdind();
                break;
            //case ILOpCode.Stind_i: throw new NotSupportedException();
            //case ILOpCode.Stind_ref: throw new NotSupportedException();
            case ILOpCode.Stloc:
                operand = _reader.ReadUInt16();
                CompileStloc((ushort)operand);
                break;
            case ILOpCode.Stloc_s:
                operand = _reader.ReadByte();
                CompileStloc((byte)operand);
                break;
            case ILOpCode.Stloc_0:
                CompileStloc(0);
                break;
            case ILOpCode.Stloc_1:
                CompileStloc(1);
                break;
            case ILOpCode.Stloc_2:
                CompileStloc(2);
                break;
            case ILOpCode.Stloc_3:
                CompileStloc(3);
                break;
            case ILOpCode.Sub:
                CompileSub();
                break;
            //case ILOpCode.Sub_ovf: throw new NotSupportedException();
            //case ILOpCode.Sub_ovf_un: throw new NotSupportedException();
            //case ILOpCode.Switch: throw new NotSupportedException();
            //case ILOpCode.Xor: throw new NotSupportedException();
            //case ILOpCode.Box: throw new NotSupportedException();
            //case ILOpCode.Castclass: throw new NotSupportedException();
            //case ILOpCode.Cpobj: throw new NotSupportedException();
            //case ILOpCode.Initobj: throw new NotSupportedException();
            //case ILOpCode.Isinst: throw new NotSupportedException();
            //case ILOpCode.Ldelem: throw new NotSupportedException();
            case ILOpCode.Ldelem_i1:
            case ILOpCode.Ldelem_i2:
            case ILOpCode.Ldelem_i4:
            case ILOpCode.Ldelem_i8:
            case ILOpCode.Ldelem_u1:
            case ILOpCode.Ldelem_u2:
            case ILOpCode.Ldelem_u4:
            case ILOpCode.Ldelem_r4:
            case ILOpCode.Ldelem_r8:
                CompileLdelem();
                break;
            //case ILOpCode.Ldelem_i: throw new NotSupportedException();
            //case ILOpCode.Ldelem_ref: throw new NotSupportedException();
            case ILOpCode.Ldelema:
                operand = _reader.ReadInt32(); // No further usage of operand required
                CompileLdelema();
                break;
            case ILOpCode.Ldfld:
                operand = _reader.ReadInt32();
                CompileLdfld((int)operand);
                break;
            //case ILOpCode.Ldflda: throw new NotSupportedException();
            //case ILOpCode.Stfld: throw new NotSupportedException();
            //case ILOpCode.Ldlen: throw new NotSupportedException();
            //case ILOpCode.Ldobj: throw new NotSupportedException();
            //case ILOpCode.Ldsfld: throw new NotSupportedException();
            //case ILOpCode.Ldsflda: throw new NotSupportedException();
            //case ILOpCode.Stsfld: throw new NotSupportedException();
            //case ILOpCode.Ldtoken: throw new NotSupportedException();
            //case ILOpCode.Ldvirtftn: throw new NotSupportedException();
            //case ILOpCode.Mkrefany: throw new NotSupportedException();
            //case ILOpCode.Newarr: throw new NotSupportedException();
            //case ILOpCode.Refanytype: throw new NotSupportedException();
            //case ILOpCode.Refanyval: throw new NotSupportedException();
            //case ILOpCode.Rethrow: throw new NotSupportedException();
            //case ILOpCode.Sizeof: throw new NotSupportedException();
            //case ILOpCode.Stelem: throw new NotSupportedException();
            case ILOpCode.Stelem_i1:
            case ILOpCode.Stelem_i2:
            case ILOpCode.Stelem_i4:
            case ILOpCode.Stelem_i8:
            case ILOpCode.Stelem_r4:
            case ILOpCode.Stelem_r8:
                CompileStelem();
                break;
            //case ILOpCode.Stelem_i: throw new NotSupportedException();
            //case ILOpCode.Stelem_ref: throw new NotSupportedException();
            //case ILOpCode.Stobj: throw new NotSupportedException();
            //case ILOpCode.Throw: throw new NotSupportedException();
            //case ILOpCode.Unbox: throw new NotSupportedException();
            //case ILOpCode.Unbox_any: throw new NotSupportedException();
            default: throw new NotSupportedException();
        }

        return (opCode, operand);
    }

    #region Compile Instructions

    #region Operations

    private void CompileAdd()
    {
        var param2 = _virtualRegisterStack.Pop();
        var param1 = _virtualRegisterStack.Pop();
        LLVMValueRef result;

        if (AreParamsCompatibleAndInt(param1, param2))
        {
            result = LLVM.BuildAdd(_builder, param1, param2, GetVirtualRegisterName());
        }
        else if (AreParamsCompatibleAndDecimal(param1, param2))
        {
            result = LLVM.BuildFAdd(_builder, param1, param2, GetVirtualRegisterName());
        }
        else
        {
            throw new ArgumentException($"Type {param1} and {param2} are not supported or have not the same type");
        }

        _virtualRegisterStack.Push(result);
    }

    private void CompileSub()
    {
        var param2 = _virtualRegisterStack.Pop();
        var param1 = _virtualRegisterStack.Pop();
        LLVMValueRef result;

        if (AreParamsCompatibleAndInt(param1, param2))
        {
            result = LLVM.BuildSub(_builder, param1, param2, GetVirtualRegisterName());
        }
        else if (AreParamsCompatibleAndDecimal(param1, param2))
        {
            result = LLVM.BuildFSub(_builder, param1, param2, GetVirtualRegisterName());
        }
        else
        {
            throw new ArgumentException($"Type {param1} and {param2} are not supported or have not the same type");
        }

        _virtualRegisterStack.Push(result);
    }

    private void CompileMul()
    {
        var param2 = _virtualRegisterStack.Pop();
        var param1 = _virtualRegisterStack.Pop();
        LLVMValueRef result;

        if (AreParamsCompatibleAndInt(param1, param2))
        {
            result = LLVM.BuildMul(_builder, param1, param2, GetVirtualRegisterName());
        }
        else if (AreParamsCompatibleAndDecimal(param1, param2))
        {
            result = LLVM.BuildFMul(_builder, param1, param2, GetVirtualRegisterName());
        }
        else
        {
            throw new ArgumentException($"Type {param1} and {param2} are not supported or have not the same type");
        }

        _virtualRegisterStack.Push(result);
    }

    private void CompileDiv()
    {
        var param2 = _virtualRegisterStack.Pop();
        var param1 = _virtualRegisterStack.Pop();
        LLVMValueRef result;

        if (AreParamsCompatibleAndInt(param1, param2))
        {
            result = LLVM.BuildSDiv(_builder, param1, param2, GetVirtualRegisterName());
        }
        else if (AreParamsCompatibleAndDecimal(param1, param2))
        {
            result = LLVM.BuildFDiv(_builder, param1, param2, GetVirtualRegisterName());
        }
        else
        {
            throw new ArgumentException($"Type {param1} and {param2} are not supported or have not the same type");
        }

        _virtualRegisterStack.Push(result);
    }

    private void CompileRem()
    {
        var param2 = _virtualRegisterStack.Pop();
        var param1 = _virtualRegisterStack.Pop();
        LLVMValueRef result;

        if (AreParamsCompatibleAndInt(param1, param2))
        {
            result = LLVM.BuildSRem(_builder, param1, param2, GetVirtualRegisterName());
        }
        else if (AreParamsCompatibleAndDecimal(param1, param2))
        {
            result = LLVM.BuildFRem(_builder, param1, param2, GetVirtualRegisterName());
        }
        else
        {
            throw new ArgumentException($"Type {param1} and {param2} are not supported or have not the same type");
        }

        _virtualRegisterStack.Push(result);
    }

    private void CompileDup()
    {
        var value = _virtualRegisterStack.Pop();
        _virtualRegisterStack.Push(value);
        _virtualRegisterStack.Push(value);
    }

    #endregion

    #region Branches

    private void CompileBr(int operand)
    {
        LLVMBasicBlockRef block;
        if (operand != 0) //Create a block after this one even if the actual branch is further down
        {
            GetBlock(_stream.Position);
        }

        var target = GetBlock(_stream.Position + operand);
        //BuildAdditionalPhis(); TODO REMOVE
        LLVM.BuildBr(_builder, target.BlockRef);

        target.Predecessors.Add(_currentBlock);
        _currentBlock.Successors.Add(target);
    }

    private void CompileBrTrue(int operand)
    {
        var predicate = _virtualRegisterStack.Pop();
        var thenBlock = GetBlock(_stream.Position + operand);
        var elseBlock = GetBlock(_stream.Position);
        //BuildAdditionalPhis(); TODO REMOVE
        LLVM.BuildCondBr(_builder, predicate, thenBlock.BlockRef, elseBlock.BlockRef);

        thenBlock.Predecessors.Add(_currentBlock);
        elseBlock.Predecessors.Add(_currentBlock);
        _currentBlock.Successors.Add(thenBlock);
        _currentBlock.Successors.Add(elseBlock);
    }

    private void CompileBrFalse(int operand)
    {
        var predicate = _virtualRegisterStack.Pop();
        var predicateNegated = LLVM.BuildNot(_builder, predicate, GetVirtualRegisterName());
        var elseBlock = GetBlock(_stream.Position);
        var thenBlock = GetBlock(_stream.Position + operand);
        //BuildAdditionalPhis(); TODO REMOVE
        LLVM.BuildCondBr(_builder, predicateNegated, thenBlock.BlockRef,
            elseBlock.BlockRef);

        elseBlock.Predecessors.Add(_currentBlock);
        thenBlock.Predecessors.Add(_currentBlock);
        _currentBlock.Successors.Add(elseBlock);
        _currentBlock.Successors.Add(thenBlock);
    }

    #endregion

    #region Comparison

    private void CompileClt()
    {
        var value2 = _virtualRegisterStack.Pop();
        var value1 = _virtualRegisterStack.Pop();
        LLVMValueRef result =
            BuildComparison(LLVMIntPredicate.LLVMIntSLT, LLVMRealPredicate.LLVMRealOLT, value1, value2);
        _virtualRegisterStack.Push(result);
    }

    private void CompileCgt()
    {

        var value2 = _virtualRegisterStack.Pop();
        var value1 = _virtualRegisterStack.Pop();
        LLVMValueRef result =
            BuildComparison(LLVMIntPredicate.LLVMIntSGT, LLVMRealPredicate.LLVMRealOGT, value1, value2);
        _virtualRegisterStack.Push(result);
    }

    private void CompileCeq()
    {
        var value2 = _virtualRegisterStack.Pop();
        var value1 = _virtualRegisterStack.Pop();
        LLVMValueRef result =
            BuildComparison(LLVMIntPredicate.LLVMIntEQ, LLVMRealPredicate.LLVMRealOEQ, value1, value2);
        _virtualRegisterStack.Push(result);
    }

    private LLVMValueRef BuildComparison(LLVMIntPredicate intPredicate, LLVMRealPredicate realPredicate,
        LLVMValueRef value1, LLVMValueRef value2)
    {

        LLVMValueRef result;
        if (AreParamsCompatibleAndInt(value1, value2))
        {
            result = LLVM.BuildICmp(_builder, LLVMIntPredicate.LLVMIntSLT, value1, value2, GetVirtualRegisterName());
        }
        else if (AreParamsCompatibleAndDecimal(value1, value2))

        {
            result = LLVM.BuildFCmp(_builder, LLVMRealPredicate.LLVMRealOLT, value1, value2, GetVirtualRegisterName());
        }
        else
        {
            throw new ArgumentException($"Type {value1} and {value2} are not supported or have not the same type");
        }

        return result;
    }

    #endregion

    #region Loads

    private void CompileLdarg(int operand)
    {
        var index = operand; //Warning: needs -1 if not static but not supported!
        var param = LLVM.GetParam(_functionsDto.Function, (uint)index);
        if (!_inputKernel.ParameterInfos[index].ParameterType.IsArray)
        {
            param = LLVM.BuildLoad(_builder, param, GetVirtualRegisterName());
        }

        _virtualRegisterStack.Push(param);
    }

    private void CompileLdcInt(int operand)
    {
        var reference = LLVM.ConstInt(LLVMTypeRef.Int32Type(), (ulong)operand, true);
        _virtualRegisterStack.Push(reference);
    }

    private void CompileLdcLong(long operand)
    {
        var reference = LLVM.ConstInt(LLVMTypeRef.Int64Type(), (ulong)operand, true);
        _virtualRegisterStack.Push(reference);
    }

    private void CompileLdcFloat(float operand)
    {
        var reference = LLVM.ConstReal(LLVMTypeRef.FloatType(), operand);
        _virtualRegisterStack.Push(reference);
    }

    private void CompileLdcDouble(double operand)
    {
        var reference = LLVM.ConstReal(LLVMTypeRef.DoubleType(), operand);
        _virtualRegisterStack.Push(reference);
    }

    private void CompileLdloc(int operand)
    {
        var param = _currentBlock.LocalVariables[operand];
        _virtualRegisterStack.Push(param);
    }

    private void CompileLdelem()
    {
        var index = _virtualRegisterStack.Pop();
        var array = _virtualRegisterStack.Pop();
        int x = 5;
        var elementPtr = LLVM.BuildGEP(_builder, array, new[] { index }, GetVirtualRegisterName());
        var value = LLVM.BuildLoad(_builder, elementPtr, GetVirtualRegisterName());
        _virtualRegisterStack.Push(value);
    }

    private void CompileLdelema()
    {
        var index = _virtualRegisterStack.Pop();
        var array = _virtualRegisterStack.Pop();
        var elementPtr = LLVM.BuildGEP(_builder, array, new[] { index }, GetVirtualRegisterName());
        _virtualRegisterStack.Push(elementPtr);
    }

    private void CompileLdfld(int operand)
    {
        if (operand < 0)
        {
            throw new BadImageFormatException($"Invalid metadata token");
        }

        if (_nameOfMethodToCall == null)
        {
            throw new ArgumentException("Name of method to call is unknown");
        }

        var field = _inputKernel.MemberInfoModule.ResolveField(operand);
        var fullQualifiedFieldName = $"{_nameOfMethodToCall}.{field?.Name}";

        var externalFunctionToCall =
            _functionsDto.ExternalFunctions.First(func => func.Item1 == fullQualifiedFieldName);
        var call = LLVM.BuildCall(_builder, externalFunctionToCall.Item2, Array.Empty<LLVMValueRef>(),
            GetVirtualRegisterName());
        _virtualRegisterStack.Push(call);
        _nameOfMethodToCall = null;
    }

    private void CompileLdind()
    {
        var elementPtr = _virtualRegisterStack.Pop();
        var value = LLVM.BuildLoad(_builder, elementPtr, GetVirtualRegisterName());
        _virtualRegisterStack.Push(value);
    }

    #endregion

    #region Stores

    private void CompileStarg(int index)
    {
        var value = _virtualRegisterStack.Pop();
        var arg = LLVM.GetParam(_functionsDto.Function, (uint)index);
        LLVM.BuildStore(_builder, value, arg);
    }

    private void CompileStdind()
    {
        var value = _virtualRegisterStack.Pop();
        var elementPtr = _virtualRegisterStack.Pop();
        var storeValue = LLVM.BuildStore(_builder, value, elementPtr);
        _virtualRegisterStack.Push(storeValue);
    }

    private void CompileStelem()
    {
        var value = _virtualRegisterStack.Pop();
        var index = _virtualRegisterStack.Pop();
        var array = _virtualRegisterStack.Pop();

        var elementPtr = LLVM.BuildGEP(_builder, array, new[] { index }, GetVirtualRegisterName());
        var newArray = LLVM.BuildStore(_builder, value, elementPtr);
        _virtualRegisterStack.Push(newArray);
    }

    private void CompileStloc(int operand)
    {
        var param = _virtualRegisterStack.Pop();
        if (_currentBlock.LocalVariables.ContainsKey(operand))
        {
            _currentBlock.LocalVariables[operand] = param;
        }
        else
        {
            _currentBlock.LocalVariables.Add(operand, param);
        }
    }

    #endregion

    #region Calls

    private void CompileCall(int operand)
    {
        if (operand < 0)
        {
            throw new BadImageFormatException($"Invalid metadata token");
        }

        var method = _inputKernel.MemberInfoModule.ResolveMethod(operand);
        _nameOfMethodToCall = $"{method?.DeclaringType?.FullName}.{method?.Name}";
        var isExternalFunction =
            _functionsDto.ExternalFunctions.Any(func => func.Item1.StartsWith(_nameOfMethodToCall));

        if (!isExternalFunction)
        {
            throw new NotSupportedException("Only calls to defined external functions are supported");
        }
    }

    #endregion

    #endregion

    #region Private Helpers

    private void BuildPhis()
    {
        LLVM.PositionBuilder(_builder, _currentBlock.BlockRef, LLVM.GetFirstInstruction(_currentBlock.BlockRef));
        for (int i = 0; i < _inputKernel.LocalVariables.Count; i++)
        {
            if (!_currentBlock.LocalVariables.ContainsKey(i))
            {
                var phi = LLVM.BuildPhi(_builder, _inputKernel.LocalVariables[i].LocalType.ToLLVMType(),
                    GetVirtualRegisterName());
                _currentBlock.LocalVariables.Add(i, phi);
                _currentBlock.PhiInstructions.Add(i, phi);
            }
        }

        LLVM.PositionBuilderAtEnd(_builder, _currentBlock.BlockRef);
    }

    private void PatchBlockGraph(BlockNode startNode)
    {
        if (startNode.Visited) return;
        startNode.Visited = true;

        foreach (var phi in startNode.PhiInstructions) //Patch Phi Instructions
        {
            foreach (var pred in startNode.Predecessors)
            {
                if (pred.LocalVariables.ContainsKey(phi.Key))
                {
                    phi.Value.AddIncoming(new[] { pred.LocalVariables[phi.Key] }, new[] { pred.BlockRef }, 1);
                }
                else //add arbitrary value
                {
                    phi.Value.AddIncoming(new[] { LLVM.ConstInt(phi.Value.TypeOf(), 0, false) },
                        new[] { pred.BlockRef }, 1);
                }
            }

            if (phi.Value.CountIncoming() == 0)
            {
                LLVM.InstructionRemoveFromParent(phi.Value);
            }
        }

        foreach (var successor in startNode.Successors)
        {
            PatchBlockGraph(successor);
        }
    }

    private BlockNode GetBlock(long index)
    {
        if (_blockList.ContainsKey(index))
        {
            return _blockList[index];
        }

        var block = LLVM.AppendBasicBlock(_functionsDto.Function, GetBlockName());
        _blockList.Add(index, new BlockNode() { BlockRef = block });
        return _blockList[index];
    }

    private ILOpCode ReadOpCode()
    {
        var opCodeByte = _reader.ReadByte();
        return (ILOpCode)(opCodeByte == 0xFE ? 0xFE00 + _reader.ReadByte() : opCodeByte);
    }

    private string GetVirtualRegisterName()
    {
        return $"reg{_virtualRegisterCounter++}";
    }

    private string GetBlockName()
    {
        return $"block{_blockCounter++}";
    }

    private bool AreParamsCompatibleAndInt(LLVMValueRef param1, LLVMValueRef param2)
    {
        return param1.TypeOf().TypeKind == LLVMTypeKind.LLVMIntegerTypeKind &&
               param2.TypeOf().TypeKind == LLVMTypeKind.LLVMIntegerTypeKind;
    }

    private bool AreParamsCompatibleAndDecimal(LLVMValueRef param1, LLVMValueRef param2)
    {
        return param1.TypeOf().TypeKind == LLVMTypeKind.LLVMFloatTypeKind &&
               param2.TypeOf().TypeKind == LLVMTypeKind.LLVMFloatTypeKind ||
               param1.TypeOf().TypeKind == LLVMTypeKind.LLVMDoubleTypeKind &&
               param2.TypeOf().TypeKind == LLVMTypeKind.LLVMDoubleTypeKind;
    }

    #endregion
}