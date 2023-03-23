using LibNVVMBinder;

namespace CuSharp.Tests.TestHelper
{
    public class TestValidator
    {
        public bool KernelIsCorrect(string llvmKernel, string kernelName)
        {
            var nvvm = new NVVMProgram();
            nvvm.AddModule(llvmKernel, kernelName);
            var result = nvvm.Verify(new string[] { });
            return result == NVVMProgram.NVVMResult.NVVM_SUCCESS;
        }
    }
}
