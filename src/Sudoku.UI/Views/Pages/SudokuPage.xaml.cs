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
		ControlFactory
			.CreateInfoBar(InfoBarSeverity.Informational, _cStackPanelDetails)
			.WithMessage(StringResource.Get("SudokuPage_InfoBar_Welcome"))
			.WithLinkButton(StringResource.Get("Link_SudokuTutorial"), StringResource.Get("Link_SudokuTutorialDescription"))
			.Open();

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
			ControlFactory
				.CreateInfoBar(InfoBarSeverity.Error, _cStackPanelDetails)
				.WithMessage(StringResource.Get("SudokuPage_InfoBar_FileIsEmpty"))
				.Open();

			return;
		}

		// Checks the validity of the file, and reads the whole content.
		string content = await File.ReadAllTextAsync(filePath);
		if (string.IsNullOrWhiteSpace(content))
		{
			ControlFactory
				.CreateInfoBar(InfoBarSeverity.Error, _cStackPanelDetails)
				.WithMessage(StringResource.Get("SudokuPage_InfoBar_FileIsEmpty"))
				.Open();

			return;
		}

		// Checks the file content.
		if (!Grid.TryParse(content, out var grid))
		{
			ControlFactory
				.CreateInfoBar(InfoBarSeverity.Error, _cStackPanelDetails)
				.WithMessage(StringResource.Get("SudokuPage_InfoBar_FileIsInvalid"))
				.Open();

			return;
		}

		// Checks the validity of the parsed grid.
		if (!grid.IsValid)
		{
			ControlFactory
				.CreateInfoBar(InfoBarSeverity.Warning, _cStackPanelDetails)
				.WithMessage(StringResource.Get("SudokuPage_InfoBar_FilePuzzleIsNotUnique"))
				.Open();
		}

		// Loads the grid.
		_cPane.Grid = grid;
		ControlFactory
			.CreateInfoBar(InfoBarSeverity.Success, _cStackPanelDetails)
			.WithMessage(StringResource.Get("SudokuPage_InfoBar_FileOpenSuccessfully"))
			.Open();
	}

	/// <summary>
	/// Triggers when the button is clicked.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void ClearAppBarButton_Click(object sender, RoutedEventArgs e)
	{
		_cPane.Grid = Grid.Empty;

		ControlFactory
			.CreateInfoBar(InfoBarSeverity.Informational, _cStackPanelDetails)
			.WithMessage(StringResource.Get("SudokuPage_InfoBar_ClearSuccessfully"))
			.Open();
	}

	/// <summary>
	/// Triggers when the button is clicked.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void CopyAppBarButton_Click([IsDiscard] object sender, [IsDiscard] RoutedEventArgs e)
	{
		ref readonly var grid = ref _cPane.GetGridByReference();
		if (grid.IsUndefined || grid.IsEmpty)
		{
			ControlFactory
				.CreateInfoBar(InfoBarSeverity.Error, _cStackPanelDetails)
				.WithMessage(StringResource.Get("SudokuPage_InfoBar_CopyFailedDueToEmpty"))
				.Open();
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
			ControlFactory
				.CreateInfoBar(InfoBarSeverity.Error, _cStackPanelDetails)
				.WithMessage(StringResource.Get("SudokuPage_InfoBar_PasteIsInvalid"))
				.Open();
			return;
		}

		// Checks the validity of the parsed grid.
		if (!grid.IsValid)
		{
			ControlFactory
				.CreateInfoBar(InfoBarSeverity.Warning, _cStackPanelDetails)
				.WithMessage(StringResource.Get("SudokuPage_InfoBar_PastePuzzleIsNotUnique"))
				.Open();
		}

		// Loads the grid.
		_cPane.Grid = grid;
		ControlFactory
			.CreateInfoBar(InfoBarSeverity.Success, _cStackPanelDetails)
			.WithMessage(StringResource.Get("SudokuPage_InfoBar_PasteSuccessfully"))
			.Open();
	}
}
