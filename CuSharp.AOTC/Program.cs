// See https://aka.ms/new-console-template for more information

using System.Reflection;
using CuSharp.CudaCompiler;
using CuSharp.CudaCompiler.Backend;
using CuSharp.CudaCompiler.Frontend;
using CuSharp.CudaCompiler.Kernels;
using CuSharp.Kernel;

public class AOTC
{ 
    static void Main(string[] args)
    {
        if ( args.Length != 2)
        {
            throw new ArgumentException(
                "Invalid amount of arguments: Arguments should be:\n<path to containing DLL> <output folder>");
        }

        string dllPath = args[0];
        string outputPath = args[1];

        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }
        
        CompileAll(dllPath, outputPath);
    }

    private static void CompileAll(string assemblyPath, string outPath)
    {
        
        var assembly = Assembly.LoadFile(assemblyPath);
        KernelDiscovery discovery = new();
        CompilationDispatcher compiler = new();
        discovery.ScanAssembly(assembly);
        var methods = discovery.GetAllMethods();
        foreach (var method in methods)
        {
            Console.Write($"Compiling\t{KernelHelpers.GetMethodIdentity(method)}:\t");

            var nnvmConfiguration = CompilationConfiguration.NvvmConfiguration;
            var attributes = method.GetCustomAttributes(typeof(KernelAttribute)).ToList();

            if (attributes is { Count: 1 })
                nnvmConfiguration.DefaultArrayMemoryLocation = ((KernelAttribute)attributes[0]).ArrayMemoryLocation;

            var ptxKernel = compiler.Compile(method.Name, method);
            WriteFile(outPath, method, ptxKernel.KernelBuffer);
            Console.WriteLine("Done");
        }
    }
        
    static void WriteFile(string outPath, MethodInfo method, byte[] kernelBuffer)
    {
        File.WriteAllBytes($"{outPath}{Path.DirectorySeparatorChar}{KernelHelpers.GetMethodIdentity(method)}ptx", kernelBuffer);
    }
}
