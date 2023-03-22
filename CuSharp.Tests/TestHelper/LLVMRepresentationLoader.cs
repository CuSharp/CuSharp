namespace CuSharp.Tests.TestHelper
{
    public class LLVMRepresentationLoader
    {
        private const string BoolType = "i1";
        private const string Int32Type = "i32";
        private const string Int64Type = "i64";
        private const string FloatType = "float";
        private const string DoubleType = "double";

        public string GetEmptyMethodBodyRepresentation(string kernelName) =>
            GetExpectedLLVMRepresentation(kernelName, string.Empty, string.Empty, GetEmptyMethodBody());

        public string GetMinimalLLVMRepresentation(string kernelName) =>
            GetExpectedLLVMRepresentation(kernelName, GetTwoParams(Int32Type), GetTwoParamTypes(Int32Type),
                GetEmptyMethodBody(), false);

        public string GetMinimalMixedParamLLVMRepresentation(string kernelName) =>
            GetExpectedLLVMRepresentation(kernelName, GetFourMixedParams(Int32Type, Int32Type, BoolType, Int32Type), GetFourMixedParamTypes(Int32Type, Int32Type, BoolType, Int32Type),
                GetEmptyMethodBody(), false);

        public string GetScalarIntAdditionWithConstTestResult(string kernelName) => GetExpectedLLVMRepresentation(
            kernelName,
            GetTwoParams(Int32Type), GetTwoParamTypes(Int32Type),
            GetScalarAdditionWithConstMethodBody(Int32Type, "12345"));

        public string GetScalarLongAdditionWithConstTestResult(string kernelName) => GetExpectedLLVMRepresentation(
            kernelName,
            GetTwoParams(Int64Type), GetTwoParamTypes(Int64Type),
            GetScalarAdditionWithConstMethodBody(Int64Type, "1234567890987"));

        public string GetScalarIntAdditionTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName,
            GetTwoParams(Int32Type), GetTwoParamTypes(Int32Type), GetScalarAdditionMethodBody(Int32Type));

        public string GetScalarIntSubtractionTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName,
            GetTwoParams(Int32Type), GetTwoParamTypes(Int32Type), GetScalarSubtractionMethodBody(Int32Type));

        public string GetScalarIntMultiplicationTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName,
            GetTwoParams(Int32Type), GetTwoParamTypes(Int32Type), GetScalarMultiplicationMethodBody(Int32Type));

        public string GetScalarFloatAdditionWithConstTestResult(string kernelName) => GetExpectedLLVMRepresentation(
            kernelName,
            GetTwoParams(FloatType), GetTwoParamTypes(FloatType),
            GetScalarAdditionWithConstMethodBody(FloatType, "0x40934948C0000000"));

        public string GetScalarDoubleAdditionWithConstTestResult(string kernelName) => GetExpectedLLVMRepresentation(
            kernelName,
            GetTwoParams(DoubleType), GetTwoParamTypes(DoubleType),
            GetScalarAdditionWithConstMethodBody(DoubleType, "0x40FE2408B0FCF80E"));

        public string GetScalarFloatAdditionTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName,
            GetTwoParams(FloatType), GetTwoParamTypes(FloatType), GetScalarAdditionMethodBody(FloatType));

        public string GetScalarFloatSubtractionTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName,
            GetTwoParams(FloatType), GetTwoParamTypes(FloatType), GetScalarSubtractionMethodBody(FloatType));

        public string GetScalarFloatMultiplicationTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName,
            GetTwoParams(FloatType), GetTwoParamTypes(FloatType), GetScalarMultiplicationMethodBody(FloatType));

        public string GetArrayIntAdditionTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName,
            GetThreeParams(Int32Type), GetThreeParamTypes(Int32Type), GetArrayAdditionMethodBody(Int32Type));

        public string GetArrayFloatAdditionTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName,
            GetThreeParams(FloatType), GetThreeParamTypes(FloatType), GetArrayAdditionMethodBody(FloatType));

        public string GetArrayIntAdditionWithKernelToolsTestResult(string kernelName) => GetExpectedLLVMRepresentation(
            kernelName, GetThreeParams(Int32Type), GetThreeParamTypes(Int32Type),
            GetArrayAdditionWithKernelToolsMethodBody(Int32Type));

        public string GetArrayFloatAdditionWithKernelToolsTestResult(string kernelName) => GetExpectedLLVMRepresentation(
            kernelName, GetThreeParams(FloatType), GetThreeParamTypes(FloatType),
            GetArrayAdditionWithKernelToolsMethodBody(FloatType));

        private string GetExpectedLLVMRepresentation(string kernelName, string parameters, string paramTypes, string methodBody, bool printCompleteOutput = true)
        {
            const string dataLayout = "target datalayout = \"e-p:64:64:64-i1:8:8-i8:8:8-i16:16:16-i32:32:32-i64:64:64-i128:128:128-f32:32:32-f64:64:64-v16:16:16-v32:32:32-v64:64:64-v128:128:128-n16:32:64\"\n";
            const string target = "target triple = \"nvptx64-nvidia-cuda\"\n";
            var outputAfterMethodBody = "\n"
                                        + "; Function Attrs: nounwind readnone\n"
                                        + "declare i32 @llvm.nvvm.read.ptx.sreg.ctaid.x() #0\n"
                                        + "\n"
                                        + "; Function Attrs: nounwind readnone\n"
                                        + "declare i32 @llvm.nvvm.read.ptx.sreg.ntid.x() #0\n"
                                        + "\n"
                                        + "; Function Attrs: nounwind readnone\n"
                                        + "declare i32 @llvm.nvvm.read.ptx.sreg.tid.x() #0\n"
                                        + "\n"
                                        + "; Function Attrs: nounwind readnone\n"
                                        + "declare i32 @llvm.nvvm.read.ptx.sreg.ctaid.y() #0\n"
                                        + "\n"
                                        + "; Function Attrs: nounwind readnone\n"
                                        + "declare i32 @llvm.nvvm.read.ptx.sreg.ntid.y() #0\n"
                                        + "\n"
                                        + "; Function Attrs: nounwind readnone\n"
                                        + "declare i32 @llvm.nvvm.read.ptx.sreg.tid.y() #0\n"
                                        + "\n"
                                        + "; Function Attrs: nounwind readnone\n"
                                        + "declare i32 @llvm.nvvm.read.ptx.sreg.ctaid.z() #0\n"
                                        + "\n"
                                        + "; Function Attrs: nounwind readnone\n"
                                        + "declare i32 @llvm.nvvm.read.ptx.sreg.ntid.z() #0\n"
                                        + "\n"
                                        + "; Function Attrs: nounwind readnone\n"
                                        + "declare i32 @llvm.nvvm.read.ptx.sreg.tid.z() #0\n"
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

            return $"; ModuleID = '{kernelName}MODULE'\n"
                 + $"source_filename = \"{kernelName}MODULE\"\n"
                 + $"{(printCompleteOutput ? dataLayout + target : string.Empty)}"
                 + "\n"
                 + $"define void @{kernelName}({parameters}) {{\n"
                 + "entry:\n"
                 + $"{methodBody}\n"
                 + "}\n"
                 + $"{(printCompleteOutput ? outputAfterMethodBody : string.Empty)}";
        }

        //private string GetExpectedArrayParameterMethodLLVM()

        #region Parameters

        private string GetTwoParams(string type)
        {
            return $"{type}* %param0, {type}* %param1";
        }

        private string GetThreeParams(string type)
        {
            return $"{type}* %param0, {type}* %param1, {type}* %param2";
        }

        private string GetFourMixedParams(string type1, string type2, string type3, string type4)
        {
            return $"{type1}* %param0, {type2}* %param1, {type3}* %param2, {type4}* %param3";
        }

        #endregion

        #region Parameter Types

        private string GetTwoParamTypes(string type)
        {
            return $"{type}*, {type}*";
        }

        private string GetThreeParamTypes(string type)
        {
            return $"{type}*, {type}*, {type}*";
        }

        private string GetFourMixedParamTypes(string type1, string type2, string type3, string type4)
        {
            return $"{type1}*, {type2}*, {type3}*, {type4}*";
        }

        #endregion

        #region Method Bodies

        private string GetEmptyMethodBody()
        {
            return "  ret void";
        }

        private string GetScalarAdditionWithConstMethodBody(string type, string constant)
        {
            var prefix = type is FloatType or DoubleType ? "f" : string.Empty;
            return $"  %reg0 = load {type}, {type}* %param0\n" +
                   $"  %reg1 = load {type}, {type}* %param1\n" +
                   $"  %reg2 = {prefix}add {type} %reg0, %reg1\n" +
                   $"  %reg3 = {prefix}add {type} %reg2, {constant}\n" +
                    "  ret void";
        }

        private string GetScalarAdditionMethodBody(string type)
        {
            var prefix = type is FloatType or DoubleType ? "f" : string.Empty;
            return $"  %reg0 = load {type}, {type}* %param0\n" +
                   $"  %reg1 = load {type}, {type}* %param1\n" +
                   $"  %reg2 = {prefix}add {type} %reg0, %reg1\n" +
                    "  ret void";
        }

        private string GetScalarSubtractionMethodBody(string type)
        {
            var prefix = type is FloatType or DoubleType ? "f" : string.Empty;
            return $"  %reg0 = load {type}, {type}* %param0\n" +
                   $"  %reg1 = load {type}, {type}* %param1\n" +
                   $"  %reg2 = {prefix}sub {type} %reg0, %reg1\n" +
                    "  ret void";
        }

        private string GetScalarMultiplicationMethodBody(string type)
        {
            var prefix = type is FloatType or DoubleType ? "f" : string.Empty;
            return $"  %reg0 = load {type}, {type}* %param0\n" +
                   $"  %reg1 = load {type}, {type}* %param1\n" +
                   $"  %reg2 = {prefix}mul {type} %reg0, %reg1\n" +
                   "  ret void";
        }

        private string GetArrayAdditionMethodBody(string type)
        {
            var prefix = type is FloatType or DoubleType ? "f" : string.Empty;
            return $"  %reg0 = getelementptr {type}, {type}* %param0, i32 0\n" +
                   $"  %reg1 = load {type}, {type}* %reg0\n" +
                   $"  %reg2 = getelementptr {type}, {type}* %param1, i32 0\n" +
                   $"  %reg3 = load {type}, {type}* %reg2\n" +
                   $"  %reg4 = {prefix}add {type} %reg1, %reg3\n" +
                   $"  %reg5 = getelementptr {type}, {type}* %param2, i32 0\n" +
                   $"  store {type} %reg4, {type}* %reg5\n" +
                   "  ret void";
        }

        private string GetArrayAdditionWithKernelToolsMethodBody(string type)
        {
            var prefix = type is FloatType or DoubleType ? "f" : string.Empty;
            return "  %reg0 = call i32 @llvm.nvvm.read.ptx.sreg.ctaid.x()\n" +
                   "  %reg1 = call i32 @llvm.nvvm.read.ptx.sreg.ntid.x()\n" +
                   "  %reg2 = mul i32 %reg0, %reg1\n" +
                   "  %reg3 = call i32 @llvm.nvvm.read.ptx.sreg.tid.x()\n" +
                   "  %reg4 = add i32 %reg2, %reg3\n" +
                   $"  %reg5 = getelementptr {type}, {type}* %param0, i32 %reg4\n" +
                   $"  %reg6 = load {type}, {type}* %reg5\n" +
                   $"  %reg7 = getelementptr {type}, {type}* %param1, i32 %reg4\n" +
                   $"  %reg8 = load {type}, {type}* %reg7\n" +
                   $"  %reg9 = {prefix}add {type} %reg6, %reg8\n" +
                   $"  %reg10 = getelementptr {type}, {type}* %param2, i32 %reg4\n" +
                   $"  store {type} %reg9, {type}* %reg10\n" +
                   $"  ret void";
        }

        #endregion
    }
}
