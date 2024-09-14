namespace Sudoku.Analytics.Traits;

/// <summary>
/// Represents a trait that describes the guardians.
/// </summary>
public interface IGuardianTrait : ITrait
{
	/// <summary>
	/// Indicates the number of guardian cells.
	/// </summary>
	public abstract int GuardianCellsCount { get; }

	/// <summary>
	/// Indicates the guardians used.
	/// </summary>
	public abstract CellMap GuardianCells { get; }
}
