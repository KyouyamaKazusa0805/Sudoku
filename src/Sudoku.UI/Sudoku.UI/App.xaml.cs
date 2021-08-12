using Application = Microsoft.Maui.Controls.Application;

namespace Sudoku.UI;

/// <summary>
/// Defines the interaction of this application.
/// </summary>
public partial class App : Application
{
	/// <summary>
	/// Initializes an <see cref="App"/> instance without parameters.
	/// </summary>
	public App()
	{
		InitializeComponent();

		MainPage = new MainPage();
	}
}
