﻿using System.Reflection;
using System.Reflection.Metadata;
using CuSharp.CudaCompiler.ControlFlowGraph;
using CuSharp.CudaCompiler.Kernels;
using CuSharp.CudaCompiler.LLVMConfiguration;
using LLVMSharp;

namespace CuSharp.CudaCompiler.Compiler;

public class MethodBodyCompiler
{
    private readonly BinaryReader _reader;
    private readonly MSILKernel _inputKernel;
    private readonly LLVMBuilderRef _builder;
    private readonly FunctionsDto _functionsDto;
    private readonly FunctionGenerator? _functionGenerator;
    private readonly MemoryStream _stream;

    private long _virtualRegisterCounter;
    private long _globalVariableCounter;
    private string? _nameOfMethodToCall;
    private ControlFlowGraphBuilder _cfg; 

    public LLVMModuleRef Module { get; set; }
    public ArrayMemoryLocation ArrayMemoryLocation { get; set; }

    #region PublicInterface
    public MethodBodyCompiler(MSILKernel inputKernel, LLVMBuilderRef builder, FunctionsDto functionsDto, FunctionGenerator? functionGenerator = null)
    {
        _inputKernel = inputKernel;
        _builder = builder;
        _functionsDto = functionsDto;
        _functionGenerator = functionGenerator;
        _stream = new MemoryStream(inputKernel.KernelBuffer);
        _reader = new BinaryReader(_stream);
    }

    public IEnumerable<(ILOpCode OpCode, object? Operand)> CompileMethodBody()
    {
        _cfg = new ControlFlowGraphBuilder(_functionsDto.Function, _inputKernel, _builder, GetVirtualRegisterName);
        
        IList<(ILOpCode, object?)> opCodes = new List<(ILOpCode, object?)>();
        
        while (_reader.BaseStream.Position < _reader.BaseStream.Length)
        {
            if (_reader.BaseStream.Position == _reader.BaseStream.Length)
            {
                throw new ArgumentOutOfRangeException("Unexpected end of method body.");
            }

            _cfg.UpdateBlocks(_stream.Position);

            opCodes.Add(CompileNextOpCode());
        }

        _cfg.PatchBlockGraph();

        return opCodes;
    }

    #endregion

