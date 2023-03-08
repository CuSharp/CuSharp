using System;

namespace CuSharp
{
    public static class KernelTools
    {
        private const string ErrorMessage = "Do not use from host or overwrite the Action to specify host semantics.";

        public static Action CallSyncThreadsAction => () => throw new NotSupportedException(ErrorMessage);
        public static Func<(uint,uint,uint)> GetBlockIndexAction => () => throw new NotSupportedException(ErrorMessage);
        public static Func<(uint,uint,uint)> GetThreadIndexAction => () => throw new NotSupportedException(ErrorMessage);
        public static Func<(uint, uint, uint)> GetGridDimensionsAction => () => throw new NotSupportedException(ErrorMessage);
        public static Func<(uint, uint, uint)> GetBlockDimensionsAction => () => throw new NotSupportedException(ErrorMessage);
            
        public static Action SyncThreads => CallSyncThreadsAction;
        
        public static (uint,uint,uint) BlockIndex => GetBlockIndexAction();
        public static (uint,uint,uint) ThreadIndex => GetThreadIndexAction();
        
        public static (uint, uint, uint) GridDimensions => GetGridDimensionsAction();
        public static (uint, uint, uint) BlockDimensions => GetBlockDimensionsAction();
    }
}