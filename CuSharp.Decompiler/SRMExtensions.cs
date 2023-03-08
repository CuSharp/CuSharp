using System.Reflection.Metadata;

namespace CuSharp.Decompiler
{
    // TODO: figure out if this can/needs be used
    //public static partial class SRMExtensions
    //{
    //	public static bool IsTypeKind(this HandleKind kind) =>
    //		kind == HandleKind.TypeDefinition || kind == HandleKind.TypeReference
    //		|| kind == HandleKind.TypeSpecification;
    //	public static bool IsMemberKind(this HandleKind kind) =>
    //		kind == HandleKind.MethodDefinition || kind == HandleKind.PropertyDefinition
    //		|| kind == HandleKind.FieldDefinition || kind == HandleKind.EventDefinition
    //		|| kind == HandleKind.MemberReference || kind == HandleKind.MethodSpecification;


    //       public static MethodDefinitionHandle GetAny(this PropertyAccessors accessors)
    //	{
    //		if (!accessors.Getter.IsNil)
    //			return accessors.Getter;
    //		return accessors.Setter;
    //	}

    //	public static MethodDefinitionHandle GetAny(this EventAccessors accessors)
    //	{
    //		if (!accessors.Adder.IsNil)
    //			return accessors.Adder;
    //		if (!accessors.Remover.IsNil)
    //			return accessors.Remover;
    //		return accessors.Raiser;
    //	}

    //	public static EntityHandle GetGenericType(this in TypeSpecification ts, MetadataReader metadata)
    //	{
    //		if (ts.Signature.IsNil)
    //			return default;
    //		// Do a quick scan using BlobReader
    //		var signature = metadata.GetBlobReader(ts.Signature);
    //		// When dealing with FSM implementations, we can safely assume that if it's a type spec,
    //		// it must be a generic type instance.
    //		if (signature.ReadByte() != (byte)SignatureTypeCode.GenericTypeInstance)
    //			return default;
    //		// Skip over the rawTypeKind: value type or class
    //		var rawTypeKind = signature.ReadCompressedInteger();
    //		if (rawTypeKind < 17 || rawTypeKind > 18)
    //			return default;
    //		// Only read the generic type, ignore the type arguments
    //		return signature.ReadTypeHandle();
    //	}

    //	public static EntityHandle GetDeclaringType(this EntityHandle entity, MetadataReader metadata)
    //	{
    //		switch (entity.Kind)
    //		{
    //			case HandleKind.TypeDefinition:
    //				var td = metadata.GetTypeDefinition((TypeDefinitionHandle)entity);
    //				return td.GetDeclaringType();
    //			case HandleKind.TypeReference:
    //				var tr = metadata.GetTypeReference((TypeReferenceHandle)entity);
    //				return tr.GetDeclaringType();
    //			case HandleKind.TypeSpecification:
    //				var ts = metadata.GetTypeSpecification((TypeSpecificationHandle)entity);
    //				return ts.GetGenericType(metadata).GetDeclaringType(metadata);
    //			case HandleKind.FieldDefinition:
    //				var fd = metadata.GetFieldDefinition((FieldDefinitionHandle)entity);
    //				return fd.GetDeclaringType();
    //			case HandleKind.MethodDefinition:
    //				var md = metadata.GetMethodDefinition((MethodDefinitionHandle)entity);
    //				return md.GetDeclaringType();
    //			case HandleKind.MemberReference:
    //				var mr = metadata.GetMemberReference((MemberReferenceHandle)entity);
    //				return mr.Parent;
    //			case HandleKind.EventDefinition:
    //				var ed = metadata.GetEventDefinition((EventDefinitionHandle)entity);
    //				return metadata.GetMethodDefinition(ed.GetAccessors().GetAny()).GetDeclaringType();
    //			case HandleKind.PropertyDefinition:
    //				var pd = metadata.GetPropertyDefinition((PropertyDefinitionHandle)entity);
    //				return metadata.GetMethodDefinition(pd.GetAccessors().GetAny()).GetDeclaringType();
    //			case HandleKind.MethodSpecification:
    //				var ms = metadata.GetMethodSpecification((MethodSpecificationHandle)entity);
    //				return ms.Method.GetDeclaringType(metadata);
    //			default:
    //				throw new ArgumentOutOfRangeException();
    //		}
    //	}

    //	public static TypeReferenceHandle GetDeclaringType(this in TypeReference tr)
    //	{
    //		switch (tr.ResolutionScope.Kind)
    //		{
    //			case HandleKind.TypeReference:
    //				return (TypeReferenceHandle)tr.ResolutionScope;
    //			default:
    //				return default(TypeReferenceHandle);
    //		}
    //	}
    //}
}
