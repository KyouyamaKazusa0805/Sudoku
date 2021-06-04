using Microsoft.CodeAnalysis;

namespace Sudoku.CodeGen.CodeAnalyzerDefaults
{
	/// <summary>
	/// Provides the format options.
	/// </summary>
	internal static class FormatOptions
	{
		/// <summary>
		/// Indicates the type format.
		/// </summary>
		public static readonly SymbolDisplayFormat TypeFormat = new(
			SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
			SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
			SymbolDisplayGenericsOptions.IncludeTypeParameters
			| SymbolDisplayGenericsOptions.IncludeTypeConstraints,
			miscellaneousOptions:
				SymbolDisplayMiscellaneousOptions.UseSpecialTypes
				| SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers
				| SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier
		);

		/// <summary>
		/// Indicates the property format.
		/// </summary>
		public static readonly SymbolDisplayFormat PropertyTypeFormat = new(
			SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
			SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
			SymbolDisplayGenericsOptions.IncludeTypeParameters,
			miscellaneousOptions:
				SymbolDisplayMiscellaneousOptions.UseSpecialTypes
				| SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers
				| SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier
		);
	}
}
