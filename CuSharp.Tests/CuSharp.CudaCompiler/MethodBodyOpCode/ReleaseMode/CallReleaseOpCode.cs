﻿using CuSharp.CudaCompiler.Frontend;
using CuSharp.Tests.TestHelper;
using LLVMSharp;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Xunit;
using static CuSharp.Tests.TestHelper.MethodInfoLoader;
using static CuSharp.Tests.TestHelper.MethodsToCompile;

namespace CuSharp.Tests.CuSharp.CudaCompiler.MethodBodyOpCode.ReleaseMode;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.UnitReleaseOnly)]
public class CallReleaseOpCode
{

    //[Fact]
    //public void TestCallIntMethod()
    //{
    //    // Arrange
    //    const string kernelName = "TestCallIntMethodOpCode";
    //    var method = GetMethodInfo<int>(CallIntMethod);
    //    var kernel = new MSILKernel(kernelName, method);
    //    var functionsDto = _functionBuilder.BuildFunctionsDto(kernelName, method.GetParameters());
    //    var builder = _functionBuilder.GetBuilderWithEntryBlock(functionsDto.Function);
    //    var functionGenerator = new FunctionGenerator(LLVM.ModuleCreateWithName(kernelName + "MODULE"), builder);

    //    // Act Main
    //    var actualMain = new MethodBodyCompiler(kernel, builder, functionsDto, functionGenerator)
    //        .CompileMethodBody().ToList();

    //    // Act Call
    //    var call = functionGenerator.FunctionsToBuild[0];
    //    functionGenerator.AppendFunction(call.function);
    //    functionsDto.Function = call.function;
    //    var actualCall = new MethodBodyCompiler(call.kernelToCall, builder, functionsDto, functionGenerator)
    //        .CompileMethodBody().ToList();

    //    var expectedMain = new List<(ILOpCode, object?)>
    //    {
    //        (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Call, actualMain[2].Operand),
    //        (ILOpCode.Starg_s, actualMain[3].Operand), (ILOpCode.Ret, null)
    //    };

    //    var expectedCall = new List<(ILOpCode, object?)>
    //    {
    //        (ILOpCode.Ldarg_0, null), (ILOpCode.Ldarg_1, null), (ILOpCode.Add, null),
    //        (ILOpCode.Ret, null)
    //    };

    //    Assert.Single(functionGenerator.FunctionsToBuild);
    //    Assert.Equal(expectedMain, actualMain);
    //    Assert.Equal(expectedCall, actualCall);
    //}
}