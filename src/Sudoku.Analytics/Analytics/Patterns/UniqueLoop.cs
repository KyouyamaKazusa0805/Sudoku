namespace Sudoku.Analytics.Patterns;

/// <summary>
/// Represents for a data set that describes the complete information about a unique loop technique.
/// </summary>
/// <param name="Loop">Indicates the cells used in this whole unique loop.</param>
/// <param name="Path">Indicates the detail path of the loop.</param>
/// <param name="DigitsMask">Indicates the digits used, represented as a mask of type <see cref="Mask"/>.</param>
public readonly record struct UniqueLoop(scoped in CellMap Loop, Cell[] Path, Mask DigitsMask) : IEquatable<UniqueLoop>
{
	/// <inheritdoc/>
	public bool Equals(UniqueLoop other) => Loop == other.Loop;

	/// <inheritdoc/>
	public override int GetHashCode() => Loop.GetHashCode();
}
