using Sudoku.SourceGeneration.Handlers;

namespace Sudoku.SourceGeneration.CollectedResults;

/// <summary>
/// Indicates the data collected via <see cref="AttachedPropertyHandler"/>.
/// </summary>
/// <seealso cref="AttachedPropertyHandler"/>
internal sealed record AttachedPropertyCollectedResult(INamedTypeSymbol Type, List<AttachedPropertyData> PropertiesData);
