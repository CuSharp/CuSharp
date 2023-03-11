using System;
using System.Reflection;
using CuSharp.CudaCompiler.Frontend;
using LibNVVMBinder;
using Xunit;

namespace CuSharp.Tests.CuSharp.CudaCompiler;

public class KernelCrossCompilerMetaInformationTests
{
    private void EmptyMethod()
    {}

    private string _expectedLLVMRepresentation = "; ModuleID = 'EmptyMethodKernelMODULE'\n"
                                                 + "source_filename = \"EmptyMethodKernelMODULE\"\n"
                                                 + "target datalayout = \"e-p:64:64:64-i1:8:8-i8:8:8-i16:16:16-i32:32:32-i64:64:64-i128:128:128-f32:32:32-f64:64:64-v16:16:16-v32:32:32-v64:64:64-v128:128:128-n16:32:64\"\n"
                                                 + "target triple = \"nvptx64-nvidia-cuda\"\n"
                                                 + "\n"
                                                 + "define void @EmptyMethodKernel() {\n"
                                                 + "entry:\n"
                                                 + "  ret void\n"
                                                 + "}\n"
                                                 + "\n"
                                                 + "; Function Attrs: nounwind readnone\n"
                                                 + "declare i32 @llvm.nvvm.read.ptx.sreg.ctaid.x() #0\n"
                                                 + "\n"
                                                 + "; Function Attrs: nounwind readnone\n"
                                                 + "declare i32 @llvm.nvvm.read.ptx.sreg.ntid.x() #0\n"
                                                 + "\n"
                                                 + "; Function Attrs: nounwind readnone\n"
                                                 + "declare i32 @llvm.nvvm.read.ptx.sreg.tid.x() #0\n"
                                                 + "\n"
                                                 + "; Function Attrs: convergent nounwind\n"
                                                 + "declare void @llvm.nvvm.barrier0() #1\n"
                                                 + "\n"
                                                 + "attributes #0 = { nounwind readnone }\n"
                                                 + "attributes #1 = { convergent nounwind }\n"
                                                 + "\n"
                                                 + "!nvvm.annotations = !{!0}\n"
                                                 + "!nvvmir.version = !{!1}\n"
                                                 + "\n"
                                                 + "!0 = !{void ()* @EmptyMethodKernel, !\"kernel\", i32 1}\n"
                                                 + "!1 = !{i32 2, i32 0, i32 3, i32 1}\n";

    private MethodInfo GetMethodInfo(Action fn)
    {
        return fn.Method;
    }


    private bool KernelIsCorrect(string llvmKernel, string kernelName)
    {
        var nvvm = new NVVMProgram();
        nvvm.AddModule(llvmKernel, kernelName);
        var result = nvvm.Verify(new string[] { });
        return result == NVVMProgram.NVVMResult.NVVM_SUCCESS;
    }

    [Fact]
    public void TestEmptyMethodCompiles()
    {
        var method = GetMethodInfo(EmptyMethod);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = "EmptyMethodKernel";
        var crossCompiler = new KernelCrossCompiler(config);
        var llvmKernel = crossCompiler.Compile(new MSILKernel("EmptyMethodKernel", method.GetMethodBody().GetILAsByteArray(), new ParameterInfo[]{}));
        Assert.Equal(_expectedLLVMRepresentation,llvmKernel.KernelBuffer);
    }
    
    [Fact]
    public void TestEmptyMethodIsCorrectIR()
    {
        var method = GetMethodInfo(EmptyMethod);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = "EmptyMethodKernel";
        var crossCompiler = new KernelCrossCompiler(config);
        var llvmKernel = crossCompiler.Compile(new MSILKernel("EmptyMethodKernel", method.GetMethodBody().GetILAsByteArray(), new ParameterInfo[]{}));
        var isCorrent = KernelIsCorrect(llvmKernel.KernelBuffer, config.KernelName);
        Assert.True(isCorrent);
    }


    private void ArrayParameterMethod(int[] A, int[] B)
    {}

    private string expectedArrayParameterMethodLLVM = "; ModuleID = 'ArrayParameterMethodMODULE'\n"
                                                      + "source_filename = \"ArrayParameterMethodMODULE\"\n"
                                                      + "\n"
                                                      + "define void @ArrayParameterMethod(i32* %param0, i32* %param1) {\n"
                                                      + "entry:\n"
                                                      + "  ret void\n"
                                                      + "}\n";

    private MethodInfo GetArrayMethodInfo(Action<int[], int[]> fn)
    {
        return fn.Method;
    }
    
    
    [Fact]
    public void TestArrayParameterMethod()
    {
        var method = GetArrayMethodInfo(ArrayParameterMethod);
        var config = new CompilationConfiguration()
        {
            KernelName = "ArrayParameterMethod"
        };
        var compiler = new KernelCrossCompiler(config);
        var llvmKernel = compiler.Compile(new MSILKernel("ArrayParameterMethod", method.GetMethodBody().GetILAsByteArray(),
            method.GetParameters()));
        Assert.Equal(expectedArrayParameterMethodLLVM, llvmKernel.KernelBuffer);
    }

    [Fact]
    public void TestArrayParameterMethodIsCorrectIR()
    {
        var method = GetArrayMethodInfo(ArrayParameterMethod);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = "ArrayParameterMethod";
        var compiler = new KernelCrossCompiler(config);
        var llvmKernel = compiler.Compile(new MSILKernel("ArrayParameterMethod",
            method.GetMethodBody().GetILAsByteArray(), method.GetParameters()));
        Assert.True(KernelIsCorrect(llvmKernel.KernelBuffer, llvmKernel.Name));
    }
    
    private void MixedParameterMethod(int[] A, int[] B, bool b, int c)
    {}

    private string expectedMixedParameterMethodLLVM = "; ModuleID = 'MixedParameterMethodMODULE'\n"
                                                      + "source_filename = \"MixedParameterMethodMODULE\"\n"
                                                      + "\n"
                                                      + "define void @MixedParameterMethod(i32* %param0, i32* %param1, i1 %param2, i32 %param3) {\n"
                                                      + "entry:\n"
                                                      + "  ret void\n"
                                                      + "}\n";

    private MethodInfo GetMixedMethodInfo(Action<int[], int[], bool, int> fn)
    {
        return fn.Method;
    }
    
    
    [Fact]
    public void TestMixedParameterMethod()
    {
        var method = GetMixedMethodInfo(MixedParameterMethod);
        var config = new CompilationConfiguration()
        {
            KernelName = "MixedParameterMethod"
        };
        var compiler = new KernelCrossCompiler(config);
        var llvmKernel = compiler.Compile(new MSILKernel("MixedParameterMethod", method.GetMethodBody().GetILAsByteArray(),
            method.GetParameters()));
        Assert.Equal(expectedMixedParameterMethodLLVM, llvmKernel.KernelBuffer);
    }

    [Fact]
    public void TestMixedParameterMethodIsCorrectIR()
    {
        var method = GetMixedMethodInfo(MixedParameterMethod);
        var config = CompilationConfiguration.NvvmConfiguration;
        config.KernelName = "MixedParameterMethod";
        var compiler = new KernelCrossCompiler(config);
        var llvmKernel = compiler.Compile(new MSILKernel("MixedParameterMethod",
            method.GetMethodBody().GetILAsByteArray(), method.GetParameters()));
        Assert.True(KernelIsCorrect(llvmKernel.KernelBuffer, llvmKernel.Name));
    }
}