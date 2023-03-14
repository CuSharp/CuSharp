namespace CuSharp.Tests.CuSharp.CudaCompiler.KernelCrossCompilerTests
{
    public class MethodsToCompile
    {
        public void ScalarIntAdditionWithConst(int a, int b)
        {
            int c = 12345;
            int d = a + b + c;
        }

        public void ScalarIntAddition(int a, int b)
        {
            int c = a + b;
        }

        public void ScalarIntSubtraction(int a, int b)
        {
            int c = a - b;
        }

        public void ScalarIntMultiplication(int a, int b)
        {
            int c = a * b;
        }

        public void ScalarFloatAddition(float a, float b)
        {
            float c = a + b;
        }

        public void ScalarFloatSubtraction(float a, float b)
        {
            float c = a - b;
        }

        public void ScalarFloatMultiplication(float a, float b)
        {
            float c = a * b;
        }


        public void ArrayIntAddition(int[] a, int[] b, int[] c)
        {
            int i = 0;
            c[i] = a[i] + b[i];
        }

        public void ArrayFloatAddition(float[] a, float[] b, float[] c)
        {
            int i = 0;
            c[i] = a[i] + b[i];
        }
    }
}
