using System;
using CuSharp.Tests.TestHelper;
using Xunit;

namespace CuSharp.Tests.Integration;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.Integration)]
public class TypeIntegrationTests
{
    [Fact]
    public void TestFloatAddition()
    {
        var dev = global::CuSharp.CuSharp.GetDefaultDevice();
        var dev1 = dev.Copy(new float[]{1});
        var dev2 = dev.Copy(new float[]{3});
        dev.Launch(TypeTestKernels.FloatAddition, (1,1,1), (1,1,1), dev1, dev2);
        var result = dev.Copy(dev1);
        Assert.Equal(4, result[0]);
    }
    
    [Fact]
    public void TestDoubleAddition()
    {
        var dev = global::CuSharp.CuSharp.GetDefaultDevice();
        var dev1 = dev.Copy(new double[]{1});
        var dev2 = dev.Copy(new double[]{3});
        dev.Launch(TypeTestKernels.DoubleAddition, (1,1,1), (1,1,1), dev1, dev2);
        var result = dev.Copy(dev1);
        Assert.Equal(4, result[0]);
    }
    [Fact]
    public void TestByteAddition()
    {
        var dev = global::CuSharp.CuSharp.GetDefaultDevice();
        var dev1 = dev.Copy(new byte[]{1});
        var dev2 = dev.Copy(new byte[]{3});
        dev.Launch(TypeTestKernels.ByteAddition, (1,1,1), (1,1,1), dev1, dev2);
        var result = dev.Copy(dev1);
        Assert.Equal(4, result[0]);
    }
    [Fact]
    public void TestShortAddition()
    {
        var dev = global::CuSharp.CuSharp.GetDefaultDevice();
        var dev1 = dev.Copy(new short[]{1});
        var dev2 = dev.Copy(new short[]{3});
        dev.Launch(TypeTestKernels.ShortAddition, (1,1,1), (1,1,1), dev1, dev2);
        var result = dev.Copy(dev1);
        Assert.Equal(4, result[0]);
    }
    [Fact]
    public void TestIntAddition()
    {
        var dev = global::CuSharp.CuSharp.GetDefaultDevice();
        var dev1 = dev.Copy(new int[]{1});
        var dev2 = dev.Copy(new int[]{3});
        dev.Launch(TypeTestKernels.IntAddition, (1,1,1), (1,1,1), dev1, dev2);
        var result = dev.Copy(dev1);
        Assert.Equal(4, result[0]);
    }

    [Fact]
    public void TestLongAddition()
    {
        var dev = global::CuSharp.CuSharp.GetDefaultDevice();
        var dev1 = dev.Copy(new long[]{1});
        var dev2 = dev.Copy(new long[]{3});
        dev.Launch(TypeTestKernels.LongAddition, (1,1,1), (1,1,1), dev1, dev2);
        var result = dev.Copy(dev1);
        Assert.Equal(4, result[0]);
    }
    [Fact]
    public void TestBooleanAnd()
    {
        var dev = global::CuSharp.CuSharp.GetDefaultDevice();
        var dev1 = dev.Copy(new bool[]{true});
        var dev2 = dev.Copy(new bool[]{false});
        dev.Launch(TypeTestKernels.BoolAnd, (1,1,1), (1,1,1), dev1, dev2);
        var result = dev.Copy(dev1);
        Assert.Equal(false, result[0]);
    }
    
    [Fact]
    public void TestSignedIntOverflow()
    {
        var dev = global::CuSharp.CuSharp.GetDefaultDevice();
        var a = new int[] {Int32.MaxValue};
        var devA = dev.Copy(a);
        dev.Launch(TypeTestKernels.SignedIntOverflow, (1,1,1), (1,1,1), devA);
        a = dev.Copy(devA);
        Assert.Equal(Int32.MinValue, a[0]);
    }

    [Fact]
    public void TestUnsignedIntOverflow()
    {
        var dev = global::CuSharp.CuSharp.GetDefaultDevice();
        var a = new uint[] {UInt32.MaxValue};
        var devA = dev.Copy(a);
        dev.Launch(TypeTestKernels.UnsignedIntOverflow, (1,1,1), (1,1,1), devA);
        a = dev.Copy(devA);
        Assert.Equal(UInt32.MinValue, a[0]);
    }
}