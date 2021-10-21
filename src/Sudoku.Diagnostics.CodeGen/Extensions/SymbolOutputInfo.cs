namespace Microsoft.CodeAnalysis;

internal sealed record SymbolOutputInfo(
	string TypeName,
	string FullTypeName,
	string NamespaceName,
	string GenericParameterList,
	string GenericParameterListWithoutConstraint,
	string TypeKind,
	string ReadOnlyKeyword,
	string InKeyword,
	string NullableAnnotation,
	bool IsGeneric
);