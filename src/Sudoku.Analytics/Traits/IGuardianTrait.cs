namespace Sudoku.Traits;

/// <summary>
/// Represents a trait that describes the guardians.
/// </summary>
public interface IGuardianTrait : IStepTrait
{
	/// <summary>
	/// Indicates the guardians used.
	/// </summary>
	public abstract CellMap GuardianCells { get; }
}
