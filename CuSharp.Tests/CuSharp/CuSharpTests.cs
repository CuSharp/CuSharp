using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [Fact]
    public void TestGetDeviceList()
    {
        var defaultDevice = global::CuSharp.CuSharp.GetDefaultDevice().ToString();
        var deviceEnumerator = global::CuSharp.CuSharp.GetDeviceList();
        IList<(int, string)> devices = new List<(int, string)>();

        while (deviceEnumerator.MoveNext())
        {
            devices.Add(deviceEnumerator.Current);
        }

        Assert.True(devices.Any(d => d.Item2 == defaultDevice));
    }
}