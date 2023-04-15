namespace SudokuStudio.Views.Pages.Operation;

/// <summary>
/// Indicates the basic operation command bar.
/// </summary>
[DependencyProperty<string>("SucceedFilePath", IsNullable = true, Accessibility = GeneralizedAccessibility.Internal, DocSummary = "Indicates the path of the saved file.")]
public sealed partial class BasicOperation : Page, IOperationProviderPage
{
	/// <summary>
	/// Defines a default puzzle generator.
	/// </summary>
	private static readonly PatternBasedPuzzleGenerator Generator = new();


	/// <summary>
	/// Initializes a <see cref="BasicOperation"/> instance.
	/// </summary>
	public BasicOperation() => InitializeComponent();


	/// <inheritdoc/>
	public AnalyzePage BasePage { get; set; } = null!;


	/// <summary>
	/// The method called by <see cref="Page_Loaded(object, RoutedEventArgs)"/> and <see cref="Page_Unloaded(object, RoutedEventArgs)"/>.
	/// </summary>
	private void OnSaveFileFailed(AnalyzePage _, SaveFileFailedEventArgs e)
		=> (e.Reason switch { SaveFileFailedReason.UnsnappingFailed => ErrorDialog_ProgramIsSnapped }).IsOpen = true;

	/// <summary>
	/// The method called by <see cref="Page_Loaded(object, RoutedEventArgs)"/> and <see cref="Page_Unloaded(object, RoutedEventArgs)"/>.
	/// </summary>
	private void OnOpenFileFailed(AnalyzePage _, OpenFileFailedEventArgs e)
		=> (
			e.Reason switch
			{
				OpenFileFailedReason.UnsnappingFailed => ErrorDialog_ProgramIsSnapped,
				OpenFileFailedReason.FileIsEmpty => ErrorDialog_FileIsEmpty,
				OpenFileFailedReason.FileIsTooLarge => ErrorDialog_FileIsOversized,
				OpenFileFailedReason.FileCannotBeParsed => ErrorDialog_FileCannotBeParsed
			}
		).IsOpen = true;


	private void Page_Loaded(object sender, RoutedEventArgs e)
	{
		BasePage.OpenFileFailed += OnOpenFileFailed;
		BasePage.SaveFileFailed += OnSaveFileFailed;
	}

	private void Page_Unloaded(object sender, RoutedEventArgs e)
	{
		BasePage.OpenFileFailed -= OnOpenFileFailed;
		BasePage.SaveFileFailed -= OnSaveFileFailed;
	}

	private async void NewPuzzleButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		BasePage.GeneratorIsNotRunning = false;

		var grid = await Generator.GenerateAsync();

		BasePage.GeneratorIsNotRunning = true;

		if (((App)Application.Current).Preference.UIPreferences.SavePuzzleGeneratingHistory)
		{
			((App)Application.Current).PuzzleGeneratingHistory.Puzzles.Add(new() { GridString = grid.ToString() });
		}

		BasePage.SudokuPane.Puzzle = grid;
	}

	private void SaveAsButton_Click(object sender, RoutedEventArgs e) => Dialog_FormatChoosing.IsOpen = true;

	private void CopyButton_Click(object sender, RoutedEventArgs e) => BasePage.CopySudokuGridText();

	private async void CopyPictureButton_ClickAsync(object sender, RoutedEventArgs e) => await BasePage.CopySudokuGridControlAsSnapshotAsync();

	private async void PasteButton_ClickAsync(object sender, RoutedEventArgs e) => await BasePage.PasteCodeToSudokuGridAsync();

	private void ResetButton_Click(object sender, RoutedEventArgs e)
	{
		var puzzle = BasePage.SudokuPane.Puzzle;
		puzzle.Reset();

		BasePage.SudokuPane.Puzzle = puzzle;
	}

	private void ClearButton_Click(object sender, RoutedEventArgs e) => Dialog_AreYouSureToReturnToEmpty.IsOpen = true;

	private void UndoButton_Click(object sender, RoutedEventArgs e) => BasePage.SudokuPane.UndoStep();

	private void RedoButton_Click(object sender, RoutedEventArgs e) => BasePage.SudokuPane.RedoStep();

	private async void Dialog_FormatChoosing_ActionButtonClickAsync(TeachingTip sender, object args)
	{
		var flags = (
			from children in FormatGroupPanel.Children
			where children is CheckBox { Tag: int and not 0, IsChecked: true }
			select (FormatFlags)(int)((CheckBox)children).Tag!
		).Aggregate(FormatFlags.None, static (interim, next) => interim | next);
		if (flags == FormatFlags.None)
		{
			return;
		}

		if (await BasePage.SaveFileInternalAsync(createFormatHandlers(flags)))
		{
			Dialog_FormatChoosing.IsOpen = false;
		}


		static ArrayList createFormatHandlers(FormatFlags flags)
		{
			var formats = new ArrayList();
			foreach (var flag in flags.GetAllFlagsDistinct()!)
			{
				formats.Add(
					flag switch
					{
						FormatFlags.InitialFormat => SusserFormat.Default,
						FormatFlags.CurrentFormat => SusserFormat.Full,
						FormatFlags.CurrentFormatIgnoringValueKind => SusserFormatTreatingValuesAsGivens.Default,
						FormatFlags.HodokuCompatibleFormat => HodokuLibraryFormat.Default,
						FormatFlags.MultipleGridFormat => MultipleLineFormat.Default,
						FormatFlags.PencilMarkFormat => PencilMarkFormat.Default,
						FormatFlags.SukakuFormat => SukakuFormat.Default,
						FormatFlags.ExcelFormat => ExcelFormat.Default,
						FormatFlags.OpenSudokuFormat => OpenSudokuFormat.Default
					}
				);
			}

			return formats;
		}
	}

	private void Dialog_AreYouSureToReturnToEmpty_ActionButtonClick(TeachingTip sender, object args)
	{
		BasePage.SudokuPane.Puzzle = Grid.Empty;

		Dialog_AreYouSureToReturnToEmpty.IsOpen = false;
	}

	private async void SaveFileButton_ClickAsync(object sender, RoutedEventArgs e) => await BasePage.SaveFileInternalAsync();

	private async void OpenFileButton_ClickAsync(object sender, RoutedEventArgs e) => await BasePage.OpenFileInternalAsync();
}

/// <summary>
/// Represents a format flag.
/// </summary>
[Flags]
file enum FormatFlags
{
	/// <summary>
	/// Indicates the default format.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	None = 0,

	/// <summary>
	/// Indicates the initial grid format.
	/// </summary>
	InitialFormat = 1,

	/// <summary>
	/// Indicates the current grid format.
	/// </summary>
	CurrentFormat = 1 << 1,

	/// <summary>
	/// Indicates the current grid format , treating all modifiable values as given ones.
	/// </summary>
	CurrentFormatIgnoringValueKind = 1 << 2,

	/// <summary>
	/// Indicates the Hodoku grid format.
	/// </summary>
	HodokuCompatibleFormat = 1 << 3,

	/// <summary>
	/// Indicates the multiple-line grid format.
	/// </summary>
	MultipleGridFormat = 1 << 4,

	/// <summary>
	/// Indicates the pencilmark format.
	/// </summary>
	PencilMarkFormat = 1 << 5,

	/// <summary>
	/// Indicates the sukaku format.
	/// </summary>
	SukakuFormat = 1 << 6,

	/// <summary>
	/// Indicates the excel format.
	/// </summary>
	ExcelFormat = 1 << 7,

	/// <summary>
	/// Indicates the open-sudoku format.
	/// </summary>
	OpenSudokuFormat = 1 << 8
}
