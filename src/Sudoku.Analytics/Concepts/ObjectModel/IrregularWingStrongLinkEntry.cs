namespace Sudoku.Concepts.ObjectModel;

/// <summary>
/// Represents an aliased type for <see cref="Dictionary{TKey, TValue}"/> on irregular wing checking.
/// </summary>
/// <param name="capacity">Indicates the capacity to be initialized.</param>
internal sealed class IrregularWingStrongLinkEntry(int capacity) : Dictionary<(House House, Digit Digit), List<StrongLinkInfo>>(capacity);