    private (ILOpCode, object?) CompileNextOpCode()
    {
        var opCode = ReadOpCode();
        object? operand = null;
        switch (opCode)
        {
            case ILOpCode.Nop:
                break;

            case ILOpCode.Constrained:
                _reader.ReadInt32();
                break;
            case ILOpCode.Add:
                CompileAdd();
                break;
            case ILOpCode.And:
                CompileAnd();
                break;
            case ILOpCode.Beq:
                operand = _reader.ReadInt32();
                CompileBeq((int)operand);
                break;
            case ILOpCode.Beq_s:
                operand = _reader.ReadSByte();
                CompileBeq((sbyte)operand);
                break;
            case ILOpCode.Bge: 
                operand = _reader.ReadInt32();
                CompileBge((int) operand);
                break;
            case ILOpCode.Bge_s:
                operand = _reader.ReadSByte();
                CompileBge((sbyte) operand);
                break;
            case ILOpCode.Bge_un:
                operand = _reader.ReadInt32();
                CompileBgeUn((int) operand);
                break;
            case ILOpCode.Bge_un_s:
                operand = _reader.ReadSByte();
                CompileBgeUn((sbyte) operand);
                break;
            case ILOpCode.Bgt: 
                operand = _reader.ReadInt32();
                CompileBgt((int) operand);
                break;
            case ILOpCode.Bgt_s: 
                operand = _reader.ReadSByte();
                CompileBgt((sbyte) operand);
                break;
            case ILOpCode.Bgt_un:
                operand = _reader.ReadInt32();
                CompileBgtUn((int) operand);
                break;
            case ILOpCode.Bgt_un_s:
                operand = _reader.ReadSByte();
                CompileBgtUn((sbyte)operand);
                break;
            case ILOpCode.Ble:
                operand = _reader.ReadInt32();
                CompileBle((int) operand);
                break;
            case ILOpCode.Ble_s: 
                operand = _reader.ReadSByte();
                CompileBle((sbyte) operand);
                break;
            case ILOpCode.Ble_un:
                operand = _reader.ReadInt32();
                CompileBleUn((int) operand);
                break;
            case ILOpCode.Ble_un_s:
                operand = _reader.ReadSByte();
                CompileBleUn((sbyte) operand);
                break;
            case ILOpCode.Blt:
                operand = _reader.ReadInt32();
                CompileBlt((int) operand);
                break;
            case ILOpCode.Blt_s:
                operand = _reader.ReadSByte();
                CompileBlt((sbyte) operand);
                break;
            case ILOpCode.Blt_un:
                operand = _reader.ReadInt32();
                CompileBleUn((int) operand);
                break;
            case ILOpCode.Blt_un_s:
                operand = _reader.ReadSByte();
                CompileBltUn((sbyte) operand);
                break;
            case ILOpCode.Bne_un:
                operand = _reader.ReadInt32();
                CompileBneUn((int)operand);
                break;
            case ILOpCode.Bne_un_s:
                operand = _reader.ReadSByte();
                CompileBneUn((sbyte)operand);
                break;
            case ILOpCode.Br:
                operand = _reader.ReadInt32();
                CompileBr((int)operand);
                break;
            case ILOpCode.Br_s:
                operand = _reader.ReadSByte();
                CompileBr((sbyte)operand);
                break;
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
                CompileCall((int) operand);
                break;
            case ILOpCode.Callvirt:
                operand = _reader.ReadInt32();
                CompileCallvirt((int) operand);
                break;
            case ILOpCode.Ceq:
                CompileCeq();
                break;
            case ILOpCode.Cgt:
            case ILOpCode.Cgt_un:
                CompileCgt();
                break;
            case ILOpCode.Clt:
            case ILOpCode.Clt_un:
                CompileClt();
                break;
            case ILOpCode.Conv_i1:
                CompileConvI1();
                break;
            case ILOpCode.Conv_i2: 
                CompileConvI2();
                break;
            case ILOpCode.Conv_i4:
                CompileConvI4();
                break;
            case ILOpCode.Conv_i8: 
                CompileConvI8(); 
                break;
            case ILOpCode.Conv_r4: 
                CompileConvR4();
                break;
            case ILOpCode.Conv_r8:
                CompileConvR8();
                break;
            case ILOpCode.Conv_u1:
                CompileConvU1();
                break;
            case ILOpCode.Conv_u2:
                CompileConvU2();
                break;
            case ILOpCode.Conv_u4:
                CompileConvU4();
                break;
            case ILOpCode.Conv_u8:
                CompileConvU8(); 
                break;
            case ILOpCode.Conv_i:
                CompileConvI();
                break;
            case ILOpCode.Conv_u:
                CompileConvU();
                break;
            case ILOpCode.Conv_r_un: 
                CompileConvRUn();
                break;
            case ILOpCode.Conv_ovf_i: 
                CompileConvI();
                break;
            case ILOpCode.Div:
                CompileDiv();
                break;
            case ILOpCode.Div_un:
                CompileDivUn();
                break;
            case ILOpCode.Dup:
                CompileDup();
                break;
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
            case ILOpCode.Mul:
                CompileMul();
                break;
            case ILOpCode.Neg:
                CompileNeg();
                break;
            case ILOpCode.Newobj:
                operand = _reader.ReadInt32();
                CompileNewobj((int) operand);
                break;
            case ILOpCode.Not:
                CompileNot();
                break;
            case ILOpCode.Or:
                CompileOr();
                break;
            case ILOpCode.Pop:
                _cfg.CurrentBlock.VirtualRegisterStack.Pop();
                break;
            case ILOpCode.Rem:
                CompileRem();
                break;
            case ILOpCode.Rem_un:
                CompileRemUn();
                break;
            case ILOpCode.Ret:
                CompileReturn();
                break;
            case ILOpCode.Shl:
                CompileShl();
                break;
            case ILOpCode.Shr:
                CompileShr();
                break;
            case ILOpCode.Shr_un:
                CompileShrUn();
                break;
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
            case ILOpCode.Xor:
                CompileXor();
                break;
            case ILOpCode.Ldelem:
                _reader.ReadInt32();
                CompileLdelem();
                break;
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
            case ILOpCode.Ldelema:
                operand = _reader.ReadInt32(); // No further usage of operand required
                CompileLdelema();
                break;
            case ILOpCode.Ldfld:
                operand = _reader.ReadInt32();
                CompileLdfld((int)operand);
                break;
            case ILOpCode.Newarr:
                var metaDataToken = _reader.ReadInt32(); 
                CompileNewarr(metaDataToken);
                break;
            case ILOpCode.Stelem: 
                _reader.ReadInt32();
                CompileStelem();
                break;
            case ILOpCode.Stelem_i1:
            case ILOpCode.Stelem_i2:
            case ILOpCode.Stelem_i4:
            case ILOpCode.Stelem_i8:
            case ILOpCode.Stelem_r4:
            case ILOpCode.Stelem_r8:
                CompileStelem();
                break;
            default:
                throw new NotSupportedException($"OpCode '{opCode}' is not supported");
        }

        return (opCode, operand);
    }

    #region Compile Instructions

    #region Operations

    private void CompileAdd()
    {
        var param2 = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        var param1 = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
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

        _cfg.CurrentBlock.VirtualRegisterStack.Push(result);
    }

    private void CompileSub()
    {
        var param2 = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        var param1 = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
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

        _cfg.CurrentBlock.VirtualRegisterStack.Push(result);
    }

    private void CompileMul()
    {
        var param2 = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        var param1 = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
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

        _cfg.CurrentBlock.VirtualRegisterStack.Push(result);
    }

    private void CompileDiv()
    {
        var param2 = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        var param1 = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
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

        _cfg.CurrentBlock.VirtualRegisterStack.Push(result);
    }

    private void CompileDivUn()
    {
        var param2 = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        var param1 = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        LLVMValueRef result;

        if (AreParamsCompatibleAndInt(param1, param2))
        {
            result = LLVM.BuildUDiv(_builder, param1, param2, GetVirtualRegisterName());
        }
        else
        {
            throw new ArgumentException($"Type {param1} and {param2} are not supported or have not the same type");
        }

        _cfg.CurrentBlock.VirtualRegisterStack.Push(result);
    }

    private void CompileRem()
    {
        var param2 = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        var param1 = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
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

        _cfg.CurrentBlock.VirtualRegisterStack.Push(result);
    }

