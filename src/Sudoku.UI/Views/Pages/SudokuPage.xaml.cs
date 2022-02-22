using Microsoft.UI.Xaml.Input;
using Sudoku.Diagnostics.CodeAnalysis;
using Sudoku.UI.Drawing;
using Sudoku.UI.Views.Controls;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Pickers;
using static Sudoku.UI.ControlFactory;

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
		InfoBar(InfoBarSeverity.Informational)
			.WithParentPanel(_cStackPanelDetails)
			.WithMessage(StringResource.Get("SudokuPage_InfoBar_Welcome"))
			.WithLinkButton(
				StringResource.Get("Link_SudokuTutorial"),
				StringResource.Get("Link_SudokuTutorialDescription")
			)
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
			InfoBar(InfoBarSeverity.Error)
				.WithParentPanel(_cStackPanelDetails)
				.WithMessage(StringResource.Get("SudokuPage_InfoBar_FileIsEmpty"))
				.Open();

			return;
		}

		// Checks the validity of the file, and reads the whole content.
		string content = await File.ReadAllTextAsync(filePath);
		if (string.IsNullOrWhiteSpace(content))
		{
			InfoBar(InfoBarSeverity.Error)
				.WithParentPanel(_cStackPanelDetails)
				.WithMessage(StringResource.Get("SudokuPage_InfoBar_FileIsEmpty"))
				.Open();

			return;
		}

		// Checks the file content.
		if (!Grid.TryParse(content, out var grid))
		{
			InfoBar(InfoBarSeverity.Error)
				.WithParentPanel(_cStackPanelDetails)
				.WithMessage(StringResource.Get("SudokuPage_InfoBar_FileIsInvalid"))
				.Open();

			return;
		}

		// Checks the validity of the parsed grid.
		if (!grid.IsValid)
		{
			InfoBar(InfoBarSeverity.Warning)
				.WithParentPanel(_cStackPanelDetails)
				.WithMessage(StringResource.Get("SudokuPage_InfoBar_FilePuzzleIsNotUnique"))
				.Open();
		}

		// Loads the grid.
		_cPane.Grid = grid;
		InfoBar(InfoBarSeverity.Success)
			.WithParentPanel(_cStackPanelDetails)
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

		InfoBar(InfoBarSeverity.Informational)
			.WithParentPanel(_cStackPanelDetails)
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
		ref readonly var grid = ref _cPane.GridByReference();
		if (grid.IsUndefined || grid.IsEmpty)
		{
			InfoBar(InfoBarSeverity.Error)
				.WithParentPanel(_cStackPanelDetails)
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
			InfoBar(InfoBarSeverity.Error)
				.WithParentPanel(_cStackPanelDetails)
				.WithMessage(StringResource.Get("SudokuPage_InfoBar_PasteIsInvalid"))
				.Open();
			return;
		}

		// Checks the validity of the parsed grid.
		if (!grid.IsValid)
		{
			InfoBar(InfoBarSeverity.Warning)
				.WithParentPanel(_cStackPanelDetails)
				.WithMessage(StringResource.Get("SudokuPage_InfoBar_PastePuzzleIsNotUnique"))
				.Open();
		}

		// Loads the grid.
		_cPane.Grid = grid;
		InfoBar(InfoBarSeverity.Success)
			.WithParentPanel(_cStackPanelDetails)
			.WithMessage(StringResource.Get("SudokuPage_InfoBar_PasteSuccessfully"))
			.Open();
	}

	/// <summary>
	/// Triggers when the button is clicked.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void ClearInfoBarsAppBarButton_Click([IsDiscard] object sender, [IsDiscard] RoutedEventArgs e)
	{
		if (_cStackPanelDetails.Children is [_, ..] children)
		{
			children.Clear();
		}
	}

	/// <summary>
	/// Triggers when the target <see cref="MenuFlyoutItem"/> is clicked.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void MakeOrDeleteMenuItem_Click(object sender, [IsDiscard] RoutedEventArgs e)
	{
		if (sender is not MenuFlyoutItem { Tag: string s } || !int.TryParse(s, out int possibleDigit))
		{
			return;
		}

		int currentCell = _cPane.CurrentCell;
		var (digit, action) = possibleDigit switch
		{
			> 0 => (possibleDigit - 1, _cPane.MakeDigit),
			< 0 => (~possibleDigit, _cPane.EliminateDigit), // '~a' is equi to '-a - 1'.
			_ => (default(int), default(Action<int, int>)!)
		};

		action(currentCell, digit);
	}

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

	/// <summary>
	/// Triggers when the sudoku pane <see cref="_cPane"/> is tapped via the right mouse.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void SudokuPane_RightTapped(object sender, RightTappedRoutedEventArgs e)
	{
		// Checks the value.
		if (sender is not SudokuPane { Size: var size, OutsideOffset: var outsideOffset } pane)
		{
			goto MakeHandledBeTrueValue;
		}

		// Gets the mouse point and converts to the real cell value.
		var point = e.GetPosition(pane);
		int cell = PointConversions.GetCell(point, size, outsideOffset);

		// Checks whether the cell value is valid.
		if (cell == -1)
		{
			goto MakeHandledBeTrueValue;
		}

		// Sets the tag with the cell value, in order to get the details by other methods later.
		pane.CurrentCell = cell;

		// Try to create the menu flyout and show the items.
		for (int i = 0; i < 9; i++)
		{
			((MenuFlyoutItem)FindName($"_cButtonMake{i + 1}")).Visibility = getVisibilityViaCandidate(cell, i);
		}
		for (int i = 0; i < 9; i++)
		{
			((MenuFlyoutItem)FindName($"_cButtonDelete{i + 1}")).Visibility = getVisibilityViaCandidate(cell, i);
		}

	MakeHandledBeTrueValue:
		// If at an invalid status, just return the method and make the property 'e.Handled' be true value.
		e.Handled = true;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		Visibility getVisibilityViaCandidate(int cell, int i) =>
			_cPane.Grid.Exists(cell, i) is true ? Visibility.Visible : Visibility.Collapsed;
	}
}
