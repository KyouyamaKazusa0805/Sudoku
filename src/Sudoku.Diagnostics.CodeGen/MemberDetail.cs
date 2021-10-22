namespace Sudoku.Diagnostics.CodeGen;

/// <summary>
/// Encapsulates the member detail.
/// </summary>
/// <param name="MemberSymbol">The member symbol.</param>
/// <param name="TypeSymbol">The type symbol.</param>
/// <param name="FullTypeName">The full type name.</param>
/// <param name="Name">The simple type name.</param>
/// <param name="Attributes">The attributes of the type.</param>
/// <param name="OutParameterDeclaration">
/// The string representation of the <see langword="out"/> parameter declaration.
/// </param>
internal sealed record MemberDetail(
	ISymbol MemberSymbol,
	INamedTypeSymbol? TypeSymbol,
	string FullTypeName,
	string Name,
	ImmutableArray<AttributeData> Attributes,
	string OutParameterDeclaration
)
{
	/// <summary>
	/// Creates the <see cref="MemberDetail"/> instance via the specified type, and the attribute marks onto
	/// the type.
	/// </summary>
	/// <param name="typeSymbol">The type symbol.</param>
	/// <param name="attributeSymbol">The type symbol which means the type is an attribute marks onto.</param>
	/// <returns>The list of <see cref="MemberDetail"/>s.</returns>
	public static IReadOnlyCollection<MemberDetail> GetDetailList(
		INamedTypeSymbol typeSymbol,
		INamedTypeSymbol? attributeSymbol
	) => GetDetailList(typeSymbol, attributeSymbol, true);

	/// <summary>
	/// Creates the <see cref="MemberDetail"/> instance via the specified type, and the attribute marks onto
	/// the type.
	/// </summary>
	/// <param name="typeSymbol">The type symbol.</param>
	/// <param name="attributeSymbol">The type symbol which means the type is an attribute marks onto.</param>
	/// <param name="handleRecursively">
	/// Indicates whether the method will handle all nested types recursively.
	/// </param>
	/// <returns>The list of <see cref="MemberDetail"/>s.</returns>
	public static IReadOnlyCollection<MemberDetail> GetDetailList(
		INamedTypeSymbol typeSymbol,
		INamedTypeSymbol? attributeSymbol,
		bool handleRecursively
	)
	{
		var members = typeSymbol.GetMembers();
		var fieldDetailList =
			from x in members.OfType<IFieldSymbol>()
			let fullTypeName = x.Type.ToDisplayString(TypeFormats.FullName)
			select new MemberDetail(
				x,
				x.Type as INamedTypeSymbol,
				fullTypeName,
				x.Name,
				x.GetAttributes(),
				$"out {fullTypeName} {x.Name.ToCamelCase()}"
			);
		var propertyDetailList =
			from x in members.OfType<IPropertySymbol>()
			let fullTypeName = x.Type.ToDisplayString(TypeFormats.FullName)
			select new MemberDetail(
				x,
				x.Type as INamedTypeSymbol,
				fullTypeName,
				x.Name,
				x.GetAttributes(),
				$"out {fullTypeName} {x.Name.ToCamelCase()}"
			);

		var result = new List<MemberDetail>(fieldDetailList.Concat(propertyDetailList));
		if (handleRecursively && typeSymbol.BaseType is { } baseType
			&& baseType.GetAttributes() is var attributesData
			&& attributesData.Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol)))
			result.AddRange(GetDetailList(baseType, attributeSymbol, true));

		return result;
	}
}