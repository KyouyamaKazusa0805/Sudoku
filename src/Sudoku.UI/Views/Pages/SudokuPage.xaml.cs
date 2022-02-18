using Sudoku.Diagnostics.CodeAnalysis;
using Windows.Storage.Pickers;

namespace Sudoku.UI.Views.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SudokuPage : Page
{
	/// <summary>
	/// Initializes a <see cref="SudokuPage"/> instance.
	/// </summary>
	public SudokuPage() => InitializeComponent();


	/// <summary>
	/// Triggers when the button is clicked.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private async void OpenAppBarButton_ClickAsync([IsDiscard] object sender, [IsDiscard] RoutedEventArgs e)
	{
		// Open a text file.
		var fop = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
		fop.FileTypeFilter.Add(CommonFileExtensions.Text);

		// When running on win32, FileOpenPicker needs to know the top-level hwnd
		// via IInitializeWithWindow.Initialize.
		fop.AwareHandleOnWin32();

		var file = await fop.PickSingleFileAsync();
		if (file is not { Path: var filePath })
		{
			return;
		}

		_cInfoBarDetails.Content = new TextBlock { Text = $"File path: {filePath}.", Padding = new(0, 0, 0, 20) };
		_cInfoBarDetails.IsOpen = true;
	}
}
