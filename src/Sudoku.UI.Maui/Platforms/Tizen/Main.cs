using System;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace Sudoku.UI.Maui;

/// <summary>
/// The program type.
/// </summary>
internal class Program : MauiApplication
{
	/// <inheritdoc/>
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();


	/// <summary>
	/// This is the main entry point of the application.
	/// </summary>
	/// <param name="args">The command line arguments.</param>
	private static void Main(string[] args)
	{
		var app = new Program();
		app.Run(args);
	}
}
