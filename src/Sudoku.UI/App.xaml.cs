using Microsoft.Maui.Controls;
using Sudoku.UI.Pages;

namespace Sudoku.UI;

/// <summary>
/// Indicates the current application.
/// </summary>
public partial class App : Application
{
	/// <summary>
	/// Initializes an <see cref="App"/> instance.
	/// </summary>
	public App()
	{
		InitializeComponent();

		MainPage = new MainPage();
	}
}
