using CuSharp.CudaCompiler.Frontend;

namespace CuSharp.Kernel;

[System.AttributeUsage(System.AttributeTargets.Method)]
public class KernelAttribute : System.Attribute
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