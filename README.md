
![logo_crop](https://github.com/dotnet4GPU/CuSharp/assets/36261505/f9f16dc4-15a6-41bb-8b97-9629fd8e6910)  
  
A GPU Compute Framework for .NET  
  
[![CuSharp-Build](https://github.com/dotnet4GPU/CuSharp/actions/workflows/cusharp-build.yml/badge.svg)](https://github.com/dotnet4GPU/CuSharp/actions/workflows/cusharp-build.yml)
[![CuSharp-Test](https://github.com/dotnet4GPU/CuSharp/actions/workflows/cusharp-test.yml/badge.svg)](https://github.com/dotnet4GPU/CuSharp/actions/workflows/cusharp-test.yml)
# Examples
## Add two int arrays
```C#
[Kernel]
static void IntAdditionKernel (int[] a , int[] b , int[] result)
{
  int index = KernelTools.BlockIndex.X * KernelTools.BlockDimensions.X + KernelTools.ThreadIndex.X;
  result[index] = a[index] + b[index];
}

public void Launch()
{
  var device = CuSharp.GetDefaultDevice();

  var arrayA = new int [] {1 ,2 ,3};
  var arrayB = new int [] {4 ,5 ,6};
  var deviceArrayA = device.Copy(arrayA);
  var deviceArrayB = device.Copy(arrayB);
  var deviceResultArray = device.Allocate(3);
  var gridDimensions = (1,1,1);
  var blockDimensions = (3,1,1);
  device.Launch(IntAdditionKernel, gridDimensions, blockDimensions, deviceArrayA, deviceArrayB, deviceResultArray);
  var arrayResult = device.Copy(deviceResultArray);
}
```

# Dependencies
- ![CUDA Toolkit](https://developer.nvidia.com/cuda-toolkit)
- ![LLVMSharp](https://github.com/dotnet/LLVMSharp)
- ![ManagedCUDA](https://github.com/kunzmi/managedCuda)
