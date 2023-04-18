using LLVMSharp;

namespace CuSharp.CudaCompiler.Frontend;

public static class LLVMSharpTypeConverter
{
    public static LLVMTypeRef ToLLVMType(this Type type)
    {
        if (type == typeof(void)) return LLVMTypeRef.VoidType();
        if (type == typeof(bool)) return LLVMTypeRef.Int1Type();
        if (type == typeof(byte)) return LLVMTypeRef.Int8Type();
        if (type == typeof(short)) return LLVMTypeRef.Int16Type();
        if (type == typeof(int)) return LLVMTypeRef.Int32Type();
        if (type == typeof(long)) return LLVMTypeRef.Int64Type();
        if (type == typeof(float)) return LLVMTypeRef.FloatType();
        if (type == typeof(double)) return LLVMTypeRef.DoubleType();
        if (type == typeof(uint)) return LLVMTypeRef.Int32Type();
        if (type.IsArray) return LLVMTypeRef.PointerType(type.GetElementType().ToLLVMType(), 0);
        throw new NotSupportedException($"Parameter type '{type}' is unsupported");
    }

    public static Type ToNativeType(this LLVMTypeRef type)
    {
        if (type.Equals(LLVMTypeRef.VoidType())) return typeof(void);
        if (type.Equals(LLVMTypeRef.Int1Type())) return typeof(bool);
        if (type.Equals(LLVMTypeRef.Int8Type())) return typeof(byte);
        if (type.Equals(LLVMTypeRef.Int16Type())) return typeof(short);
        if (type.Equals(LLVMTypeRef.Int32Type())) return typeof(int);
        if (type.Equals(LLVMTypeRef.Int64Type())) return typeof(long);
        if (type.Equals(LLVMTypeRef.FloatType())) return typeof(float);
        if (type.Equals(LLVMTypeRef.DoubleType())) return typeof(double);
        if (type.Equals(LLVMTypeRef.PointerType(type.GetElementType(), 0)))
            return type.GetElementType().ToNativeType().MakeArrayType();

        throw new NotSupportedException("Parameter type unsupported");
    }
}