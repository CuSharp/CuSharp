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
        Assert.Throws<ArgumentException>( () => Cu.GetDeviceById(int.MaxValue) );
    }

    [Fact]
    public void TestDefaultDeviceIsZero()
    {
        Assert.Equal(Cu.GetDeviceById(0).ToString(), Cu.GetDefaultDevice().ToString());
    }

    [Fact]
    public void TestTimerIsZero()
    {
        var dev = Cu.GetDefaultDevice(); //done just to initialize
        var first = Cu.CreateEvent();
        var second = Cu.CreateEvent();
        first.Record();
        second.Record();
        var time = first.GetDeltaTo(second);
        Assert.True(time < 0.5);
    }

    [Fact]
    public void TestGetDeviceList()
    {
        var defaultDevice = Cu.GetDefaultDevice().ToString();
        var deviceEnumerator = Cu.GetDeviceList();
        IList<(int, string)> devices = new List<(int, string)>();

        while (deviceEnumerator.MoveNext())
        {
            devices.Add(deviceEnumerator.Current);
        }

        Assert.True(devices.Any(d => d.Item2 == defaultDevice));
    }
}