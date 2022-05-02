namespace Sudoku.Diagnostics.CodeGen.Generators;

partial class StepSearcherOptionsGenerator
{
	private sealed record class Tuple(
		INamedTypeSymbol StepSearcherSymbol, INamespaceSymbol ContainingNamespace,
		int Priority, byte DisplayingLevel, string StepSearcherName,
		ImmutableArray<KeyValuePair<string, TypedConstant>> NamedArguments);
}