    private void CompileRemUn()
    {
        var param2 = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        var param1 = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        LLVMValueRef result;

        if (AreParamsCompatibleAndInt(param1, param2))
        {
            result = LLVM.BuildURem(_builder, param1, param2, GetVirtualRegisterName());
        }
        else
        {
            throw new ArgumentException($"Type {param1} and {param2} are not supported or have not the same type");
        }

        _cfg.CurrentBlock.VirtualRegisterStack.Push(result);
    }

    private void CompileDup()
    {
        var value = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        _cfg.CurrentBlock.VirtualRegisterStack.Push(value);
        _cfg.CurrentBlock.VirtualRegisterStack.Push(value);
    }

    private void CompileAnd()
    {
        var param2 = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        var param1 = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        LLVMValueRef result;
        
        if (AreParamsCompatibleAndInt(param1, param2))
        {
            result = LLVM.BuildAnd(_builder, param1, param2, GetVirtualRegisterName());
        }
        else
        {
            throw new ArgumentException($"Type {param1} and {param2} are not supported or have not the same type");
        }

        _cfg.CurrentBlock.VirtualRegisterStack.Push(result);
    }

    private void CompileNeg()
    {
        var param = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        LLVMValueRef result;

        if (param.TypeOf().TypeKind == LLVMTypeKind.LLVMIntegerTypeKind)
        {
            result = LLVM.BuildNeg(_builder, param, GetVirtualRegisterName());
        }
        else if (param.TypeOf().TypeKind == LLVMTypeKind.LLVMFloatTypeKind ||
                 param.TypeOf().TypeKind == LLVMTypeKind.LLVMDoubleTypeKind)
        {
            result = LLVM.BuildFNeg(_builder, param, GetVirtualRegisterName());
        }
        else
        {
            throw new ArgumentException($"Type {param} cannot be negotiated");
        }

        _cfg.CurrentBlock.VirtualRegisterStack.Push(result);
    }

    private void CompileNot()
    {
        var param = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        LLVMValueRef result;

        if (param.TypeOf().TypeKind == LLVMTypeKind.LLVMIntegerTypeKind)
        {
            result = LLVM.BuildNot(_builder, param, GetVirtualRegisterName());
        }
        else
        {
            throw new ArgumentException($"Type {param} is not a boolean");
        }

        _cfg.CurrentBlock.VirtualRegisterStack.Push(result);
    }

    private void CompileOr()
    {
        var param2 = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        var param1 = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        LLVMValueRef result;

        if (AreParamsCompatibleAndInt(param1, param2))
        {
            result = LLVM.BuildOr(_builder, param1, param2, GetVirtualRegisterName());
        }
        else
        {
            throw new ArgumentException($"Type {param1} and {param2} are not supported or have not the same type");
        }

        _cfg.CurrentBlock.VirtualRegisterStack.Push(result);
    }

    private void CompileShl()
    {
        var shiftAmount = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        var param = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        LLVMValueRef result;

        if (param.TypeOf().TypeKind == LLVMTypeKind.LLVMIntegerTypeKind)
        {
            result = LLVM.BuildShl(_builder, param, shiftAmount, GetVirtualRegisterName());
        }
        else
        {
            throw new ArgumentException($"Type {param} cannot be shifted");
        }

        _cfg.CurrentBlock.VirtualRegisterStack.Push(result);
    }

    private void CompileShr()
    {
        var shiftAmount = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        var param = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        LLVMValueRef result;

        if (param.TypeOf().TypeKind == LLVMTypeKind.LLVMIntegerTypeKind)
        {
            result = LLVM.BuildAShr(_builder, param, shiftAmount, GetVirtualRegisterName());
        }
        else
        {
            throw new ArgumentException($"Type {param} cannot be shifted");
        }

        _cfg.CurrentBlock.VirtualRegisterStack.Push(result);
    }

    private void CompileShrUn()
    {
        var shiftAmount = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        var param = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        LLVMValueRef result;

        if (param.TypeOf().TypeKind == LLVMTypeKind.LLVMIntegerTypeKind)
        {
            result = LLVM.BuildLShr(_builder, param, shiftAmount, GetVirtualRegisterName());
        }
        else
        {
            throw new ArgumentException($"Type {param} cannot be shifted");
        }

        _cfg.CurrentBlock.VirtualRegisterStack.Push(result);
    }

    private void CompileXor()
    {
        var param2 = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        var param1 = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        LLVMValueRef result;

        if (AreParamsCompatibleAndInt(param1, param2))
        {
            result = LLVM.BuildXor(_builder, param1, param2, GetVirtualRegisterName());
        }
        else
        {
            throw new ArgumentException($"Type {param1} and {param2} are not supported or have not the same type");
        }

        _cfg.CurrentBlock.VirtualRegisterStack.Push(result);
    }

    #endregion

    #region Branches

    private void CompileBr(int operand)
    {
        if (operand != 0) //Create a block after this one even if the actual branch is further down
        {
            _cfg.GetBlock(_stream.Position, _functionsDto.Function);
        }

        var target = _cfg.GetBlock(_stream.Position + operand, _functionsDto.Function);
        LLVM.BuildBr(_builder, target.BlockRef);

        _cfg.CurrentBlock.AddSuccessors(target);
    }

