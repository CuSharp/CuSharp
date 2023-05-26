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
        Assert.Throws<ArgumentException>( () => global::CuSharp.Cu.GetDeviceById(int.MaxValue) );
    }

    [Fact]
    public void TestDefaultDeviceIsZero()
    {
        Assert.Equal(global::CuSharp.Cu.GetDeviceById(0).ToString(), global::CuSharp.Cu.GetDefaultDevice().ToString());
    }

    [Fact]
    public void TestTimerIsZero()
    {
        var dev = global::CuSharp.Cu.GetDefaultDevice(); //done just to initialize
        var first = global::CuSharp.Cu.CreateEvent();
        var second = global::CuSharp.Cu.CreateEvent();
        first.Record();
        second.Record();
        var time = first.GetDeltaTo(second);
        Assert.True(time < 0.5);
    }

    [Fact]
    public void TestGetDeviceList()
    {
        var defaultDevice = global::CuSharp.Cu.GetDefaultDevice().ToString();
        var deviceEnumerator = global::CuSharp.Cu.GetDeviceList();
        IList<(int, string)> devices = new List<(int, string)>();

        while (deviceEnumerator.MoveNext())
        {
            devices.Add(deviceEnumerator.Current);
        }

        Assert.True(devices.Any(d => d.Item2 == defaultDevice));
    }
}