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
	public PointCalculator PointCalculator { get; set; } = null!;

	/// <summary>
	/// Indicates the image generator that can generates the images which can be shown
	/// on the <see cref="Image"/> control instances.
	/// </summary>
	/// <seealso cref="Image"/>
	public GridImageGenerator ImageGenerator { get; set; } = null!;

	/// <summary>
	/// Indicates the instance that stores the settings interacting with UI that can be changed by user.
	/// </summary>
	public Preference Preference { get; set; } = null!;


	/// <inheritdoc/>
	public static SudokuPanelDataContext CreateInstance() => new();
}
