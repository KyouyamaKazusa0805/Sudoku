using Sudoku.SourceGeneration.Handlers;

namespace Sudoku.SourceGeneration.CollectedResults;

/// <summary>
/// Indicates the data collected via <see cref="ImplicitFieldHandler"/>.
/// </summary>
/// <seealso cref="ImplicitFieldHandler"/>
public sealed record ImplicitFieldCollectedResult(INamedTypeSymbol ContainingType, IPropertySymbol Property, bool ReadOnlyModifier);
