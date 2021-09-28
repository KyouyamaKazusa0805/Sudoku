using static Microsoft.CodeAnalysis.SymbolDisplayGenericsOptions;
using static Microsoft.CodeAnalysis.SymbolDisplayGlobalNamespaceStyle;
using static Microsoft.CodeAnalysis.SymbolDisplayMiscellaneousOptions;
using static Microsoft.CodeAnalysis.SymbolDisplayTypeQualificationStyle;

namespace Sudoku.UI.CodeGenerating;

/// <summary>
/// Provides the format options.
/// </summary>
public static class FormatOptions
{
	/// <summary>
	/// Indicates the type format.
	/// </summary>
	public static readonly SymbolDisplayFormat TypeFormat = new(
		OmittedAsContaining,
		NameAndContainingTypesAndNamespaces,
		IncludeTypeParameters | IncludeTypeConstraints,
		miscellaneousOptions: UseSpecialTypes | EscapeKeywordIdentifiers | IncludeNullableReferenceTypeModifier
	);

	/// <summary>
	/// Indicates the property format. Sometimes the option can also be used on field member symbol output.
	/// </summary>
	public static readonly SymbolDisplayFormat PropertyTypeFormat = new(
		OmittedAsContaining,
		NameAndContainingTypesAndNamespaces,
		IncludeTypeParameters,
		miscellaneousOptions: UseSpecialTypes | EscapeKeywordIdentifiers | IncludeNullableReferenceTypeModifier
	);
}
