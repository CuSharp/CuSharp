namespace CuSharp.MSILtoLLVMCompiler
{
    public class LLVMKernel : Kernel<string>
    {
        public string Name { get; }
        public string KernelBuffer { get; }
    }
}