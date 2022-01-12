namespace Sudoku.UI;

/// <summary>
/// Indicates the main program configuration.
/// </summary>
public static class MauiProgram
{
	/// <summary>
	/// Creates the MAUI application.
	/// </summary>
	/// <returns>The MAUI application instance.</returns>
	public static MauiApp CreateMauiApp() =>
		MauiApp
			.CreateBuilder()
			.UseMauiApp<App>()
			.ConfigureFonts(static fonts => fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"))
#if WINDOWS && false
			.ConfigureLifecycleEvents(
				static lifecycle =>
				{
					lifecycle.AddWindows(
						static windows =>
							windows.OnLaunched(
								static (app, args) =>
								{
									var winuiApp = (Microsoft.UI.Xaml.Window)MauiWinUIApplication.Current.Application.Windows[0].Handler!.NativeView!;
									winuiApp.SetIcon("Platforms/Windows/trayicon.ico");
								}
							)
					);
				}
			)
#endif
			.Build();
}