    private void CompileBrTrue(int operand)
    {
        var predicate = _cfg.CurrentBlock.VirtualRegisterStack.Pop();

        if (!predicate.TypeOf().Equals(LLVMTypeRef.Int1Type())) //converting to bool (i1)
        {
            predicate = LLVM.BuildICmp(_builder, LLVMIntPredicate.LLVMIntNE, predicate,
                LLVM.ConstInt(predicate.TypeOf(), 0, false), GetVirtualRegisterName());
        }
        
        BuildConditionalBranch(_stream.Position + operand, _stream.Position, predicate);
    }

    private void CompileBrFalse(int operand)
    {
        var predicate = _cfg.CurrentBlock.VirtualRegisterStack.Pop();

        if (!predicate.TypeOf().Equals(LLVMTypeRef.Int1Type())) //converting to bool (i1)
        {
            predicate = LLVM.BuildICmp(_builder, LLVMIntPredicate.LLVMIntNE, predicate,
                LLVM.ConstInt(predicate.TypeOf(), 0, false), GetVirtualRegisterName());
        }
        
        var predicateNegated = LLVM.BuildNot(_builder, predicate, GetVirtualRegisterName());
        
        BuildConditionalBranch(_stream.Position + operand, _stream.Position, predicateNegated);
    }

    private void CompileBneUn(int operand)
    {
        var predicate = BuildPredicateFromStack(LLVMIntPredicate.LLVMIntNE, LLVMRealPredicate.LLVMRealUNE);
        BuildConditionalBranch(_stream.Position + operand, _stream.Position, predicate);
    }

    private void CompileBeq(int operand)
    {
        var predicate = BuildPredicateFromStack(LLVMIntPredicate.LLVMIntEQ, LLVMRealPredicate.LLVMRealOEQ);
        BuildConditionalBranch(_stream.Position + operand, _stream.Position, predicate);
    }

    private void CompileBge(int operand)
    {
        var predicate = BuildPredicateFromStack(LLVMIntPredicate.LLVMIntSGE, LLVMRealPredicate.LLVMRealOGE);
        BuildConditionalBranch(_stream.Position + operand, _stream.Position, predicate);
    }

    private void CompileBgeUn(int operand)
    {
        var predicate = BuildPredicateFromStack(LLVMIntPredicate.LLVMIntUGE, LLVMRealPredicate.LLVMRealUGE);
        BuildConditionalBranch(_stream.Position + operand, _stream.Position, predicate);
    }

    private void CompileBgt(int operand)
    {
        var predicate = BuildPredicateFromStack(LLVMIntPredicate.LLVMIntSGT, LLVMRealPredicate.LLVMRealOGT);
        BuildConditionalBranch(_stream.Position + operand, _stream.Position, predicate);
    }

    private void CompileBgtUn(int operand)
    {
        var predicate = BuildPredicateFromStack(LLVMIntPredicate.LLVMIntUGT, LLVMRealPredicate.LLVMRealUGT);
        BuildConditionalBranch(_stream.Position + operand, _stream.Position, predicate);
    }

    private void CompileBle(int operand)
    {
        var predicate = BuildPredicateFromStack(LLVMIntPredicate.LLVMIntSLE, LLVMRealPredicate.LLVMRealOLE);
        BuildConditionalBranch(_stream.Position + operand, _stream.Position, predicate);
    }

    private void CompileBleUn(int operand)
    {
        var predicate = BuildPredicateFromStack(LLVMIntPredicate.LLVMIntULE, LLVMRealPredicate.LLVMRealULE);
        BuildConditionalBranch(_stream.Position + operand, _stream.Position, predicate);
    }

    private void CompileBlt(int operand)
    {
        var predicate = BuildPredicateFromStack(LLVMIntPredicate.LLVMIntSLT, LLVMRealPredicate.LLVMRealOLT);
        BuildConditionalBranch(_stream.Position + operand, _stream.Position, predicate);
    }

    private void CompileBltUn(int operand)
    {
        var predicate = BuildPredicateFromStack(LLVMIntPredicate.LLVMIntULT, LLVMRealPredicate.LLVMRealULT);
        BuildConditionalBranch(_stream.Position + operand, _stream.Position, predicate);
    }


    #endregion

    #region Comparison

    private void CompileClt()
    {
        var value2 = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        var value1 = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        value2 = CastValue2IfIncompatibleInts(value1, value2);
        LLVMValueRef result =
            BuildComparison(LLVMIntPredicate.LLVMIntSLT, LLVMRealPredicate.LLVMRealOLT, value1, value2);
        _cfg.CurrentBlock.VirtualRegisterStack.Push(result);
    }

    private void CompileCgt()
    {
        var value2 = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        var value1 = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        value2 = CastValue2IfIncompatibleInts(value1, value2);
        LLVMValueRef result =
            BuildComparison(LLVMIntPredicate.LLVMIntSGT, LLVMRealPredicate.LLVMRealOGT, value1, value2);
        _cfg.CurrentBlock.VirtualRegisterStack.Push(result);
    }

    private void CompileCeq()
    {
        var value2 = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        var value1 = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        value2 = CastValue2IfIncompatibleInts(value1, value2);
        LLVMValueRef result =
            BuildComparison(LLVMIntPredicate.LLVMIntEQ, LLVMRealPredicate.LLVMRealOEQ, value1, value2);
        _cfg.CurrentBlock.VirtualRegisterStack.Push(result);
    }

