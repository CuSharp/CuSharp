using CuSharp.Tests.TestHelper;
using LibNVVMBinder;
using System.Linq;
using Xunit;

namespace CuSharp.Tests.CuSharp.NVVMBinder
{
    [Collection("Sequential")]
    [Trait(TestCategories.TestCategory, TestCategories.Unit)]
    public class LibNVVMBinderTests
    {
        private readonly NVVMProgram _program = new ();
        private readonly string _programText = ReadFile("resources/kernel.ll");

        private static string ReadFile(string path)
        {
            return System.IO.File.ReadAllText(path);
        }
    
        [Fact]
        public void TestAddModuleToProgram()
        {
            NVVMProgram.NVVMResult result = _program.AddModule(_programText, "kernel");
            Assert.Equal( NVVMProgram.NVVMResult.NVVM_SUCCESS, result);
        }

        [Fact]
        public void TestCompileWithoutModuleFails()
        {
            NVVMProgram.NVVMResult result = _program.Compile(new string[0]);
            Assert.Equal(NVVMProgram.NVVMResult.NVVM_ERROR_NO_MODULE_IN_PROGRAM, result );
        }
    
        [Fact]
        public void TestLazyAddModuleToProgram2()
        {
            NVVMProgram.NVVMResult result = _program.LazyAddModule(_programText, "kernel");
            Assert.Equal(NVVMProgram.NVVMResult.NVVM_SUCCESS, result);
        }

        [Fact]
        public void TestVerify()
        {
            _program.AddModule(_programText,"kernel");
            NVVMProgram.NVVMResult result = _program.Verify(new string[0]);
            Assert.Equal(NVVMProgram.NVVMResult.NVVM_SUCCESS, result);
        }

        [Fact]
        public void TestVerifyFails()
        {
            _program.AddModule("asdf","kernel");
            NVVMProgram.NVVMResult result = _program.Verify(new string[0]);
            Assert.Equal(NVVMProgram.NVVMResult.NVVM_ERROR_INVALID_IR, result);
        }

        [Fact]
        public void TestCompilationRuns()
        {
            _program.AddModule(_programText,"kernel");
            NVVMProgram.NVVMResult result = _program.Compile(new string[0]);
            Assert.Equal(NVVMProgram.NVVMResult.NVVM_SUCCESS, result);
        }

        [Fact]
        public void TestCompilationFails()
        {
            _program.AddModule("asdf","kernel");
            NVVMProgram.NVVMResult result = _program.Compile(new string[0]);
            Assert.Equal(NVVMProgram.NVVMResult.NVVM_ERROR_COMPILATION, result);
        }

        [Fact]
        public void TestCompilationOutput()
        {
            _program.AddModule(_programText, "kernel");
            _program.Compile(new string[0]);
            string result;
            _program.GetCompiledResult(out result);
            result = RemoveComments(result);

            var expected = ReadFile("resources/kernel.ptx").Replace("\r", "");
            expected = RemoveComments(expected);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void TestErrorLogsAfterFail()
        {
            _program.AddModule("asdf", "kernel");
            _program.Compile(new string[0]);
            string log;
            _program.GetProgramLog(out log);
            Assert.NotEmpty(log);
        }

        [Fact]
        public void TestErrorLogsAfterSuccess()
        {
            _program.AddModule(_programText, "kernel");
            _program.Compile(new string[0]);
            string log;
            _program.GetProgramLog(out log);
            Assert.Empty(log);
        }

        [Fact]
        public void TestDisposeDoesNotThrow()
        {
            var exception = Record.Exception(() => _program.Dispose());
            Assert.Null(exception);
        }

        private string RemoveComments(string input)
        {
            string[] actualLines = input.Split('\n');
            actualLines = actualLines.Where(line => !line.TrimStart().StartsWith("//")).ToArray();
            return string.Join("\n", actualLines);
        }
    }
}