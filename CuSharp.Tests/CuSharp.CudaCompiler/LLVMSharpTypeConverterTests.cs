using System;
using CuSharp.CudaCompiler.Frontend;
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

    [Fact]
    public void TestShortToLLVMShort()
    {
        Assert.True(TestNativeToLLVM(LLVMTypeRef.Int16Type(), typeof(short)));
    }

    [Fact]
    public void TestLLVMShortToShort()
    {
        Assert.True(TestLLVMToNative(LLVMTypeRef.Int16Type(), typeof(short)));
    }

    [Fact]
    public void TestIntToLLVMInt()
    {
        Assert.True(TestNativeToLLVM(LLVMTypeRef.Int32Type(), typeof(int)));
    }

    [Fact]
    public void TestLLVMIntToInt()
    {
        Assert.True(TestLLVMToNative(LLVMTypeRef.Int32Type(), typeof(int)));
    }

    [Fact]
    public void TestFloatToLLVMFLoat()
    {
        Assert.True(TestNativeToLLVM(LLVMTypeRef.FloatType(), typeof(float)));
    }

    [Fact]
    public void TestLLVMFloatToFloat()
    {
        Assert.True(TestLLVMToNative(LLVMTypeRef.FloatType(), typeof(float)));
    }

    [Fact]
    public void TestDoubleToLLVMDouble()
    {
        Assert.True(TestNativeToLLVM(LLVMTypeRef.DoubleType(), typeof(double)));
    }

    [Fact]
    public void TestLLVMDoubleToDouble()
    {
        Assert.True(TestLLVMToNative(LLVMTypeRef.DoubleType(), typeof(double)));
    }

    [Fact]
    public void TestInvalidTypeThrows()
    {
        Assert.Throws<NotSupportedException>(() => typeof(Action).ToLLVMType());
    }

    [Fact]
    public void TestInvalidLLVMTypeThrows()
    {
        Assert.Throws<NotSupportedException>(() => LLVMTypeRef.HalfType().ToNativeType());
    }
    
}