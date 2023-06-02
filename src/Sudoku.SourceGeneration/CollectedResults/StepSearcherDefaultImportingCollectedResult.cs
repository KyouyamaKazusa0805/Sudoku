namespace Sudoku.SourceGeneration.CollectedResults;

/// <summary>
/// Indicates the data collected via <see cref="StepSearcherDefaultImportingHandler"/>.
/// </summary>
/// <seealso cref="StepSearcherDefaultImportingHandler"/>
internal sealed record StepSearcherDefaultImportingCollectedResult(
	INamespaceSymbol Namespace,
	INamedTypeSymbol BaseType,
	int PriorityValue,
	int StepSearcherLevel,
	string TypeName,
	NamedArgs NamedArguments,
	bool IsPolymorphism
);
