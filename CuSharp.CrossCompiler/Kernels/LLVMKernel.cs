namespace CuSharp.CudaCompiler.Kernels
{
    public class LLVMKernel : IKernel<string>
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