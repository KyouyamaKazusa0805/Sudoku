using Foundation;

namespace Sudoku.UI.Maui;

/// <summary>
/// The application entry point class.
/// </summary>
[Register(nameof(AppDelegate))]
public class AppDelegate : MauiUIApplicationDelegate
{
	/// <inheritdoc/>
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
