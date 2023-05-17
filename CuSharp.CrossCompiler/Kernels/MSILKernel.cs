using System.Reflection;
using System.Runtime.InteropServices.JavaScript;

namespace CuSharp.CudaCompiler.Frontend
{
    public class MSILKernel : Kernel<byte[]>
    {
        public MSILKernel(string name, MethodInfo methodInfo, bool isMainFunction = true) // TODO: Remove default value
        {
            if (!methodInfo.IsStatic) throw new NotSupportedException("Method to compile is not static. Only static methods are supported");

            Name = name;
            MethodInfo = methodInfo;
            IsMainFunction = isMainFunction;
            KernelBuffer = methodInfo.GetMethodBody().GetILAsByteArray();
            ReturnType = methodInfo.ReturnType;
            ParameterInfos = methodInfo.GetParameters();
            LocalVariables = methodInfo.GetMethodBody().LocalVariables;
            MemberInfoModule = methodInfo.Module;
        }

        public string Name { get; }
        public MethodInfo MethodInfo { get; }
        public bool IsMainFunction { get; }
        public byte[] KernelBuffer { get; }
        public Type ReturnType { get; set; }
        public ParameterInfo[] ParameterInfos { get; set; }
        public Module MemberInfoModule { get; set; }
        public IList<LocalVariableInfo> LocalVariables { get; }
    }
}