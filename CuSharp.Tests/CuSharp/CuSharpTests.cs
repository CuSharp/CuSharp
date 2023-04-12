using CuSharp.Tests.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CuSharp.Tests.CuSharp;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.Integration)]
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
        var dev = global::CuSharp.CuSharp.GetDefaultDevice(); //done just to initialize
        var first = global::CuSharp.CuSharp.CreateEvent();
        var second = global::CuSharp.CuSharp.CreateEvent();
        first.Record();
        second.Record();
        var time = first.GetDeltaTo(second);
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