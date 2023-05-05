namespace Sudoku.Diagnostics.CodeGen;

/// <summary>
/// Indicates the data collected via <see cref="AttachedPropertyHandler"/>
/// </summary>
/// <seealso cref="AttachedPropertyHandler"/>
internal sealed record AttachedPropertyCollectedResult(INamedTypeSymbol Type, List<AttachedPropertyData> PropertiesData);
