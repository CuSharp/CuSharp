namespace CuSharp.CudaCompiler.Kernels;
public class PTXKernel : IKernel<byte[]>
{
    public PTXKernel(string name, byte[] kernelBuffer)
    {
        Name = name;
        KernelBuffer = kernelBuffer;
    }
    public string Name { get; }
    public byte[] KernelBuffer { get; }
}