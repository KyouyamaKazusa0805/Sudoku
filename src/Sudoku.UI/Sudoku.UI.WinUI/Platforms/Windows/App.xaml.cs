using Microsoft.Maui;
using Microsoft.Maui.Essentials;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Sudoku.UI.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default <see cref="Application"/> class.
/// </summary>
public partial class App : MauiWinUIApplication
{
	/// <summary>
	/// <para>
	/// Initializes the singleton application object.
	/// </para>
	/// <para>
	/// This is the first line of authored code executed,
	/// and as such is the logical equivalent of <c>Main()</c> or <c>WinMain()</c>.
	/// </para>
	/// </summary>
	public App() => InitializeComponent();


	/// <summary>
	/// Creates and returns a startup instance.
	/// </summary>
	/// <returns>The instance that can startup by user or IDE.</returns>
	protected override IStartup OnCreateStartup() => new Startup();

	/// <summary>
	/// Execute when the current application is launched.
	/// </summary>
	/// <param name="args">The arguments that provides for the handling.</param>
	protected override void OnLaunched(LaunchActivatedEventArgs args)
	{
		base.OnLaunched(args);

		Platform.OnLaunched(args);
	}
}
