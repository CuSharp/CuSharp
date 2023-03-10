using System.Reflection.Metadata;
using LLVMSharp;

namespace CuSharp.MSILtoLLVMCompiler;

public class MethodBodyCompiler
{
    private BinaryReader _reader;
    private LLVMModuleRef _module;
    private LLVMBuilderRef _builder;
    private ILOpCode ReadOpcode()
    {
        return (ILOpCode) _reader.ReadByte();
    }
    
    public MethodBodyCompiler(byte[] kernelBuffer, LLVMModuleRef module, LLVMBuilderRef builder)
    {
        _module = module;
        _builder = builder;
        var stream = new MemoryStream(kernelBuffer);
        _reader = new BinaryReader(stream);
    }
    
    public void CompileMethodBody()
    {
        var op = ReadOpcode();
        switch (op)
        {
            case ILOpCode.Ldc_i4: CompileLDCI32(_reader.ReadInt32());
                break;
            
        }
    }
    private void CompileLDCI32(Int32 operand)
    {
        
    }
}