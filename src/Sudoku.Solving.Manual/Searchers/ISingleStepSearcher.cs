namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Defines a step searcher that searches for single steps.
/// </summary>
public interface ISingleStepSearcher : IStepSearcher
{
	/// <summary>
	/// Indicates whether the solver enables the technique full house.
	/// </summary>
	public abstract bool EnableFullHouse { get; set; }

	/// <summary>
	/// Indicates whether the solver enables the technique last digit.
	/// </summary>
	public abstract bool EnableLastDigit { get; set; }

	/// <summary>
	/// Indicates whether the solver checks for hidden single in block firstly.
	/// </summary>
	public abstract bool HiddenSinglesInBlockFirst { get; set; }
}
