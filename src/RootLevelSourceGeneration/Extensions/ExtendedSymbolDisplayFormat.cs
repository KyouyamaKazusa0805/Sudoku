namespace Microsoft.CodeAnalysis;

/// <summary>
/// Provides extended <see cref="SymbolDisplayFormat"/>s instance that is for types.
/// </summary>
/// <seealso cref="SymbolDisplayFormat"/>
internal static class ExtendedSymbolDisplayFormat
{
	/// <summary>
	/// Indicates the type format that is the full name of the type,
	/// and is with type argument constraints.
	/// </summary>
	public static SymbolDisplayFormat FullyQualifiedFormatWithConstraints
		=> SymbolDisplayFormat
			.FullyQualifiedFormat
			.WithGenericsOptions(SymbolDisplayGenericsOptions.IncludeTypeParameters | SymbolDisplayGenericsOptions.IncludeTypeConstraints);
}