    private LLVMValueRef CastValue2IfIncompatibleInts(LLVMValueRef value1, LLVMValueRef value2)
    {
        if(value2.TypeOf().ToNativeType() != value1.TypeOf().ToNativeType()) 
                    value2 = LLVM.BuildIntCast(_builder, value2, value1.TypeOf(), GetVirtualRegisterName());
        return value2;
    }
    private LLVMValueRef BuildComparison(LLVMIntPredicate intPredicate, LLVMRealPredicate realPredicate,
        LLVMValueRef value1, LLVMValueRef value2)
    {

        LLVMValueRef result;
        if (AreParamsCompatibleAndInt(value1, value2))
        {
            value2 = CastValue2IfIncompatibleInts(value1, value2);
            result = LLVM.BuildICmp(_builder, intPredicate, value1, value2, GetVirtualRegisterName());
        }
        else if (AreParamsCompatibleAndDecimal(value1, value2))

        {
            result = LLVM.BuildFCmp(_builder, realPredicate, value1, value2, GetVirtualRegisterName());
        }
        else
        {
            throw new ArgumentException($"Type {value1} and {value2} are not supported or have not the same type");
        }

        return result;
    }

    private void CompileNewarr(int metaDataToken)
    {
        Type inputType;
        try
        {
            inputType = _inputKernel.MethodInfo.Module.ResolveType(metaDataToken);
            
        }
        catch (Exception e)
        {
            var genericArgs = _inputKernel.MethodInfo.GetGenericArguments();
            inputType = _inputKernel.MethodInfo.Module.ResolveType(metaDataToken, genericArgs, genericArgs);
        }
        var elementCount = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        var len = LLVM.ConstIntGetZExtValue(elementCount);

        var type = LLVM.ArrayType(inputType.ToLLVMType(), (uint) len);
        var arr = LLVM.AddGlobalInAddressSpace(Module, type, GetGlobalVariableName(), (uint)ArrayMemoryLocation);
        arr.SetLinkage(LLVMLinkage.LLVMInternalLinkage);
        arr.SetInitializer(LLVM.GetUndef(type));
        _cfg.CurrentBlock.VirtualRegisterStack.Push(arr);
    }
    
    private void CompileNewobj(int operand) 
    {
        if (operand < 0)
        {
            throw new BadImageFormatException($"Invalid metadata token");
        }

        MethodBase ctor;
        try
        {
            ctor = _inputKernel.MemberInfoModule.ResolveMethod(operand)!;
        }
        catch (Exception e)
        {
            var genericArgs = _inputKernel.MethodInfo.GetGenericArguments();
            ctor = _inputKernel.MemberInfoModule.ResolveMethod(operand, genericArgs, genericArgs)!;
        }

        if (!ctor.DeclaringType!.HasElementType || !ctor.DeclaringType.GetElementType()!.IsPrimitive)
        {
            throw new NotSupportedException("Unsupported use of the \"new\" keyword");
        }

        var sizeY = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        var sizeX = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        var lenY = LLVM.ConstIntGetZExtValue(sizeY);
        var lenX = LLVM.ConstIntGetZExtValue(sizeX);
        var type = LLVM.ArrayType(ctor.DeclaringType.GetElementType().ToLLVMType(), (uint) (lenY * lenX));

        var arr = LLVM.AddGlobalInAddressSpace(Module, type, GetGlobalVariableName(), (uint) ArrayMemoryLocation);
        arr.SetLinkage(LLVMLinkage.LLVMInternalLinkage);
        arr.SetInitializer(LLVM.GetUndef(type));
        
        var multiType =
            LLVM.ArrayType(LLVM.PointerType(ctor.DeclaringType.GetElementType().ToLLVMType(), 0),  (uint) lenX);
        var multiArr = LLVM.AddGlobalInAddressSpace(Module, multiType, GetGlobalVariableName(), (uint) ArrayMemoryLocation);
        multiArr.SetLinkage(LLVMLinkage.LLVMInternalLinkage);
        multiArr.SetInitializer(LLVM.GetUndef(multiType));

        for (int i = 0; i < (uint) lenX; i++)
        {
            var multiArrIndex = BuildGEP(multiArr, LLVM.ConstInt(LLVMTypeRef.Int32Type(), (ulong) i, false));
            var arrIndex = BuildGEP(arr, LLVM.ConstInt(LLVMTypeRef.Int32Type(), (ulong) i * lenY, false));
            arrIndex = LLVM.BuildPointerCast(_builder, arrIndex,
                LLVM.PointerType(ctor.DeclaringType.GetElementType().ToLLVMType(), 0), GetVirtualRegisterName());
            LLVM.BuildStore(_builder, arrIndex, multiArrIndex);
        }
        
        _cfg.CurrentBlock.VirtualRegisterStack.Push(multiArr);
    }
    #endregion

    #region Loads

    private void CompileLdarg(int operand)
    {
        var param = _cfg.CurrentBlock.Parameters[operand];//= LLVM.GetParam(_functionsDto.Function, (uint)index);
        _cfg.CurrentBlock.VirtualRegisterStack.Push(param);
    }

    private void CompileLdcInt(int operand)
    {
        var reference = LLVM.ConstInt(LLVMTypeRef.Int32Type(), (ulong)operand, true);
        _cfg.CurrentBlock.VirtualRegisterStack.Push(reference);
    }

    private void CompileLdcLong(long operand)
    {
        var reference = LLVM.ConstInt(LLVMTypeRef.Int64Type(), (ulong)operand, true);
        _cfg.CurrentBlock.VirtualRegisterStack.Push(reference);
    }

