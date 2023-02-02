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
	public ProgramPreference ProgramPreference { get; } = new();

	/// <summary>
	/// Defines a set of environment variables used.
	/// </summary>
	internal EnvironmentVariable EnvironmentVariables { get; } = new();


	/// <summary>
	/// Indicates the assembly version.
	/// </summary>
	[DebuggerHidden]
	internal static Version AssemblyVersion => typeof(App).Assembly.GetName().Version!;


	/// <inheritdoc/>
	[MemberNotNull(nameof(RunningWindow))]
	protected override void OnLaunched(LaunchActivatedEventArgs args)
	{
		// Register resource-fetching service.
		R.RegisterAssembly(typeof(App).Assembly);

		// Handles on opening event. This value will be used if not opening by program entry.
		PreinstantiateProgram();

		// Activicate the main window.
		(RunningWindow = new MainWindow()).Activate();
	}

	/// <summary>
	/// Pre-instantiate program.
	/// </summary>
	private void PreinstantiateProgram()
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

		EnvironmentVariables.FirstGrid = grid;
	}
}
