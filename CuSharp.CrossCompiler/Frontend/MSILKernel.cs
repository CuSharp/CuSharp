using System.Reflection;

namespace CuSharp.CudaCompiler.Frontend
{
    public class MSILKernel : Kernel<byte[]>
    {
        public MSILKernel(string name, MethodInfo methodInfo)
        {
            Name = name;
            KernelBuffer = methodInfo.GetMethodBody().GetILAsByteArray();
            ParameterInfos = methodInfo.GetParameters();
            MemberInfoModule = methodInfo.Module;
        }
        public string Name { get; }
        public byte[] KernelBuffer { get; }

        public ParameterInfo[] ParameterInfos { get; set; }
        public Module MemberInfoModule { get; set; }
    }
}