    private void CompileLdcFloat(float operand)
    {
        var reference = LLVM.ConstReal(LLVMTypeRef.FloatType(), operand);
        _cfg.CurrentBlock.VirtualRegisterStack.Push(reference);
    }

    private void CompileLdcDouble(double operand)
    {
        var reference = LLVM.ConstReal(LLVMTypeRef.DoubleType(), operand);
        _cfg.CurrentBlock.VirtualRegisterStack.Push(reference);
    }

    private void CompileLdloc(int operand)
    {
        var param = _cfg.CurrentBlock.LocalVariables[operand];
        _cfg.CurrentBlock.VirtualRegisterStack.Push(param);
    }

    private void CompileLdelem()
    {
        var index = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        var array = _cfg.CurrentBlock.VirtualRegisterStack.Pop();

        var elementPtr = BuildGEP(array, index);

        var value = LLVM.BuildLoad(_builder, elementPtr, GetVirtualRegisterName());
        _cfg.CurrentBlock.VirtualRegisterStack.Push(value);
    }

    private void CompileLdelema()
    {
        var index = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        var array = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        var elementPtr = BuildGEP(array, index);
        _cfg.CurrentBlock.VirtualRegisterStack.Push(elementPtr);
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

        if (_functionsDto.ExternalFunctions.Any(func => func.Item1.StartsWith(_nameOfMethodToCall)))
        {
            var fieldName = _inputKernel.MemberInfoModule.ResolveField(operand)?.Name;
            var fullQualifiedFieldName = $"{_nameOfMethodToCall}.{fieldName}";
            BuildExternalFunctionCall(fullQualifiedFieldName);
        }
        else
        {
            throw new NotSupportedException("Only fields of external functions can be called");
        }

        
        _nameOfMethodToCall = null;
    }

    private void CompileLdind()
    {
        var elementPtr = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        var value = LLVM.BuildLoad(_builder, elementPtr, GetVirtualRegisterName());
        _cfg.CurrentBlock.VirtualRegisterStack.Push(value);
    }

    #endregion

    #region Stores

    private void CompileStarg(int index)
    {
        var value = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        _cfg.CurrentBlock.Parameters[index] = value;
    }

    private void CompileStdind()
    {
        var value = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        var elementPtr = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        LLVM.BuildStore(_builder, value, elementPtr);
    }

    private void CompileStelem()
    {
        var value = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        var index = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        var array = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        LLVMValueRef[] indices;

        if (array.TypeOf().GetElementType().TypeKind == LLVMTypeKind.LLVMArrayTypeKind) //needs two indexes: one to deref array, one to deref element in array
        { 
            indices = new[] {LLVM.ConstInt(LLVM.Int32Type(), 0, false), index};
        }
        else if (array.TypeOf().ToNativeType() == typeof(bool[]) && value.TypeOf().ToNativeType() != typeof(bool))
        {
            value = LLVM.BuildTrunc(_builder, value, LLVMTypeRef.Int1Type(), GetVirtualRegisterName());
            indices = new[] {index};
        }
        else
        {
            indices = new[] {index};
        }
        var elementPtr = LLVM.BuildGEP(_builder, array, indices, GetVirtualRegisterName());
        LLVM.BuildStore(_builder, value, elementPtr);    
    }

    private void CompileStloc(int operand)
    {
        var param = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        if (_cfg.CurrentBlock.LocalVariables.ContainsKey(operand))
        {
            _cfg.CurrentBlock.LocalVariables[operand] = param;
        }
        else
        {
            _cfg.CurrentBlock.LocalVariables.Add(operand, param);
        }
    }

    #endregion

    #region Calls

    private void CompileCallvirt(int operand)
    {
        if (_functionsDto.ExternalFunctions.All(e => e.Item1 != _nameOfMethodToCall)) //Is only used for NVVM intrinsics
        {
            throw new Exception("Cannot call external virtual functions");
        }
        
        
        var externalFunctionToCall = _functionsDto.ExternalFunctions.First(func => func.Item1 == _nameOfMethodToCall);
        LLVM.BuildCall(_builder, externalFunctionToCall.Item2, externalFunctionToCall.Item3, "");
    }
    private void CompileCall(int operand)
    {
        if (operand < 0)
        {
            throw new BadImageFormatException($"Invalid metadata token");
        }

        MethodBase? method;
        try
        {
            method = _inputKernel.MemberInfoModule.ResolveMethod(operand);
        }
        catch (Exception e)
        {
            var genericArguments = _inputKernel.MethodInfo.GetGenericArguments();
            method = _inputKernel.MethodInfo.Module.ResolveMethod(operand, genericArguments, genericArguments);
        }
        
        _nameOfMethodToCall = $"{method?.DeclaringType?.FullName}.{method?.Name}";
        var isIntrinsicFunction =
            _functionsDto.ExternalFunctions.Any(func => func.Item1.StartsWith(_nameOfMethodToCall));
        if (_nameOfMethodToCall == "System.Activator.CreateInstance") //Happens when new T()
        {
            BuildCreateInstanceCall((MethodInfo) method);
        }
        else if ((method.DeclaringType.GetElementType() != null && method.DeclaringType.GetElementType()!.IsPrimitive) || method.DeclaringType.Namespace == "System.Numerics")
        {
            BuildPrimitiveInstanceCall(method.Name);
        } 
        else if (!isIntrinsicFunction)
        {
            BuildStaticFunctionCall((MethodInfo) method);
        }
        else if (isIntrinsicFunction && _nameOfMethodToCall == "CuSharp.Kernel.KernelTools.get_WarpSize")
        {
            BuildExternalFunctionCall(_nameOfMethodToCall);
        }
    }

