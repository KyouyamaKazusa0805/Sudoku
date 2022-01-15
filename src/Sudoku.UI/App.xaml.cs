using System;
using Microsoft.Maui.Controls;
using Sudoku.Diagnostics.CodeAnalysis;
using Sudoku.UI.Pages;

namespace Sudoku.UI;

/// <summary>
/// Indicates the current application.
/// </summary>
public partial class App : Application
{
#nullable disable
	/// <summary>
	/// Initializes an <see cref="App"/> instance.
	/// </summary>
	public App()
	{
		InitializeComponent();

		if (Device.Idiom == TargetIdiom.Phone)
		{
			Shell.Current.CurrentItem = PhoneTabs;
		}

		Routing.RegisterRoute("settings", typeof(SettingsPage));
	}
#nullable restore


	private void TapGestureRecognizer_Tapped([IsDiscard] object? _, [IsDiscard] EventArgs __) =>
		Shell.Current.GoToAsync("///settings");
}
