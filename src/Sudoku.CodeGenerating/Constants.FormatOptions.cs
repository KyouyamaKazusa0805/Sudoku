namespace Sudoku.CodeGenerating;

partial class Constants
{
	/// <summary>
	/// Provides <see cref="SymbolDisplayFormat"/> instance that is for types.
	/// </summary>
	public static class TypeFormats
	{
		/// <summary>
		/// Indicates the type format that is the full name of the type,
		/// and is with type argument constraints.
		/// </summary>
		public static readonly SymbolDisplayFormat FullNameWithConstraints = new(
			globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
			typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
			genericsOptions:
				SymbolDisplayGenericsOptions.IncludeTypeParameters
				| SymbolDisplayGenericsOptions.IncludeTypeConstraints,
			miscellaneousOptions:
				SymbolDisplayMiscellaneousOptions.UseSpecialTypes
				| SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers
				| SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier
		);

		/// <summary>
		/// Indicates the type format that is the full name of the type,
		/// and is without type argument constraints.
		/// </summary>
		public static readonly SymbolDisplayFormat FullName = new(
			globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
			typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
			genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
			miscellaneousOptions:
				SymbolDisplayMiscellaneousOptions.UseSpecialTypes
				| SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers
				| SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier
		);
	}
}
