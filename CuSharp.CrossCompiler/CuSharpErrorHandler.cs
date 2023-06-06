using System.Diagnostics;

namespace CuSharp.CudaCompiler;

public static class CuSharpErrorHandler
{
    public static void HandleUnrecoverable(this Exception e, string message)
    {
        Console.WriteLine(message);
        Console.WriteLine(e.Message);
        throw new Exception("CuSharp process failed");
    }
}