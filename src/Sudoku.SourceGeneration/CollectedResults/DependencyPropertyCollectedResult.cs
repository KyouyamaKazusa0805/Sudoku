namespace Sudoku.SourceGeneration.CollectedResults;

/// <summary>
/// Indicates the data collected via <see cref="DependencyPropertyHandler"/>
/// </summary>
/// <seealso cref="DependencyPropertyHandler"/>
internal sealed record DependencyPropertyCollectedResult(INamedTypeSymbol Type, List<DependencyPropertyData> PropertiesData);
