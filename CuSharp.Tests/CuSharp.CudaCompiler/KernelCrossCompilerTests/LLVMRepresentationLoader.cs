namespace CuSharp.Tests.CuSharp.CudaCompiler.KernelCrossCompilerTests
{
    public class LLVMRepresentationLoader
    {
        private const string Int32Type = "i32";
        private const string FloatType = "float";
        private const string Int32ArrType = "i32*";
        private const string FloatArrType = "float*";

        public string GetScalarIntAdditionTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName,
            GetTwoParams(Int32Type), GetTwoParamTypes(Int32Type), GetScalarAdditionMethodBody(Int32Type));

        public string GetScalarIntSubtractionTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName,
            GetTwoParams(Int32Type), GetTwoParamTypes(Int32Type), GetScalarSubtractionMethodBody(Int32Type));

        public string GetScalarIntMultiplicationTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName,
            GetTwoParams(Int32Type), GetTwoParamTypes(Int32Type), GetScalarMultiplicationMethodBody(Int32Type));

        public string GetScalarFloatAdditionTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName,
            GetTwoParams(FloatType), GetTwoParamTypes(FloatType), GetScalarAdditionMethodBody(FloatType));

        public string GetScalarFloatSubtractionTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName,
            GetTwoParams(FloatType), GetTwoParamTypes(FloatType), GetScalarSubtractionMethodBody(FloatType));

        public string GetScalarFloatMultiplicationTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName,
            GetTwoParams(FloatType), GetTwoParamTypes(FloatType), GetScalarMultiplicationMethodBody(FloatType));

        public string GetArrayIntAdditionTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName,
            GetThreeParams(Int32ArrType), GetThreeParamTypes(Int32ArrType), GetArrayAdditionMethodBody(Int32Type, Int32ArrType));

        public string GetArrayFloatAdditionTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName,
            GetThreeParams(FloatArrType), GetThreeParamTypes(FloatArrType), GetArrayAdditionMethodBody(FloatType, FloatArrType));

        private string GetExpectedLLVMRepresentation(string kernelName, string parameters, string paramTypes, string methodBody)
        {
            return $"; ModuleID = '{kernelName}MODULE'\n"
                 + $"source_filename = \"{kernelName}MODULE\"\n"
                 + "target datalayout = \"e-p:64:64:64-i1:8:8-i8:8:8-i16:16:16-i32:32:32-i64:64:64-i128:128:128-f32:32:32-f64:64:64-v16:16:16-v32:32:32-v64:64:64-v128:128:128-n16:32:64\"\n"
                 + "target triple = \"nvptx64-nvidia-cuda\"\n"
                 + "\n"
                 + $"define void @{kernelName}({parameters}) {{\n"
                 + "entry:\n"
                 + $"{methodBody}\n"
                 + "}\n"
                 + "\n"
                 + "; Function Attrs: nounwind readnone\n"
                 + "declare i32 @llvm.nvvm.read.ptx.sreg.ctaid.x() #0\n"
                 + "\n"
                 + "; Function Attrs: nounwind readnone\n"
                 + "declare i32 @llvm.nvvm.read.ptx.sreg.ntid.x() #0\n"
                 + "\n"
                 + "; Function Attrs: nounwind readnone\n"
                 + "declare i32 @llvm.nvvm.read.ptx.sreg.tid.x() #0\n"
                 + "\n"
                 + "; Function Attrs: convergent nounwind\n"
                 + "declare void @llvm.nvvm.barrier0() #1\n"
                 + "\n"
                 + "attributes #0 = { nounwind readnone }\n"
                 + "attributes #1 = { convergent nounwind }\n"
                 + "\n"
                 + "!nvvm.annotations = !{!0}\n"
                 + "!nvvmir.version = !{!1}\n"
                 + "\n"
                 + $"!0 = !{{void ({paramTypes})* @{kernelName}, !\"kernel\", i32 1}}\n"
                 + "!1 = !{i32 2, i32 0, i32 3, i32 1}\n";
        }

        #region Parameters

        private string GetTwoParams(string type)
        {
            return $"{type} %param0, {type} %param1";
        }

        private string GetThreeParams(string type)
        {
            return $"{type} %param0, {type} %param1, {type} %param2";
        }

        #endregion

        #region Parameter Types

        private string GetTwoParamTypes(string type)
        {
            return $"{type}, {type}";
        }

        private string GetThreeParamTypes(string type)
        {
            return $"{type}, {type}, {type}";
        }

        #endregion

        #region Method Bodies

        private string GetScalarAdditionMethodBody(string type)
        {
            return $"  %reg0 = add {type} %param0, %param1\n" +
                   "  ret void";
        }

        private string GetScalarSubtractionMethodBody(string type)
        {
            return $"  %reg0 = sub {type} %param0, %param1\n" +
                   "  ret void";
        }

        private string GetScalarMultiplicationMethodBody(string type)
        {
            return $"  %reg0 = mul {type} %param0, %param1\n" +
                   "  ret void";
        }

        private string GetArrayAdditionMethodBody(string type, string arrayType)
        {
            return $"  %reg0 = getelementptr {type}, {arrayType} %param0, i32 0\n" +
                   $"  %reg1 = load {type}, {arrayType} %reg0\n" +
                   $"  %reg2 = getelementptr {type}, {arrayType} %param1, i32 0\n" +
                   $"  %reg3 = load {type}, {arrayType} %reg2\n" +
                   $"  %reg4 = add {type} %reg1, %reg3\n" +
                   $"  %reg5 = getelementptr {type}, {arrayType} %param2, i32 0\n" +
                   $"  store {type} %reg4, {arrayType} %reg5\n" +
                   "  ret void";
        }

        #endregion
    }
}
