using CuSharp.CudaCompiler.LLVMConfiguration;

namespace CuSharp.Kernel;

[AttributeUsage(AttributeTargets.Method)]
public class KernelAttribute : Attribute
{
    /// <summary>
    /// Memory Location of Arrays Allocated with 'new ...' inside of a Kernel
    /// </summary>
    public ArrayMemoryLocation ArrayMemoryLocation { get; }

    public KernelAttribute()
    {
        ArrayMemoryLocation = ArrayMemoryLocation.GLOBAL;
    }

    public KernelAttribute(ArrayMemoryLocation arrayMemoryLocation)
    {
        ArrayMemoryLocation = arrayMemoryLocation;
    }
}