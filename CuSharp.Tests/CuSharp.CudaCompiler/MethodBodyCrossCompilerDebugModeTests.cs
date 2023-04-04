using CuSharp.CudaCompiler.Frontend;
using CuSharp.Tests.TestHelper;
using Xunit;
using Xunit.Abstractions;

namespace CuSharp.Tests.CuSharp.CudaCompiler
{
    public class MethodBodyCrossCompilerDebugModeTests
    {
        private readonly MethodInfoLoader _methodLoader = new();
        private readonly LLVMRepresentationLoader _llvmLoader = new();
        private readonly TestValidator _validator = new();

        private readonly ITestOutputHelper _output;

        public MethodBodyCrossCompilerDebugModeTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestLogicalAnd()
        {
            const string kernelName = "LogicalAnd";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.LogicalAnd);
            var config = CompilationConfiguration.NvvmConfiguration;
            config.KernelName = kernelName;
            var crossCompiler = new KernelCrossCompiler(config);
            var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

            var expected = string.Empty; //_llvmLoader.GetLogicalAndToolsTestResult(kernelName, TypesAsString.IntType);
            var actual = llvmKernel.KernelBuffer;

            _output.WriteLine($"Used gpu device: '{actual.ToString()}'");

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

            var expected = string.Empty; //_llvmLoader.GetLogicalAndToolsTestResult(kernelName, TypesAsString.IntType);
            var actual = llvmKernel.KernelBuffer;

            _output.WriteLine($"Used gpu device: '{actual.ToString()}'");

            //Assert.Equal(expected, actual);
            Assert.True(_validator.KernelIsCorrect(actual, kernelName));
        }
    }
}
