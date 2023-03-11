using System.Collections.Generic;

namespace CuSharp.CudaCompiler
{
    public interface Kernel<out T>
    {
        public string Name { get; }
        public T KernelBuffer { get; }
    }
}