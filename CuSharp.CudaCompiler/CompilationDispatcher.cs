using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using CuSharp.MSILtoLLVMCompiler;
using LibNVVMBinder;

namespace CuSharp.CudaCompiler
{
    public class CompilationDispatcher
    {
        private Dictionary<string, PTXKernel> kernelCache = new Dictionary<string, PTXKernel>();

        public PTXKernel Compile(string kernelName, MethodInfo methodInfo)
        {
            if (kernelCache.TryGetValue(kernelName, out PTXKernel ptxKernel)) return ptxKernel;
            
            MSILKernel kernel = new MSILKernel(); /*CALL Disassembler to get MSILKernel*/
            var msilToLlvmCrosscompiler = new Crosscompiler();
            var llvmKernel = msilToLlvmCrosscompiler.Compile(kernel);
            return CompileLlvmToPTX(llvmKernel);
        }

        private PTXKernel CompileLlvmToPTX(LLVMKernel llvmKernel)
        {
            var nvvmHandle = new NVVMProgram();
            nvvmHandle.AddModule(llvmKernel.KernelBuffer, llvmKernel.Name);
            var compilationResult = nvvmHandle.Compile(new string[0]);
            if (compilationResult != NVVMProgram.NVVMResult.NVVM_SUCCESS)
            {
                nvvmHandle.GetProgramLog(out string log);
                /*throw with program log*/
            }

            nvvmHandle.GetCompiledResult(out string ptx);
            return new PTXKernel(llvmKernel.Name, Encoding.ASCII.GetBytes(ptx));
        }
    }
}