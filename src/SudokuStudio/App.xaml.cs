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
	/// Indicates the project-wide <see cref="Sudoku.Analytics.Collector"/> instance.
	/// </summary>
	/// <seealso cref="Sudoku.Analytics.Collector"/>
	public Collector Collector { get; } = new();

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
	/// Indicates the internal libraries.
	/// </summary>
	internal List<Library> Libraries { get; } = [];


	/// <summary>
	/// Indicates the assembly version.
	/// </summary>
	internal static Version AssemblyVersion => CurrentAssembly.GetName().Version!;

	/// <summary>
	/// Indicates the current culture, used by <see cref="ResourceDictionary.Get(string, CultureInfo?, Assembly?)"/>.
	/// </summary>
	/// <seealso cref="ResourceDictionary.Get(string, CultureInfo?, Assembly?)"/>
	internal static CultureInfo CurrentCulture
		=> ((App)Current).Preference.UIPreferences.Language is var cultureInfoId and not 0 ? new(cultureInfoId) : CultureInfo.CurrentUICulture;

	/// <summary>
	/// Indicates the configured application theme.
	/// </summary>
	internal static ApplicationTheme CurrentTheme
		=> ((App)Current).Preference.UIPreferences.CurrentTheme switch
		{
			Theme.Default when !ShouldSystemUseDarkMode() => ApplicationTheme.Light,
			Theme.Default => ApplicationTheme.Dark,
			Theme.Light => ApplicationTheme.Light,
			_ => ApplicationTheme.Dark
		};


	/// <summary>
	/// Cover the settings via the application theme.
	/// </summary>
	/// <param name="pane">The sudoku pane to be set.</param>
	internal void CoverSettingsToSudokuPaneViaApplicationTheme(SudokuPane pane)
	{
		var uiPref = ((App)Current).Preference.UIPreferences;
		Action themeChanger = CurrentTheme switch { ApplicationTheme.Light => setSudokuPaneColors_Light, _ => setSudokuPaneColors_Dark };

		themeChanger();


		void setSudokuPaneColors_Light()
		{
			pane.GivenColor = uiPref.GivenFontColor;
			pane.ModifiableColor = uiPref.ModifiableFontColor;
			pane.PencilmarkColor = uiPref.PencilmarkFontColor;
			pane.CoordinateLabelColor = uiPref.CoordinateLabelFontColor;
			pane.BabaGroupLabelColor = uiPref.BabaGroupingFontColor;
			pane.DeltaCandidateColor = uiPref.DeltaPencilmarkColor;
			pane.DeltaCellColor = uiPref.DeltaValueColor;
			pane.BorderColor = uiPref.SudokuPaneBorderColor;
			pane.CursorBackgroundColor = uiPref.CursorBackgroundColor;
			pane.LinkColor = uiPref.ChainColor;
			pane.NormalColor = uiPref.NormalColor;
			pane.AssignmentColor = uiPref.AssignmentColor;
			pane.OverlappedAssignmentColor = uiPref.OverlappedAssignmentColor;
			pane.EliminationColor = uiPref.EliminationColor;
			pane.CannibalismColor = uiPref.CannibalismColor;
			pane.ExofinColor = uiPref.ExofinColor;
			pane.EndofinColor = uiPref.EndofinColor;
			pane.HouseCompletedFeedbackColor = uiPref.HouseCompletedFeedbackColor;
			pane.AuxiliaryColors = uiPref.AuxiliaryColors;
			pane.DifficultyLevelForegrounds = uiPref.DifficultyLevelForegrounds;
			pane.DifficultyLevelBackgrounds = uiPref.DifficultyLevelBackgrounds;
			pane.UserDefinedColorPalette = uiPref.UserDefinedColorPalette;
			pane.AlmostLockedSetsColors = uiPref.AlmostLockedSetsColors;
		}

		void setSudokuPaneColors_Dark()
		{
			pane.GivenColor = uiPref.GivenFontColor_Dark;
			pane.ModifiableColor = uiPref.ModifiableFontColor_Dark;
			pane.PencilmarkColor = uiPref.PencilmarkFontColor_Dark;
			pane.CoordinateLabelColor = uiPref.CoordinateLabelFontColor_Dark;
			pane.BabaGroupLabelColor = uiPref.BabaGroupingFontColor_Dark;
			pane.DeltaCandidateColor = uiPref.DeltaPencilmarkColor_Dark;
			pane.DeltaCellColor = uiPref.DeltaValueColor_Dark;
			pane.BorderColor = uiPref.SudokuPaneBorderColor_Dark;
			pane.CursorBackgroundColor = uiPref.CursorBackgroundColor_Dark;
			pane.LinkColor = uiPref.ChainColor_Dark;
			pane.NormalColor = uiPref.NormalColor_Dark;
			pane.AssignmentColor = uiPref.AssignmentColor_Dark;
			pane.OverlappedAssignmentColor = uiPref.OverlappedAssignmentColor_Dark;
			pane.EliminationColor = uiPref.EliminationColor_Dark;
			pane.CannibalismColor = uiPref.CannibalismColor_Dark;
			pane.ExofinColor = uiPref.ExofinColor_Dark;
			pane.EndofinColor = uiPref.EndofinColor_Dark;
			pane.HouseCompletedFeedbackColor = uiPref.HouseCompletedFeedbackColor_Dark;
			pane.AuxiliaryColors = uiPref.AuxiliaryColors_Dark;
			pane.DifficultyLevelForegrounds = uiPref.DifficultyLevelForegrounds_Dark;
			pane.DifficultyLevelBackgrounds = uiPref.DifficultyLevelBackgrounds_Dark;
			pane.UserDefinedColorPalette = uiPref.UserDefinedColorPalette_Dark;
			pane.AlmostLockedSetsColors = uiPref.AlmostLockedSetsColors_Dark;
		}
	}

	/// <summary>
	/// Try to fetch an <see cref="Sudoku.Analytics.Analyzer"/> instance via the specified running <see cref="SudokuPane"/>.
	/// </summary>
	/// <param name="sudokuPane">The sudoku pane.</param>
	/// <param name="difficultyLevel">
	/// The limit difficulty level. Step searchers hard than it will be filtered and not be used in the analysis.
	/// </param>
	/// <returns>The final <see cref="Sudoku.Analytics.Analyzer"/> instance.</returns>
	internal Analyzer GetAnalyzerConfigured(SudokuPane sudokuPane, DifficultyLevel difficultyLevel = default)
	{
		var disallowHighTimeComplexity = Preference.AnalysisPreferences.LogicalSolverIgnoresSlowAlgorithms;
		var disallowSpaceTimeComplexity = Preference.AnalysisPreferences.LogicalSolverIgnoresHighAllocationAlgorithms;
		return Analyzer
			.WithStepSearchers(((App)Current).GetStepSearchers(), difficultyLevel)
			.WithRuntimeIdentifierSetters(sudokuPane)
			.WithCulture(CurrentCulture)
			.WithAlgorithmLimits(disallowHighTimeComplexity, disallowSpaceTimeComplexity)
			.WithUserDefinedOptions(CreateStepSearcherOptions());
	}

	/// <inheritdoc/>
	protected override void OnLaunched(LaunchActivatedEventArgs args)
	{
		HandleOnProgramOpeningEntryCase();
		LoadConfigurationFileFromLocal();
		CheckPuzzleLibraries();
		ActivateMainWindow();
	}

	/// <summary>
	/// Creates a window, and activate it.
	/// </summary>
	private void ActivateMainWindow()
	{
		var window = WindowManager.CreateWindow<MainWindow>();
		if (window.Content is FrameworkElement control)
		{
			control.RequestedTheme = ((App)Current).Preference.UIPreferences.CurrentTheme switch
			{
				Theme.Default => ElementTheme.Default,
				Theme.Light => ElementTheme.Light,
				Theme.Dark => ElementTheme.Dark,
				//_ => App.ShouldSystemUseDarkMode() ? ElementTheme.Dark : ElementTheme.Light
			};
		}
		//window.SystemBackdrop = ((App)Current).Preference.UIPreferences.Backdrop.GetBackdrop();

		window.Activate();
	}

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

	/// <summary>
	/// Check for puzzle libraries.
	/// </summary>
	private void CheckPuzzleLibraries()
	{
		Library.RegisterConfigFileExtension(FileExtensions.PuzzleLibrary);

		if (CommonPaths.Library is var libFolder && Directory.Exists(libFolder))
		{
			foreach (var filePath in Directory.EnumerateFiles(libFolder))
			{
				if (io::Path.GetExtension(filePath) == FileExtensions.PuzzleLibrary
					&& File.ReadLines(filePath).FirstOrDefault() == Library.ConfigFileHeader)
				{
					Libraries.Add(new(libFolder, io::Path.GetFileNameWithoutExtension(filePath)));
				}
			}
		}
	}


	/// <summary>
	/// Get system theme mode.
	/// </summary>
	/// <returns>A <see cref="bool"/> result indicating whether the system theme is dark.</returns>
