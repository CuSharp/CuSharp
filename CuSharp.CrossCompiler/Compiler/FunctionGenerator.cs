using System.Reflection;
using CuSharp.CudaCompiler.Kernels;
using LLVMSharp;

namespace CuSharp.CudaCompiler.Frontend
{
    public class FunctionGenerator
    {
        private readonly LLVMModuleRef _module;
        private readonly LLVMBuilderRef _builder;

        private List<(MSILKernel msilFunction, LLVMValueRef llvmFunction)> FunctionsToBuild { get; } = new(); 
        
        public FunctionGenerator(LLVMModuleRef module, LLVMBuilderRef builder)
        {
            _module = module;
            _builder = builder;
        }

        private readonly Dictionary<string, LLVMValueRef> _functionCache = new();
        
        public LLVMValueRef GetOrDeclareFunction(MSILKernel msilFunction)
        {
            var kernelIdentity = KernelHelpers.GetMethodIdentity(msilFunction.MethodInfo);
            if (_functionCache.ContainsKey(kernelIdentity))
            {
                return _functionCache[kernelIdentity];
            }
            
            var paramTypes = GenerateLLVMParameters(msilFunction.ParameterInfos);
            var function = LLVM.AddFunction(_module, msilFunction.Name, LLVM.FunctionType(msilFunction.ReturnType.ToLLVMType(), paramTypes, false));
            LLVM.SetLinkage(function, LLVMLinkage.LLVMExternalLinkage);
            NameFunctionParameters(function, "param");
            _functionCache.Add(kernelIdentity, function);
            FunctionsToBuild.Add((msilFunction, function));
            return function;
        }

        public IEnumerable<(MSILKernel msilFunction, LLVMValueRef llvmFunction)> AllFunctionsToCompile()
        {
            while (FunctionsToBuild.Any())
            {
                var nextFunction = FunctionsToBuild.First();
                FunctionsToBuild.Remove(nextFunction);
                yield return nextFunction;
            }
        }
        
        private static LLVMTypeRef[] GenerateLLVMParameters(ParameterInfo[] msilParameters)
        {
            
            var paramsListBuilder = new List<LLVMTypeRef>();
            foreach (var paramInfo in msilParameters)
            {
                LLVMTypeRef type;
                if (paramInfo.ParameterType.IsSZArray)
                {
                    type = LLVMTypeRef.PointerType(paramInfo.ParameterType.GetElementType().ToLLVMType(), 0);
                }
                else if (paramInfo.ParameterType.IsArray) //MultiDimArray [,]
                {
                    type = LLVMTypeRef.PointerType(
                        LLVMTypeRef.PointerType(paramInfo.ParameterType.GetElementType().ToLLVMType(),0)
                        ,0);
                }
                else 
                {
                    type = paramInfo.ParameterType.ToLLVMType();
                }

                paramsListBuilder.Add(type);
            }

            return paramsListBuilder.ToArray();
        }
        private void NameFunctionParameters(LLVMValueRef function, string prefix)
        {
            var parameters = LLVM.GetParams(function);

            var counter = 0;
            foreach (var param in parameters)
            {
                LLVM.SetValueName(param, $"{prefix}{counter}");
                counter++;
            }
        }
    }
}