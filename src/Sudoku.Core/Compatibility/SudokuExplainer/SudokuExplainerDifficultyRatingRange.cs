namespace Sudoku.Compatibility.SudokuExplainer;

/// <summary>
/// Defines a range of difficulty rating value that is applied to a technique implemented by Sudoku Explainer.
/// </summary>
/// <param name="min">The minimum possible value.</param>
/// <param name="max">The maximum possible value.</param>
public readonly struct SudokuExplainerDifficultyRatingRange(Half min, Half max)
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
	/// Indicates the minimum possible value.
	/// </summary>
	public Half Min { get; } = min;

	/// <summary>
	/// Indicates the maximum possible value.
	/// </summary>
	public Half Max { get; } = max;

	/// <summary>
	/// Indicates whether the current range is a real range, i.e. property <see cref="Max"/> holds different value
	/// with <see cref="Min"/>.
	/// </summary>
	public bool IsRange => Max != Min;
}
