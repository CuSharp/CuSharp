using static CuSharp.Tests.TestHelper.TypesAsString;

namespace CuSharp.Tests.TestHelper;

public static class TypesAsString
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
        GetExpectedLLVMRepresentation(kernelName, GetEmptyMethodBody(), true);

    public string GetMinimalLLVMRepresentation(string kernelName) => GetExpectedLLVMRepresentation(kernelName, 
        GetEmptyMethodBody(), false, ArrayOf(Int32Type), ArrayOf(Int32Type));

    public string GetMinimalMixedParamLLVMRepresentation(string kernelName) => GetExpectedLLVMRepresentation(kernelName, 
        GetEmptyMethodBody(), false, ArrayOf(Int32Type), ArrayOf(Int32Type), BoolType, Int32Type);

    public string GetScalarIntAdditionWithEachConstTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName, 
        GetScalarAdditionWithEachConstMethodBody(Int32Type, "12345"), true, Int32Type, Int32Type);
        
    public string GetScalarLongAdditionWithConstTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName, 
        GetScalarAdditionWithConstMethodBody(Int64Type, "1234567890987"), true, Int64Type, Int64Type);

    public string GetScalarIntAdditionTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName, 
        GetScalarAdditionMethodBody(Int32Type), true, Int32Type, Int32Type);

    public string GetScalarIntSubtractionTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName, 
        GetScalarSubtractionMethodBody(Int32Type), true, Int32Type, Int32Type);

    public string GetScalarIntMultiplicationTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName, 
        GetScalarMultiplicationMethodBody(Int32Type), true, Int32Type, Int32Type);

    public string GetScalarIntDivisionTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName, 
        GetScalarDivisionMethodBody(Int32Type), true, Int32Type, Int32Type);

    public string GetScalarIntRemainderTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName, 
        GetScalarRemainderMethodBody(Int32Type), true, Int32Type, Int32Type);

    public string GetScalarFloatAdditionWithConstTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName,
        GetScalarAdditionWithConstMethodBody(FloatType, "0x40934948C0000000"), true, FloatType, FloatType);

    public string GetScalarDoubleAdditionWithConstTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName, 
        GetScalarAdditionWithConstMethodBody(DoubleType, "0x40FE2408B0FCF80E"), true, DoubleType, DoubleType);

    public string GetScalarFloatAdditionTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName, 
        GetScalarAdditionMethodBody(FloatType), true, FloatType, FloatType);

    public string GetScalarFloatSubtractionTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName,
        GetScalarSubtractionMethodBody(FloatType), true, FloatType, FloatType);

    public string GetScalarFloatMultiplicationTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName, 
        GetScalarMultiplicationMethodBody(FloatType), true, FloatType, FloatType);

    public string GetScalarFloatDivisionTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName, 
        GetScalarDivisionMethodBody(FloatType), true, FloatType, FloatType);

    public string GetScalarFloatRemainderTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName, 
        GetScalarRemainderMethodBody(FloatType), true, FloatType, FloatType);

    public string GetArrayIntAdditionTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName, 
        GetArrayAdditionMethodBody(Int32Type), true, ArrayOf(Int32Type), ArrayOf(Int32Type), ArrayOf(Int32Type));

    public string GetArrayFloatAdditionTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName,
        GetArrayAdditionMethodBody(FloatType), true, ArrayOf(FloatType), ArrayOf(FloatType), ArrayOf(FloatType));

    public string GetArrayIntAdditionWithKernelToolsTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName, 
        GetArrayAdditionWithKernelToolsMethodBody(Int32Type), true, ArrayOf(Int32Type), ArrayOf(Int32Type), ArrayOf(Int32Type));

    public string GetArrayFloatAdditionWithKernelToolsTestResult(string kernelName) => GetExpectedLLVMRepresentation(kernelName,
        GetArrayAdditionWithKernelToolsMethodBody(FloatType), true, ArrayOf(FloatType), ArrayOf(FloatType), ArrayOf(FloatType));

    public string GetArrayShortHandOperationsWithKernelToolsTestResult(string kernelName, string type) => GetExpectedLLVMRepresentation(kernelName,
        GetArrayShortHandOperationsWithKernelToolsMethodBody(type), true, ArrayOf(type), ArrayOf(type));

    private string GetExpectedLLVMRepresentation(string kernelName, string methodBody, bool printCompleteOutput, params string[] types)
    {
        var parameters = GetParams(types);
        var paramTypes = GetParamTypes(types);

        const string dataLayout = "target datalayout = \"e-p:64:64:64-i1:8:8-i8:8:8-i16:16:16-i32:32:32-i64:64:64-i128:128:128-f32:32:32-f64:64:64-v16:16:16-v32:32:32-v64:64:64-v128:128:128-n16:32:64\"\n";
        const string target = "target triple = \"nvptx64-nvidia-cuda\"\n";
        var outputAfterMethodBody = "; Function Attrs: nounwind readnone\n"
                                    + "declare i32 @llvm.nvvm.read.ptx.sreg.nctaid.x() #0\n"
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
                                    + "; Function Attrs: nounwind readnone\n"
                                    + "declare i32 @llvm.nvvm.read.ptx.sreg.nctaid.y() #0\n"
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
                                    + "declare i32 @llvm.nvvm.read.ptx.sreg.nctaid.z() #0\n"
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
                                    + "; Function Attrs: nounwind readnone"
                                    + "\ndeclare i32 @llvm.nvvm.read.ptx.sreg.warpsize() #0\n"
                                    + "\n"
                                    + "; Function Attrs: convergent nounwind\n"
                                    + "declare void @llvm.nvvm.barrier0() #1\n"
                                    + "\n"
                                    + "; Function Attrs: nounwind\n"
                                    + "declare void @llvm.nvvm.membar.gl() #2\n"
                                    + "\n"
                                    + "; Function Attrs: nounwind\n"
                                    + "declare void @llvm.nvvm.membar.sys() #2\n"
                                    + "\n";
        var metaInfos = "\n"
                        + "attributes #0 = { nounwind readnone }\n"
                        + "attributes #1 = { convergent nounwind }\n"
                        + "attributes #2 = { nounwind }\n"
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
               + $"{(printCompleteOutput ? outputAfterMethodBody : string.Empty)}"
               + $"define void @{kernelName}({parameters}) {{\n"
               + "entry:\n"
               + "  br label %block0\n\n"
               + "block0:                                           ; preds = %entry\n"
               + GetBlock0Phis(types)
               + $"{methodBody}\n"
               + "}\n"
               + $"{(printCompleteOutput ? metaInfos: string.Empty)}";
    }

    //private string GetExpectedArrayParameterMethodLLVM()

    #region Parameters

    private string GetParams(params string[] types)
    {
        var typeString = string.Empty;
        for (var i = 0; i < types.Length; i++)
        {
            typeString += $"{types[i]} %param{i}, ";
        }
        return typeString.TrimEnd().TrimEnd(',');
    }

    private string GetParamTypes(params string[] types)
    {
        var typeString = string.Empty;
        foreach (var type in types)
        {
            typeString += $"{type}, ";
        }
        return typeString.TrimEnd().TrimEnd(',');
    }

    private string ArrayOf(string type)
    {
        return $"{type}*";
    }

    //private string GetTwoParams(string type)
    //{
    //    return $"{type}* %param0, {type}* %param1";
    //}

    //private string GetThreeParams(string type)
    //{
    //    return $"{type}* %param0, {type}* %param1, {type}* %param2";
    //}

    //private string GetFourMixedParams(string type1, string type2, string type3, string type4)
    //{
    //    return $"{type1}* %param0, {type2}* %param1, {type3}* %param2, {type4}* %param3";
    //}

    #endregion

    //#region Parameter Types

    //private string GetTwoParamTypes(string type)
    //{
    //    return $"{type}*, {type}*";
    //}

    //private string GetThreeParamTypes(string type)
    //{
    //    return $"{type}*, {type}*, {type}*";
    //}

    //private string GetFourMixedParamTypes(string type1, string type2, string type3, string type4)
    //{
    //    return $"{type1}*, {type2}*, {type3}*, {type4}*";
    //}

    //#endregion

    #region Method Bodies

    private string GetBlock0Phis(string[] paramTypes)
    {
        string str = "";
        var index = 0;
        foreach (var type in paramTypes)
        {
            str += $"  %reg{index} = phi {type} [ %param{index}, %entry ]\n";
            index++;
        }

        return str;
    }
    private string GetEmptyMethodBody()
    {
        return "  ret void";
    }

    private string GetScalarAdditionWithConstMethodBody(string type, string constant)
    {
        var prefix = GetPrefix(type);
        return $"  %reg0 = {prefix}add {type} %param0, %param1\n" +
               $"  %reg1 = {prefix}add {type} %reg0, {constant}\n" +
               "  ret void";
    }

    private string GetScalarAdditionWithEachConstMethodBody(string type, string constant)
    {
        var prefix = GetPrefix(type);
        return $"  %reg0 = {prefix}add {type} %param0, %param1\n" +
               $"  %reg1 = {prefix}add {type} %reg0, {constant}\n" +
               $"  %reg2 = add i32 %reg1, 0\n" +
               $"  %reg3 = add i32 %reg2, -1\n" +
               $"  %reg4 = add i32 %reg3, 1\n" +
               $"  %reg5 = add i32 %reg4, 2\n" +
               $"  %reg6 = add i32 %reg5, 3\n" +
               $"  %reg7 = add i32 %reg6, 4\n" +
               $"  %reg8 = add i32 %reg7, 5\n" +
               $"  %reg9 = add i32 %reg8, 6\n" +
               $"  %reg10 = add i32 %reg9, 7\n" +
               $"  %reg11 = add i32 %reg10, 8\n" +
               $"  ret void";
    }

    private string GetScalarAdditionMethodBody(string type)
    {
        var prefix = GetPrefix(type);
        return $"  %reg0 = {prefix}add {type} %param0, %param1\n" +
                "  ret void";
    }

    private string GetScalarSubtractionMethodBody(string type)
    {
        var prefix = GetPrefix(type);
        return $"  %reg0 = {prefix}sub {type} %param0, %param1\n" +
               "  ret void";
    }

    private string GetScalarMultiplicationMethodBody(string type)
    {
        var prefix = GetPrefix(type);
        return $"  %reg0 = {prefix}mul {type} %param0, %param1\n" +
               "  ret void";
    }

    private string GetScalarDivisionMethodBody(string type)
    {
        var prefix = GetPrefix(type);
        return $"  %reg0 = {GetDivRemPrefix(prefix)}div {type} %param0, %param1\n" +
               "  ret void";
    }

    private string GetScalarRemainderMethodBody(string type)
    {
        var prefix = GetPrefix(type);
        return $"  %reg0 = {GetDivRemPrefix(prefix)}rem {type} %param0, %param1\n" +
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
        return (type == FloatType || type == DoubleType) ? "f" : string.Empty;
    }

    private string GetDivRemPrefix(string prefix)
    {
        return prefix != string.Empty ? prefix : "s";
    }
}