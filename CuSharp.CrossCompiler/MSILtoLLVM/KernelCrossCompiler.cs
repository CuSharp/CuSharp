using System;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using LLVMSharp;

namespace CuSharp.MSILtoLLVMCompiler
{
    public class KernelCrossCompiler
    {
        private CompilationConfiguration _config;
        private LLVMModuleRef _module;
        private LLVMBuilderRef _builder;
        public KernelCrossCompiler(MSILKernel kernel, CompilationConfiguration configuration)
        {
            _config = configuration;
            _module = LLVM.ModuleCreateWithName(_config.KernelName + "MODULE");
            _builder = LLVM.CreateBuilder();

        }


        public LLVMKernel Compile(MSILKernel inputKernel)
        {
            GenerateDataLayoutAndTarget();

            new MethodBodyCompiler(inputKernel.KernelBuffer, _module, _builder).CompileMethodBody();
            
            var ptr = LLVM.PrintModuleToString(_module);
            var unmanagedString = LLVM.PrintModuleToString(_module) ;
            var kernelString = Marshal.PtrToStringAuto(unmanagedString);
            return new LLVMKernel(inputKernel.Name, kernelString);
        }



        private void GenerateDataLayoutAndTarget()
        {
            if (_config.DataLayout != "")
            {
                LLVM.SetDataLayout(_module, _config.DataLayout);
            }
            if (_config.Target != "")
            { 
                LLVM.SetTarget(_module, _config.DataLayout);
            }
        }
    }
}