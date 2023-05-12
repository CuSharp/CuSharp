using System;
using CuSharp.Tests.TestHelper;
using Xunit;

namespace CuSharp.Tests.Integration;

public class ParameterReassignIntegrationTests
{
    
    [Fact]
    public void TestArrayReassign()
    {
        var result = RunStandardizedTestInt(ParameterReassignKernels.ArrayReassign, new[] {42}, new[] {1337}, 666);
        Assert.Equal(666, result.a[0]);
    }

    [Fact]
    public void TestArrayReassignBranchedC()
    {
        var result = RunStandardizedTestInt(ParameterReassignKernels.ArrayReassignIfBranchedC, new[] {42}, new[] {1337}, 101);
        Assert.Equal(102, result.a[0]);
    }
    
    [Fact]
    public void TestArrayReassignBranchedC2()
    {
        var result = RunStandardizedTestInt(ParameterReassignKernels.ArrayReassignIfBranchedC, new[] {42}, new[] {1337}, 99);
        Assert.Equal(101, result.a[0]);
    }

    [Fact]
    public void TestBranchedArrayReassign()
    {
        var result = RunStandardizedTestInt(ParameterReassignKernels.BranchedArrayReassign, new[] {42}, new[] {1337}, 101);
        Assert.Equal(101, result.a[0]);
    }

    [Fact]
    public void TestBranchedArrayReassign2()
    {
        var result = RunStandardizedTestInt(ParameterReassignKernels.BranchedArrayReassign, new[] {42}, new[] {1337}, 99);
        Assert.Equal(42, result.a[0]);
    }

    [Fact]
    public void TestLocalArrayReassign()
    {
        var result = RunStandardizedTestInt(ParameterReassignKernels.LocalArrayVariables, new[] {42}, new[] {1337}, 101);
        Assert.Equal(42, result.a[0]);
        Assert.Equal(101, result.b[0]);
    }
    
    [Fact]
    public void TestLocalArrayReassign2()
    {
        var result = RunStandardizedTestInt(ParameterReassignKernels.LocalArrayVariables, new[] {42}, new[] {1337}, 99);
        Assert.Equal(99, result.a[0]);
        Assert.Equal(1337, result.b[0]);
    }
    
    [Fact]
    public void TestAssignLocalVarToArg()
    {
        var result = RunStandardizedTestInt(ParameterReassignKernels.AssignLocalVariableToArg, new[] {42}, new[] {1337}, 101);
        Assert.Equal(101, result.a[0]);
        Assert.Equal(1337, result.b[0]);
    }
    
    [Fact]
    public void TestAssignLocalVarToArg2()
    {
        var result = RunStandardizedTestInt(ParameterReassignKernels.AssignLocalVariableToArg, new[] {42}, new[] {1337}, 99);
        Assert.Equal(42, result.a[0]);
        Assert.Equal(99, result.b[0]);
    }
    
    private (int[] a, int[] b) RunStandardizedTestInt(Action<int[],int[], int> methodToRun, int[] arrayA, int[] arrayB, int scalar)
    {
        var dev = global::CuSharp.CuSharp.GetDefaultDevice();
        var devA = dev.Copy(arrayA);
        var devB = dev.Copy(arrayB);
        dev.Launch<int[],int[],int>(methodToRun, (1,1,1),(1,1,1), devA, devB, scalar);
        return (dev.Copy(devA), dev.Copy(devB));
    }
}