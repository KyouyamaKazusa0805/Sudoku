using Microsoft.Maui;
using Microsoft.Maui.Essentials;
using Microsoft.Maui.Hosting;
using Microsoft.UI.Xaml;
using WindowsApplication = Microsoft.Maui.Controls.Application;

namespace Sudoku.UI.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default
/// <see cref="WindowsApplication"/> class.
/// </summary>
/// <seealso cref="WindowsApplication"/>
public partial class App : MauiWinUIApplication
{
	/// <summary>
	/// Initializes the singleton application object.
	/// This is the first line of authored code executed,
	/// and as such is the logical equivalent of <c>main()</c> or <c>WinMain()</c>.
	/// </summary>
	public App() => InitializeComponent();


	/// <inheritdoc/>
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

	/// <inheritdoc/>
	protected override void OnLaunched(LaunchActivatedEventArgs args)
	{
		base.OnLaunched(args);

		Platform.OnLaunched(args);
	}
}
