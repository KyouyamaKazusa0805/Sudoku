namespace Sudoku.Platforms.QQ.Configurations;

/// <summary>
/// Defines an option that determines what kind of case that the candidate will be printed by <see cref="ISudokuPainter"/>.
/// </summary>
/// <seealso cref="ISudokuPainter"/>
public enum CandidatePrintingOptions
{
	/// <summary>
	/// Indicates <see cref="ISudokuPainter"/> never prints candidates.
	/// </summary>
	Never,

	/// <summary>
	/// <para>Indicates <see cref="ISudokuPainter"/> prints candidates when the puzzle cannot be solved using SSTS only.</para>
	/// <para>
	/// All SSTS are:
	/// <list type="bullet">
	/// <item>Hidden Single</item>
	/// <item>Naked Single</item>
	/// <item>Locked Candidates</item>
	/// <item>Hidden Subset</item>
	/// <item>Naked Subset</item>
	/// </list>
	/// </para>
	/// </summary>
	PrintIfPuzzleIndirect,

	/// <summary>
	/// Indicates <see cref="ISudokuPainter"/> always prints candidates.
	/// </summary>
	Always
}
