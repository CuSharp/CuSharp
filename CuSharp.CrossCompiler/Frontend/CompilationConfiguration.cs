using System.Reflection;
using System.Runtime.CompilerServices;
using LLVMSharp;
using Microsoft.VisualBasic.CompilerServices;

namespace CuSharp.CudaCompiler.Frontend;

public class CompilationConfiguration
{
    public string DataLayout { get; private init; } = "";
    public string Target { get; private init; } = "";
    public string KernelName { get; set; }

    public Action<LLVMModuleRef, LLVMValueRef>[] DeclareAnnotations { get; set; } = 
        Array.Empty<Action<LLVMModuleRef, LLVMValueRef>>();
    public Func<LLVMModuleRef, LLVMValueRef>[] DeclareExternalFunctions { get; set; } = 
        Array.Empty<Func<LLVMModuleRef, LLVMValueRef>>();

    //Default Configuration for NVVM Kernels
    public static CompilationConfiguration NvvmConfiguration = new CompilationConfiguration()
    {
        DataLayout = "e-p:64:64:64-i1:8:8-i8:8:8-i16:16:16-i32:32:32-i64:64:64-i128:128:128-f32:32:32-f64:64:64-v16:16:16-v32:32:32-v64:64:64-v128:128:128-n16:32:64",
        KernelName = "",
        Target = "nvptx64-nvidia-cuda",
        DeclareAnnotations = new[]
        {
            (LLVMModuleRef moduleRef, LLVMValueRef functionRef) =>
            {
                var mdString = LLVM.MDString("kernel", 6);
                var node = LLVM.MDNode(new LLVMValueRef[] {functionRef, mdString, LLVM.ConstInt(LLVM.Int32Type(),1,false)});
                LLVM.AddNamedMetadataOperand(moduleRef, "nvvm.annotations", node);
            },
            (moduleRef, _) =>
            {
                var irVersionNode = LLVM.MDNode(new LLVMValueRef[]
                {
                    LLVM.ConstInt(LLVM.Int32Type(), 2, false),
                    LLVM.ConstInt(LLVM.Int32Type(), 0, false),
                    LLVM.ConstInt(LLVM.Int32Type(), 3, false),
                    LLVM.ConstInt(LLVM.Int32Type(),1,false),
                });
                LLVM.AddNamedMetadataOperand(moduleRef, "nvvmir.version", irVersionNode);
            }
        },
        DeclareExternalFunctions = new[]
        {
            (LLVMModuleRef moduleRef) =>
            {
                return LLVM.AddFunction(moduleRef, "llvm.nvvm.read.ptx.sreg.ctaid.x",
                    LLVM.FunctionType(LLVMTypeRef.Int32Type(), new LLVMTypeRef[]{} , false));
            },
            (moduleRef) =>
            {
                return LLVM.AddFunction(moduleRef,  "llvm.nvvm.read.ptx.sreg.ntid.x",
                    LLVM.FunctionType(LLVMTypeRef.Int32Type(), new LLVMTypeRef[]{} , false));
            },
            (moduleRef) =>
            {
                return LLVM.AddFunction(moduleRef, "llvm.nvvm.read.ptx.sreg.tid.x",
                    LLVM.FunctionType(LLVMTypeRef.Int32Type(), new LLVMTypeRef[]{} , false));
            },
            (moduleRef) =>
            {
                return LLVM.AddFunction(moduleRef, "llvm.nvvm.barrier0",
                    LLVM.FunctionType(LLVMTypeRef.VoidType(), new LLVMTypeRef[]{} , false));
            }
        }

    };
}