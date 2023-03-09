using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace CuSharp.MSILtoLLVMCompiler
{
    public class MSILKernel : Kernel<IEnumerable<System.Reflection.Emit.OpCode>>
    {
        public string Name { get; }
        public IEnumerable<OpCode> KernelBuffer { get; }
    }
}