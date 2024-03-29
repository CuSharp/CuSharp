﻿using CuSharp.Tests.TestHelper;
using CuSharp.Tests.TestKernels;
using Xunit;

namespace CuSharp.Tests.Integration;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.Integration)]
public class TypeIntegrationTests
{
    [Fact]
    public void TestFloatAddition()
    {
        var dev = Cu.GetDefaultDevice();
        var dev1 = dev.Copy(new float[]{1});
        var dev2 = dev.Copy(new float[]{3});

        dev.Launch(TypeTestKernels.FloatAddition, (1,1,1), (1,1,1), dev1, dev2);
        var result = dev.Copy(dev1);

        dev1.Dispose();
        dev2.Dispose();
        dev.Dispose();

        Assert.Equal(4, result[0]);
    }
    
    [Fact]
    public void TestDoubleAddition()
    {
        var dev = Cu.GetDefaultDevice();
        var dev1 = dev.Copy(new double[]{1});
        var dev2 = dev.Copy(new double[]{3});

        dev.Launch(TypeTestKernels.DoubleAddition, (1,1,1), (1,1,1), dev1, dev2);
        var result = dev.Copy(dev1);

        dev1.Dispose();
        dev2.Dispose();
        dev.Dispose();

        Assert.Equal(4, result[0]);
    }

    [Fact]
    public void TestByteAddition()
    {
        var dev = Cu.GetDefaultDevice();
        var dev1 = dev.Copy(new byte[]{1});
        var dev2 = dev.Copy(new byte[]{3});

        dev.Launch(TypeTestKernels.ByteAddition, (1,1,1), (1,1,1), dev1, dev2);
        var result = dev.Copy(dev1);

        dev1.Dispose();
        dev2.Dispose();
        dev.Dispose();

        Assert.Equal(4, result[0]);
    }

    [Fact]
    public void TestShortAddition()
    {
        var dev = Cu.GetDefaultDevice();
        var dev1 = dev.Copy(new short[]{1});
        var dev2 = dev.Copy(new short[]{3});

        dev.Launch(TypeTestKernels.ShortAddition, (1,1,1), (1,1,1), dev1, dev2);
        var result = dev.Copy(dev1);

        dev1.Dispose();
        dev2.Dispose();
        dev.Dispose();

        Assert.Equal(4, result[0]);
    }

    [Fact]
    public void TestIntAddition()
    {
        var dev = Cu.GetDefaultDevice();
        var dev1 = dev.Copy(new int[]{1});
        var dev2 = dev.Copy(new int[]{3});

        dev.Launch(TypeTestKernels.IntAddition, (1,1,1), (1,1,1), dev1, dev2);
        var result = dev.Copy(dev1);

        dev1.Dispose();
        dev2.Dispose();
        dev.Dispose();

        Assert.Equal(4, result[0]);
    }

    [Fact]
    public void TestLongAddition()
    {
        var dev = Cu.GetDefaultDevice();
        var dev1 = dev.Copy(new long[]{1});
        var dev2 = dev.Copy(new long[]{3});

        dev.Launch(TypeTestKernels.LongAddition, (1,1,1), (1,1,1), dev1, dev2);
        var result = dev.Copy(dev1);

        dev1.Dispose();
        dev2.Dispose();
        dev.Dispose();

        Assert.Equal(4, result[0]);
    }

    [Fact]
    public void TestBooleanAnd()
    {
        var dev = Cu.GetDefaultDevice();
        var dev1 = dev.Copy(new bool[]{true});
        var dev2 = dev.Copy(new bool[]{false});

        dev.Launch(TypeTestKernels.BoolAnd, (1,1,1), (1,1,1), dev1, dev2);
        var result = dev.Copy(dev1);

        dev1.Dispose();
        dev2.Dispose();
        dev.Dispose();

        Assert.Equal(false, result[0]);
    }
    
    [Fact]
    public void TestSignedIntOverflow()
    {
        var dev = Cu.GetDefaultDevice();
        var a = new [] {int.MaxValue};
        var devA = dev.Copy(a);

        dev.Launch(TypeTestKernels.SignedIntOverflow, (1,1,1), (1,1,1), devA);
        a = dev.Copy(devA);

        devA.Dispose();
        dev.Dispose();

        Assert.Equal(int.MinValue, a[0]);
    }

    [Fact]
    public void TestUnsignedIntOverflow()
    {
        var dev = Cu.GetDefaultDevice();
        var a = new [] {uint.MaxValue};
        var devA = dev.Copy(a);

        dev.Launch(TypeTestKernels.UnsignedIntOverflow, (1,1,1), (1,1,1), devA);
        a = dev.Copy(devA);

        devA.Dispose();
        dev.Dispose();

        Assert.Equal(uint.MinValue, a[0]);
    }
}