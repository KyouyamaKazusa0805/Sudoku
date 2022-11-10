namespace Sudoku.Maui;

/// <summary>
/// Defines an application.
/// </summary>
public sealed partial class App : Application
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
