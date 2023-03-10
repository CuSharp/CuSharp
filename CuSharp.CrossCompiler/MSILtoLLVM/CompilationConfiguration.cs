using System.Reflection;
using System.Runtime.CompilerServices;
using LLVMSharp;
using Microsoft.VisualBasic.CompilerServices;

namespace CuSharp.MSILtoLLVMCompiler;

public class CompilationConfiguration
{
    public string DataLayout { get; set; } = "";
    public string Target { get; set; } = "";
    public string KernelName { get; set; }
    public Type[] ParameterTypes { get; set; }
    public Dictionary<string, (Type, string)> IntrinsicFunctions { get; set; }
    
}