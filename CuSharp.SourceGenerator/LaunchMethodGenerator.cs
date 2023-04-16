using System.Text;
using Microsoft.CodeAnalysis;
using System.Reflection; //needed for generated source-code!

namespace CuSharp.CodeGenerator
{
    [Generator]
    public class LaunchMethodGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var methodStrings = new StringBuilder();
            for (int i = 1; i <= 10; i++)
            {
                methodStrings.Append(GenerateMethodString(i));
                methodStrings.Append("\n");
                /*methodStrings.Append(GenerateAsyncMethodString(i));
                methodStrings.Append("\n");*/
            }

            var source = $@"
using ManagedCuda;
using ManagedCuda.VectorTypes;
using System.Reflection;

namespace CuSharp;
public partial class CuDevice
{{
    {methodStrings}
}}
";
            context.AddSource("CuDevice.g.cs", source);
        }

        private string GenerateAsyncMethodString(int amount)
        {

            return $@"
    public Task LaunchAsync<{GenericParameterPack(amount)}>(Action<{GenericParameterPack(amount)}> method, (uint, uint, uint) gridSize, (uint, uint, uint) blockSize, {ParameterString(amount)}) 
        {GenerateConstraintsString(amount)}
    {{
        return Task.Run(() => {{

        var cudaKernel = CompileAndGetKernel(method.GetMethodInfo(), gridSize, blockSize);
        cudaKernel.Run(new Object[] {{ {ArgumentPack(amount)} }});
        }});
    }}
";
        }

        private string GenerateMethodString(int amount)
        {
            return $@"
    public void Launch<{GenericParameterPack(amount)}>(Action<{GenericParameterPack(amount)}> method, (uint, uint, uint) gridSize, (uint, uint, uint) blockSize, {ParameterString(amount)}) 
    {{
        var cudaKernel = CompileAndGetKernel(method.GetMethodInfo(), gridSize, blockSize);
        var lengths = new [] {{{LengthPack(amount)}}};
        CudaDeviceVariable<int> devLengths = lengths;
        cudaKernel.Run({ArgumentPack(amount)}, devLengths.DevicePointer);
    }}
            ";
        }

        private string ArgumentPack(int amount)
        {
            return GenerateList(amount, "((CudaTensor<T{0}>)param{0}).DevicePointer", ',');
        }

        private string LengthPack(int amount)
        {
            return GenerateList(amount, "((CudaTensor<T{0}>)param{0}).Length", ',');
        }
        private string GenericParameterPack(int amount)
        {
            return GenerateList(amount, "T{0}", ',');
        }
        
        private string ParameterString(int amount)
        {
            return GenerateList(amount, "Tensor<T{0}> param{0}", ',');
        }

        private string GenerateConstraintsString(int amount)
        {
            return GenerateList(amount, "where T{0} : struct", ' ');
        }

        private string GenerateList(int amount, string formatString, char delimiter)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < amount; i++)
            {
                builder.AppendFormat(formatString, i);
                if (i < amount - 1)
                {
                    builder.Append(delimiter);
                }
            }

            return builder.ToString();
        }
    }
}