using System.Windows;
using System.Windows.Media;
using Sudoku.Data;
using Sudoku.Solving.Manual;
using Sudoku.Windows.Extensions;

namespace Sudoku.Windows;

/// <summary>
/// Interaction logic for <c>ErrorInfoWindow.xaml</c>.
/// </summary>
public partial class ErrorInfoWindow : Window
{
	/// <summary>
	/// Initializes a default <see cref="ErrorInfoWindow"/> instance.
	/// </summary>
	public ErrorInfoWindow() => InitializeComponent();


	/// <summary>
	/// Indicates the error text to show.
	/// </summary>
	private string ErrorText { init => _textBoxInfo.Text = value; }

	/// <summary>
	/// Indicates the picture to show.
	/// </summary>
	private ImageSource Picture { init => _imageErrorStep.Source = value; }


	/// <summary>
	/// Creates an <see cref="ErrorInfoWindow"/> with the inner information text
	/// and the error step.
	/// </summary>
	/// <param name="errorStep">The error step to display to the user.</param>
	/// <param name="grid">The grid used.</param>
	/// <returns>The instance.</returns>
	public static ErrorInfoWindow Create(StepInfo errorStep, in SudokuGrid grid) =>
		new()
		{
			ErrorText = $"{Application.Current.Resources["ErrorInfoGrid"]}{grid.ToString("#")}\r\n{errorStep}",
			Picture = errorStep.CreateBitmap(grid).ToImageSource()
		};

	/// <summary>
	/// Creates an <see cref="ErrorInfoWindow"/> with the specified title, the inner information text
	/// and the error step.
	/// </summary>
	/// <param name="title">The title.</param>
	/// <param name="errorStep">The error step to display to the user.</param>
	/// <param name="grid">The grid used.</param>
	/// <returns>The instance.</returns>
	public static ErrorInfoWindow Create(string title, StepInfo errorStep, in SudokuGrid grid) =>
		new()
		{
			Title = title,
			ErrorText = $"{Application.Current.Resources["ErrorInfoGrid"]}{grid.ToString("#")}\r\n{errorStep}",
			Picture = errorStep.CreateBitmap(grid).ToImageSource()
		};
}
