using System.Runtime.CompilerServices;
using CuSharp.Kernel;

namespace CuSharp.Tests.TestHelper
{
    public class MethodsToCompile
    {
        public void NonStaticEmptyMethod() { }

        public static void EmptyMethod() { }

        public static void EmptyMixedParameterMethod(int[] A, int[] B, bool b, int c) { }

        public static void EmptyIntArrayMethod(int[] a) { }

        public static void EmptyTwoIntArrayMethod(int[] a, int[] b) { }

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
            int i = (int)(KernelTools.BlockIndex.Item1 * KernelTools.BlockDimensions.Item1 + KernelTools.ThreadIndex.Item1);
            c[i] = a[i] + b[i];
        }

        public static void ArrayFloatAdditionWithKernelTools(float[] a, float[] b, float[] c)
        {
            int i = (int)(KernelTools.BlockIndex.Item1 * KernelTools.BlockDimensions.Item1 + KernelTools.ThreadIndex.Item1);
            c[i] = a[i] + b[i];
        }
        
        public static void ArrayIntScalarAdd(int[] a, int b)
        {
            int i = (int)(KernelTools.BlockDimensions.Item1 * KernelTools.BlockIndex.Item1 + KernelTools.ThreadIndex.Item1);
            a[i] = a[i] + b;
        }

        public static void ArrayIntShortHandOperationsWithKernelTools(int[] a, int[] b)
        {
            int i = (int)(KernelTools.BlockIndex.Item1 * KernelTools.BlockDimensions.Item1 + KernelTools.ThreadIndex.Item1);
            a[i] += b[i];
            a[i] -= b[i];
            a[i] *= b[i];
            a[i] /= b[i];
            a[i] %= b[i];
        }

        public static void ArrayLongShortHandOperationsWithKernelTools(long[] a, long[] b)
        {
            int i = (int)(KernelTools.BlockIndex.Item1 * KernelTools.BlockDimensions.Item1 + KernelTools.ThreadIndex.Item1);
            a[i] += b[i];
            a[i] -= b[i];
            a[i] *= b[i];
            a[i] /= b[i];
            a[i] %= b[i];
        }

        public static void ArrayFloatShortHandOperationsWithKernelTools(float[] a, float[] b)
        {
            int i = (int)(KernelTools.BlockIndex.Item1 * KernelTools.BlockDimensions.Item1 + KernelTools.ThreadIndex.Item1);
            a[i] += b[i];
            a[i] -= b[i];
            a[i] *= b[i];
            a[i] /= b[i];
            a[i] %= b[i];
        }

        public static void ArrayDoubleShortHandOperationsWithKernelTools(double[] a, double[] b)
        {
            int i = (int)(KernelTools.BlockIndex.Item1 * KernelTools.BlockDimensions.Item1 + KernelTools.ThreadIndex.Item1);
            a[i] += b[i];
            a[i] -= b[i];
            a[i] *= b[i];
            a[i] /= b[i];
            a[i] %= b[i];
        }

        public static void NotSupportedNestedCall(int a, int b)
        {
            ScalarIntAddition(a, b);
        }

        public static void IntMatrixMultiplication(int[] a, int[] b, int[] c, int matrixWidth)
        {
            var row = KernelTools.BlockDimensions.Item2 * KernelTools.BlockIndex.Item2 + KernelTools.ThreadIndex.Item2;
            var col = KernelTools.BlockDimensions.Item1 * KernelTools.BlockIndex.Item1 + KernelTools.ThreadIndex.Item1;
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
    }
}
