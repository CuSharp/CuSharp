namespace CuSharp.Tests.TestHelper
{
    public struct TypesAsString
    {
        public static string BoolType => "i1";
        public static string Int32Type => "i32";
        public static string Int64Type => "i64";
        public static string FloatType => "float";
        public static string DoubleType => "double";
    }

    public class LLVMRepresentationLoader
    {

        public string GetEmptyMethodBodyRepresentation(string kernelName) =>
            GetExpectedLLVMRepresentation(kernelName, string.Empty, string.Empty, GetEmptyMethodBody());

        public string GetMinimalLLVMRepresentation(string kernelName) =>
            GetExpectedLLVMRepresentation(kernelName, GetTwoParams(TypesAsString.Int32Type), GetTwoParamTypes(TypesAsString.Int32Type),
                GetEmptyMethodBody(), false);

        public string GetMinimalMixedParamLLVMRepresentation(string kernelName) =>
            GetExpectedLLVMRepresentation(kernelName, GetFourMixedParams(TypesAsString.Int32Type, TypesAsString.Int32Type, TypesAsString.BoolType, TypesAsString.Int32Type), GetFourMixedParamTypes(TypesAsString.Int32Type, TypesAsString.Int32Type, TypesAsString.BoolType, TypesAsString.Int32Type),
                GetEmptyMethodBody(), false);

        public string GetScalarIntAdditionWithEachConstTestResult(string kernelName) => GetExpectedLLVMRepresentation(
            kernelName,
            GetTwoParams(TypesAsString.Int32Type), GetTwoParamTypes(TypesAsString.Int32Type),
            GetScalarAdditionWithEachConstMethodBody(TypesAsString.Int32Type, "12345"));

        public string GetScalarLongAdditionWithConstTestResult(string kernelName) => GetExpectedLLVMRepresentation(
            kernelName,
            GetTwoParams(TypesAsString.Int64Type), GetTwoParamTypes(TypesAsString.Int64Type),
            GetScalarAdditionWithConstMethodBody(TypesAsString.Int64Type, "1234567890987"));

        public string GetScalarIntAdditionTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName,
            GetTwoParams(TypesAsString.Int32Type), GetTwoParamTypes(TypesAsString.Int32Type), GetScalarAdditionMethodBody(TypesAsString.Int32Type));

        public string GetScalarIntSubtractionTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName,
            GetTwoParams(TypesAsString.Int32Type), GetTwoParamTypes(TypesAsString.Int32Type), GetScalarSubtractionMethodBody(TypesAsString.Int32Type));

        public string GetScalarIntMultiplicationTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName,
            GetTwoParams(TypesAsString.Int32Type), GetTwoParamTypes(TypesAsString.Int32Type), GetScalarMultiplicationMethodBody(TypesAsString.Int32Type));

        public string GetScalarIntDivisionTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName,
            GetTwoParams(TypesAsString.Int32Type), GetTwoParamTypes(TypesAsString.Int32Type), GetScalarDivisionMethodBody(TypesAsString.Int32Type));

        public string GetScalarIntRemainderTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName,
            GetTwoParams(TypesAsString.Int32Type), GetTwoParamTypes(TypesAsString.Int32Type), GetScalarRemainderMethodBody(TypesAsString.Int32Type));

        public string GetScalarFloatAdditionWithConstTestResult(string kernelName) => GetExpectedLLVMRepresentation(
            kernelName,
            GetTwoParams(TypesAsString.FloatType), GetTwoParamTypes(TypesAsString.FloatType),
            GetScalarAdditionWithConstMethodBody(TypesAsString.FloatType, "0x40934948C0000000"));

        public string GetScalarDoubleAdditionWithConstTestResult(string kernelName) => GetExpectedLLVMRepresentation(
            kernelName,
            GetTwoParams(TypesAsString.DoubleType), GetTwoParamTypes(TypesAsString.DoubleType),
            GetScalarAdditionWithConstMethodBody(TypesAsString.DoubleType, "0x40FE2408B0FCF80E"));

        public string GetScalarFloatAdditionTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName,
            GetTwoParams(TypesAsString.FloatType), GetTwoParamTypes(TypesAsString.FloatType), GetScalarAdditionMethodBody(TypesAsString.FloatType));

        public string GetScalarFloatSubtractionTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName,
            GetTwoParams(TypesAsString.FloatType), GetTwoParamTypes(TypesAsString.FloatType), GetScalarSubtractionMethodBody(TypesAsString.FloatType));

        public string GetScalarFloatMultiplicationTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName,
            GetTwoParams(TypesAsString.FloatType), GetTwoParamTypes(TypesAsString.FloatType), GetScalarMultiplicationMethodBody(TypesAsString.FloatType));

        public string GetScalarFloatDivisionTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName,
            GetTwoParams(TypesAsString.FloatType), GetTwoParamTypes(TypesAsString.FloatType), GetScalarDivisionMethodBody(TypesAsString.FloatType));

        public string GetScalarFloatRemainderTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName,
            GetTwoParams(TypesAsString.FloatType), GetTwoParamTypes(TypesAsString.FloatType), GetScalarRemainderMethodBody(TypesAsString.FloatType));

        public string GetArrayIntAdditionTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName,
            GetThreeParams(TypesAsString.Int32Type), GetThreeParamTypes(TypesAsString.Int32Type), GetArrayAdditionMethodBody(TypesAsString.Int32Type));

        public string GetArrayFloatAdditionTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName,
            GetThreeParams(TypesAsString.FloatType), GetThreeParamTypes(TypesAsString.FloatType), GetArrayAdditionMethodBody(TypesAsString.FloatType));

        public string GetArrayIntAdditionWithKernelToolsTestResult(string kernelName) => GetExpectedLLVMRepresentation(
            kernelName, GetThreeParams(TypesAsString.Int32Type), GetThreeParamTypes(TypesAsString.Int32Type),
            GetArrayAdditionWithKernelToolsMethodBody(TypesAsString.Int32Type));

        public string GetArrayFloatAdditionWithKernelToolsTestResult(string kernelName) => GetExpectedLLVMRepresentation(
            kernelName, GetThreeParams(TypesAsString.FloatType), GetThreeParamTypes(TypesAsString.FloatType),
            GetArrayAdditionWithKernelToolsMethodBody(TypesAsString.FloatType));

        public string GetArrayShortHandOperationsWithKernelToolsTestResult(string kernelName, string type) =>
            GetExpectedLLVMRepresentation(
                kernelName, GetTwoParams(type), GetTwoParamTypes(type),
                GetArrayShortHandOperationsWithKernelToolsMethodBody(type));

        public string GetArrayLengthAttributeTestResult(string kernelName, string type) =>
            GetExpectedLLVMRepresentation(
                kernelName, GetTwoParams(type), GetTwoParamTypes(type),
                GetArrayLengthAttributeMethodBody(type));
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
            return $"{type}* %param0, {type}* %param1, i32* %param2";
        }

        private string GetThreeParams(string type)
        {
            return $"{type}* %param0, {type}* %param1, {type}* %param2, i32* %param3";
        }

        private string GetFourMixedParams(string type1, string type2, string type3, string type4)
        {
            return $"{type1}* %param0, {type2}* %param1, {type3}* %param2, {type4}* %param3, i32* %param4";
        }

        #endregion

        #region Parameter Types

        private string GetTwoParamTypes(string type)
        {
            return $"{type}*, {type}*, i32*";
        }

        private string GetThreeParamTypes(string type)
        {
            return $"{type}*, {type}*, {type}*, i32*";
        }

        private string GetFourMixedParamTypes(string type1, string type2, string type3, string type4)
        {
            return $"{type1}*, {type2}*, {type3}*, {type4}*, i32*";
        }

        #endregion

        #region Method Bodies

        private string GetEmptyMethodBody()
        {
            return "  ret void";
        }

        private string GetScalarAdditionWithConstMethodBody(string type, string constant)
        {
            var prefix = GetPrefix(type);
            return $"  %reg0 = load {type}, {type}* %param0\n" +
                   $"  %reg1 = load {type}, {type}* %param1\n" +
                   $"  %reg2 = {prefix}add {type} %reg0, %reg1\n" +
                   $"  %reg3 = {prefix}add {type} %reg2, {constant}\n" +
                    "  ret void";
        }

        private string GetScalarAdditionWithEachConstMethodBody(string type, string constant)
        {
            var prefix = GetPrefix(type);
            return $"  %reg0 = load {type}, {type}* %param0\n" +
                   $"  %reg1 = load {type}, {type}* %param1\n" +
                   $"  %reg2 = {prefix}add {type} %reg0, %reg1\n" +
                   $"  %reg3 = {prefix}add {type} %reg2, {constant}\n" +
                   $"  %reg4 = add i32 %reg3, 0\n" +
                   $"  %reg5 = add i32 %reg4, -1\n" +
                   $"  %reg6 = add i32 %reg5, 1\n" +
                   $"  %reg7 = add i32 %reg6, 2\n" +
                   $"  %reg8 = add i32 %reg7, 3\n" +
                   $"  %reg9 = add i32 %reg8, 4\n" +
                   $"  %reg10 = add i32 %reg9, 5\n" +
                   $"  %reg11 = add i32 %reg10, 6\n" +
                   $"  %reg12 = add i32 %reg11, 7\n" +
                   $"  %reg13 = add i32 %reg12, 8\n" +
                   $"  ret void";
        }

        private string GetScalarAdditionMethodBody(string type)
        {
            var prefix = GetPrefix(type);
            return $"  %reg0 = load {type}, {type}* %param0\n" +
                   $"  %reg1 = load {type}, {type}* %param1\n" +
                   $"  %reg2 = {prefix}add {type} %reg0, %reg1\n" +
                    "  ret void";
        }

        private string GetScalarSubtractionMethodBody(string type)
        {
            var prefix = GetPrefix(type);
            return $"  %reg0 = load {type}, {type}* %param0\n" +
                   $"  %reg1 = load {type}, {type}* %param1\n" +
                   $"  %reg2 = {prefix}sub {type} %reg0, %reg1\n" +
                    "  ret void";
        }

        private string GetScalarMultiplicationMethodBody(string type)
        {
            var prefix = GetPrefix(type);
            return $"  %reg0 = load {type}, {type}* %param0\n" +
                   $"  %reg1 = load {type}, {type}* %param1\n" +
                   $"  %reg2 = {prefix}mul {type} %reg0, %reg1\n" +
                   "  ret void";
        }

        private string GetScalarDivisionMethodBody(string type)
        {
            var prefix = GetPrefix(type);
            return $"  %reg0 = load {type}, {type}* %param0\n" +
                   $"  %reg1 = load {type}, {type}* %param1\n" +
                   $"  %reg2 = {GetDivRemPrefix(prefix)}div {type} %reg0, %reg1\n" +
                   "  ret void";
        }

        private string GetScalarRemainderMethodBody(string type)
        {
            var prefix = GetPrefix(type);
            return $"  %reg0 = load {type}, {type}* %param0\n" +
                   $"  %reg1 = load {type}, {type}* %param1\n" +
                   $"  %reg2 = {GetDivRemPrefix(prefix)}rem {type} %reg0, %reg1\n" +
                   "  ret void";
        }

        private string GetArrayAdditionMethodBody(string type)
        {
            var prefix = GetPrefix(type);
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
            var prefix = GetPrefix(type);
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

        private string GetArrayShortHandOperationsWithKernelToolsMethodBody(string type)
        {
            var prefix = GetPrefix(type);
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
                   $"  store {type} %reg9, {type}* %reg5\n" +
                   $"  %reg10 = getelementptr {type}, {type}* %param0, i32 %reg4\n" +
                   $"  %reg11 = load {type}, {type}* %reg10\n" +
                   $"  %reg12 = getelementptr {type}, {type}* %param1, i32 %reg4\n" +
                   $"  %reg13 = load {type}, {type}* %reg12\n" +
                   $"  %reg14 = {prefix}sub {type} %reg11, %reg13\n" +
                   $"  store {type} %reg14, {type}* %reg10\n" +
                   $"  %reg15 = getelementptr {type}, {type}* %param0, i32 %reg4\n" +
                   $"  %reg16 = load {type}, {type}* %reg15\n" +
                   $"  %reg17 = getelementptr {type}, {type}* %param1, i32 %reg4\n" +
                   $"  %reg18 = load {type}, {type}* %reg17\n" +
                   $"  %reg19 = {prefix}mul {type} %reg16, %reg18\n" +
                   $"  store {type} %reg19, {type}* %reg15\n" +
                   $"  %reg20 = getelementptr {type}, {type}* %param0, i32 %reg4\n" +
                   $"  %reg21 = load {type}, {type}* %reg20\n" +
                   $"  %reg22 = getelementptr {type}, {type}* %param1, i32 %reg4\n" +
                   $"  %reg23 = load {type}, {type}* %reg22\n" +
                   $"  %reg24 = {GetDivRemPrefix(prefix)}div {type} %reg21, %reg23\n" +
                   $"  store {type} %reg24, {type}* %reg20\n" +
                   $"  %reg25 = getelementptr {type}, {type}* %param0, i32 %reg4\n" +
                   $"  %reg26 = load {type}, {type}* %reg25\n" +
                   $"  %reg27 = getelementptr {type}, {type}* %param1, i32 %reg4\n" +
                   $"  %reg28 = load {type}, {type}* %reg27\n" +
                   $"  %reg29 = {GetDivRemPrefix(prefix)}rem {type} %reg26, %reg28\n" +
                   $"  store {type} %reg29, {type}* %reg25\n" +
                   $"  ret void";
        }
        
        private string GetArrayLengthAttributeMethodBody(string type)
        {
            return $"  %reg0 = getelementptr {type}, {type}* %param2, {type} 0\n" +
                   $"  %reg1 = load {type}, {type}* %reg0\n" +
                   $"  store {type} %reg1, {type}* %param1\n" +
                   $"  ret void";
        }
        #endregion

        private string GetPrefix(string type)
        {
            return (type == TypesAsString.FloatType || type == TypesAsString.DoubleType) ? "f" : string.Empty;
        }

        private string GetDivRemPrefix(string prefix)
        {
            return prefix != string.Empty ? prefix : "s";
        }
    }
}
