namespace Sudoku.UI;

/// <summary>
/// Provides application-specific behavior to supplement the default <see cref="Application"/> class.
/// </summary>
/// <seealso cref="Application"/>
public partial class App : Application
{
	/// <summary>
	/// Indicates the main window in this application in the current interaction logic.
	/// </summary>
	private Window _window = null!;


	/// <summary>
	/// <para>Initializes the singleton application object.</para>
	/// <para>
	/// This is the first line of authored code executed,
	/// and as such is the logical equivalent of <c>main()</c> or <c>WinMain()</c>.
	/// </para>
	/// </summary>
	public App()
	{
		// Calls the base initialization method.
		InitializeComponent();

		// Then we should append a new router method to get the resource.
		// In this way, we can just use the unified code to get the resource.
		// The classic way to get the resource is 'Application.Current.Resources[key]',
		// Now we can use 'ExternalResourceManager.Shared[key]'.
		ExternalResourceManager.Shared.Routers += static key => Current.Resources[key] as string;
	}


	/// <summary>
	/// <para>Invoked when the application is launched normally by the end user.</para>
	/// <para>
	/// Other entry points will be used such as when the application is launched to open a specific file.
	/// </para>
	/// </summary>
	/// <param name="args">Details about the launch request and process.</param>
	protected override void OnLaunched(LaunchActivatedEventArgs args)
	{
		_window = new MainWindow { Title = ExternalResourceManager.Shared["ProgramName"] };
		_window.Activate();
	}
}
