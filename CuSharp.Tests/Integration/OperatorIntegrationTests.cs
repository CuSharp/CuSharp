using System;
using CuSharp.Tests.TestHelper;
using Xunit;

namespace CuSharp.Tests.Integration;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.Integration)]
public class OperatorIntegrationTests
{
    [Fact]
    public void AdditionTest()
    {
        var result = RunFloatArrayTest(OperatorTestKernels.FloatAddition, new float[] {42}, new float[]{2});
        Assert.Equal(44, result[0]);
    }

    [Fact]
    public void SubtractionTest()
    {
        var result = RunFloatArrayTest(OperatorTestKernels.FloatSubtraction, new float[] {42}, new float[]{2});
        Assert.Equal(40, result[0]);
    }

    [Fact]
    public void MultiplicationTest()
    {
        var result = RunFloatArrayTest(OperatorTestKernels.FloatMultiplication, new float[] {42}, new float[]{2});
        Assert.Equal(84, result[0]);
    }

    [Fact]
    public void DivisionTest()
    {
        var result = RunFloatArrayTest(OperatorTestKernels.FloatDivision, new float[] {42}, new float[]{2});
        Assert.Equal(21, result[0]);
    }

    [Fact]
    public void ModuloTest()
    {
        var result = RunFloatArrayTest(OperatorTestKernels.FloatModulo, new float[] {42}, new float[]{4});
        Assert.Equal(2, result[0]);
    }

    private float[] RunFloatArrayTest(Action<float[], float[]> kernel, float[] a, float[] b)
    {
        var dev = global::CuSharp.CuSharp.GetDefaultDevice();
        var devA = dev.Copy(a);
        var devB = dev.Copy(b);
        dev.Launch(kernel, (1,1,1), (1,1,1), devA, devB);
        a = dev.Copy(devA);
        return a;
    }
    
    [Fact]
    public void TestNot()
    {
        var dev = global::CuSharp.CuSharp.GetDefaultDevice();
        bool[] a = new bool[] {false, false};
        bool b = true;
        var devA = dev.Copy(a);
        var devB = dev.CreateScalar(b);
        dev.Launch(OperatorTestKernels.NotTest, (1, 1, 1), (1, 1, 1), devA, devB);
        a = dev.Copy(devA);
        Assert.True(a[0]);
        Assert.True(!a[1]);
    }
}