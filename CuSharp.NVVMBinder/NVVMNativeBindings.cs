using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace LibNVVMBinder
{
    internal static class NVVMNativeBindings
    {
        
        private const string WINDOWS_DLL_NAME = "nvvm64_40_0";
        private const string UNIX_DLL_NAME = "nvvm";
        static NVVMNativeBindings()
        {
            NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), DllImportResolver);
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

        private static IntPtr DllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return NativeLibrary.Load(WINDOWS_DLL_NAME, assembly, searchPath);
            }
            else
            {
                return NativeLibrary.Load(UNIX_DLL_NAME, assembly, searchPath);
            }
        }
        
        [DllImport(WINDOWS_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVVMProgram.NVVMResult nvvmCompileProgram(IntPtr program, UIntPtr numOptions, [In]string[] options);
        
        [DllImport(WINDOWS_DLL_NAME)]
        public static extern NVVMProgram.NVVMResult nvvmAddModuleToProgram(IntPtr program, [MarshalAs(UnmanagedType.LPStr)]string buffer, UIntPtr size, [MarshalAs(UnmanagedType.LPStr)]string name);

        [DllImport(WINDOWS_DLL_NAME)]
        public static extern NVVMProgram.NVVMResult nvvmCreateProgram(out IntPtr program);

        [DllImport(WINDOWS_DLL_NAME)]
        public static extern NVVMProgram.NVVMResult nvvmDestroyProgram(IntPtr program);

        [DllImport(WINDOWS_DLL_NAME)]
        public static extern NVVMProgram.NVVMResult nvvmGetCompiledResult(IntPtr program, StringBuilder buffer); //out strings have to be StringBuilder

        [DllImport(WINDOWS_DLL_NAME)]
        public static extern NVVMProgram.NVVMResult nvvmGetCompiledResultSize(IntPtr program, out IntPtr size);

        [DllImport(WINDOWS_DLL_NAME)]
        public static extern NVVMProgram.NVVMResult nvvmGetProgramLogSize(IntPtr program, out IntPtr size);

        [DllImport(WINDOWS_DLL_NAME)]
        public static extern NVVMProgram.NVVMResult nvvmGetProgramLog(IntPtr program, StringBuilder buffer);
        
        [DllImport(WINDOWS_DLL_NAME)]
        public static extern NVVMProgram.NVVMResult nvvmLazyAddModuleToProgram (IntPtr program, string buffer, UIntPtr size, string name); 
        
        [DllImport(WINDOWS_DLL_NAME)]
        public static extern NVVMProgram.NVVMResult nvvmVerifyProgram (IntPtr program, int  numOptions, string[] options);
    }
}