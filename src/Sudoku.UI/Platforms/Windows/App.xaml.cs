using Microsoft.UI.Xaml;

namespace Sudoku.UI.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default
/// <see cref="Microsoft.Maui.Controls.Application"/> class.
/// </summary>
/// <seealso cref="Microsoft.Maui.Controls.Application"/>
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
