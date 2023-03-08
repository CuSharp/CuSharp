using System.Reflection;
using CuSharp.CudaCompiler;
using ManagedCuda;

namespace CuSharp
{
    public class DeviceContext
    {
        private CudaContext cudaDeviceContext = new CudaContext();
        private static KernelDiscovery kernelMethodDiscovery = new KernelDiscovery();
        public DeviceContext()
        {
            
        }

        public static PTXKernel CompileKernel(string methodName)
        {
            return new PTXKernel();
        }


        public void Launch<T>(PTXKernel kernel, params T[] parameters) where T : struct
        {
            var cudaKernel = cudaDeviceContext.LoadKernelPTX(kernel.Bytes, kernel.Name);
            cudaKernel.Run(parameters);
        }
        
        public Tensor<T> Copy<T>(T[] hostTensor) where T : struct
        {
            CudaDeviceVariable<T> deviceVariable = hostTensor;
            return new CudaTensor<T>(deviceVariable);
        }

        public T[] Copy<T>(Tensor<T> deviceTensor) where T : struct
        {
            var cudaDeviceTensor = deviceTensor as CudaTensor<T>;
            return cudaDeviceTensor.DeviceVariable;
        }
    }
}