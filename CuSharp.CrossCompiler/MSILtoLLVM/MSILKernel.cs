using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;

namespace CuSharp.MSILtoLLVMCompiler
{
    public class MSILKernel : Kernel<byte[]>
    {
        public MSILKernel(string name, byte[] kernelBuffer)
        {
            Name = name;
            KernelBuffer = kernelBuffer;
        }
        public string Name { get; }
        public byte[] KernelBuffer { get; }
    }
}