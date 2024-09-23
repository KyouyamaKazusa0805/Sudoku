namespace Sudoku.Analytics.Construction.Patterns;

/// <summary>
/// The base overrides for complex senior exocet steps.
/// </summary>
public interface IComplexSeniorExocet
{
	/// <summary>
	/// The mask that holds a list of cross-line houses used.
	/// </summary>
	public abstract HouseMask CrosslineHousesMask { get; }

	/// <summary>
	/// The mask that holds a list of extra houses used.
	/// </summary>
	public abstract HouseMask ExtraHousesMask { get; }
}
