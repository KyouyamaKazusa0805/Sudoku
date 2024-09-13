namespace Sudoku.Runtime.InteropServices.SudokuExplainer;

/// <summary>
/// Defines a range of difficulty rating value that is applied to a technique implemented by Sudoku Explainer.
/// </summary>
/// <param name="min">Indicates the minimum possible value.</param>
/// <param name="max">Indicates the maximum possible value.</param>
public readonly partial struct SudokuExplainerRatingRange(
	[PrimaryConstructorParameter] Half min,
	[PrimaryConstructorParameter] Half max
)
{
	/// <summary>
	/// Initializes a <see cref="SudokuExplainerRatingRange"/> instance via the specified difficulty rating value.
	/// </summary>
	/// <param name="min">The difficulty rating value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SudokuExplainerRatingRange(Half min) : this(min, min)
	{
	}


	/// <summary>
	/// Indicates whether the current range is a real range, i.e. property <see cref="Max"/> holds different value
	/// with <see cref="Min"/>.
	/// </summary>
	public bool IsRange => Max != Min;
}
