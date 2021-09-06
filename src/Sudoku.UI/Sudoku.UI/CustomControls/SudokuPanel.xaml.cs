namespace Sudoku.UI.CustomControls;

/// <summary>
/// Defines a basic sudoku panel.
/// </summary>
public sealed partial class SudokuPanel : UserControl
{
	/// <summary>
	/// Initializes a <see cref="SudokuPanel"/> instance.
	/// </summary>
	public SudokuPanel()
	{
		InitializeComponent();

#if false
		Preference = new();
		PointCalculator = (PointCalculator)PointCalculator.CreateConverter(540, 10);
		ImageGenerator = new(PointCalculator, Preference, SudokuGrid.Empty);

		PutGeneratedImageOntoControl();
#endif
	}


#if false
	/// <summary>
	/// Indicates the calculator that calculates the pixels and interactes with sudoku data structures.
	/// </summary>
	public PointCalculator PointCalculator { get; private set; }

	/// <summary>
	/// Indicates the image generator that can generates the images which can be shown
	/// on the <see cref="Image"/> control instances.
	/// </summary>
	/// <seealso cref="Image"/>
	public GridImageGenerator ImageGenerator { get; private set; }

	/// <summary>
	/// Indicates the instance that stores the settings interacting with UI that can be changed by user.
	/// </summary>
	public Preference Preference { get; private set; }


	/// <summary>
	/// Puts the specified image result onto the image control to display it.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void PutGeneratedImageOntoControl()
	{
		var image = ImageGenerator.DrawManually();

		PutGeneratedImageOntoControl(image);
	}

	/// <summary>
	/// Puts the specified image result onto the image control to display it.
	/// </summary>
	/// <param name="image">The image control.</param>
	/// <exception cref="InvalidOperationException">Throws when the source image is invalid.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void PutGeneratedImageOntoControl(gdip::Image image) =>
		Image.Source = image is Bitmap src
			? BitmapImage.FromAbi(src.GetHbitmap())
			: throw new InvalidOperationException("The source image is invalid.");
#endif
}
