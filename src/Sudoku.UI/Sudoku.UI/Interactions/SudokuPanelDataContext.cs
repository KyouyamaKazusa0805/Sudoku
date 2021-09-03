namespace Sudoku.UI.Interactions;

/// <summary>
/// Provides with the data context that binds with <see cref="SudokuPanel"/>.
/// </summary>
/// <seealso cref="SudokuPanel"/>
public sealed class SudokuPanelDataContext : IDataContext<SudokuPanelDataContext>
{
	///// <summary>
	///// Indicates the default size of the each picture.
	///// </summary>
	//private const float DefaultSize = 540;


	///// <summary>
	///// Indicates the point calculator.
	///// </summary>
	//private readonly IPointCalculator _pointCalculator;

	///// <summary>
	///// Indicates the image generator.
	///// </summary>
	//private readonly IGridImageGenerator _imageGenerator;

	///// <summary>
	///// Indicates the preference instance.
	///// </summary>
	//private readonly IPreference _preference;


	///// <summary>
	///// Initializes a <see cref="SudokuPanelDataContext"/> instance.
	///// </summary>
	//public SudokuPanelDataContext()
	//{
	//	_pointCalculator = PointCalculator.CreateConverter(DefaultSize);
	//	_preference = new Preference();
	//	_imageGenerator = new GridImageGenerator(_pointCalculator, _preference, SudokuGrid.Empty);
	//}


	/// <inheritdoc/>
	public static SudokuPanelDataContext CreateInstance() => new();
}
