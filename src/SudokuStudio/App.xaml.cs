namespace SudokuStudio;

/// <summary>
/// Provides application-specific behavior to supplement the default <see cref="Application"/> class.
/// </summary>
/// <seealso cref="Application"/>
public partial class App : Application
{
	/// <summary>
	/// <para>Initializes the singleton application object via command-line arguments.</para>
	/// <para>
	/// This is the first line of authored code executed, and as such is the logical equivalent of <c>main()</c> or <c>WinMain()</c>.
	/// </para>
	/// </summary>
	public App() => InitializeComponent();


	/// <summary>
	/// Indicates the program-reserved user preference.
	/// </summary>
	public ProgramPreference Preference { get; } = new();

	/// <summary>
	/// Indicates the puzzle-generating history.
	/// </summary>
	public PuzzleGenertingHistory PuzzleGeneratingHistory { get; } = new();

	/// <summary>
	/// Indicates the project-wide <see cref="Sudoku.Analytics.Analyzer"/> instance.
	/// </summary>
	/// <seealso cref="Sudoku.Analytics.Analyzer"/>
	public Analyzer Analyzer { get; } = PredefinedAnalyzers.Default;

	/// <summary>
	/// Indicates the project-wide <see cref="Sudoku.Analytics.StepCollector"/> instance.
	/// </summary>
	/// <seealso cref="Sudoku.Analytics.StepCollector"/>
	public StepCollector StepCollector { get; } = new();

	/// <summary>
	/// Indicates a <see cref="GridInfo"/> instance that will be initialized when opening the application via extension-binding files.
	/// </summary>
	internal GridInfo? AppStartingGridInfo { get; set; }

	/// <summary>
	/// Indicates the window manager.
	/// </summary>
	/// <remarks>
	/// <para>This property is used by checking running window, which is helpful on multiple-window interaction.</para>
	/// <para>
	/// This property can also be used by <see cref="FileOpenPicker"/> and <see cref="FileSavePicker"/> cases, for getting the running window
	/// that the current control lies in.
	/// </para>
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
	internal WindowManager WindowManager { get; } = new();


	/// <summary>
	/// Indicates the assembly version.
	/// </summary>
	internal static Version AssemblyVersion => CurrentAssembly.GetName().Version!;


	/// <inheritdoc/>
	protected override void OnLaunched(LaunchActivatedEventArgs args)
	{
		HandleOnProgramOpeningEntryCase();
		ActivateMainWindow();
		LoadConfigurationFileFromLocal();
	}

	/// <summary>
	/// Creates a window, and activate it.
	/// </summary>
	private void ActivateMainWindow() => WindowManager.CreateWindow<MainWindow>().Activate();

	/// <summary>
	/// Handle the cases how user opens this program.
	/// </summary>
	private void HandleOnProgramOpeningEntryCase()
	{
		switch (AppInstance.GetCurrent().GetActivatedEventArgs())
		{
#if false
			case { Kind: ExtendedActivationKind.Protocol, Data: IProtocolActivatedEventArgs { Uri: _ } }:
			{
				break;
			}
#endif
			case
			{
				Kind: ExtendedActivationKind.File,
				Data: IFileActivatedEventArgs { Files: [StorageFile { FileType: FileExtensions.Text, Path: var filePath }] }
			}
			when SudokuFileHandler.Read(filePath) is [var instance, ..]:
			{
				AppStartingGridInfo = instance;
				break;
			}
		}
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
