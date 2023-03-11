using System.Collections.Generic;

namespace CuSharp.CudaCompiler.Backend;
public class PTXKernel : Kernel<byte[]>
{
    public PTXKernel(string name, byte[] kernelBuffer)
    {
        Name = name;
        KernelBuffer = kernelBuffer;
    }
    public string Name { get; }
    public byte[] KernelBuffer { get; }
}