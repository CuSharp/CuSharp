// See https://aka.ms/new-console-template for more information
public class AOTC
{ 
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException(
                "Invalid amount of arguments: The argument should be the path to the dll containing kernel-methods");
        }

        string dllPath = args[0];
        
    }
}
