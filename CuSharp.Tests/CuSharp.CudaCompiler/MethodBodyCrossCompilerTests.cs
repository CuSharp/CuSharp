using CuSharp.CudaCompiler.Frontend;
using CuSharp.Tests.TestHelper;
using Xunit;

namespace CuSharp.Tests.CuSharp.CudaCompiler
{
    public class MethodBodyCrossCompilerTests
    {
        private readonly MethodInfoLoader _methodLoader = new();
        private readonly LLVMRepresentationLoader _llvmLoader = new();
        private readonly TestValidator _validator = new();

        [Fact]
        public void TestScalarIntAdditionWithConst()
        {
            const string kernelName = "ScalarIntAdditionWithConst";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.ScalarIntAdditionWithConst);
            var config = CompilationConfiguration.NvvmConfiguration;
            config.KernelName = kernelName;
            var crossCompiler = new KernelCrossCompiler(config);
            var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

            var expected = _llvmLoader.GetScalarIntAdditionWithConstTestResult(kernelName);
            var actual = llvmKernel.KernelBuffer;

            Assert.Equal(expected, actual);
            Assert.True(_validator.KernelIsCorrect(actual, kernelName));
        }

        [Fact]
        public void TestScalarLongAdditionWithConst()
        {
            const string kernelName = "ScalarLongAdditionWithConst";
            var method = _methodLoader.GetScalarLongMethodInfo(MethodsToCompile.ScalarLongAdditionWithConst);
            var config = CompilationConfiguration.NvvmConfiguration;
            config.KernelName = kernelName;
            var crossCompiler = new KernelCrossCompiler(config);
            var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

            var expected = _llvmLoader.GetScalarLongAdditionWithConstTestResult(kernelName);
            var actual = llvmKernel.KernelBuffer;

            Assert.Equal(expected, actual);
            Assert.True(_validator.KernelIsCorrect(actual, kernelName));
        }

        [Fact]
        public void TestScalarIntAddition()
        {
            const string kernelName = "ScalarIntAddition";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.ScalarIntAddition);
            var config = CompilationConfiguration.NvvmConfiguration;
            config.KernelName = kernelName;
            var crossCompiler = new KernelCrossCompiler(config);
            var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

            var expected = _llvmLoader.GetScalarIntAdditionTestResult(kernelName);
            var actual = llvmKernel.KernelBuffer;

