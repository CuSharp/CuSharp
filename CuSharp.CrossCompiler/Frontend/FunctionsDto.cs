using LLVMSharp;

namespace CuSharp.CudaCompiler.Frontend
{
    public class FunctionsDto
    {
        public LLVMValueRef Function { get; set; }
        public (string, LLVMValueRef)[] ExternalFunctions { get; set; }
        
        public int ArrayParameterCount { get; }

        public FunctionsDto(LLVMValueRef function, (string, LLVMValueRef)[] externalFunctions, int amountOfArrayParams)
        {
            Function = function;
            ExternalFunctions = externalFunctions;
            ArrayParameterCount = amountOfArrayParams;
        }
    }
}
