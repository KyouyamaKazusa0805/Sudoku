namespace Sudoku.UI.Maui;

/// <summary>
/// Defines an application abstraction type.
/// </summary>
public partial class App : Application
{
	/// <summary>
	/// Initializes an <see cref="App"/> instance.
	/// </summary>
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();
	}
}
