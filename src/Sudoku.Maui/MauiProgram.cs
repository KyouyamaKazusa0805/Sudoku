namespace Sudoku.Maui;

/// <summary>
/// Represents a type that is used for creating a <see cref="MauiApp"/> instance.
/// </summary>
/// <remarks>
/// <b><i>
/// Please do not modify the contents in this file. The file is global one, which will influence to multiple platforms:
/// </i></b>
/// <list type="bullet">
/// <item>Windows</item>
/// <item>Android</item>
/// <item>iOS</item>
/// <item>Mac</item>
/// <item>Tizen (Optional)</item>
/// </list>
/// </remarks>
public static class MauiProgram
{
	/// <summary>
	/// Creates a MAUI application, returning the instance <see cref="MauiApp"/>.
	/// </summary>
	/// <returns>The application instance.</returns>
	public static MauiApp CreateMauiApp()
	{
		return MauiApp.CreateBuilder()
			.UseMauiApp<App>()
			.ConfigureFonts(fontConfiguring)
			.ConfigureDebugLogging()
			.Build();


		static void fontConfiguring(IFontCollection f)
		{
			f.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			f.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
		}
	}
}

/// <summary>
/// The local extensions.
/// </summary>
file static class Extensions
{
	/// <summary>
	/// Configures debugging on logging.
	/// </summary>
	/// <remarks>
	/// If the <c>DEBUG</c> symbol is not defined, the method will directly returns <paramref name="this"/>
	/// without any operations.
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static MauiAppBuilder ConfigureDebugLogging(this MauiAppBuilder @this)
	{
#if DEBUG
		@this.Logging.AddDebug();
#endif

		return @this;
	}
}
