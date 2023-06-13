using System;
using CuSharp.CudaCompiler.Frontend;
using CuSharp.Tests.TestHelper;
using LLVMSharp;
using Xunit;

namespace CuSharp.Tests.CuSharp.CudaCompiler;

[Collection("Sequential")]
[Trait(TestCategories.TestCategory, TestCategories.Unit)]
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
    public void TestVoidToLLVMVoid()
    {
        Assert.True(TestNativeToLLVM(LLVMTypeRef.VoidType(), typeof(void)));
    }

    [Fact]
    public void TestLLVMVoidToVoid()
    {
        Assert.True(TestLLVMToNative(LLVMTypeRef.VoidType(), typeof(void)));
    }
    
    [Fact]
    public void TestBoolToLLVMBool()
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
    public void TestLongToLLVMLong()
    {
        Assert.True(TestNativeToLLVM(LLVMTypeRef.Int64Type(), typeof(long)));
    }

    [Fact]
    public void TestLLVMLongToLong()
    {
        Assert.True(TestLLVMToNative(LLVMTypeRef.Int64Type(), typeof(long)));
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
    public void TestUintToLLVMInt()
    {
        Assert.True(TestNativeToLLVM(LLVMTypeRef.Int32Type(), typeof(uint)));
    }

    [Fact]
    public void TestArrayToLLVMPointerType()
    {
        Assert.True(TestNativeToLLVM(LLVMTypeRef.PointerType(LLVMTypeRef.Int32Type(), 0), typeof(int[])));
    }

    [Fact]
    public void TestLLVMPointerTypeToArray()
    {
        Assert.True(TestLLVMToNative(LLVMTypeRef.PointerType(LLVMTypeRef.Int32Type(), 0), typeof(int[])));
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