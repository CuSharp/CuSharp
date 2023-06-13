using CuSharp.Tests.TestHelper;
using CuSharp.Tests.TestKernels;
using Xunit;

namespace CuSharp.Tests.Integration;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.Integration)]
public class KernelToolsTests
{
    [Theory]
    [InlineData(1, 1, 1)]
    [InlineData(1, 2, 3)]
    [InlineData(10, 10, 10)]
    public void GetBlockDimensions_Integration(uint blockDimX, uint blockDimY, uint blockDimZ)
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        var blockDims = new uint[3];
        var expectedBlockDims = new [] { blockDimX, blockDimY, blockDimZ };
        var devBlockDims = dev.Copy(blockDims);
        
        // Act
        dev.Launch(KernelToolsKernels.GetBlockDimensions, (1, 1, 1), (blockDimX, blockDimY, blockDimZ), devBlockDims);
        blockDims = dev.Copy(devBlockDims);
        devBlockDims.Dispose();
        dev.Dispose();

        // Assert
        Assert.Equal(expectedBlockDims, blockDims);
    }

    [Theory]
    [InlineData(1, 1, 1)]
    [InlineData(1, 2, 3)]
    [InlineData(10, 10, 10)]
    public void GetGridDimensions_Integration(uint gridDimX, uint gridDimY, uint gridDimZ)
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        var blockDims = new uint[3];
        var expectedBlockDims = new[] { gridDimX, gridDimY, gridDimZ };
        var devBlockDims = dev.Copy(blockDims);

        // Act
        dev.Launch(KernelToolsKernels.GetGridDimensions, (gridDimX, gridDimY, gridDimZ), (1, 1, 1), devBlockDims);
        blockDims = dev.Copy(devBlockDims);
        devBlockDims.Dispose();
        dev.Dispose();

        // Assert
        Assert.Equal(expectedBlockDims, blockDims);
    }

    [Fact]
    public void GetWarpSize_Integration()
    {
        // Arrange
        var dev = Cu.GetDefaultDevice();
        var warpSize = new uint[1];
        var expectedWarpSize = new uint[] { 32 };
        var devWarpSize = dev.Copy(warpSize);

        // Act
        dev.Launch(KernelToolsKernels.GetWarpSize, (1, 1, 1), (1, 1, 1), devWarpSize);
        warpSize= dev.Copy(devWarpSize);
        devWarpSize.Dispose();
        dev.Dispose();

        // Assert
        Assert.Equal(expectedWarpSize, warpSize);
    }
}