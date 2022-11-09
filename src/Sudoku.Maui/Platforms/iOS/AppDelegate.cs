using Foundation;

namespace Sudoku.Maui;

/// <summary>
/// The application delegate.
/// </summary>
[Register(nameof(AppDelegate))]
public sealed class AppDelegate : MauiUIApplicationDelegate
{
	/// <inheritdoc/>
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
