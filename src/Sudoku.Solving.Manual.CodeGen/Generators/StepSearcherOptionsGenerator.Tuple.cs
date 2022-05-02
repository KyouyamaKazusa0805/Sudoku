namespace Sudoku.Diagnostics.CodeGen.Generators;

partial class StepSearcherOptionsGenerator
{
	private readonly record struct Tuple(
		INamedTypeSymbol StepSearcherSymbol, INamespaceSymbol ContainingNamespace,
		int Priority, DisplayingLevel DisplayingLevel, string StepSearcherName,
		ImmutableArray<KeyValuePair<string, TypedConstant>> NamedArguments);
}
