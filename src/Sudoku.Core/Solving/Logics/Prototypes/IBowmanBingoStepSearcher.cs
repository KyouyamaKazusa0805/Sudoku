namespace Sudoku.Solving.Logics.Prototypes;

/// <summary>
/// Provides with a <b>Bowman's Bingo</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Bowman's Bingo</item>
/// </list>
/// </summary>
public interface IBowmanBingoStepSearcher : ILastResortStepSearcher
{
	/// <summary>
	/// Indicates the maximum length of the bowman bingo you want to search for.
	/// </summary>
	public abstract int MaxLength { get; set; }
}
