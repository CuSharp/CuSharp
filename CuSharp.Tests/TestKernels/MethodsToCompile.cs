﻿using System.Numerics;
using CuSharp.CudaCompiler.LLVMConfiguration;
using CuSharp.Kernel;

namespace CuSharp.Tests.TestKernels;

public class MethodsToCompile
{
    public void NonStaticEmptyMethod()
    {
    }

    public static void EmptyMethod()
    {
    }

    public static void EmptyMixedParameterMethod(int[] A, int[] B, bool b, int c)
    {
    }

    public static void EmptyIntArrayMethod(int[] a)
    {
    }

    public static void EmptyTwoIntArrayMethod(int[] a, int[] b)
    {
    }

    public static void ScalarIntAddition5Args(int a, int b, int c, int d, int e)
    {
        e += a + b + c + d;
    }

    public static void ScalarIntAdditionWithConst(int a, int b)
    {
        int c = 12345;
        int d = 0;
        int e = a + b + c + d + -1 + 1 + 2 + 3 + 4 + 5 + 6 + 7 + 8;
    }

    public static void ScalarLongAdditionWithConst(long a, long b)
    {
        long c = 1234567890987;
        long d = a + b + c;
    }

    public static void ScalarIntAddition(int a, int b)
    {
        int c = a + b;
    }

    public static void ScalarIntSubtraction(int a, int b)
    {
        int c = a - b;
    }

    public static void ScalarIntMultiplication(int a, int b)
    {
        int c = a * b;
    }

    public static void ScalarIntDivision(int a, int b)
    {
        int c = a / b;
    }

    public static void ScalarIntRemainder(int a, int b)
    {
        int c = a % b;
    }

    public static void ScalarFloatAdditionWithConst(float a, float b)
    {
        float c = 1234.321F;
        float d = a + b + c;
    }

    public static void ScalarDoubleAdditionWithConst(double a, double b)
    {
        double c = 123456.54321;
        double d = a + b + c;
    }

    public static void ScalarFloatAddition(float a, float b)
    {
        float c = a + b;
    }

    public static void ScalarFloatSubtraction(float a, float b)
    {
        float c = a - b;
    }

    public static void ScalarFloatMultiplication(float a, float b)
    {
        float c = a * b;
    }

    public static void ScalarFloatDivision(float a, float b)
    {
        float c = a / b;
    }

    public static void ScalarFloatRemainder(float a, float b)
    {
        float c = a % b;
    }

    [Kernel]
    public static void ArrayIntAddition(int[] a, int[] b, int[] c)
    {
        int i = 0;
        c[i] = a[i] + b[i];
    }

    public static void ArrayFloatAddition(float[] a, float[] b, float[] c)
    {
        int i = 0;
        c[i] = a[i] + b[i];
    }

    public static void ArrayIntAdditionWithKernelTools(int[] a, int[] b, int[] c)
    {
        int i = (int)(KernelTools.BlockIndex.X * KernelTools.BlockDimension.X + KernelTools.ThreadIndex.X);
        c[i] = a[i] + b[i];
    }

    [Kernel]
    public static void ArrayIntMultiplicationWithKernelTools(int[] a, int[] b, int[] c)
    {
        int i = (int)(KernelTools.BlockIndex.X * KernelTools.BlockDimension.X + KernelTools.ThreadIndex.X);
        c[i] = a[i] * b[i];
    }
    
    [Kernel]
    public static void AOTCArrayIntAddition(int[] a, int[] b, int[] c)
    {
        //Empty because aotc
    }

    public static void ArrayFloatAdditionWithKernelTools(float[] a, float[] b, float[] c)
    {
        int i = (int)(KernelTools.BlockIndex.X * KernelTools.BlockDimension.X + KernelTools.ThreadIndex.X);
        c[i] = a[i] + b[i];
    }

    public static void ArrayIntScalarAdd(int[] a, int b)
    {
        int i = (int)(KernelTools.BlockDimension.X * KernelTools.BlockIndex.X + KernelTools.ThreadIndex.X);
        a[i] = a[i] + b;
    }

    public static void ArrayIntShortHandOperationsWithKernelTools(int[] a, int[] b)
    {
        int i = (int)(KernelTools.BlockIndex.X * KernelTools.BlockDimension.X + KernelTools.ThreadIndex.X);
        a[i] += b[i];
        a[i] -= b[i];
        a[i] *= b[i];
        a[i] /= b[i];
        a[i] %= b[i];
    }

    public static void ArrayLongShortHandOperationsWithKernelTools(long[] a, long[] b)
    {
        int i = (int)(KernelTools.BlockIndex.X * KernelTools.BlockDimension.X + KernelTools.ThreadIndex.X);
        a[i] += b[i];
        a[i] -= b[i];
        a[i] *= b[i];
        a[i] /= b[i];
        a[i] %= b[i];
    }

