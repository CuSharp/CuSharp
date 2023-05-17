namespace CuSharp.Kernel;

public static class KernelTools
{
    private const string ErrorMessage = "Do not use from host or overwrite the Action to specify host semantics.";

    public static Action CallSyncThreadsAction { get; set; } = () => throw new NotSupportedException(ErrorMessage);

    public static Func<(uint X, uint Y, uint Z)> GetBlockIndexAction { get; set; } = () => throw new NotSupportedException(ErrorMessage);

    public static Func<(uint X, uint Y, uint Z)> GetThreadIndexAction { get; set; } = () => throw new NotSupportedException(ErrorMessage);

    public static Func<(uint X, uint Y, uint Z)> GetBlockDimensionAction { get; set; } = () => throw new NotSupportedException(ErrorMessage);

    public static Action SyncThreads => CallSyncThreadsAction;
    
    public static Action ThreadFence => CallSyncThreadsAction;
    public static (uint X, uint Y, uint Z) BlockIndex => GetBlockIndexAction();
    
    public static (uint X, uint Y, uint Z) ThreadIndex => GetThreadIndexAction();
    
    public static (uint X, uint Y, uint Z) BlockDimension => GetBlockDimensionAction();
}