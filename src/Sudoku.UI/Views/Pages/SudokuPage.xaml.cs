using Sudoku.Diagnostics.CodeAnalysis;
using Windows.ApplicationModel.DataTransfer;
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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SudokuPage() => InitializeComponent();


	/// <summary>
	/// Triggers when the current page is loaded.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void Page_Loaded([IsDiscard] object sender, [IsDiscard] RoutedEventArgs e) =>
		_cInfoBoard.AddMessage(
			InfoBarSeverity.Informational, StringResource.Get("SudokuPage_InfoBar_Welcome"),
			StringResource.Get("Link_SudokuTutorial"), StringResource.Get("Link_SudokuTutorialDescription"));

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
			_cInfoBoard.AddMessage(InfoBarSeverity.Error, StringResource.Get("SudokuPage_InfoBar_FileIsEmpty"));

			return;
		}

		// Checks the validity of the file, and reads the whole content.
		string content = await File.ReadAllTextAsync(filePath);
		if (string.IsNullOrWhiteSpace(content))
		{
			_cInfoBoard.AddMessage(InfoBarSeverity.Error, StringResource.Get("SudokuPage_InfoBar_FileIsEmpty"));

			return;
		}

		// Checks the file content.
		if (!Grid.TryParse(content, out var grid))
		{
			_cInfoBoard.AddMessage(InfoBarSeverity.Error, StringResource.Get("SudokuPage_InfoBar_FileIsInvalid"));

			return;
		}

		// Checks the validity of the parsed grid.
		if (!grid.IsValid)
		{
			_cInfoBoard.AddMessage(InfoBarSeverity.Warning, StringResource.Get("SudokuPage_InfoBar_FilePuzzleIsNotUnique"));
		}

		// Loads the grid.
		_cPane.Grid = grid;
		_cInfoBoard.AddMessage(InfoBarSeverity.Success, StringResource.Get("SudokuPage_InfoBar_FileOpenSuccessfully"));
	}

	/// <summary>
	/// Triggers when the button is clicked.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void ClearAppBarButton_Click(object sender, RoutedEventArgs e)
	{
		_cPane.Grid = Grid.Empty;
		_cInfoBoard.AddMessage(InfoBarSeverity.Informational, StringResource.Get("SudokuPage_InfoBar_ClearSuccessfully"));
	}

	/// <summary>
	/// Triggers when the button is clicked.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void CopyAppBarButton_Click([IsDiscard] object sender, [IsDiscard] RoutedEventArgs e)
	{
		ref readonly var grid = ref _cPane.GridByReference();
		if (grid.IsUndefined || grid.IsEmpty)
		{
			_cInfoBoard.AddMessage(InfoBarSeverity.Error, StringResource.Get("SudokuPage_InfoBar_CopyFailedDueToEmpty"));
			return;
		}

		new DataPackage { RequestedOperation = DataPackageOperation.Copy }.SetText(grid.ToString("#"));
	}

	/// <summary>
	/// Triggers when the button is clicked.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private async void PasteAppBarButton_ClickAsync([IsDiscard] object sender, [IsDiscard] RoutedEventArgs e)
	{
		var dataPackageView = Clipboard.GetContent();
		if (!dataPackageView.Contains(StandardDataFormats.Text))
		{
			return;
		}

		string gridStr = await dataPackageView.GetTextAsync();
		if (!Grid.TryParse(gridStr, out var grid))
		{
			_cInfoBoard.AddMessage(InfoBarSeverity.Error, StringResource.Get("SudokuPage_InfoBar_PasteIsInvalid"));
			return;
		}

		// Checks the validity of the parsed grid.
		if (!grid.IsValid)
		{
			_cInfoBoard.AddMessage(InfoBarSeverity.Warning, StringResource.Get("SudokuPage_InfoBar_PastePuzzleIsNotUnique"));
		}

		// Loads the grid.
		_cPane.Grid = grid;
		_cInfoBoard.AddMessage(InfoBarSeverity.Success, StringResource.Get("SudokuPage_InfoBar_PasteSuccessfully"));
	}

	/// <summary>
	/// Triggers when the button is clicked.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void ClearInfoBarsAppBarButton_Click([IsDiscard] object sender, [IsDiscard] RoutedEventArgs e) =>
		_cInfoBoard.ClearMessages();

	/// <summary>
	/// Triggers when the button is clicked.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void UndoOrRedo_Click(object sender, [IsDiscard] RoutedEventArgs e) =>
		(
			sender is AppBarButton { Tag: var tag }
				? tag switch { "Undo" => _cPane.UndoStep, "Redo" => _cPane.RedoStep }
				: default(Action)!
		)();
}