#if false
	[LibraryImport("UXTheme", SetLastError = true, EntryPoint = "#138")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool ShouldSystemUseDarkMode();
#else
	public static bool ShouldSystemUseDarkMode()
	{
		try
		{
			using var process = new Process
			{
				StartInfo =
				{
					FileName = "cmd.exe",
					Arguments = @"/C reg query HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize\",
					UseShellExecute = false,
					RedirectStandardOutput = true,
					CreateNoWindow = true
				}
			};
			process.Start();

			var output = process.StandardOutput.ReadToEnd();
			foreach (var key in output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
			{
				if (key.Contains("AppsUseLightTheme"))
				{
					return key.EndsWith('0');
				}
			}
		}
		catch
		{
		}

		return false;
	}
#endif

	/// <summary>
	/// To determine whether the current application view is in an unsnapped state.
	/// </summary>
	/// <returns>The <see cref="bool"/> value indicating that.</returns>
	internal static bool EnsureUnsnapped()
	{
		// 'FileOpenPicker' APIs will not work if the application is in a snapped state.
		// If an app wants to show a 'FileOpenPicker' while snapped, it must attempt to unsnap first.
		var unsnapped = ApplicationView.Value != ApplicationViewState.Snapped || ApplicationView.TryUnsnap();
		if (!unsnapped)
		{
			throw new InvalidOperationException("Ensure the file should be unsnapped.");
		}

		return unsnapped;
	}

	/// <summary>
	/// Try to get main window the program uses. This operation can be used for locating pages.
	/// </summary>
	/// <typeparam name="T">The type of the calling instance.</typeparam>
	/// <param name="this">The instance of the control itself.</param>
	/// <returns>The <see cref="MainWindow"/> found.</returns>
	/// <exception cref="InvalidOperationException">Throws when the main window isn't found.</exception>
	internal static MainWindow GetMainWindow<T>(T @this) where T : UIElement
		=> ((App)Current).WindowManager.GetWindowForElement(@this) switch
		{
			MainWindow mainWindow => mainWindow,
			_ => throw new InvalidOperationException("Main window cannot be found.")
		};

	/// <summary>
	/// Creates a <see cref="StepSearcherOptions"/> instance via the currently-configured preferences.
	/// </summary>
	/// <returns>A <see cref="StepSearcherOptions"/> instance whose internal values referenced the preferences configured by user.</returns>
	internal static StepSearcherOptions CreateStepSearcherOptions()
	{
		var uiPref = ((App)Current).Preference.UIPreferences;
		var analysisPref = ((App)Current).Preference.AnalysisPreferences;
		return StepSearcherOptions.Default with
		{
			Converter = uiPref.ConceptNotationBasedKind switch
			{
				CoordinateType.Literal => new LiteralCoordinateConverter(
					uiPref.DefaultSeparatorInNotation,
					uiPref.DigitsSeparatorInNotation,
					CurrentCulture
				),
				CoordinateType.RxCy => new RxCyConverter(
					uiPref.MakeLettersUpperCaseInRxCyNotation,
					uiPref.MakeDigitBeforeCellInRxCyNotation,
					uiPref.HouseNotationOnlyDisplayCapitalsInRxCyNotation,
					uiPref.DefaultSeparatorInNotation,
					uiPref.DigitsSeparatorInNotation,
					CurrentCulture
				),
				CoordinateType.K9 => new K9Converter(
					uiPref.MakeLettersUpperCaseInK9Notation,
					uiPref.FinalRowLetterInK9Notation,
					uiPref.DefaultSeparatorInNotation,
					uiPref.DigitsSeparatorInNotation,
					CurrentCulture
				),
				CoordinateType.Excel => new ExcelCoordinateConverter(
					uiPref.MakeLettersUpperCaseInExcelNotation,
					uiPref.DefaultSeparatorInNotation,
					uiPref.DigitsSeparatorInNotation,
					CurrentCulture
				)
			},
			IsDirectMode = !uiPref.DisplayCandidates,
			DistinctDirectMode = analysisPref.DistinctDirectAndIndirectModes
		};
	}
}
