using System.Reflection.Metadata;
using LLVMSharp;

namespace CuSharp.CudaCompiler.Frontend;

public class MethodBodyCompiler
{
    private readonly BinaryReader _reader;
    private readonly LLVMBuilderRef _builder;
    private readonly LLVMValueRef _function;
    private readonly Stack<LLVMValueRef> _virtualRegisterStack = new();
    private readonly List<LLVMValueRef> _localVariableList = new();
    
    private long _virtualRegisterCounter;

    public MethodBodyCompiler(byte[] kernelBuffer, LLVMBuilderRef builder, LLVMValueRef function)
    {
        _builder = builder;
        _function = function;
        _reader = new BinaryReader(new MemoryStream(kernelBuffer));
    }

    public void CompileMethodBody()
    {
        while (_reader.BaseStream.Position < _reader.BaseStream.Length)
        {
            if (_reader.BaseStream.Position == _reader.BaseStream.Length)
            {
                throw new ArgumentOutOfRangeException("Unexpected end of method body.");
            }
            
            var opCode = ReadOpCode();

            switch (opCode)
            {
                case ILOpCode.Nop:
                    continue;

                case ILOpCode.Constrained: throw new NotSupportedException();
                case ILOpCode.Readonly: throw new NotSupportedException();
                case ILOpCode.Tail: throw new NotSupportedException();
                case ILOpCode.Unaligned: throw new NotSupportedException();
                case ILOpCode.Volatile: throw new NotSupportedException();
                case ILOpCode.Add:
                    CompileAdd();
                    break;
                case ILOpCode.Add_ovf: throw new NotSupportedException();
                case ILOpCode.Add_ovf_un: throw new NotSupportedException();
                case ILOpCode.And: throw new NotSupportedException();
                case ILOpCode.Arglist: throw new NotSupportedException();
                case ILOpCode.Beq: throw new NotSupportedException();
                case ILOpCode.Beq_s: throw new NotSupportedException();
                case ILOpCode.Bge: throw new NotSupportedException();
                case ILOpCode.Bge_s: throw new NotSupportedException();
                case ILOpCode.Bge_un: throw new NotSupportedException();
                case ILOpCode.Bge_un_s: throw new NotSupportedException();
                case ILOpCode.Bgt: throw new NotSupportedException();
                case ILOpCode.Bgt_s: throw new NotSupportedException();
                case ILOpCode.Bgt_un: throw new NotSupportedException();
                case ILOpCode.Bgt_un_s: throw new NotSupportedException();
                case ILOpCode.Ble: throw new NotSupportedException();
                case ILOpCode.Ble_s: throw new NotSupportedException();
                case ILOpCode.Ble_un: throw new NotSupportedException();
                case ILOpCode.Ble_un_s: throw new NotSupportedException();
                case ILOpCode.Blt: throw new NotSupportedException();
                case ILOpCode.Blt_s: throw new NotSupportedException();
                case ILOpCode.Blt_un: throw new NotSupportedException();
                case ILOpCode.Blt_un_s: throw new NotSupportedException();
                case ILOpCode.Bne_un: throw new NotSupportedException();
                case ILOpCode.Bne_un_s: throw new NotSupportedException();
                case ILOpCode.Br: throw new NotSupportedException();
                case ILOpCode.Br_s: throw new NotSupportedException();
                case ILOpCode.Break: throw new NotSupportedException();
                case ILOpCode.Brfalse: throw new NotSupportedException();
                case ILOpCode.Brfalse_s: throw new NotSupportedException();
                case ILOpCode.Brtrue: throw new NotSupportedException();
                case ILOpCode.Brtrue_s: throw new NotSupportedException();
                case ILOpCode.Call: throw new NotSupportedException();
                case ILOpCode.Callvirt: throw new NotSupportedException();
                case ILOpCode.Calli: throw new NotSupportedException();
                case ILOpCode.Ceq: throw new NotSupportedException();
                case ILOpCode.Cgt: throw new NotSupportedException();
                case ILOpCode.Cgt_un: throw new NotSupportedException();
                case ILOpCode.Clt: throw new NotSupportedException();
                case ILOpCode.Clt_un: throw new NotSupportedException();
                case ILOpCode.Ckfinite: throw new NotSupportedException();
                case ILOpCode.Conv_i1: throw new NotSupportedException();
                case ILOpCode.Conv_i2: throw new NotSupportedException();
                case ILOpCode.Conv_i4: throw new NotSupportedException();
                case ILOpCode.Conv_i8: throw new NotSupportedException();
                case ILOpCode.Conv_r4: throw new NotSupportedException();
                case ILOpCode.Conv_r8: throw new NotSupportedException();
                case ILOpCode.Conv_u1: throw new NotSupportedException();
                case ILOpCode.Conv_u2: throw new NotSupportedException();
                case ILOpCode.Conv_u4: throw new NotSupportedException();
                case ILOpCode.Conv_u8: throw new NotSupportedException();
                case ILOpCode.Conv_i: throw new NotSupportedException();
                case ILOpCode.Conv_u: throw new NotSupportedException();
                case ILOpCode.Conv_r_un: throw new NotSupportedException();
                case ILOpCode.Conv_ovf_i1: throw new NotSupportedException();
                case ILOpCode.Conv_ovf_i2: throw new NotSupportedException();
                case ILOpCode.Conv_ovf_i4: throw new NotSupportedException();
                case ILOpCode.Conv_ovf_i8: throw new NotSupportedException();
                case ILOpCode.Conv_ovf_u1: throw new NotSupportedException();
                case ILOpCode.Conv_ovf_u2: throw new NotSupportedException();
                case ILOpCode.Conv_ovf_u4: throw new NotSupportedException();
                case ILOpCode.Conv_ovf_u8: throw new NotSupportedException();
                case ILOpCode.Conv_ovf_i: throw new NotSupportedException();
                case ILOpCode.Conv_ovf_u: throw new NotSupportedException();
                case ILOpCode.Conv_ovf_i1_un: throw new NotSupportedException();
                case ILOpCode.Conv_ovf_i2_un: throw new NotSupportedException();
                case ILOpCode.Conv_ovf_i4_un: throw new NotSupportedException();
                case ILOpCode.Conv_ovf_i8_un: throw new NotSupportedException();
                case ILOpCode.Conv_ovf_u1_un: throw new NotSupportedException();
                case ILOpCode.Conv_ovf_u2_un: throw new NotSupportedException();
                case ILOpCode.Conv_ovf_u4_un: throw new NotSupportedException();
                case ILOpCode.Conv_ovf_u8_un: throw new NotSupportedException();
                case ILOpCode.Conv_ovf_i_un: throw new NotSupportedException();
                case ILOpCode.Conv_ovf_u_un: throw new NotSupportedException();
                case ILOpCode.Cpblk: throw new NotSupportedException();
                case ILOpCode.Div: throw new NotSupportedException();
                case ILOpCode.Div_un: throw new NotSupportedException();
                case ILOpCode.Dup: throw new NotSupportedException();
                case ILOpCode.Endfilter: throw new NotSupportedException();
                case ILOpCode.Endfinally: throw new NotSupportedException();
                case ILOpCode.Initblk: throw new NotSupportedException();
                case ILOpCode.Jmp: throw new NotSupportedException();
                case ILOpCode.Ldarg:
                    CompileLdarg(_reader.ReadUInt16());
                    break;
                case ILOpCode.Ldarg_s: 
                    CompileLdarg(_reader.ReadByte());
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
                case ILOpCode.Ldarga: throw new NotSupportedException();
                case ILOpCode.Ldarga_s: throw new NotSupportedException();
                case ILOpCode.Ldc_i4:
                    CompileLdcInt(_reader.ReadInt32());
                    break;
                case ILOpCode.Ldc_i8:
                    CompileLdcLong(_reader.ReadInt64());
                    break;
                case ILOpCode.Ldc_r4:
                    CompileLdcFloat(_reader.ReadSingle());
                    break;
                case ILOpCode.Ldc_r8:
                    CompileLdcDouble(_reader.ReadDouble());
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
                    CompileLdcInt(_reader.ReadSByte());
                    break;
                case ILOpCode.Ldnull: throw new NotSupportedException();
                case ILOpCode.Ldstr: throw new NotSupportedException();
                case ILOpCode.Ldftn: throw new NotSupportedException();
                case ILOpCode.Ldind_i1: throw new NotSupportedException();
                case ILOpCode.Ldind_i2: throw new NotSupportedException();
                case ILOpCode.Ldind_i4: throw new NotSupportedException();
                case ILOpCode.Ldind_i8: throw new NotSupportedException();
                case ILOpCode.Ldind_u1: throw new NotSupportedException();
                case ILOpCode.Ldind_u2: throw new NotSupportedException();
                case ILOpCode.Ldind_u4: throw new NotSupportedException();
                case ILOpCode.Ldind_r4: throw new NotSupportedException();
                case ILOpCode.Ldind_r8: throw new NotSupportedException();
                case ILOpCode.Ldind_i: throw new NotSupportedException();
                case ILOpCode.Ldind_ref: throw new NotSupportedException();
                case ILOpCode.Ldloc:
                    CompileLdloc(_reader.ReadUInt16());
                    break;
                case ILOpCode.Ldloc_s:
                    CompileLdloc(_reader.ReadByte());
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
                case ILOpCode.Ldloca: throw new NotSupportedException();
                case ILOpCode.Ldloca_s: throw new NotSupportedException();
                case ILOpCode.Leave: throw new NotSupportedException();
                case ILOpCode.Leave_s: throw new NotSupportedException();
                case ILOpCode.Localloc: throw new NotSupportedException();
                case ILOpCode.Mul:
                    CompileMul();
                    break;
                case ILOpCode.Mul_ovf: throw new NotSupportedException();
                case ILOpCode.Mul_ovf_un: throw new NotSupportedException();
                case ILOpCode.Neg: throw new NotSupportedException();
                case ILOpCode.Newobj: throw new NotSupportedException();
                case ILOpCode.Not: throw new NotSupportedException();
                case ILOpCode.Or: throw new NotSupportedException();
                case ILOpCode.Pop: throw new NotSupportedException();
                case ILOpCode.Rem: throw new NotSupportedException();
                case ILOpCode.Rem_un: throw new NotSupportedException();
                case ILOpCode.Ret: 
                    LLVM.BuildRetVoid(_builder);
                    break;
                case ILOpCode.Shl: throw new NotSupportedException();
                case ILOpCode.Shr: throw new NotSupportedException();
                case ILOpCode.Shr_un: throw new NotSupportedException();
                case ILOpCode.Starg: throw new NotSupportedException();
                case ILOpCode.Starg_s: throw new NotSupportedException();
                case ILOpCode.Stind_i1: throw new NotSupportedException();
                case ILOpCode.Stind_i2: throw new NotSupportedException();
                case ILOpCode.Stind_i4: throw new NotSupportedException();
                case ILOpCode.Stind_i8: throw new NotSupportedException();
                case ILOpCode.Stind_r4: throw new NotSupportedException();
                case ILOpCode.Stind_r8: throw new NotSupportedException();
                case ILOpCode.Stind_i: throw new NotSupportedException();
                case ILOpCode.Stind_ref: throw new NotSupportedException();
                case ILOpCode.Stloc:
                    CompileStloc(_reader.ReadUInt16());
                    break;
                case ILOpCode.Stloc_s:
                    CompileStloc(_reader.ReadByte());
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
                case ILOpCode.Sub_ovf: throw new NotSupportedException();
                case ILOpCode.Sub_ovf_un: throw new NotSupportedException();
                case ILOpCode.Switch: throw new NotSupportedException();
                case ILOpCode.Xor: throw new NotSupportedException();
                case ILOpCode.Box: throw new NotSupportedException();
                case ILOpCode.Castclass: throw new NotSupportedException();
                case ILOpCode.Cpobj: throw new NotSupportedException();
                case ILOpCode.Initobj: throw new NotSupportedException();
                case ILOpCode.Isinst: throw new NotSupportedException();
                case ILOpCode.Ldelem: throw new NotSupportedException();
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
                case ILOpCode.Ldelem_i: throw new NotSupportedException();
                case ILOpCode.Ldelem_ref: throw new NotSupportedException();
                case ILOpCode.Ldelema: throw new NotSupportedException();
                case ILOpCode.Ldfld: throw new NotSupportedException();
                case ILOpCode.Ldflda: throw new NotSupportedException();
                case ILOpCode.Stfld: throw new NotSupportedException();
                case ILOpCode.Ldlen: throw new NotSupportedException();
                case ILOpCode.Ldobj: throw new NotSupportedException();
                case ILOpCode.Ldsfld: throw new NotSupportedException();
                case ILOpCode.Ldsflda: throw new NotSupportedException();
                case ILOpCode.Stsfld: throw new NotSupportedException();
                case ILOpCode.Ldtoken: throw new NotSupportedException();
                case ILOpCode.Ldvirtftn: throw new NotSupportedException();
                case ILOpCode.Mkrefany: throw new NotSupportedException();
                case ILOpCode.Newarr: throw new NotSupportedException();
                case ILOpCode.Refanytype: throw new NotSupportedException();
                case ILOpCode.Refanyval: throw new NotSupportedException();
                case ILOpCode.Rethrow: throw new NotSupportedException();
                case ILOpCode.Sizeof: throw new NotSupportedException();
                case ILOpCode.Stelem: throw new NotSupportedException();
                case ILOpCode.Stelem_i1:
                case ILOpCode.Stelem_i2:
                case ILOpCode.Stelem_i4:
                case ILOpCode.Stelem_i8:
                case ILOpCode.Stelem_r4:
                case ILOpCode.Stelem_r8:
                    CompileStelem();
                    break;
                case ILOpCode.Stelem_i: throw new NotSupportedException();
                case ILOpCode.Stelem_ref: throw new NotSupportedException();
                case ILOpCode.Stobj: throw new NotSupportedException();
                case ILOpCode.Throw: throw new NotSupportedException();
                case ILOpCode.Unbox: throw new NotSupportedException();
                case ILOpCode.Unbox_any: throw new NotSupportedException();
                default: throw new NotSupportedException();
            }
        }
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

    private void CompileLdelem()
    {
        var index = _virtualRegisterStack.Pop();
        var array = _virtualRegisterStack.Pop();

        var elementPtr = LLVM.BuildGEP(_builder, array, new[] { index }, GetVirtualRegisterName());
        var value = LLVM.BuildLoad(_builder, elementPtr, GetVirtualRegisterName());
        _virtualRegisterStack.Push(value);
    }

    private void CompileLdarg(int operand)
    {
        var index = operand - 1;
        var param = LLVM.GetParam(_function, (uint)index);
        _virtualRegisterStack.Push(param);
    }

    private void CompileMul()
    {
        var param2 = _virtualRegisterStack.Pop();
        var param1 = _virtualRegisterStack.Pop();
        var result = LLVM.BuildMul(_builder, param1, param2, GetVirtualRegisterName());
        _virtualRegisterStack.Push(result);
    }

    private void CompileAdd()
    {
        var param2 = _virtualRegisterStack.Pop();
        var param1 = _virtualRegisterStack.Pop();
        var result = LLVM.BuildAdd(_builder, param1, param2, GetVirtualRegisterName());
        _virtualRegisterStack.Push(result);
    }

    private void CompileSub()
    {
        var param2 = _virtualRegisterStack.Pop();
        var param1 = _virtualRegisterStack.Pop();
        var result = LLVM.BuildSub(_builder, param1, param2, GetVirtualRegisterName());
        _virtualRegisterStack.Push(result);
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
        var reference = LLVM.ConstInt(LLVMTypeRef.FloatType(), (ulong)operand, true);
        _virtualRegisterStack.Push(reference);
    }

    private void CompileLdcDouble(double operand)
    {
        var reference = LLVM.ConstInt(LLVMTypeRef.DoubleType(), (ulong)operand, true);
        _virtualRegisterStack.Push(reference);
    }

    private void CompileLdloc(int operand)
    {
        var param = _localVariableList[operand];
        _virtualRegisterStack.Push(param);
    }

    private void CompileStloc(int operand)
    {
        var param = _virtualRegisterStack.Pop();

        if (_localVariableList.Count > operand)
        {
            _localVariableList[operand] = param;
        }
        else
        {
            _localVariableList.Insert(operand, param);
        }
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
}