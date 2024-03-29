﻿using System.Text;

namespace LibNVVMBinder
{
    public class NVVMProgram : IDisposable
    {
        private IntPtr _program;
        
        public enum NVVMResult //Explicit numbering because of driver mapping (must be correct)
        {
            NVVM_SUCCESS = 0,
            NVVM_ERROR_OUT_OF_MEMORY = 1,
            NVVM_ERROR_PROGRAM_CREATION_FAILURE = 2,
            NVVM_ERROR_IR_VERSION_MISMATCH = 3,
            NVVM_ERROR_INVALID_INPUT = 4,
            NVVM_ERROR_INVALID_PROGRAM = 5,
            NVVM_ERROR_INVALID_IR = 6,
            NVVM_ERROR_INVALID_OPTION = 7,
            NVVM_ERROR_NO_MODULE_IN_PROGRAM = 8,
            NVVM_ERROR_COMPILATION = 9
        }

        public NVVMProgram()
        {
            CreateProgram();
        }

        public NVVMResult Compile(string[] options)
        {
            return NVVMNativeBindings.nvvmCompileProgram(_program, new UIntPtr( (uint) options.Length), options);
        }

        public NVVMResult Verify(string[] options)
        {
            return NVVMNativeBindings.nvvmVerifyProgram(_program, options.Length, options);
        }

        public NVVMResult AddModule(string buffer, string name)
        {
            return NVVMNativeBindings.nvvmAddModuleToProgram(_program, buffer, new UIntPtr((uint) buffer.Length), name);
        }

        public NVVMResult LazyAddModule(string buffer, string name)
        {
            return NVVMNativeBindings.nvvmLazyAddModuleToProgram(_program, buffer, new UIntPtr((uint) buffer.Length), name);
        }

        private NVVMResult CreateProgram()
        {
            return NVVMNativeBindings.nvvmCreateProgram(out _program);
        }

        private NVVMResult DestroyProgram()
        {
            return NVVMNativeBindings.nvvmDestroyProgram(_program);
        }

        public NVVMResult GetCompiledResult(out string buffer)
        {
            IntPtr resultSize;
            NVVMNativeBindings.nvvmGetCompiledResultSize(_program, out resultSize);
            NVVMResult result;
            StringBuilder outBuilder = new(resultSize.ToInt32());
            result = NVVMNativeBindings.nvvmGetCompiledResult(_program, outBuilder);
            buffer = outBuilder.ToString();
            return result;
        }

        public NVVMResult GetProgramLog(out string log)
        {
            NVVMResult result;
            IntPtr logSize;
            NVVMNativeBindings.nvvmGetProgramLogSize(_program, out logSize);
            StringBuilder outBuilder = new(logSize.ToInt32());
            result = NVVMNativeBindings.nvvmGetProgramLog(_program, outBuilder);
            log = outBuilder.ToString();
            return result;
        }

        public void Dispose()
        {
            DestroyProgram();
        }
    }
}