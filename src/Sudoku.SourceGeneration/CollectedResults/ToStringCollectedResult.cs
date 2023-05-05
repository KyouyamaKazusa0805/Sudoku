namespace Sudoku.SourceGeneration.CollectedResults;

/// <summary>
/// Indicates the data collected via <see cref="ToStringOverriddenHandler"/>
/// </summary>
/// <seealso cref="ToStringOverriddenHandler"/>
internal sealed record ToStringCollectedResult(
	int GeneratedMode,
	SyntaxTokenList MethodModifiers,
	INamedTypeSymbol Type,
	INamedTypeSymbol SpecialAttributeType,
	IEnumerable<string> ExpressionValueNames
);
