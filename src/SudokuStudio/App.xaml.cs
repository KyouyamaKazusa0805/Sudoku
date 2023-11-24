using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Sudoku.Analytics;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Concepts;
using Sudoku.Concepts.Converters;
using SudokuStudio.Configuration;
using SudokuStudio.Storage;
using SudokuStudio.Views.Attached;
using SudokuStudio.Views.Controls;
using SudokuStudio.Views.Windows;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
using WinRT.Interop;
using static SudokuStudio.ProjectWideConstants;
using LaunchActivatedEventArgs = Microsoft.UI.Xaml.LaunchActivatedEventArgs;

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
			.WithActionIfMatch(
				difficultyLevel != DifficultyLevel.Unknown,
				static a => a.WithStepSearchers(((App)Current).GetStepSearchers()),
				a => a.WithStepSearchers(((App)Current).GetStepSearchers(), difficultyLevel)
			)
			.WithRuntimeIdentifierSetters(sudokuPane)
			.WithAlgorithmLimits(disallowHighTimeComplexity, disallowSpaceTimeComplexity)
			.WithUserDefinedOptions(CreateStepSearcherOptions());
	}

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
		return StepSearcherOptions.Default with
		{
			Converter = uiPref.ConceptNotationBasedKind switch
			{
				CoordinateType.Literal => new LiteralCoordinateConverter(
					uiPref.DefaultSeparatorInNotation,
					uiPref.DigitsSeparatorInNotation
				),
				CoordinateType.RxCy => new RxCyConverter(
					uiPref.MakeLettersUpperCaseInRxCyNotation,
					uiPref.MakeDigitBeforeCellInRxCyNotation,
					uiPref.HouseNotationOnlyDisplayCapitalsInRxCyNotation,
					uiPref.DefaultSeparatorInNotation,
					uiPref.DigitsSeparatorInNotation
				),
				CoordinateType.K9 => new K9Converter(
					uiPref.MakeLettersUpperCaseInK9Notation,
					uiPref.FinalRowLetterInK9Notation,
					uiPref.DefaultSeparatorInNotation,
					uiPref.DigitsSeparatorInNotation
				),
				CoordinateType.Excel => new ExcelCoordinateConverter(
					uiPref.MakeLettersUpperCaseInExcelNotation,
					uiPref.DefaultSeparatorInNotation,
					uiPref.DigitsSeparatorInNotation
				)
			},
			IsDirectMode = !uiPref.DisplayCandidates
		};
	}
}
