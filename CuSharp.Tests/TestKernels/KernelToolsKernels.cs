using CuSharp.Kernel;

namespace CuSharp.Tests.TestKernels;

public static class KernelToolsKernels
{
    public static void GetBlockDimensions(uint[] blockDims)
    {
        blockDims[0] = KernelTools.BlockDimension.X;
        blockDims[1] = KernelTools.BlockDimension.Y;
        blockDims[2] = KernelTools.BlockDimension.Z;
    }

    public static void GetGridDimensions(uint[] gridDims)
    {
        gridDims[0] = KernelTools.GridDimension.X;
        gridDims[1] = KernelTools.GridDimension.Y;
        gridDims[2] = KernelTools.GridDimension.Z;
    }

    public static void GetWarpSize(uint[] warpSize)
    {
        warpSize[0] = KernelTools.WarpSize;
    }
}