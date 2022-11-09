namespace Sudoku.UI.Maui;

/// <summary>
/// The application creator type.
/// </summary>
public static class MauiProgram
{
	/// <summary>
	/// Creates a MAUI application.
	/// </summary>
	/// <returns>The created application.</returns>
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder()
			.UseMauiApp<App>()
			.ConfigureFonts(static f => f.AddFont("OpenSans-Regular.ttf", "OpenSansRegular").AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold"));

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