            Assert.Equal(expected, actual);
            Assert.True(_validator.KernelIsCorrect(actual, kernelName));
        }

        [Fact]
        public void TestScalarIntSubtraction()
        {
            const string kernelName = "ScalarIntSubtraction";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.ScalarIntSubtraction);
            var config = CompilationConfiguration.NvvmConfiguration;
            config.KernelName = kernelName;
            var crossCompiler = new KernelCrossCompiler(config);
            var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

            var expected = _llvmLoader.GetScalarIntSubtractionTestResult(kernelName);
            var actual = llvmKernel.KernelBuffer;

            Assert.Equal(expected, actual);
            Assert.True(_validator.KernelIsCorrect(actual, kernelName));
        }

        [Fact]
        public void TestScalarIntMultiplication()
        {
            const string kernelName = "ScalarIntMultiplication";
            var method = _methodLoader.GetScalarIntMethodInfo(MethodsToCompile.ScalarIntMultiplication);
            var config = CompilationConfiguration.NvvmConfiguration;
            config.KernelName = kernelName;
            var crossCompiler = new KernelCrossCompiler(config);
            var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

            var expected = _llvmLoader.GetScalarIntMultiplicationTestResult(kernelName);
            var actual = llvmKernel.KernelBuffer;

            Assert.Equal(expected, actual);
            Assert.True(_validator.KernelIsCorrect(actual, kernelName));
        }

        [Fact]
        public void TestScalarFloatAdditionWithConst()
        {
            const string kernelName = "ScalarFloatAdditionWithConst";
            var method = _methodLoader.GetScalarFloatMethodInfo(MethodsToCompile.ScalarFloatAdditionWithConst);
            var config = CompilationConfiguration.NvvmConfiguration;
            config.KernelName = kernelName;
            var crossCompiler = new KernelCrossCompiler(config);
            var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

            var expected = _llvmLoader.GetScalarFloatAdditionWithConstTestResult(kernelName);
            var actual = llvmKernel.KernelBuffer;

            Assert.Equal(expected, actual);
            Assert.True(_validator.KernelIsCorrect(actual, kernelName));
        }

        [Fact]
        public void TestScalarDoubleAdditionWithConst()
        {
            const string kernelName = "ScalarDoubleAdditionWithConst";
            var method = _methodLoader.GetScalarDoubleMethodInfo(MethodsToCompile.ScalarDoubleAdditionWithConst);
            var config = CompilationConfiguration.NvvmConfiguration;
            config.KernelName = kernelName;
            var crossCompiler = new KernelCrossCompiler(config);
            var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

            var expected = _llvmLoader.GetScalarDoubleAdditionWithConstTestResult(kernelName);
            var actual = llvmKernel.KernelBuffer;

            Assert.Equal(expected, actual);
            Assert.True(_validator.KernelIsCorrect(actual, kernelName));
        }

        [Fact]
        public void TestScalarFloatAddition()
        {
            const string kernelName = "ScalarFloatAddition";
            var method = _methodLoader.GetScalarFloatMethodInfo(MethodsToCompile.ScalarFloatAddition);
            var config = CompilationConfiguration.NvvmConfiguration;
            config.KernelName = kernelName;
            var crossCompiler = new KernelCrossCompiler(config);
            var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

            var expected = _llvmLoader.GetScalarFloatAdditionTestResult(kernelName);
            var actual = llvmKernel.KernelBuffer;

            Assert.Equal(expected, actual);
            Assert.True(_validator.KernelIsCorrect(actual, kernelName));
        }

        [Fact]
        public void TestScalarFloatSubtraction()
        {
            const string kernelName = "ScalarFloatSubtraction";
            var method = _methodLoader.GetScalarFloatMethodInfo(MethodsToCompile.ScalarFloatSubtraction);
            var config = CompilationConfiguration.NvvmConfiguration;
            config.KernelName = kernelName;
            var crossCompiler = new KernelCrossCompiler(config);
            var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

            var expected = _llvmLoader.GetScalarFloatSubtractionTestResult(kernelName);
            var actual = llvmKernel.KernelBuffer;

            Assert.True(_validator.KernelIsCorrect(actual, kernelName));
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestScalarFloatMultiplication()
        {
            const string kernelName = "ScalarFloatMultiplication";
            var method = _methodLoader.GetScalarFloatMethodInfo(MethodsToCompile.ScalarFloatMultiplication);
            var config = CompilationConfiguration.NvvmConfiguration;
            config.KernelName = kernelName;
            var crossCompiler = new KernelCrossCompiler(config);
            var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

            var expected = _llvmLoader.GetScalarFloatMultiplicationTestResult(kernelName);
            var actual = llvmKernel.KernelBuffer;

            Assert.Equal(expected, actual);
            Assert.True(_validator.KernelIsCorrect(actual, kernelName));
        }

        [Fact]
        public void TestArrayIntAddition()
        {
            const string kernelName = "ArrayIntAddition";
            var method = _methodLoader.GetArrayIntMethodInfo(MethodsToCompile.ArrayIntAddition);
            var config = CompilationConfiguration.NvvmConfiguration;
            config.KernelName = kernelName;
            var crossCompiler = new KernelCrossCompiler(config);
            var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

            var expected = _llvmLoader.GetArrayIntAdditionTestResult(kernelName);
            var actual = llvmKernel.KernelBuffer;

            Assert.Equal(expected, actual);
            Assert.True(_validator.KernelIsCorrect(actual, kernelName));
        }

        [Fact]
        public void TestArrayFloatAddition()
        {
            const string kernelName = "ArrayIntAddition";
            var method = _methodLoader.GetArrayFloatMethodInfo(MethodsToCompile.ArrayFloatAddition);
            var config = CompilationConfiguration.NvvmConfiguration;
            config.KernelName = kernelName;
            var crossCompiler = new KernelCrossCompiler(config);
            var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

            var expected = _llvmLoader.GetArrayFloatAdditionTestResult(kernelName);
            var actual = llvmKernel.KernelBuffer;

            Assert.Equal(expected, actual);
            Assert.True(_validator.KernelIsCorrect(actual, kernelName));
        }

        [Fact]
        public void TestArrayIntAdditionWithKernelTools()
        {
            const string kernelName = "ArrayIntAdditionWithKernelTools";
            var method = _methodLoader.GetArrayIntMethodInfo(MethodsToCompile.ArrayIntAdditionWithKernelTools);
            var config = CompilationConfiguration.NvvmConfiguration;
            config.KernelName = kernelName;
            var crossCompiler = new KernelCrossCompiler(config);
            var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

            var expected = _llvmLoader.GetArrayIntAdditionWithKernelToolsTestResult(kernelName);
            var actual = llvmKernel.KernelBuffer;

            Assert.Equal(expected, actual);
            Assert.True(_validator.KernelIsCorrect(actual, kernelName));
        }

        [Fact]
        public void TestArrayFloatAdditionWithKernelTools()
        {
            const string kernelName = "ArrayFloatAdditionWithKernelTools";
            var method = _methodLoader.GetArrayFloatMethodInfo(MethodsToCompile.ArrayFloatAdditionWithKernelTools);
            var config = CompilationConfiguration.NvvmConfiguration;
            config.KernelName = kernelName;
            var crossCompiler = new KernelCrossCompiler(config);
            var llvmKernel = crossCompiler.Compile(new MSILKernel(kernelName, method));

            var expected = _llvmLoader.GetArrayFloatAdditionWithKernelToolsTestResult(kernelName);
            var actual = llvmKernel.KernelBuffer;

            Assert.Equal(expected, actual);
            Assert.True(_validator.KernelIsCorrect(actual, kernelName));
        }
    }
}
