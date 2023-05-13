using CuSharp.Tests.TestHelper;
using System;
using System.Reflection;
using CuSharp.CudaCompiler.Frontend;
using Xunit;
using static CuSharp.Tests.TestHelper.MethodInfoLoader;

namespace CuSharp.Tests.CuSharp.CudaCompiler.MethodBodyOpCode;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.Unit)]
public class CallOpCodeTests
{
    [Fact]
    public void TestNotSupportedNonStaticCall()
    {
        // Arrange
        var kernelName = MethodBase.GetCurrentMethod()!.Name;
        var method = GetMethodInfo(new MethodsToCompile().NonStaticEmptyMethod);

        // Assert
        Assert.Throws<NotSupportedException>(() =>
            // Act
            new MSILKernel(kernelName, method));
    }
}