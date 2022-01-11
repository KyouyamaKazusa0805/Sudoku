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
			.Build();
}
