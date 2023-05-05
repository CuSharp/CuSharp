using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

namespace CuSharp.Tests.TestHelper
{
    public static class ReleaseAdaption
    {
        public static List<(ILOpCode, object?)> RemoveNopInReleaseMode(List<(ILOpCode, object?)> opCodesWithOperands)
        {
#if (RELEASE)
            opCodesWithOperands = opCodesWithOperands.Where(inst => inst.Item1 != ILOpCode.Nop).ToList();
#endif
            return opCodesWithOperands;
        }

        public static int GetReleaseIndex(int index, int amountToDecrease)
        {
#if (RELEASE)
            index -= amountToDecrease;
#endif
            return index;
        }
    }
}
