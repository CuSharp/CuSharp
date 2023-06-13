using System.Reflection;

namespace CuSharp.CudaCompiler.Frontend
{
    public class MSILKernel : Kernel<byte[]>
    {
        public MSILKernel(string name, MethodInfo methodInfo)
        {
            if (!methodInfo.IsStatic) throw new NotSupportedException("Method to compile is not static. Only static methods are supported");

            Name = name;
            MethodInfo = methodInfo;
            KernelBuffer = methodInfo.GetMethodBody().GetILAsByteArray();
            ReturnType = methodInfo.ReturnType;
            ParameterInfos = methodInfo.GetParameters();
            LocalVariables = methodInfo.GetMethodBody().LocalVariables;
            MemberInfoModule = methodInfo.Module;
        }

        public string Name { get; }
        public MethodInfo MethodInfo { get; }
        public byte[] KernelBuffer { get; }
        public Type ReturnType { get; set; }
        public ParameterInfo[] ParameterInfos { get; set; }
        public Module MemberInfoModule { get; set; }
        public IList<LocalVariableInfo> LocalVariables { get; }
    }
}