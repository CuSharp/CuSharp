using System;
using System.Runtime.InteropServices;
using System.Text;

namespace LibNVVMBinder
{
    internal static class NVVMNativeBindings
    {
        
        private const string DLL_NAME = "nvvm64_40_0";
        static NVVMNativeBindings()
        {
            string? nvvmPath = Environment.GetEnvironmentVariable("CUDA_PATH");
            string? envPath = Environment.GetEnvironmentVariable("PATH");
            if (nvvmPath == null)
            {
                throw new Exception("CUDA Toolkit could not be found. Please install.");
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                nvvmPath += "\\nvvm\\bin\\";
            }
            else
            {
                nvvmPath += "/nvvm/bin/";
            }
            Environment.SetEnvironmentVariable("PATH", envPath + ";" + nvvmPath);
        }
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVVMProgram.NVVMResult nvvmCompileProgram(IntPtr program, UIntPtr numOptions, [In]string[] options);
        
        [DllImport(DLL_NAME)]
        public static extern NVVMProgram.NVVMResult nvvmAddModuleToProgram(IntPtr program, [MarshalAs(UnmanagedType.LPStr)]string buffer, UIntPtr size, [MarshalAs(UnmanagedType.LPStr)]string name);

        [DllImport(DLL_NAME)]
        public static extern NVVMProgram.NVVMResult nvvmCreateProgram(out IntPtr program);

        [DllImport(DLL_NAME)]
        public static extern NVVMProgram.NVVMResult nvvmDestroyProgram(IntPtr program);

        [DllImport(DLL_NAME)]
        public static extern NVVMProgram.NVVMResult nvvmGetCompiledResult(IntPtr program, StringBuilder buffer); //out strings have to be StringBuilder

        [DllImport(DLL_NAME)]
        public static extern NVVMProgram.NVVMResult nvvmGetCompiledResultSize(IntPtr program, out IntPtr size);

        [DllImport(DLL_NAME)]
        public static extern NVVMProgram.NVVMResult nvvmGetProgramLogSize(IntPtr program, out IntPtr size);

        [DllImport(DLL_NAME)]
        public static extern NVVMProgram.NVVMResult nvvmGetProgramLog(IntPtr program, StringBuilder buffer);
        
        [DllImport(DLL_NAME)]
        public static extern NVVMProgram.NVVMResult nvvmLazyAddModuleToProgram (IntPtr program, string buffer, UIntPtr size, string name); 
        
        [DllImport(DLL_NAME)]
        public static extern NVVMProgram.NVVMResult nvvmVerifyProgram (IntPtr program, int  numOptions, string[] options);
    }
}