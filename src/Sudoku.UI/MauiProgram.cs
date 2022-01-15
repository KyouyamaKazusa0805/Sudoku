#pragma warning disable IDE0005

using Microsoft.Maui; // Windows
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents; // Windows
using Microsoft.UI.Xaml; // Windows
using Sudoku.Diagnostics.CodeAnalysis; // Windows

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
	public static MauiApp CreateMauiApp()
	{
		return MauiApp
			.CreateBuilder()
			.UseMauiApp<App>()
			.ConfigureFonts(static f => f.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"))
#if WINDOWS
			.ConfigureLifecycleEvents(static l => l.AddWindows(static w => w.OnLaunched(setIcon)))
#endif
			.Build();


#if WINDOWS
		static void setIcon([IsDiscard] Application _, [IsDiscard] LaunchActivatedEventArgs __)
		{
			var app = (MauiWinUIWindow)MauiWinUIApplication.Current.Application.Windows[0].Handler!.NativeView!;
			app.SetIcon("Resources/Icon/appicon.ico");

			app.ExtendsContentIntoTitleBar = false;
		}
#endif
	}
}
