using CuSharp.CudaCompiler.Frontend;
using Xunit;

namespace CuSharp.Tests.CuSharp.CudaCompiler.KernelCrossCompilerTests
{
    public class MethodBodyCrossCompilerTests
    {
        private readonly LLVMRepresentationLoader _representationLoader = new();
        private readonly MethodsToCompile _methods = new();
        private readonly MethodInfoLoader _methodInfo = new();

        [Fact]
        public void TestScalarIntAdditionWithConst()
        {
            const string kernelName = "ScalarIntAdditionWithConst";

            var method = _methodInfo.GetScalarIntMethodInfo(_methods.ScalarIntAdditionWithConst);
            var config = CompilationConfiguration.NvvmConfiguration;
            config.KernelName = kernelName;
            var crossCompiler = new KernelCrossCompiler(config);
            var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

            var expected = _representationLoader.GetScalarIntAdditionWithConstTestResult(kernelName);
            var actual = llvmKernel.KernelBuffer;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestScalarIntAddition()
        {
            const string kernelName = "ScalarIntAddition";

            var method = _methodInfo.GetScalarIntMethodInfo(_methods.ScalarIntAddition);
            var config = CompilationConfiguration.NvvmConfiguration;
            config.KernelName = kernelName;
            var crossCompiler = new KernelCrossCompiler(config);
            var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));
            
            var expected = _representationLoader.GetScalarIntAdditionTestResult(kernelName);
            var actual = llvmKernel.KernelBuffer;
            
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestScalarIntSubtraction()
        {
            const string kernelName = "ScalarIntSubtraction";

            var method = _methodInfo.GetScalarIntMethodInfo(_methods.ScalarIntSubtraction);
            var config = CompilationConfiguration.NvvmConfiguration;
            config.KernelName = kernelName;
            var crossCompiler = new KernelCrossCompiler(config);
            var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

            var expected = _representationLoader.GetScalarIntSubtractionTestResult(kernelName);
            var actual = llvmKernel.KernelBuffer;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestScalarIntMultiplication()
        {
            const string kernelName = "ScalarIntMultiplication";

            var method = _methodInfo.GetScalarIntMethodInfo(_methods.ScalarIntMultiplication);
            var config = CompilationConfiguration.NvvmConfiguration;
            config.KernelName = kernelName;
            var crossCompiler = new KernelCrossCompiler(config);
            var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

            var expected = _representationLoader.GetScalarIntMultiplicationTestResult(kernelName);
            var actual = llvmKernel.KernelBuffer;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestScalarFloatAddition()
        {
            const string kernelName = "ScalarFloatAddition";

            var method = _methodInfo.GetScalarFloatMethodInfo(_methods.ScalarFloatAddition);
            var config = CompilationConfiguration.NvvmConfiguration;
            config.KernelName = kernelName;
            var crossCompiler = new KernelCrossCompiler(config);
            var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

            var expected = _representationLoader.GetScalarFloatAdditionTestResult(kernelName);
            var actual = llvmKernel.KernelBuffer;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestScalarFloatSubtraction()
        {
            const string kernelName = "ScalarFloatSubtraction";

            var method = _methodInfo.GetScalarFloatMethodInfo(_methods.ScalarFloatSubtraction);
            var config = CompilationConfiguration.NvvmConfiguration;
            config.KernelName = kernelName;
            var crossCompiler = new KernelCrossCompiler(config);
            var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

            var expected = _representationLoader.GetScalarFloatSubtractionTestResult(kernelName);
            var actual = llvmKernel.KernelBuffer;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestScalarFloatMultiplication()
        {
            const string kernelName = "ScalarFloatMultiplication";

            var method = _methodInfo.GetScalarFloatMethodInfo(_methods.ScalarFloatMultiplication);
            var config = CompilationConfiguration.NvvmConfiguration;
            config.KernelName = kernelName;
            var crossCompiler = new KernelCrossCompiler(config);
            var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

            var expected = _representationLoader.GetScalarFloatMultiplicationTestResult(kernelName);
            var actual = llvmKernel.KernelBuffer;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestArrayIntAddition()
        {
            const string kernelName = "ArrayIntAddition";

            var method = _methodInfo.GetArrayIntMethodInfo(_methods.ArrayIntAddition);
            var config = CompilationConfiguration.NvvmConfiguration;
            config.KernelName = kernelName;
            var crossCompiler = new KernelCrossCompiler(config);
            var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

            var expected = _representationLoader.GetArrayIntAdditionTestResult(kernelName);
            var actual = llvmKernel.KernelBuffer;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestArrayFloatAddition()
        {
            const string kernelName = "ArrayIntAddition";

            var method = _methodInfo.GetArrayFloatMethodInfo(_methods.ArrayFloatAddition);
            var config = CompilationConfiguration.NvvmConfiguration;
            config.KernelName = kernelName;
            var crossCompiler = new KernelCrossCompiler(config);
            var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

            var expected = _representationLoader.GetArrayFloatAdditionTestResult(kernelName);
            var actual = llvmKernel.KernelBuffer;

            Assert.Equal(expected, actual);
        }
    }
}
