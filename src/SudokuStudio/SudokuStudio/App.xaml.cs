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
	/// Indicates the program-reserved user preference.
	/// </summary>
	public ProgramPreference Preference { get; } = new();

	/// <summary>
	/// Indicates the program solver.
	/// </summary>
	public LogicalSolver ProgramSolver { get; } = new();

	/// <summary>
	/// Indicates the program step gatherer.
	/// </summary>
	public StepsGatherer ProgramGatherer { get; } = new();

	/// <summary>
	/// Indicates the first-opened grid.
	/// </summary>
	[DisallowNull]
	[DebuggerHidden]
	internal Grid? FirstGrid { get; set; }

	/// <summary>
	/// Indicates the window manager.
	/// </summary>
	/// <remarks>
	/// <para>This property is used by checking running window, which is helpful on multiple-window interaction.</para>
	/// <para><i>
	/// This property can also be used by <see cref="FileOpenPicker"/> and <see cref="FileSavePicker"/> cases, for getting the running window
	/// that the current control lies in. However, due to not implementing of WinUI 3 official APIs, when call
	/// <see cref="FileOpenPicker.PickSingleFileAsync()"/> and <see cref="FileSavePicker.PickSaveFileAsync"/> will directly cause
	/// an exception thrown, which scenario is nearly same as
	/// <see href="https://github.com/microsoft/microsoft-ui-xaml/issues/2716">this issue</see> mentioned as an issue in GitHub.
	/// In addition, I found <see href="https://github.com/microsoft/WindowsAppSDK/discussions/1887">this discussion</see>
	/// to describe about <see cref="InitializeWithWindow"/> handling, which wants to solve this problem.
	/// </i></para>
	/// <para><i>
	/// However, today's API still throw exception, which means we can still not use <see cref="FileOpenPicker"/>
	/// and <see cref="FileSavePicker"/> at present.
	/// </i></para>
	/// </remarks>
	/// <seealso cref="FileOpenPicker"/>
	/// <seealso cref="FileSavePicker"/>
	/// <seealso cref="FolderPicker"/>
	/// <seealso cref="InitializeWithWindow"/>
	/// <seealso cref="FileOpenPicker.PickSingleFileAsync()"/>
	/// <seealso cref="FileSavePicker.PickSaveFileAsync"/>
	/// <seealso href="https://github.com/microsoft/microsoft-ui-xaml/issues/2716">
	/// <see cref="FileOpenPicker"/>, <see cref="FileSavePicker"/>, and <see cref="FolderPicker"/> break in WinUI3 Desktop
	/// </seealso>
	/// <seealso href="https://github.com/microsoft/WindowsAppSDK/discussions/1887">
	/// Improved APIs for Picker/Dialog classes that implement <see cref="InitializeWithWindow"/>
	/// </seealso>
	internal ProjectWideWindowManager WindowManager { get; } = new();


	/// <summary>
	/// Indicates the assembly version.
	/// </summary>
	[DebuggerHidden]
	internal static Version AssemblyVersion => typeof(App).Assembly.GetName().Version!;


	/// <inheritdoc/>
	protected override void OnLaunched(LaunchActivatedEventArgs args)
	{
		RegisterResourceFetching();
		HandleOnProgramOpeningEntryCase();
		ActivicateMainWindow();
		LoadConfigurationFileFromLocal();
	}

	/// <summary>
	/// Register resource-fetching service.
	/// </summary>
	private void RegisterResourceFetching() => MergedResources.R.RegisterAssembly(typeof(App).Assembly);

	/// <summary>
	/// Creates a window, and activicate it.
	/// </summary>
	private void ActivicateMainWindow() => WindowManager.CreateWindow<MainWindow>().Activate();

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

	/// <summary>
	/// Loads configuration file from local path.
	/// </summary>
	private void LoadConfigurationFileFromLocal()
	{
		var targetPath = CommonPaths.UserPreference;
		if (File.Exists(targetPath) && ProgramPreferenceFileHandler.Read(targetPath) is { } loadedConfig)
		{
			Preference.CoverBy(loadedConfig);
		}
	}
}
