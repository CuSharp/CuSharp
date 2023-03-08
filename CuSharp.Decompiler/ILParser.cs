// Copyright (c) 2018 Siegfried Pammer
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Dotnet4Gpu.Decompilation.Util;

namespace Dotnet4Gpu.Decompilation;
public static class ILParser
{
    public static ILOpCode DecodeOpCode(this BinaryReader reader)
    {
        byte opCodeByte = reader.ReadByte();
        return (ILOpCode)(opCodeByte == 0xFE ? 0xFE00 + reader.ReadByte() : opCodeByte);
    }

    public static void SkipOperand(this BinaryReader reader, ILOpCode opCode)
    {
        var stream = reader.BaseStream;
        switch (opCode.GetOperandType())
        {
            // 64-bit
            case OperandType.I8:
            case OperandType.R:
                stream.Position += 8;
                break;
            // 32-bit
            case OperandType.BrTarget:
            case OperandType.Field:
            case OperandType.Method:
            case OperandType.I:
            case OperandType.Sig:
            case OperandType.String:
            case OperandType.Tok:
            case OperandType.Type:
            case OperandType.ShortR:
                stream.Position += 4;
                break;
            // (n + 1) * 32-bit
            case OperandType.Switch:
                uint n = reader.ReadUInt32();
                stream.Position += (int)(n * 4);
                break;
            // 16-bit
            case OperandType.Variable:
                stream.Position += 2;
                break;
            // 8-bit
            case OperandType.ShortVariable:
            case OperandType.ShortBrTarget:
            case OperandType.ShortI:
                stream.Position++;
                break;
        }
    }

    public static int DecodeBranchTarget(this BinaryReader reader, ILOpCode opCode)
    {
        return (opCode.GetBranchOperandSize() == 4 ? reader.ReadInt32() : reader.ReadSByte()) + (int) reader.BaseStream.Position;
    }

    public static int[] DecodeSwitchTargets(this BinaryReader reader)
    {
        int[] targets = new int[reader.ReadUInt32()];
        int offset = (int)(reader.BaseStream.Position) + 4 * targets.Length;
        for (int i = 0; i < targets.Length; i++)
            targets[i] = reader.ReadInt32() + offset;
        return targets;
    }

    public static string DecodeUserString(this BinaryReader reader, MetadataReader metadata)
    {
        return metadata.GetUserString(MetadataTokens.UserStringHandle(reader.ReadInt32()));
    }

    public static int DecodeIndex(this BinaryReader reader, ILOpCode opCode)
    {
        switch (opCode.GetOperandType())
        {
            case OperandType.ShortVariable:
                return reader.ReadByte();
            case OperandType.Variable:
                return reader.ReadUInt16();
            default:
                throw new ArgumentException($"{opCode} not supported!");
        }
    }

    public static void SetBranchTargets(BinaryReader reader, BitSet branchTargets)
    {
        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            var opCode = reader.DecodeOpCode();
            if (opCode == ILOpCode.Switch)
            {
                foreach (var target in reader.DecodeSwitchTargets())
                {
                    if (target >= 0 && target < reader.BaseStream.Length)
                        branchTargets.Set(target);
                }
            }
            else if (opCode.IsBranch())
            {
                int target = reader.DecodeBranchTarget(opCode);
                if (target >= 0 && target < reader.BaseStream.Length)
                    branchTargets.Set(target);
            }
            else
            {
                reader.SkipOperand(opCode);
            }
        }
    }

    public static string OffsetToString(int offset)
    {
        return $"IL_{offset:x4}";
    }
}