using CuSharp.Tests.TestHelper;
using CuSharp.Tests.TestKernels;
using Xunit;

namespace CuSharp.Tests.Integration;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.Integration)]
public class ControlFlowTests
{
    [Fact]
    public void Int_Switch_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const int a = 1;
        const int b = 54321;
        int[] result = new int[1];
        const int expectedResult = a + b;
        var devResult = dev.Copy(result);

        // Act
        dev.Launch<int, int, int[]>(ControlFlowKernels.Switch, (1, 1, 1), (1, 1, 1), a, b, devResult);
        result = dev.Copy(devResult);
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void Int_For_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const int a = 1;
        const int b = 321;
        int[] result = new int[a];
        int[] expectedResult = new int[a];
        for (int i = 0; i < a; i++) { expectedResult[i] = a + b + i; }
        var devResult = dev.Copy(result);

        // Act
        dev.Launch<int, int, int[]>(ControlFlowKernels.For, (1, 1, 1), (1, 1, 1), a, b, devResult);
        result = dev.Copy(devResult);
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void Int_While_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const int a = 1;
        const int b = 321;
        int[] result = new int[a];
        int expectedResult = b;
        var devResult = dev.Copy(result);

        // Act
        dev.Launch<int, int, int[]>(ControlFlowKernels.While, (1, 1, 1), (1, 1, 1), a, b, devResult);
        result = dev.Copy(devResult);
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void Int_DoWhile_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const int a = 1;
        const int b = 321;
        int[] result = new int[a];
        int expectedResult = b;
        var devResult = dev.Copy(result);

        // Act
        dev.Launch<int, int, int[]>(ControlFlowKernels.DoWhile, (1, 1, 1), (1, 1, 1), a, b, devResult);
        result = dev.Copy(devResult);
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void Int_WhileWithBreak_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const int a = 1;
        const int b = 321;
        const int c = 0;
        int[] result = new int[a];
        int expectedResult = a + 1;
        var devResult = dev.Copy(result);

        // Act
        dev.Launch<int, int, int, int[]>(ControlFlowKernels.WhileWithBreak, (1, 1, 1), (1, 1, 1), a, b, c, devResult);
        result = dev.Copy(devResult);
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void Int_WhileWithContinue_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const int a = 1;
        const int b = 321;
        const int c = 0;
        int[] result = new int[a];
        int expectedResult = b;
        var devResult = dev.Copy(result);

        // Act
        dev.Launch<int, int, int, int[]>(ControlFlowKernels.WhileWithContinue, (1, 1, 1), (1, 1, 1), a, b, c, devResult);
        result = dev.Copy(devResult);
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }

    [Fact]
    public void Int_Goto_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        const int a = 1;
        const int b = 321;
        int[] result = new int[a];
        int expectedResult = b - 1;
        var devResult = dev.Copy(result);

        // Act
        dev.Launch<int, int, int[]>(ControlFlowKernels.Goto, (1, 1, 1), (1, 1, 1), a, b, devResult);
        result = dev.Copy(devResult);
        devResult.Dispose();

        // Assert
        Assert.Equal(expectedResult, result[0]);
    }
}