    private void BuildExternalFunctionCall(string fullQualifiedName)
    {
        var externalFunctionToCall = _functionsDto.ExternalFunctions.First(func => func.Item1 == fullQualifiedName);
        var call = LLVM.BuildCall(_builder, externalFunctionToCall.Item2, Array.Empty<LLVMValueRef>(), GetVirtualRegisterName());
        _cfg.CurrentBlock.VirtualRegisterStack.Push(call);
    }

    private void BuildCreateInstanceCall(MethodInfo method)
    {
        var type = method.ReturnType;
        LLVMValueRef value;
        if (type == typeof(double) || type == typeof(float))
        {
            value = LLVM.ConstReal(type.ToLLVMType(), 0);
        }
        else
        {
            value = LLVM.ConstInt(type.ToLLVMType(), 0, false);
        }
        _cfg.CurrentBlock.VirtualRegisterStack.Push(value);
    }
    private void BuildPrimitiveInstanceCall(string methodName)
    {
        switch (methodName)
        {
            case "op_Addition":
                CompileAdd();
                break;
            case "op_Subtraction":
                CompileSub();
                break;
            case "op_Multiply":
                CompileMul();
                break;
            case "op_Division":
                CompileDiv();
                break;
            case "op_Modulus":
                CompileRem();
                break;
            case "Address":
            {
                var arrayAccess = PopMultiDimArrayAccess();
                var elemPtr = BuildNestedGEP(arrayAccess.array, arrayAccess.indexX, arrayAccess.indexY);
                _cfg.CurrentBlock.VirtualRegisterStack.Push(elemPtr);
                break;
            }
            case "Set":
            {
                var value = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
                var arrayAccess = PopMultiDimArrayAccess();
                var col = BuildNestedGEP(arrayAccess.array, arrayAccess.indexX, arrayAccess.indexY);
                LLVM.BuildStore(_builder, value, col);
                break;
            }
            case "Get":
            {
                var arrayAccess = PopMultiDimArrayAccess();
                var col = BuildNestedGEP(arrayAccess.array, arrayAccess.indexX, arrayAccess.indexY);
                var elem = LLVM.BuildLoad(_builder, col, GetVirtualRegisterName());
                _cfg.CurrentBlock.VirtualRegisterStack.Push(elem);
                break;
            }
            default:
                throw new NotSupportedException("Unsupported Methodcall");
        }
    }
    
    private void CompileReturn()
    {
        if (_cfg.CurrentBlock.VirtualRegisterStack.Count == 0)
        {
            LLVM.BuildRetVoid(_builder);
        }
        else
        {
            var returnValue = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
            LLVM.BuildRet(_builder, returnValue);
        }
    }

    #endregion

    #region Converts

    private void BuildICast(LLVMTypeRef targetType)
    {
        var value = _cfg.CurrentBlock.VirtualRegisterStack.Pop(); //Assumes Signed int or Floating-Point value
        
        if (IsDecimal(value))
        {
            LLVM.BuildFPToSI(_builder, value, targetType, GetVirtualRegisterName());
        }
        else
        {
            if (targetType.TypeKind < value.TypeOf().TypeKind)
            {
                value = LLVM.BuildSExt(_builder, value, targetType, GetVirtualRegisterName());
            }
            value = LLVM.BuildIntCast(_builder, value, targetType, GetVirtualRegisterName());
        }
        _cfg.CurrentBlock.VirtualRegisterStack.Push(value);
    }
    private void CompileConvI() { CompileConvI4(); }

    private void CompileConvI1()
    {
        BuildICast(LLVMTypeRef.Int8Type());
    }

    private void CompileConvI2()
    {
        BuildICast(LLVMTypeRef.Int16Type());
    }
    private void CompileConvI4()
    {
        BuildICast(LLVMTypeRef.Int32Type());
    }

    private void CompileConvI8()
    {
        BuildICast(LLVMTypeRef.Int64Type());
    }

    private void CompileConvRUn()
    {
        var value = _cfg.CurrentBlock.VirtualRegisterStack.Pop(); //Assumes: Unsigned int type
        var converted = LLVM.BuildUIToFP(_builder, value, LLVMTypeRef.FloatType(), GetVirtualRegisterName());
        _cfg.CurrentBlock.VirtualRegisterStack.Push(converted);
    }


    private void BuildRCast(LLVMTypeRef targetType)
    {
        var value = _cfg.CurrentBlock.VirtualRegisterStack.Pop(); //assumes signed int or floating point
        if (IsDecimal(value))
        {
            value = LLVM.BuildFPCast(_builder, value, targetType, GetVirtualRegisterName());
        }
        else
        {
            value = LLVM.BuildSIToFP(_builder, value, targetType, GetVirtualRegisterName());
        }
        _cfg.CurrentBlock.VirtualRegisterStack.Push(value);
    }
    
    private void CompileConvR4()
    {
        BuildRCast(LLVMTypeRef.FloatType());
    }


    private void CompileConvR8()
    {
        BuildRCast(LLVMTypeRef.DoubleType());
    }

