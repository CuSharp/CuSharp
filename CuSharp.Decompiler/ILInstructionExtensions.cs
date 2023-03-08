#nullable enable

using CuSharp.Decompiler;
using CuSharp.Decompiler.Instructions;
using CuSharp.Decompiler.Util;

namespace CuSharp.Decompiler
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
