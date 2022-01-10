using Kind = Microsoft.CodeAnalysis.TypeKind;

namespace Microsoft.CodeAnalysis;

/// <summary>
/// Provides the basic information used for output a source code via an <see cref="INamedTypeSymbol"/>.
/// </summary>
/// <param name="TypeName">The type name of the type symbol.</param>
/// <param name="FullTypeName">The full type name of the type symbol.</param>
/// <param name="NamespaceName">The namespace name that the type symbol belongs to.</param>
/// <param name="GenericParameterList">
/// The generic parameter list if the type symbol contains the type parameters.
/// </param>
/// <param name="GenericParameterListWithoutConstraint">
/// The generic parameter list without the constraints if the type symbol contains the type parameters.
/// </param>
/// <param name="TypeKind">
/// The type kind of the type symbol. All possible type kinds are:
/// <list type="bullet">
/// <item><see langword="class"/></item>
/// <item><see langword="struct"/></item>
/// <item><see langword="record class"/> (Denotes as a keyword <c>record</c>)</item>
/// <item><see langword="record struct"/></item>
/// </list>
/// </param>
/// <param name="ReadOnlyKeyword">
/// Indicates whether the type symbol is a <see langword="struct"/>. If so, this property will keep the value
/// as a modifier (i.e. keyword <see langword="readonly"/>) onto the members to implement.
/// </param>
/// <param name="InKeyword">
/// Indicates whether the type symbol is a <see langword="struct"/>. If so, this property will keep the value
/// as a modifier (i.e. keyword <see langword="in"/>) onto the parameters in methods.
/// </param>
/// <param name="NullableAnnotation">
/// Indicates whether the type symbol is a <see langword="class"/>. If so, this property will keep the value
/// as a nullable annotation <c>?</c> onto the parameter types.
/// </param>
/// <param name="IsGeneric">A <see cref="bool"/> value indicating whether the type is a generic type.</param>
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
	/// <summary>
	/// Creates a <see cref="SymbolOutputInfo"/> instance via the specified <paramref name="symbol"/>,
	/// and set a <see cref="bool"/> value indicating whether the method will check whether the type
	/// is a <see langword="ref struct"/>.
	/// </summary>
	/// <param name="symbol">The type symbol.</param>
	/// <param name="checkNotRefStruct">
	/// A <see cref="bool"/> value indicating whether the method will check whether the type
	/// is a <see langword="ref struct"/>.
	/// </param>
	/// <returns>The <see cref="SymbolOutputInfo"/> instance.</returns>
	public static SymbolOutputInfo FromSymbol(INamedTypeSymbol symbol, bool checkNotRefStruct = false)
	{
		string typeName = symbol.Name;
		string fullTypeName = symbol.ToDisplayString(TypeFormats.FullNameWithConstraints);
		string namespaceName = symbol.ContainingNamespace.ToDisplayString();

		int i = fullTypeName.IndexOf('<');
		bool isGeneric = i != -1;
		string genericParametersList = i == -1 ? string.Empty : fullTypeName[i..];

		int j = fullTypeName.IndexOf('>');
		string genericParametersListWithoutConstraint = i == -1 ? string.Empty : fullTypeName[i..j];

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
