using LibNVVMBinder;
using Xunit;

namespace CuSharp.Tests
{
    public class LibNVVMBinderTests
    {
        private NVVMProgram program = new ();
        private string programText = ReadFile("resources/kernel.ll");

        private static string ReadFile(string path)
        {
            return System.IO.File.ReadAllText(path);
        }
    
        [Fact]
        public void TestAddModuleToProgram()
        {
            NVVMProgram.NVVMResult result = program.AddModule(programText, "kernel");
            Assert.Equal( NVVMProgram.NVVMResult.NVVM_SUCCESS, result);
        }

        [Fact]
        public void TestCompileWithoutModuleFails()
        {
            NVVMProgram.NVVMResult result = program.Compile(new string[0]);
            Assert.Equal(NVVMProgram.NVVMResult.NVVM_ERROR_NO_MODULE_IN_PROGRAM, result );
        }
    
        [Fact]
        public void TestLazyAddModuleToProgram2()
        {
            NVVMProgram.NVVMResult result = program.LazyAddModule(programText, "kernel");
            Assert.Equal(NVVMProgram.NVVMResult.NVVM_SUCCESS, result);
        }
        [Fact]
        public void TestVerify()
        {
            program.AddModule(programText,"kernel");
            NVVMProgram.NVVMResult result = program.Verify(new string[0]);
            Assert.Equal(NVVMProgram.NVVMResult.NVVM_SUCCESS, result);
        }
        [Fact]
        public void TestVerifyFails()
        {
            program.AddModule("asdf","kernel");
            NVVMProgram.NVVMResult result = program.Verify(new string[0]);
            Assert.Equal(NVVMProgram.NVVMResult.NVVM_ERROR_INVALID_IR, result);
        }   
        [Fact]
        public void TestCompilationRuns()
        {
            program.AddModule(programText,"kernel");
            NVVMProgram.NVVMResult result = program.Compile(new string[0]);
            Assert.Equal(NVVMProgram.NVVMResult.NVVM_SUCCESS, result);
        }

        [Fact]
        public void TestCompilationFails()
        {
            program.AddModule("asdf","kernel");
            NVVMProgram.NVVMResult result = program.Compile(new string[0]);
            Assert.Equal(NVVMProgram.NVVMResult.NVVM_ERROR_COMPILATION, result);
        }

        [Fact]
        public void TestCompilationOutput()
        {
            program.AddModule(programText, "kernel");
            program.Compile(new string[0]);
            string result;
            program.GetCompiledResult(out result);
            Assert.Equal(ReadFile("resources/kernel.ptx"), result);
        }

        [Fact]
        public void TestErrorLogsAfterFail()
        {
            program.AddModule("asdf", "kernel");
            program.Compile(new string[0]);
            string log;
            program.GetProgramLog(out log);
            Assert.NotEmpty(log);
        }
        [Fact]
        public void TestErrorLogsAfterSuccess()
        {
            program.AddModule(programText, "kernel");
            program.Compile(new string[0]);
            string log;
            program.GetProgramLog(out log);
            Assert.Empty(log);
        }
    }
}