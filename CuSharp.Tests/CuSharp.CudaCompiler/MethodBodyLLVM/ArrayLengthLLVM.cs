using CuSharp.CudaCompiler.Frontend;
using CuSharp.Tests.TestHelper;
using Xunit;
using static CuSharp.Tests.TestHelper.MethodInfoLoader;
using static CuSharp.Tests.TestHelper.MethodsToCompile;

namespace CuSharp.Tests.CuSharp.CudaCompiler.MethodBodyLLVM;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.Unit)]
public class ArrayLengthLLVM
{
    private readonly LLVMRepresentationLoader _llvmLoader = new();
    private readonly TestValidator _validator = new();

    // TODO: Remove
    //[Fact]
    //public void ArrayLengthAttributeGeneratesAdditionalLengthParams_LLVM()
    //{
    //    const string kernelName = "ArrayLengthAttributes";
    //    var method = GetMethodInfo<int[], int>(ArrayLengthAttribute);
    //    var config = CompilationConfiguration.NvvmConfiguration;
    //    config.KernelName = kernelName;
    //    var crossCompiler = new KernelCrossCompiler(config);
    //    var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

    //    var expected = "";
    //    var actual = llvmKernel.KernelBuffer;

    //    Assert.Equal(expected, actual);
    //    Assert.True(_validator.KernelIsCorrect(actual, kernelName));
    //}
}