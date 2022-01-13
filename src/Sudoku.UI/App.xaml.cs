using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Sudoku.UI.Pages;
using Sudoku.Diagnostics.CodeAnalysis;

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


	/// <inheritdoc/>
	protected override Window CreateWindow([IsDiscard] IActivationState? activationState) =>
		new(MainPage ??= new MainPage()) { Title = "Project Nano (Sunnie's Sudoku Solution)" };
}
