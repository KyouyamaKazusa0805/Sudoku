namespace SudokuStudio;

/// <summary>
/// Provides application-specific behavior to supplement the default <see cref="Application"/> class.
/// </summary>
/// <seealso cref="Application"/>
public partial class App : Application
{
	/// <summary>
	/// Indicates the command-line arguments.
	/// </summary>
	private readonly string[] _commandLineArgs;


	/// <summary>
	/// <para>Initializes the singleton application object via command-line arguments.</para>
	/// <para>
	/// This is the first line of authored code executed, and as such is the logical equivalent of <c>main()</c> or <c>WinMain()</c>.
	/// </para>
	/// </summary>
	/// <param name="args">The command-line arguments.</param>
	public App(string[] args)
	{
		InitializeComponent();

		_commandLineArgs = args;
	}


	/// <summary>
	/// Indicates the main window that the program is running. If <see langword="null"/>, no window will be run.
	/// </summary>
	public Window? RunningWindow { get; private set; }

	/// <summary>
	/// Indicates the program-reserved user preference.
	/// </summary>
	public ProgramPreference Preference { get; } = new();

	/// <summary>
	/// Indicates the first-opened grid.
	/// </summary>
	[DisallowNull]
	[DebuggerHidden]
	internal Grid? FirstGrid { get; set; }

	/// <summary>
	/// Defines the sudoku pane used.
	/// </summary>
	[DebuggerHidden]
	internal SudokuPane? SudokuPane { get; set; }

	/// <summary>
	/// Indicates the program solver.
	/// </summary>
	[DebuggerHidden]
	internal LogicalSolver ProgramSolver => (LogicalSolver)SudokuPane!.Resources[nameof(ProgramSolver)];

	/// <summary>
	/// Indicates the program step gatherer.
	/// </summary>
	[DebuggerHidden]
	internal StepsGatherer ProgramGatherer => (StepsGatherer)SudokuPane!.Resources[nameof(ProgramGatherer)];


	/// <summary>
	/// Indicates the assembly version.
	/// </summary>
	[DebuggerHidden]
	internal static Version AssemblyVersion => typeof(App).Assembly.GetName().Version!;


	/// <inheritdoc/>
	[MemberNotNull(nameof(RunningWindow))]
	protected override void OnLaunched(LaunchActivatedEventArgs args)
	{
		RegisterResourceFetching();
		HandleOnProgramOpeningEntryCase();
		ActivicateMainWindow<MainWindow>();
	}

	/// <summary>
	/// Register resource-fetching service.
	/// </summary>
	private void RegisterResourceFetching() => MergedResources.R.RegisterAssembly(typeof(App).Assembly);

	/// <summary>
	/// Creates a window, and activicate it.
	/// </summary>
	/// <typeparam name="TWindow">The type of the window you should activicate.</typeparam>
	[MemberNotNull(nameof(RunningWindow))]
	private void ActivicateMainWindow<TWindow>() where TWindow : Window, new() => (RunningWindow = new TWindow()).Activate();

	/// <summary>
	/// Handle the cases how user opens this program.
	/// </summary>
	private void HandleOnProgramOpeningEntryCase()
	{
		if (_commandLineArgs is null or not [])
		{
			return;
		}

		if (AppInstance.GetCurrent().GetActivatedEventArgs() is not
			{
				Kind: ExtendedActivationKind.File,
				Data: IFileActivatedEventArgs { Files: [StorageFile { FileType: CommonFileExtensions.Text, Path: var filePath } file, ..] }
			})
		{
			return;
		}

		if (SudokuFileHandler.Read(filePath) is not [{ GridString: var gridStr }, ..] || !Grid.TryParse(gridStr, out var grid))
		{
			return;
		}

		FirstGrid = grid;
	}
}
