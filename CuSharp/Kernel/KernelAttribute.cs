using CuSharp.CudaCompiler.Frontend;

namespace CuSharp.Kernel;

[System.AttributeUsage(System.AttributeTargets.Method)]
public class KernelAttribute : System.Attribute
{
    public ArrayMemoryLocation ArrayMemoryLocation { get; }

    public KernelAttribute()
    {
        ArrayMemoryLocation = ArrayMemoryLocation.NVVM_GLOBAL;
    }

    public KernelAttribute(ArrayMemoryLocation arrayMemoryLocation)
    {
        ArrayMemoryLocation = arrayMemoryLocation;
    }
}