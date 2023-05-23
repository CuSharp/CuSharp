﻿using LLVMSharp;

namespace CuSharp.CudaCompiler.Frontend
{
    public class FunctionsDto
    {
        public LLVMValueRef Function { get; set; }
        public (string, LLVMValueRef, LLVMValueRef[])[] ExternalFunctions { get; set; }
        

        public FunctionsDto(LLVMValueRef function, (string, LLVMValueRef, LLVMValueRef[])[] externalFunctions)
        {
            Function = function;
            ExternalFunctions = externalFunctions;
        }
    }
}