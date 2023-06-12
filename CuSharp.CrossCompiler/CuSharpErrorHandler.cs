namespace CuSharp.CudaCompiler;

public static class CuSharpErrorHandler
{
    public static void HandleUnrecoverable(this Exception e, string message)
    {
        Console.WriteLine(message);
        throw new CuSharpException("CuSharp process failed:\n" + e.Message);
    }
}

public class CuSharpException : Exception
{
    public CuSharpException(string message) : base(message)
    { }
}