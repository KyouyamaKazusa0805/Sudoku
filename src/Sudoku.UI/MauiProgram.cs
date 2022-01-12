using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;

#if WINDOWS
using Microsoft.Maui;
using Microsoft.Maui.LifecycleEvents;
#endif

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
#if WINDOWS
			.ConfigureLifecycleEvents(
				static lifecycle =>
					lifecycle.AddWindows(
						static windows =>
							windows.OnLaunched(
								static (_, _) =>
								{
									var app = (MauiWinUIWindow)MauiWinUIApplication.Current.Application.Windows[0].Handler!.NativeView!;
									app.SetIcon("Resources/Icon/appicon.ico");
								}
							)
					)
			)
#endif
			.Build();
}
