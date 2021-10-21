using Kind = Microsoft.CodeAnalysis.TypeKind;

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
)
{
	public static SymbolOutputInfo FromSymbol(INamedTypeSymbol symbol, bool checkNotRefStruct = false)
	{
		string typeName = symbol.Name;
		string fullTypeName = symbol.ToDisplayString(TypeFormats.FullNameWithConstraints);
		string namespaceName = symbol.ContainingNamespace.ToDisplayString();

		int i = fullTypeName.IndexOf('<');
		bool isGeneric = i != -1;
		string genericParametersList = i == -1 ? string.Empty : fullTypeName.Substring(i);

		int j = fullTypeName.IndexOf('>');
		string genericParametersListWithoutConstraint =
			i == -1
				? string.Empty
				: fullTypeName.Substring(i, j - i + 1);

		string typeKind = (symbol.IsRecord, symbol.TypeKind) switch
		{
			(IsRecord: true, TypeKind: Kind.Class) => "record ",
			(IsRecord: true, TypeKind: Kind.Struct) => "record struct ",
			(IsRecord: false, TypeKind: Kind.Class) => "class ",
			(IsRecord: false, TypeKind: Kind.Struct) => "struct "
		};
		string readonlyKeyword = (
			checkNotRefStruct
				? symbol is { TypeKind: Kind.Struct, IsRefLikeType: false, IsReadOnly: false }
				: symbol is { TypeKind: Kind.Struct, IsReadOnly: false }
		) ? "readonly " : string.Empty;
		string inKeyword = symbol.TypeKind == Kind.Struct ? "in " : string.Empty;
		string nullableAnnotation = symbol.TypeKind == Kind.Class ? "?" : string.Empty;

		return new(
			typeName, fullTypeName, namespaceName, genericParametersList,
			genericParametersListWithoutConstraint, typeKind, readonlyKeyword,
			inKeyword, nullableAnnotation, isGeneric
		);
	}
}
