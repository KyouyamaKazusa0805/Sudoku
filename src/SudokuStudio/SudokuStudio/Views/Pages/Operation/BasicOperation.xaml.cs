namespace SudokuStudio.Views.Pages.Operation;

/// <summary>
/// Indicates the basic operation command bar.
/// </summary>
public sealed partial class BasicOperation : Page, INotifyPropertyChanged
{
	/// <summary>
	/// Defines a default puzzle generator.
	/// </summary>
	private static readonly PatternBasedPuzzleGenerator Generator = new();


	/// <summary>
	/// Indicates the path of the saved file.
	/// </summary>
	[NotifyBackingField(Accessibility = GeneralizedAccessibility.Internal)]
	private string? _succeedFilePath;


	/// <summary>
	/// Initializes a <see cref="BasicOperation"/> instance.
	/// </summary>
	public BasicOperation() => InitializeComponent();


	/// <summary>
	/// Indicates the base page.
	/// </summary>
	public AnalyzePage BasePage { get; set; } = null!;


	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged;


	private void Page_Loaded(object sender, RoutedEventArgs e)
	{
		BasePage.OpenFileFailed += onOpenFileFailed;
		BasePage.SaveFileFailed += onSaveFileFailed;


		void onOpenFileFailed(AnalyzePage _, OpenFileFailedEventArgs e)
			=> (
				e.Reason switch
				{
					OpenFileFailedReason.UnsnappingFailed => ErrorDialog_ProgramIsSnapped,
					OpenFileFailedReason.FileIsEmpty => ErrorDialog_FileIsEmpty,
					OpenFileFailedReason.FileIsTooLarge => ErrorDialog_FileIsOversized,
					OpenFileFailedReason.FileCannotBeParsed => ErrorDialog_FileCannotBeParsed
				}
			).IsOpen = true;

		void onSaveFileFailed(AnalyzePage _, SaveFileFailedEventArgs e)
			=> (
				e.Reason switch
				{
					SaveFileFailedReason.UnsnappingFailed => ErrorDialog_ProgramIsSnapped
				}
			).IsOpen = true;
	}

	private async void OpenFileButton_ClickAsync(object sender, RoutedEventArgs e) => await BasePage.OpenFileInternalAsync();

	private async void SaveFileButton_ClickAsync(object sender, RoutedEventArgs e) => await BasePage.SaveFileInternalAsync();

	private async void NewPuzzleButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		BasePage.GeneratorIsNotRunning = false;

		var grid = await Generator.GenerateAsync();

		BasePage.GeneratorIsNotRunning = true;

		BasePage.SudokuPane.Puzzle = grid;
	}

	private void SaveAsButton_Click(object sender, RoutedEventArgs e) => Dialog_FormatChoosing.IsOpen = true;

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
}

/// <summary>
/// Represents a format flag.
/// </summary>
[Flags]
file enum FormatFlags : int
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
