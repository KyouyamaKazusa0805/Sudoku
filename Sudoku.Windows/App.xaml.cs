using System.Windows;

namespace Sudoku.Windows
{
	/// <summary>
	/// Interaction logic for <c>App.xaml</c>.
	/// </summary>
	public partial class App : Application
	{
		/// <inheritdoc cref="Application.OnStartup(StartupEventArgs)"/>
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			string[] args = e.Args;
			var current = args.Length switch
			{
				0 => ShowMainWindowDefault(),
				1 => ShowMainWindowWithGridCode(args[0]),
				2 => args[0] switch
				{
					"-g" or "--grid" => ShowMainWindowWithGridCode(args[1]),
					_ => null
				},
				_ => null
			};

			current?.Show();
		}


		private partial Window ShowMainWindowDefault();
		private partial Window? ShowMainWindowWithGridCode(string str);
	}
}
