#nullable enable

using Dotnet4Gpu.Decompilation.Instructions;
using Dotnet4Gpu.Decompilation.Util;

namespace Dotnet4Gpu.Decompilation
{
    internal static class ILInstructionExtensions
    {
        public static T WithILRange<T>(this T target, ILInstruction sourceInstruction) where T : ILInstruction
        {
            target.AddILRange(sourceInstruction);
            return target;
        }

        public static T WithILRange<T>(this T target, Interval range) where T : ILInstruction
        {
            target.AddILRange(range);
            return target;
        }
    }
}
