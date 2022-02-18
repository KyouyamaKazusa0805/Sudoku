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
		var file = await new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary }
			.AddFileTypeFilter(CommonFileExtensions.Text)
			.AwareHandleOnWin32()
			.PickSingleFileAsync();
		if (file is not { Path: var filePath })
		{
			return;
		}

		if (new FileInfo(filePath).Length == 0)
		{
			_cInfoBarDetails
				.WithText(InfoBarSeverity.Error, (string)Application.Current.Resources["SudokuPage_InfoBar_FileIsEmpty"])
				.Open();

			return;
		}

		// Checks the validity of the file, and reads the whole content.
		string content = await File.ReadAllTextAsync(filePath);
		if (string.IsNullOrWhiteSpace(content))
		{
			_cInfoBarDetails
				.WithText(InfoBarSeverity.Error, (string)Application.Current.Resources["SudokuPage_InfoBar_FileIsEmpty"])
				.Open();

			return;
		}

		// Checks the file content.
		if (!Grid.TryParse(content, out var grid))
		{
			_cInfoBarDetails
				.WithText(InfoBarSeverity.Error, (string)Application.Current.Resources["SudokuPage_InfoBar_FileIsInvalid"])
				.Open();

			return;
		}

		// Checks the validity of the parsed grid.
		if (!grid.IsValid)
		{
			_cInfoBarDetails
				.WithText(InfoBarSeverity.Warning, (string)Application.Current.Resources["SudokuPage_InfoBar_FilePuzzleIsNotUnique"])
				.Open();
		}

		// Loads the grid.
		_cPane.Grid = grid;
		_cInfoBarDetails
			.WithText(InfoBarSeverity.Success, (string)Application.Current.Resources["SudokuPage_InfoBar_FileOpenSuccessfully"])
			.Open();
	}
}
