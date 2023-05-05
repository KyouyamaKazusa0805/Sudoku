namespace Sudoku.SourceGeneration.CollectedResults;

/// <summary>
/// Indicates the data collected via <see cref="InstanceDeconstructionMethodHandler"/>
/// </summary>
/// <seealso cref="InstanceDeconstructionMethodHandler"/>
internal sealed record InstanceDeconstructionMethodCollectedResult(
	INamedTypeSymbol ContainingType,
	IMethodSymbol Method,
	ImmutableArray<IParameterSymbol> Parameters,
	SyntaxTokenList Modifiers,
	INamedTypeSymbol AttributeType,
	string AssemblyName
);
