using System.Windows;

namespace Sudoku.Windows
{
	/// <summary>
	/// Interaction logic for <c>App.xaml</c>.
	/// </summary>
	public partial class App : Application
	{
		/// <inheritdoc/>
		protected override unsafe void OnStartup(StartupEventArgs e)
		{
			if (e is { Args: { Length: not 0 } args })
			{
				delegate* managed<string[], bool> method = args[0] switch
				{
					_ => &DoNothing
				};

				method(args[1..]);

				Shutdown();
			}
			else
			{
				base.OnStartup(e);
			}
		}
	}
}
