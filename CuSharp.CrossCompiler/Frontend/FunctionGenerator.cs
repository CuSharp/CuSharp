using LLVMSharp;

namespace CuSharp.CudaCompiler.Frontend
{
    public class FunctionGenerator
    {
        private readonly LLVMModuleRef _module;
        private readonly LLVMBuilderRef _builder;

        public List<(MSILKernel kernelToCall, LLVMValueRef function)> FunctionsToBuild { get; set; } = new();

        public FunctionGenerator(LLVMModuleRef module, LLVMBuilderRef builder)
        {
            _module = module;
            _builder = builder;
        }

        public LLVMValueRef GenerateFunctionAndPositionBuilderAtEntry(MSILKernel inputKernel)
        {
            var paramsListBuilder = new List<LLVMTypeRef>();
            foreach (var paramInfo in inputKernel.ParameterInfos)
            {
                LLVMTypeRef type;
                if (!inputKernel.IsMainFunction && !paramInfo.ParameterType.IsArray)
                {
                    type = paramInfo.ParameterType.ToLLVMType();
                }
                else if (paramInfo.ParameterType.IsArray)
                {
                    type = LLVMTypeRef.PointerType(paramInfo.ParameterType.GetElementType().ToLLVMType(), 0);
                }
                else
                {
                    type = LLVMTypeRef.PointerType(paramInfo.ParameterType.ToLLVMType(), 0);
                }
                paramsListBuilder.Add(type);
            }

            if (paramsListBuilder.Any() && inputKernel.IsMainFunction ||
                inputKernel.ParameterInfos.Any(p => p.ParameterType.IsArray))
            {
                paramsListBuilder.Add(LLVMTypeRef.PointerType(LLVMTypeRef.Int32Type(), 0)); // Array length list
            }

            var paramTypes = paramsListBuilder.ToArray();

            // TODO: If return type is array: inputKernel.ReturnType.ToLLVMType(ElementCount)
            var function = LLVM.AddFunction(_module, inputKernel.Name, LLVM.FunctionType(inputKernel.ReturnType.ToLLVMType(), paramTypes, false));
            LLVM.SetLinkage(function, LLVMLinkage.LLVMExternalLinkage);
            NameFunctionParameters(function, "param");

            if (inputKernel.IsMainFunction)
            {
                AppendFunction(function);
            }
            else
            {
                FunctionsToBuild.Add((inputKernel, function));
            }

            return function;
        }

        public void AppendFunction(LLVMValueRef function)
        {
            var entryBlock = LLVM.AppendBasicBlock(function, "entry");
            LLVM.PositionBuilderAtEnd(_builder, entryBlock);
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