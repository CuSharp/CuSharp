using System;
using Xunit;

namespace CuSharp.Tests.CuSharp;

[Collection("Sequential")]
public class CuSharpTests
{
    [Fact]
    public void TestInvalidDeviceIdThrows()
    {
        Assert.Throws<ArgumentException>( () => global::CuSharp.CuSharp.GetDeviceById(int.MaxValue) );
    }

    [Fact]
    public void TestDefaultDeviceIsZero()
    {
        Assert.Equal(global::CuSharp.CuSharp.GetDeviceById(0).ToString(), global::CuSharp.CuSharp.GetDefaultDevice().ToString());
    }

    [Fact]
    public void TestTimerIsZero()
    {
        global::CuSharp.CuSharp.GetDefaultDevice(); //done just to initialize
        global::CuSharp.CuSharp.StartTimer();
        var time = global::CuSharp.CuSharp.GetTimeMS();
        Assert.True(time < 0.5);
    }
}