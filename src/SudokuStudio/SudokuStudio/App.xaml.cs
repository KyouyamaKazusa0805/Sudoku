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
	/// <para>Initializes the singleton application object.</para>
	/// <para>
	/// This is the first line of authored code executed, and as such is the logical equivalent of <c>main()</c> or <c>WinMain()</c>.
	/// </para>
	/// </summary>
	public App(string[] args)
	{
		InitializeComponent();

		_commandLineArgs = args;
	}


	/// <summary>
	/// Indicates the running context.
	/// </summary>
	public RunningContext RunningContext { get; } = new();


	/// <inheritdoc/>
	protected override void OnLaunched(LaunchActivatedEventArgs args)
	{
		// Register resource-fetching service.
		R.RegisterAssembly(typeof(App).Assembly);

		// Handles on opening event. This value will be used if not opening by program entry.
		PreinstantiateProgram();

		// Activicate the main window.
		(RunningContext.MainWindow = new MainWindow()).Activate();
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

		RunningContext.FirstGrid = grid;
	}
}
