namespace Sudoku.Analytics.Steps;

/// <summary>
/// The base overrides for complex senior exocet steps.
/// </summary>
internal interface IComplexSeniorExocetStepBaseOverrides
{
	public abstract HouseMask CrosslineHousesMask { get; }

	public abstract HouseMask ExtraHousesMask { get; }
}
