﻿namespace CuSharp.MSILtoLLVMCompiler
{
    public class LLVMKernel : Kernel<string>
    {
        public LLVMKernel(string name, string kernelBuffer)
        {
            Name = name;
            KernelBuffer = kernelBuffer;
        }
        public string Name { get; }
        public string KernelBuffer { get; }
    }
}