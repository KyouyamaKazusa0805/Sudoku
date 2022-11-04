namespace Sudoku.Compatibility.SudokuExplainer;

/// <summary>
/// Defines a range of difficulty rating value that is applied to a technique implemented by Sudoku Explainer.
/// </summary>
/// <param name="Min">The minimum possible value.</param>
/// <param name="Max">The maximum possible value.</param>
public readonly record struct SudokuExplainerDifficultyRatingRange(Half Min, Half Max) :
	IEquatable<SudokuExplainerDifficultyRatingRange>,
	IEqualityOperators<SudokuExplainerDifficultyRatingRange, SudokuExplainerDifficultyRatingRange, bool>
{
	/// <summary>
	/// Initializes a <see cref="SudokuExplainerDifficultyRatingRange"/> instance
	/// via the specified difficulty rating value.
	/// </summary>
	/// <param name="min">The difficulty rating value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SudokuExplainerDifficultyRatingRange(Half min) : this(min, min)
	{
	}


	/// <summary>
	/// Indicates whether the current range is a real range, i.e. property <see cref="Max"/> holds different value
	/// with <see cref="Min"/>.
	/// </summary>
	public bool IsRange => Max != Min;
}
