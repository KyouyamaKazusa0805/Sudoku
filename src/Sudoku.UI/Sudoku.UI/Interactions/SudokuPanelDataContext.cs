namespace Sudoku.UI.Interactions;

/// <summary>
/// Provides with the data context that binds with <see cref="SudokuPanel"/>.
/// </summary>
/// <seealso cref="SudokuPanel"/>
public sealed class SudokuPanelDataContext : IDataContext<SudokuPanelDataContext>
{
	/// <summary>
	/// Indicates the calculator that calculates the pixels and interactes with sudoku data structures.
	/// </summary>
	[DisallowNull]
	public PointCalculator? PointCalculator { get; set; }

	/// <summary>
	/// Indicates the image generator that can generates the images which can be shown
	/// on the <see cref="Image"/> control instances.
	/// </summary>
	/// <seealso cref="Image"/>
	[DisallowNull]
	public GridImageGenerator? ImageGenerator { get; set; }

	/// <summary>
	/// Indicates the instance that stores the settings interacting with UI that can be changed by user.
	/// </summary>
	[DisallowNull]
	public Preference? Preference { get; set; }


	/// <inheritdoc/>
	public static SudokuPanelDataContext CreateInstance() => new();
}
