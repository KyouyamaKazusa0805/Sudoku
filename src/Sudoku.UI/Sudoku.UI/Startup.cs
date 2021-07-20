using Microsoft.Maui;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;

namespace Sudoku.UI
{
	/// <summary>
	/// Defines a startup entry point.
	/// </summary>
	public sealed class Startup : IStartup
	{
		/// <summary>
		/// To configure the current UI project, and creates the base information.
		/// </summary>
		/// <param name="appBuilder">The application builder host.</param>
		public void Configure(IAppHostBuilder appBuilder) =>
			appBuilder
				.UseMauiApp<App>()
				.ConfigureFonts(static fonts => fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"));
	}
}
