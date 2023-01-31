namespace SudokuStudio;

/// <summary>
/// Provides application-specific behavior to supplement the default <see cref="Application"/> class.
/// </summary>
/// <seealso cref="Application"/>
public partial class App : Application
{
	/// <summary>
	/// <para>Initializes the singleton application object.</para>
	/// <para>
	/// This is the first line of authored code executed, and as such is the logical equivalent of <c>main()</c> or <c>WinMain()</c>.
	/// </para>
	/// </summary>
	public App() => InitializeComponent();


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
		switch (AppInstance.GetCurrent().GetActivatedEventArgs())
		{
			case
			{
				Kind: ExtendedActivationKind.File,
				Data: IFileActivatedEventArgs { Files: [StorageFile { FileType: var fileType, Path: var filePath } file, ..] }
			}:
			{
				switch (fileType)
				{
					case CommonFileExtensions.Text:
					{
						RunningContext.PreinstantiationInfo.OpenedSudoku = SudokuFileHandler.Read(filePath);
						break;
					}
#if false
					case CommonFileExtensions.UserPreference:
					{
						RunningContext.PreinstantiationInfo.OpenedProgramPreference = ProgramPreferenceFileHandler.Read(filePath);
						break;
					}
#endif
				}

				break;
			}
		}
	}
}
