using Windows.Storage.Pickers;

namespace SudokuStudio.Views.Pages.Operation;

/// <summary>
/// Indicates the basic operation command bar.
/// </summary>
public sealed partial class BasicOperation : Page
{
	/// <summary>
	/// Defines a default puzzle generator.
	/// </summary>
	private static readonly PatternBasedPuzzleGenerator Generator = new();


	/// <summary>
	/// Initializes a <see cref="BasicOperation"/> instance.
	/// </summary>
	public BasicOperation() => InitializeComponent();


	/// <summary>
	/// Indicates the base page.
	/// </summary>
	public AnalyzePage BasePage { get; set; } = null!;


	/// <summary>
	/// To determine whether the current application view is in an unsnapped state.
	/// </summary>
	/// <returns>The <see cref="bool"/> value indicating that.</returns>
	private bool EnsureUnsnapped()
	{
		/// <see cref="FileOpenPicker"/> APIs will not work if the application is in a snapped state.
		/// If an app wants to show a <see cref="FileOpenPicker"/> while snapped, it must attempt to unsnap first.
		var unsnapped = ApplicationView.Value != ApplicationViewState.Snapped || ApplicationView.TryUnsnap();
		if (!unsnapped)
		{
			ErrorDialog_ProgramIsSnapped.IsOpen = true;
		}

		return unsnapped;
	}


	private async void OpenFileButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		var fop = new FileOpenPicker()
			.WithSuggestedStartLocation(PickerLocationId.DocumentsLibrary)
			.AddFileTypeFilter(CommonFileExtensions.Text)
			.AddFileTypeFilter(CommonFileExtensions.PlainText)
			.WithAwareHandleOnWin32();

		if (await fop.PickSingleFileAsync() is not { Path: var filePath } file)
		{
			return;
		}

		var fileInfo = new FileInfo(filePath);
		var (errorDialog, targetGrid) = fileInfo.Length switch
		{
			0 => (ErrorDialog_FileIsEmpty, null),
			> 1024 => (ErrorDialog_FileIsOversized, null),
			_ when await FileIO.ReadTextAsync(file) is var content => string.IsNullOrWhiteSpace(content) switch
			{
				true => (ErrorDialog_FileIsEmpty, null),
				false => Grid.TryParse(content, out var g) switch
				{
					false => (ErrorDialog_FileCannotBeParsed, null),
					true => g.IsValid() switch
					{
						false => (ErrorDialog_FileGridIsNotUnique, null),
						true => (null, g)
					}
				}
			},
			_ => default((TeachingTip?, Grid?))
		};

		if (errorDialog is not null)
		{
			errorDialog.IsOpen = true;

			return;
		}

		if (targetGrid is { } grid)
		{
			BasePage.SudokuPane.Puzzle = grid;
		}
	}

	private async void NewPuzzleButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		BasePage.GeneratorIsNotRunning = false;

		var grid = await Generator.GenerateAsync();

		BasePage.GeneratorIsNotRunning = true;

		BasePage.SudokuPane.Puzzle = grid;
	}
}
