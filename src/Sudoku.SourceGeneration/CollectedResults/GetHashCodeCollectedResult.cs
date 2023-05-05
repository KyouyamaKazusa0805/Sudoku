namespace Sudoku.Diagnostics.CodeGen;

/// <summary>
/// Indicates the data collected via <see cref="GetHashCodeOveriddenHandler"/>
/// </summary>
/// <seealso cref="GetHashCodeOveriddenHandler"/>
internal sealed record GetHashCodeCollectedResult(
	int GeneratedMode,
	SyntaxTokenList MethodModifiers,
	INamedTypeSymbol Type,
	IEnumerable<string> ExpressionValueNames
);
