namespace Sudoku.Solving.Manual.Searchers.LastResorts;

/// <summary>
/// Defines a step searcher that searches for bowman's bingo steps.
/// </summary>
public interface IBowmanBingoStepSearcher : ILastResortStepSearcher
{
	/// <summary>
	/// Indicates the maximum length of the bowman bingo you want to search for.
	/// </summary>
	int MaxLength { get; set; }
}
