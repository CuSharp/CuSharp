using CuSharp.CudaCompiler.Frontend;
using CuSharp.Tests.TestHelper;
using Xunit;

namespace CuSharp.Tests.CuSharp.CudaCompiler
{
    [Collection("Sequential")]
    [Trait(TestCategories.TestCategory, TestCategories.UnitDebugOnly)]
    public class MethodBodyCrossCompilerDebugModeTests
    {
        private readonly MethodInfoLoader _methodLoader = new();
        private readonly LLVMRepresentationLoader _llvmLoader = new();
        private readonly TestValidator _validator = new();

        [Fact]
        public void TestLogicalAnd()
        {
            const string kernelName = "LogicalAnd";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.LogicalAnd);
            var config = CompilationConfiguration.NvvmConfiguration;
            config.KernelName = kernelName;
            var crossCompiler = new KernelCrossCompiler(config);
            var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

            var expected = string.Empty; // TODO: Load expected output
            var actual = llvmKernel.KernelBuffer;

            //Assert.Equal(expected, actual);
            Assert.True(_validator.KernelIsCorrect(actual, kernelName));
        }

        [Fact]
        public void TestLogicalOr()
        {
            const string kernelName = "LogicalOr";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.LogicalOr);
            var config = CompilationConfiguration.NvvmConfiguration;
            config.KernelName = kernelName;
            var crossCompiler = new KernelCrossCompiler(config);
            var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

            var expected = string.Empty; // TODO: Load expected output
            var actual = llvmKernel.KernelBuffer;

            //Assert.Equal(expected, actual);
            Assert.True(_validator.KernelIsCorrect(actual, kernelName));
        }
    }
}
