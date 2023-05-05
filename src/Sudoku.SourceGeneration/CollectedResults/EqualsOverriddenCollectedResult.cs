namespace Sudoku.SourceGeneration.CollectedResults;

/// <summary>
/// Indicates the data collected via <see cref="EqualsOverriddenHandler"/>
/// </summary>
/// <seealso cref="EqualsOverriddenHandler"/>
internal sealed record EqualsOverriddenCollectedResult(
	int GeneratedMode,
	SyntaxTokenList MethodModifiers,
	INamedTypeSymbol Type,
	string ParameterName
);
