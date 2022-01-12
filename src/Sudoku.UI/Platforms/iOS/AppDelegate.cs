using Foundation;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace Sudoku.UI;

/// <summary>
/// Indicates the application delegate.
/// </summary>
[Register(nameof(AppDelegate))]
public class AppDelegate : MauiUIApplicationDelegate
{
	/// <inheritdoc/>
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
