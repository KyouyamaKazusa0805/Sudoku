namespace Sudoku.Traits;

/// <summary>
/// Represents a trait that describes the extra cell list.
/// </summary>
public interface IExtraCellListTrait : ITrait
{
	/// <summary>
	/// Indicates the number of extra cells.
	/// </summary>
	public abstract int ExtraCellSize { get; }
}
