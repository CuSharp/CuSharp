
![logo_crop](./resources/logo_crop.png)  
  
A GPU Compute Framework for .NET  
  
[![CuSharp-Build](https://github.com/CuSharp/CuSharp/actions/workflows/cusharp-build.yml/badge.svg)](https://github.com/dotnet4GPU/CuSharp/actions/workflows/cusharp-build.yml)
[![CuSharp-Test](https://github.com/CuSharp/CuSharp/actions/workflows/cusharp-test.yml/badge.svg)](https://github.com/dotnet4GPU/CuSharp/actions/workflows/cusharp-test.yml)

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
- [Main CuSharp Package](https://www.nuget.org/packages/CuSharp)
- [Crosscompiler](https://www.nuget.org/packages/CuSharp.CrossCompiler)
- [NVVM Bindings](https://www.nuget.org/packages/CuSharp.CrossCompiler)

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
  var device = Cu.GetDefaultDevice();

  var arrayA = new int [] {1 ,2 ,3};
  var arrayB = new int [] {4 ,5 ,6};
  var deviceArrayA = device.Copy(arrayA);
  var deviceArrayB = device.Copy(arrayB);
  var deviceResultArray = device.Allocate<int>(3);
  var gridDimensions = (1,1,1);
  var blockDimensions = (3,1,1);
  device.Launch(IntAdditionKernel, gridDimensions, blockDimensions, deviceArrayA, deviceArrayB, deviceResultArray);
  var arrayResult = device.Copy(deviceResultArray);
}
```

## Matrix Multiplication Kernel
```C#
[Kernel]
public static void MatrixMultiplication<T>(T[] a, T[] b, T[] c, int matrixWidth) where T : INumber<T>, new()
{
    var row = KernelTools.BlockDimension.Y * KernelTools.BlockIndex.Y + KernelTools.ThreadIndex.Y;
    var col = KernelTools.BlockDimension.X * KernelTools.BlockIndex.X + KernelTools.ThreadIndex.X;
    T result = new T(); 
    if (row < matrixWidth && col < matrixWidth)
    {
        for (int i = 0; i < matrixWidth; i++)
        {
            //KernelTools.SyncThreads();
            result += a[matrixWidth * row + i] * b[i * matrixWidth + col];
        }

        c[row * matrixWidth + col] = result;
    }
}
```

## Matrix Multiplication Kernel using Shared Memory
```C#
[Kernel(ArrayMemoryLocation.SHARED)]
public static void TiledIntMatrixMultiplication<T>(T[] a, T[] b, T[] c, int matrixWidth, int tileWidth, int nofTiles) where T : INumber<T>, new()
{
    var tx = KernelTools.ThreadIndex.X;
    var ty = KernelTools.ThreadIndex.Y;
    var col = KernelTools.BlockIndex.X * tileWidth + tx;
    var row = KernelTools.BlockIndex.Y * tileWidth + ty;

    var aSub = new T[1024];
    var bSub = new T[1024];

    T sum = new T();
    for (int tile = 0; tile < nofTiles; tile++)
    {
        if (row < matrixWidth && tile * tileWidth + tx < matrixWidth)
        {
            aSub[ty * tileWidth + tx] = a[row * matrixWidth + tile * tileWidth + tx];
        }

        if (col < matrixWidth && tile * tileWidth + ty < matrixWidth)
        {
            bSub[ty * tileWidth + tx] = b[(tile * tileWidth + ty) * matrixWidth + col];
        }

        KernelTools.SyncThreads();

        if (row < matrixWidth && col < matrixWidth)
        {
            for (int ksub = 0; ksub < tileWidth; ksub++)
            {
                if (tile * tileWidth + ksub < matrixWidth)
                {
                    sum += aSub[ty * tileWidth + ksub] * bSub[ksub * tileWidth + tx];
                }
            }
        }
        KernelTools.SyncThreads();
    }
    if (row < matrixWidth && col < matrixWidth)
    {
        c[row * matrixWidth + col] = sum;
    }
}
```

## Complete Examples
More complete examples can be found in the following project directories:
- CuSharp.MandelbrotExample: A WPF-Project visualizing Mandelbrot-sets using CuSharp
- CuSharp.PerformanceEvaluation: A console-application measuring the performance of matrix-multiplications

# API
## Static Class: Cu
### Properties
- `bool EnableOptimizer`: Enables or disables the built-in optimizer. Default: True (in Debug mode), False (in Release mode).
- `string AotKernelFolder`: Specifies the folder where the framework should look for kernels that were ahead-of-time compiled.

### Static Methods
- `IEnumerator<(int, string)> GetDeviceList()`: Returns a list of pairs of device-id and device-name.
- `CuDevice GetDefaultDevice()`: Returns a handle for the device with ID: 0.
- `CuDevice GetDeviceById(int deviceId)`: Returns a for the device with ID: `deviceId`.
- `CuEvent CreateEvent()`: Returns a handle to a Cuda-Event used to measure performance.

## Class: CuDevice
- Implements IDisposable
### Methods
- `string ToString()`: Returns the devices name.
- `void Synchronize()`: Blocks until all tasks on the device are finished.
- `Tensor<T[]> Allocate<T>(int size)`: Allocates an array of `size` elements on the device and returns its handle.
- `Tensor<T[,]> Allocate<T>(int sizeX, int sizeY)`:  allocates a 2D-array of size `sizeX` * `sizeY` on the device and returns its handle.
- `Tensor<T[]> Copy<T>(T[] hostTensor)`: Copies `hostTensor` to the device and returns a handle to the copied array.
- `Tensor<T[,]> Copy<T>(T[,] hostTensor)`: Copies `hostTensor` to the device and returns a handle to the copied array.
- `Tensor<T> CreateScalar<T>(T hostScalar)`: Copies `hostScalar` to the device and returns a handle to the copied value.
- `T[] Copy<T>(Tensor<T[]> deviceTensor)`: Copies `deviceTensor` from the device and returns the array.
- `T[,] Copy<T>(Tensor<T[,]> deviceTensor)`: Copies `deviceTensor` from the device and returns the 2D-array.
- `void Launch<T1, ..., TN>(Action<T1, ..., TN> kernel, (uint,uint,uint) gridDimensions, (uint,uint,uint) blockDimensions, Tensor<T1> param1, ... , Tensor<TN> paramN)`: JIT-compiles (if needed) and launches `kernel` on the device with the specified dimensions and `Tensor<T>`-parameters.
- `void Dispose()`: Disposes all allocated ressources of the device-handle.

## Interface: ICuEvent
- Implements IDisposable
### Methods
- `void Record()`: Records the point in time this method-was called relative to the GPU-Runtime.
- `float GetDeltaTo(CuEvent event)`: Returns the time delta between `this` CuEvent and `event`.
- `void Dispose()`: Disposes all allocated ressources of the event-handle.

## Static Class: KernelTools
- A class to be used inside the kernel to access GPU-capabilities.
- The properties below are compiled to NVVM intrinsic functions. The properties all point to a corresponding functor that by default throws an exception. The corresponding functors can be overriden to repurpose the KernelTools Properties.
### Properties (to be used only inside kernels)
- `(uint X, uint Y, uint Z) GridDimension`: Returns the grid dimensions of the current kernel launch.
- `(uint X, uint Y, uint Z) BlockDimension`: Returns the block dimension of the current kernel launch.
- `(uint X, uint Y, uint Z) BlockIndex`: Returns the block index inside the grid.
- `(uint X, uint Y, uint Z) ThreadIndex`: Returns the thread index relative to the threads block.
- `uint WarpSize`: Returns the warpsize of the executing device.
- `Action SyncThreads`: Waits until all threads inside the current block reach this point when called.
- `Action GlobalThreadFence`: Halts until all writes to global and shared memory of the current thread are visible to other threads when called.
- `Action SystemThreadFence`: Halts until all writes (system wide) of the current threaad are visible to other threads when called.  

# Language Specification

This chapter describes the C# subset that is supported by CuSharp.

## Supported C# Features

### Supported Types

| **C# Type** | **LLVM Type** | **Description**                                                                         |
|:------------|:--------------|:----------------------------------------------------------------------------------------|
| `void`      | `void`          | Void, represents the absence of a value                                                 |
| `bool`      | `i1`          | Boolean value, aligned to 1 byte                                                        |
| `byte`      | `i8`          | Byte value, aligned to 1 byte                                                           |
| `short`     | `i16`         | Short value, aligned to 2 bytes                                                         |
| `int`       | `i32`         | Integer value, aligned to 4 bytes                                                       |
| `uint`      | `i32`         | Unsigned integer value, aligned to 4 bytes                                              |
| `long`      | `i64`         | Long integer value, aligned to 8 bytes                                                  |
| `float`     | `float`       | Floating point value, aligned to 4 bytes                                                |
| `double`    | `double`      | Double floating point value, aligned to 8 bytes                                         |
| `T[]`       | `T*`          | Array of numeric elements                                                               |
| `T[,]`      | `T**`         | Two dimensional array of numeric elements, may be slower than single dimensional arrays |

### Generic Types

Generic types can be used in a kernel. For mathematical calculations the
constraint `INumber<T>` can be used. Allocations of type T within the
kernel are enabled with the constraint `new()`. `new()` will initialize
a variable with its default value. A possible kernel method signature
with generics can look like this:

`public static void KernelName<T>(T param) where T : INumber<T>, new()`

### Control Flow Statements

The following control flow statements are supported. They can be nested.

| **Statement**      | **Description**                                                                                                                                                                              |
|:-------------------|:---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| if-else statement  | Used for conditional branching based on the evaluation of a Boolean expression.                                                                                                              |
| switch statement   | Used for selecting one of several possible code blocks to execute based on the value of an expression.                                                                                       |
| for loop           | Used for executing a code block a specific number of times.                                                                                                                                  |
| while loop         | Used for executing a code block repeatedly as long as a Boolean expression evaluates to true.                                                                                                |
| do-while loop      | Used for executing a code block repeatedly as long as a Boolean expression evaluates to true, but the block is executed at least once regardless of the initial evaluation of the expression |
| break statement    | Used for breaking out of a loop or switch statement.                                                                                                                                         |
| continue statement | Used for skipping the rest of the current iteration of a loop and moving on to the next iteration.                                                                                           |
| goto statement     | Used for jumping to a labeled statement in the code.                                                                                                                                         |

### Arithmetic Operators

All arithmetic operators are supported as expanded version (e.g.
`a = a + b`) and as shorthand version (e.g. `a += b`).

| **Sign** | **Name**       | **Description**                                                                 |
|:---------|:---------------|:--------------------------------------------------------------------------------|
| `+`      | Addition       | Adds two operands together.                                                     |
| `-`      | Subtraction    | Subtracts the right operand from the left operand.                              |
| `*`      | Multiplication | Multiplies two operands together.                                               |
| `/`      | Division       | Divides the left operand by the right operand.                                  |
| `%`        | Remainder      | Returns the remainder of the division of the left operand by the right operand. |


### Logical Operators

If possible the operators are evaluated short circuit.

| **Sign** | **Name**    | **Description**                                                                                                          |
|:---------|:------------|:-------------------------------------------------------------------------------------------------------------------------|
| `&&`     | Logical AND | Returns true if both operands are true, otherwise returns false.                                                         |
| `\|\|`     | Logical OR  | Returns true if at least one operand is true, otherwise returns false.                                                   |
| `!`      | Logical NOT | Reverses the logical state of its operand. If the operand is true, returns false. If the operand is false, returns true. |

Supported Logical Operators.

### Bitwise Operators

| **Sign** | **Name**             | **Description**                                                                                                                                                                  |
|:---------|:---------------------|:---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `&`      | Bitwise AND          | Performs a bitwise AND operation between two operands, resulting in a new value where every bit is set to 1 if both bits of the operands are the same, otherwise it is set to 0. |
| `\|`      | Bitwise OR           | Performs a bitwise OR operation between two operands, resulting in a new value where every bit is set to 1 if at least one bit is 1, otherwise it is set to 0.                   |
| `^`      | Bitwise XOR          | Performs a bitwise XOR operation between two operands, resulting in a new value where every bit is set to 1 if the bits of the operands are different, otherwise it is set to 0. |
| `~`      | Bitwise NOT          | Flips every bit.                                                                                                                                                                 |
| `<<`     | Shift Left           | Shifts the bits of an operand to the left by a specified number of positions.                                                                                                    |
| `>>`     | Signed Right Shift   | Sifts the bits of an operand to the right by a specified number of positions.                                                                                                    |
| `>>`     | Unsigned Right Shift | Shifts the bits of an operand to the right by a specified number of positions and fills the shifted positions with zeros.                                                        |

### Comparison Operators
| **Sign** | **Name**                 | **Description**                                                                 |
|:---------|:-------------------------|:--------------------------------------------------------------------------------|
| `<`      | Less than                | Returns true if the left operand is less than the right operand.                |
| `<=`     | Less than or equal to    | Returns true if the left operand is less than or equal to the right operand.    |
| `>`      | Greater than             | Returns true if the left operand is greater than the right operand.             |
| `>=`     | Greater than or equal to | Returns true if the left operand is greater than or equal to the right operand. |
| `==`     | Equal to                 | Returns true if the left and right operands are equal.                          |
| `!=`     | Not equal to             | Returns true if the left and right operands are not equal.                      |

### Calls
#### Kernel Tools
The following table lists the kernel tool functions and variables with
there CUDA C++ equivalent.

| **C# Property**                       | **CUDA C++ Equivalent**  |
|:--------------------------------------|:-------------------------|
| `KernelTools.GridDimension.[X\|Y\|Z]`   | `gridDim.[x\|y\|z]`        |
| `KernelTools.BlockIndex.[X\|Y\|Z]`      | `blockIdx.[x\|y\|z]`       |
| `KernelTools.ThreadIndex.[X\|Y\|Z]`     | `threadIdx.[x\|y\|z]`      |
| `KernelTools.BlockDimensions.[X\|Y\|Z]` | `blockDim.[x\|y\|z]`       |
| `KernelTools.SyncThreads()`           | `__syncthreads()`        |
| `KernelTools.GlobalThreadFence()`     | `__threadfence()`        |
| `KernelTools.SystemThreadFence()`     | `__threadfence_system()` |
| `KernelTools.WarpSize`                | `warpSize`               |

#### Nested Calls

Static methods can be called from a kernel. These methods can have a
numeric return type (primitive and as array) or void. The method calls
can be nested over several levels. Primitive data types or numeric
arrays can be passed as parameters. The primitive data types are passed
as *call by value*, the arrays are passed as *call by reference*.

### Implicit Casts

All implicit casts in C# such as `int` to `long` work.

## Unsupported Features

All features that are not described in the preceding chapters are
generally unsupported. It may be that individual functionalities work
anyway, but they have not been tested or checked for side effects.

There are some C# functions that one would expect to work, but that are
unsupported at this time. These are, including but not limited to:

-   Array length property
-   Array initialization using an array initializer with values instead
    of parameters and variables (e.g. `var a = new int[]{1,2,3};`)
-   Foreach loop (due to the missing array length property)
-   Any calls to non-static methods and non-intrinsic functions
-   Anonymous functions i.e. lambdas
-   LINQ queries (query syntax and method syntax)
-   Data types other than primitives
-   Native int
-   Overflow checks by using the *checked* keyword
-   …

# Dependencies
- [CUDA Toolkit](https://developer.nvidia.com/cuda-toolkit)
- [LLVMSharp](https://github.com/dotnet/LLVMSharp)
- [ManagedCUDA](https://github.com/kunzmi/managedCuda)
