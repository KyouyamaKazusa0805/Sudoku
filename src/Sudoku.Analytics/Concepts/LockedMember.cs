namespace Sudoku.Concepts;

/// <summary>
/// Represents a locked member. This type is only used by searching for exocets.
/// </summary>
/// <param name="LockedCells">Indicates the locked cells.</param>
/// <param name="LockedBlock">Indicates the locked block.</param>
public sealed record LockedMember(scoped ref readonly CellMap LockedCells, House LockedBlock);