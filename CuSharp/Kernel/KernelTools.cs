namespace CuSharp.Kernel;

public static class KernelTools
{
    private const string ErrorMessage = "Do not use from host or overwrite the Action to specify host semantics.";

    public static Action CallSyncThreadsAction { get; set; } = () => throw new NotSupportedException(ErrorMessage);

    public static Func<(uint, uint, uint)> GetBlockIndexAction { get; set; } = () => throw new NotSupportedException(ErrorMessage);

    public static Func<(uint, uint, uint)> GetThreadIndexAction { get; set; } = () => throw new NotSupportedException(ErrorMessage);

    public static Func<(uint, uint, uint)> GetBlockDimensionsAction { get; set; } = () => throw new NotSupportedException(ErrorMessage);

    public static Action SyncThreads => CallSyncThreadsAction;

    public static (uint,uint,uint) BlockIndex => GetBlockIndexAction();
    public static (uint,uint,uint) ThreadIndex => GetThreadIndexAction();
    
    public static (uint, uint, uint) BlockDimensions => GetBlockDimensionsAction();
}