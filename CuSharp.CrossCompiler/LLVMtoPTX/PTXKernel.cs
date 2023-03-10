using System.Collections.Generic;
using CuSharp.MSILtoLLVMCompiler;

namespace CuSharp.CudaCompiler;
public class PTXKernel : Kernel<byte[]>
{
    public PTXKernel(string name, byte[] kernelBuffer)
    {
        this.Name = name;
        this.KernelBuffer = kernelBuffer;
    }
    public string Name { get; }
    public byte[] KernelBuffer { get; }
}