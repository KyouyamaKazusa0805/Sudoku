namespace Sudoku.Diagnostics.CodeGen;

/// <summary>
/// Encapsulates the type detail.
/// </summary>
/// <param name="FullTypeName">The full type name.</param>
/// <param name="Name">The simple type name.</param>
/// <param name="Symbol">The type symbol.</param>
/// <param name="Attributes">The attributes of the type.</param>
/// <param name="OutParameterDeclaration">
/// The string representation of the <see langword="out"/> parameter declaration.
/// </param>
internal sealed record TypeDetail(
	string FullTypeName,
	string Name,
	INamedTypeSymbol? Symbol,
	ImmutableArray<AttributeData> Attributes,
	string OutParameterDeclaration
)
{
	/// <summary>
	/// Creates the <see cref="TypeDetail"/> instance via the specified type, and the attribute marks onto
	/// the type.
	/// </summary>
	/// <param name="typeSymbol">The type symbol.</param>
	/// <param name="attributeSymbol">The type symbol which means the type is an attribute marks onto.</param>
	/// <returns>The list of <see cref="TypeDetail"/>s.</returns>
	public static IReadOnlyCollection<TypeDetail> GetDetailList(
		INamedTypeSymbol typeSymbol,
		INamedTypeSymbol? attributeSymbol
	) => GetDetailList(typeSymbol, attributeSymbol, true);

	/// <summary>
	/// Creates the <see cref="TypeDetail"/> instance via the specified type, and the attribute marks onto
	/// the type.
	/// </summary>
	/// <param name="typeSymbol">The type symbol.</param>
	/// <param name="attributeSymbol">The type symbol which means the type is an attribute marks onto.</param>
	/// <param name="handleRecursively">
	/// Indicates whether the method will handle all nested types recursively.
	/// </param>
	/// <returns>The list of <see cref="TypeDetail"/>s.</returns>
	public static IReadOnlyCollection<TypeDetail> GetDetailList(
		INamedTypeSymbol typeSymbol,
		INamedTypeSymbol? attributeSymbol,
		bool handleRecursively
	)
	{
		var members = typeSymbol.GetMembers();
		var fieldDetailList =
			from x in members.OfType<IFieldSymbol>()
			let fullTypeName = x.Type.ToDisplayString(TypeFormats.FullName)
			select new TypeDetail(
				fullTypeName,
				x.Name,
				x.Type as INamedTypeSymbol,
				x.GetAttributes(),
				$"out {fullTypeName} {x.Name.ToCamelCase()}"
			);
		var propertyDetailList =
			from x in members.OfType<IPropertySymbol>()
			let fullTypeName = x.Type.ToDisplayString(TypeFormats.FullName)
			select new TypeDetail(
				fullTypeName,
				x.Name,
				x.Type as INamedTypeSymbol,
				x.GetAttributes(),
				$"out {fullTypeName} {x.Name.ToCamelCase()}"
			);

		var result = new List<TypeDetail>(fieldDetailList.Concat(propertyDetailList));
		if (handleRecursively && typeSymbol.BaseType is { } baseType
			&& baseType.GetAttributes() is var attributesData
			&& attributesData.Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol)))
			result.AddRange(GetDetailList(baseType, attributeSymbol, true));

		return result;
	}
}