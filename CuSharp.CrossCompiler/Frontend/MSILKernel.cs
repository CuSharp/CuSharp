using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;

namespace CuSharp.MSILtoLLVMCompiler
{
    public class MSILKernel : Kernel<byte[]>
    {
        public MSILKernel(string name, byte[] kernelBuffer, ParameterInfo[] parameterInfos)
        {
            Name = name;
            KernelBuffer = kernelBuffer;
            ParameterInfos = parameterInfos;
        }
        public string Name { get; }
        public byte[] KernelBuffer { get; }
        
        public ParameterInfo[] ParameterInfos { get; set; }
    }
}