using System;
using CuSharp.MSILtoLLVMCompiler;
using LLVMSharp;
using Xunit;

namespace CuSharp.Tests.CuSharp.CudaCompiler;

public class LLVMSharpTypeConverterTests
{
    private bool TestNativeToLLVM(LLVMTypeRef llvmType, Type nativeType)
    {
        return nativeType.ToLLVMType().Equals(llvmType);
    }

    private bool TestLLVMToNative(LLVMTypeRef llvmType, Type nativeType)
    {
        return llvmType.ToNativeType() == nativeType;
    } 
    
    [Fact]
    public void TestBoolToLLVMBoolSuccess()
    {
        Assert.True(TestNativeToLLVM(LLVMTypeRef.Int1Type(), typeof(bool)));
    }

    [Fact]
    public void TestLLVMBoolToBool()
    {
        Assert.True(TestLLVMToNative(LLVMTypeRef.Int1Type(), typeof(bool)));
    }

    [Fact]
    public void TestByteToLLVMByte()
    {
        Assert.True(TestNativeToLLVM(LLVMTypeRef.Int8Type(), typeof(byte)));
    }

    [Fact]
    public void TestLLVMByteToByte()
    {
        Assert.True(TestLLVMToNative(LLVMTypeRef.Int8Type(), typeof(byte)));
    }
}