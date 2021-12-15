namespace Sudoku.UI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
	/// <summary>
	/// Initializes the singleton application object.  This is the first line of authored code
	/// executed, and as such is the logical equivalent of main() or WinMain().
	/// </summary>
	public App() => InitializeComponent();


#nullable disable warnings
	/// <summary>
	/// Indicates the main window.
	/// </summary>
	[DisallowNull]
	internal static Window? MainWindow { get; private set; }
#nullable restore warnings


	/// <summary>
	/// Invoked when the application is launched normally by the end user.  Other entry points
	/// will be used such as when the application is launched to open a specific file.
	/// </summary>
	/// <param name="args">Details about the launch request and process.</param>
	protected override void OnLaunched([IsDiscard] LaunchActivatedEventArgs args)
	{
		var mainWindow = new MainWindow { ExtendsContentIntoTitleBar = true };
		mainWindow.SetTitleBar(mainWindow.CustomTitleBar);

		MainWindow = mainWindow;
		MainWindow.Activate();
	}
}
