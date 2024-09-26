namespace SudokuStudio.Drawing;

/// <summary>
/// Extracted type that creates <see cref="Shape"/>-related instances.
/// </summary>
/// <typeparam name="TInput">The type of input values.</typeparam>
/// <typeparam name="T">The type of output values.</typeparam>
/// <param name="pane">Indicates the sudoku pane control.</param>
/// <param name="converter">Indicates the position converter.</param>
/// <seealso cref="Shape"/>
internal abstract partial class CreatorBase<TInput, T>(
	[PrimaryConstructorParameter] SudokuPane pane,
	[PrimaryConstructorParameter] SudokuPanePositionConverter converter
)
{
	/// <summary>
	/// Indicates the square root of 2.
	/// </summary>
	protected const double SqrtOf2 = 1.4142135623730951;

	/// <summary>
	/// Indicates the rotate angle (45 degrees).
	/// </summary>
	protected const double RotateAngle = PI / 4;


	/// <summary>
	/// Creates a list of <see cref="Shape"/> instances representing grouped nodes.
	/// </summary>
	/// <param name="nodes">The link view nodes.</param>
	/// <returns>A <see cref="Shape"/> instance.</returns>
	public abstract ReadOnlySpan<T> CreateShapes(ReadOnlySpan<TInput> nodes);
}
