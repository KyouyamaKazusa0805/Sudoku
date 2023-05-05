namespace Sudoku.SourceGeneration.CollectedResults;

/// <summary>
/// Indicates the data collected via <see cref="StepSearcherDefaultImportingHandler"/>.
/// </summary>
/// <seealso cref="StepSearcherDefaultImportingHandler"/>
internal sealed record StepSearcherDefaultImportingCollectedResult(
	INamespaceSymbol Namespace,
	INamedTypeSymbol BaseType,
	int PriorityValue,
	byte DifficultyLevel,
	string TypeName,
	NamedArgs NamedArguments,
	bool IsPolymorphism
);