    public static void ArrayFloatShortHandOperationsWithKernelTools(float[] a, float[] b)
    {
        int i = (int)(KernelTools.BlockIndex.X * KernelTools.BlockDimension.X + KernelTools.ThreadIndex.X);
        a[i] += b[i];
        a[i] -= b[i];
        a[i] *= b[i];
        a[i] /= b[i];
        a[i] %= b[i];
    }

    public static void ArrayDoubleShortHandOperationsWithKernelTools(double[] a, double[] b)
    {
        int i = (int)(KernelTools.BlockIndex.X * KernelTools.BlockDimension.X + KernelTools.ThreadIndex.X);
        a[i] += b[i];
        a[i] -= b[i];
        a[i] *= b[i];
        a[i] /= b[i];
        a[i] %= b[i];
    }

    public static void IntMatrixMultiplication(int[] a, int[] b, int[] c, int matrixWidth)
    {
        var row = KernelTools.BlockDimension.Y * KernelTools.BlockIndex.Y + KernelTools.ThreadIndex.Y;
        var col = KernelTools.BlockDimension.X * KernelTools.BlockIndex.X + KernelTools.ThreadIndex.X;
        int result = 0;
        if (row < matrixWidth && col < matrixWidth)
        {
            for (int i = 0; i < matrixWidth; i++)
            {
                result += a[matrixWidth * row + i] * b[i * matrixWidth + col];
            }

            c[row * matrixWidth + col] = result;
        }
    }

    public static void LogicalAnd(int a, int b, int c)
    {
        if (a == b && b == c)
        {
            a += 1;
        }

        c = a * b;
    }

    public static void LogicalOr(int a, int b, int c)
    {
        if (a == b || b == c)
        {
            a += 1;
        }

        c = a * b;
    }

    public static void BranchesWithInt32Target(uint a, uint b)
    {
        uint i = 0;
        if (a < b)
        {
            if (a <= b)
            {
                if (a > b)
                {
                    if (a >= b)
                    {
                        if (a == b)
                        {
                            if (a != b)
                            {
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                            }
                        }
                    }
                }
            }
        }
    }

    public static void BranchesWithIn32Target(int a, int b)
    {
        int i = 0;
        if (a < b)
        {
            if (a <= b)
            {
                if (a > b)
                {
                    if (a >= b)
                    {
                        if (a == b)
                        {
                            if (a != b)
                            {
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                                i += b;
                            }
                        }
                    }
                }
            }
        }
    }

    public static void BranchesInt8TargetUnsigned(uint a, uint b)
    {
        if (a < b)
        {
            if (a <= b)
            {
                if (a > b)
                {
                    if (a >= b)
                    {
                        if (a == b)
                        {
                            a += b;
                        }
                    }
                }
            }
        }
    }

    public static void Newarr(int[] b)
    {
        int[] a = new int[500];
        a[499] = b[4];
        b[0] = a[499];
    }

    const int size = 1;
    [Kernel(ArrayMemoryLocation.SHARED)]
    public static void SharedMemoryTestKernel(int[] a, int b)
    {
        
        var newA = new int[size];
        var newB = new int[size];
        newA[0] = a[0];
        newB[0] = b;
        b = newA[0] * newB[0];
        a[0] = b;
    }

    public static void TestScalars(int[] a, int b)
    {
        a[0] = b;
    }

    public static void ThreadFence(int[] a)
    {
        KernelTools.GlobalThreadFence();
        KernelTools.SystemThreadFence();
    }

    public static void IAmGeneric<T>(T[] a, T[] b) where T : INumber<T>
    {
        a[0] += b[0];
    }

    public static void GenericNewArray<T>(T[] a) where T : INumber<T>
    {
        var b = new T[5];
        b[0] = a[1];
        a[0] = b[0];
    }

    public static void GenericMultiDimNewArray<T>(T[,] a) where T : INumber<T>
    {
        var b = new T[5,6];
        b[0, 0] = a[0, 1];
        a[0, 0] = b[0, 0];
    }

    public static void NewArrayPassAsArgument(int[] a)
    {
        int[] b = new int[1];
        InitializeArray(b);
        a[0] = b[0];
    }

    public static void InitializeArray(int[] b)
    {
        b[0] = 42;
    }
    public static void New2DArrayPassAsArgument(int[,] a)
    {
        int[,] b = new int[1,1];
        Initialize2DArray(b);
        a[0,0] = b[0,0];
    }

    public static void Initialize2DArray(int[,] b)
    {
        b[0,0] = 42;
    }
}
