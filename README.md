
![logo_crop](https://github.com/dotnet4GPU/CuSharp/assets/36261505/f9f16dc4-15a6-41bb-8b97-9629fd8e6910)  
  
A GPU Compute Framework for .NET  
  
[![CuSharp-Build](https://github.com/dotnet4GPU/CuSharp/actions/workflows/cusharp-build.yml/badge.svg)](https://github.com/dotnet4GPU/CuSharp/actions/workflows/cusharp-build.yml)
[![CuSharp-Test](https://github.com/dotnet4GPU/CuSharp/actions/workflows/cusharp-test.yml/badge.svg)](https://github.com/dotnet4GPU/CuSharp/actions/workflows/cusharp-test.yml)

# The Thesis
This project was created as a Bachelors-thesis at the University of Applied Sciences of Eastern Switzerland (OST). The main-document of the thesis, describing this project in detail can be found [here (UPDATEME)](TOBEANNOUNCED).

# Project Layout
- CuSharp: All parts of the frontend of the framework.
- CuSharp.AOTC: An executable, used to AOT-compile C#-methods to PTX-Kernels
- CuSharp.CrossCompiler: The crosscompiler compiling MSIL-opcodes to PTX instructions
- CuSharp.NVVMBinder: Bindings for libNVVM
- CuSharp.PerformanceEvaluation: Examples used to evaluate the performance of the framework
- CuSharp.Tests: Unit and integration tests used to test the functionality of the framework
- CuSharp.MandelbrotExample: An example WPF application, generating mandelbrot-sets, using CuSharp

# Nuget-Packages:
- To be announced

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
- [CUDA Toolkit](https://developer.nvidia.com/cuda-toolkit)
- [LLVMSharp](https://github.com/dotnet/LLVMSharp)
- [ManagedCUDA](https://github.com/kunzmi/managedCuda)
