using System;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace Sudoku.Maui;

/// <summary>
/// The program class.
/// </summary>
/// <remarks>
/// This type is defaultly not supported. If you want to support Tizen platform on MAUI,
/// plese manually configure <c><![CDATA[<TargetFrameworks>$(TargetFrameworks);net7.0-tizen</TargetFrameworks>]]></c>
/// in project configuration file <c>Sudoku.Maui.csproj</c>.
/// </remarks>
internal sealed class Program : MauiApplication
{
	/// <inheritdoc/>
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();


	/// <summary>
	/// The entry point method.
	/// </summary>
	/// <param name="args">The command line arguments.</param>
	private static void Main(string[] args)
	{
		var app = new Program();
		app.Run(args);
	}
}
