namespace CuSharp.CudaCompiler.Kernels
{
    public interface IKernel<out T>
    {
        public string Name { get; }
        public T KernelBuffer { get; }
    }
}