    private void BuildUCast(LLVMTypeRef targetType)
    {
        var value = _cfg.CurrentBlock.VirtualRegisterStack.Pop(); //Assumes: unsigned value
        value = LLVM.BuildIntCast(_builder, value, targetType, GetVirtualRegisterName());
        _cfg.CurrentBlock.VirtualRegisterStack.Push(value);
    }

    private void CompileConvU()
    {
        CompileConvU4();
    }
    
    private void CompileConvU1()
    {
        BuildUCast(LLVMTypeRef.Int8Type());
    }

    private void CompileConvU2()
    {
        BuildUCast(LLVMTypeRef.Int16Type());
    }

    private void CompileConvU4()
    {
        BuildUCast(LLVMTypeRef.Int32Type());
    }
    
    private void CompileConvU8()
    {
        BuildUCast(LLVMTypeRef.Int64Type());
    }
    
    #endregion

    #region Private Helpers



    private LLVMValueRef BuildGEP(LLVMValueRef array, LLVMValueRef offset)
    {
        LLVMValueRef[] indices;
        if (array.TypeOf().GetElementType().TypeKind == LLVMTypeKind.LLVMArrayTypeKind) //needs dual index: one to deref array, one to deref element in array
        {
            indices = new[] {LLVM.ConstInt(LLVM.Int32Type(), 0, false), offset};
        }
        else
        {
            indices = new[] {offset};
        }

        return LLVM.BuildGEP(_builder, array, indices, GetVirtualRegisterName());
    }

    private LLVMValueRef BuildNestedGEP(LLVMValueRef array, LLVMValueRef offsetX, LLVMValueRef offsetY)
    {
        var vec = BuildGEP(array, offsetX);
        vec = LLVM.BuildLoad(_builder, vec, GetVirtualRegisterName());
        return BuildGEP(vec, offsetY);
    }
    
    private (LLVMValueRef indexX, LLVMValueRef indexY, LLVMValueRef array) PopMultiDimArrayAccess()
    {
        var indexY = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        var indexX = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        var arr = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        return (indexX, indexY, arr);
    }
    
    private void BuildStaticFunctionCall(MethodInfo methodInfo)
    {
        if (_functionGenerator == null)
        {
            throw new ArgumentNullException("FunctionGenerator is required to compile calls, but it is null.");
        }

        var kernelToCall = new MSILKernel(methodInfo.Name, methodInfo);
        var function = _functionGenerator.GetOrDeclareFunction(kernelToCall);

        var parameters = methodInfo.GetParameters().ToArray();

        LLVMValueRef[] args;
        
        args = new LLVMValueRef[parameters.Length];
        
        for (var i = parameters.Length - 1; i >= 0; i--)
        {
            var param = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
            if (param.TypeOf().TypeKind == LLVMTypeKind.LLVMPointerTypeKind && param.TypeOf().GetElementType().TypeKind == LLVMTypeKind.LLVMArrayTypeKind)
            {
                if (param.TypeOf().GetElementType().GetElementType().TypeKind == LLVMTypeKind.LLVMPointerTypeKind)
                {
                    param = LLVM.BuildPointerCast(_builder, param,
                        LLVM.PointerType(
                            LLVM.PointerType(param.TypeOf().GetElementType().GetElementType().GetElementType(), 0), 0),
                        GetVirtualRegisterName());
                }
                else
                {
                    param = LLVM.BuildPointerCast(_builder, param,
                                     LLVM.PointerType(param.TypeOf().GetElementType().GetElementType(), 0), GetVirtualRegisterName());   
                }
            }
            args[i] = param;
        }

        if (methodInfo.ReturnType == typeof(void))
        {
            LLVM.BuildCall(_builder, function, args, String.Empty);
        }
        else
        {
            var call = LLVM.BuildCall(_builder, function, args, GetVirtualRegisterName());
            
            _cfg.CurrentBlock.VirtualRegisterStack.Push(call);
        }
    }
    private void BuildConditionalBranch(long thenOffset, long elseOffset, LLVMValueRef predicate)
    {
        var elseBlock = _cfg.GetBlock(elseOffset, _functionsDto.Function);
        var thenBlock = _cfg.GetBlock(thenOffset, _functionsDto.Function);
        LLVM.BuildCondBr(_builder, predicate, thenBlock.BlockRef, elseBlock.BlockRef);
        
        _cfg.CurrentBlock.AddSuccessors(thenBlock, elseBlock);
    }

    private LLVMValueRef BuildPredicateFromStack(LLVMIntPredicate intPredicate, LLVMRealPredicate realPredicate)
    {
        var value2 = _cfg.CurrentBlock.VirtualRegisterStack.Pop();
        var value1 = _cfg.CurrentBlock.VirtualRegisterStack.Pop();

        return BuildComparison(intPredicate, realPredicate, value1, value2);
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

    private string GetGlobalVariableName()
    {
        return $"glob{_globalVariableCounter++}";
    }

    private bool IsDecimal(LLVMValueRef value)
    {
        return IsFloat(value) || IsDouble(value);
    }

    private bool IsFloat(LLVMValueRef value)
    {
        return value.TypeOf().ToNativeType() == typeof(float);
    }

    private bool IsDouble(LLVMValueRef value)
    {
        return value.TypeOf().ToNativeType() == typeof(double);
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

    #endregion
}