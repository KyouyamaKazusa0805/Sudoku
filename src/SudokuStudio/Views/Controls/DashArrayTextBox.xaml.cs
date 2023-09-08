using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using SudokuStudio.Collection;
using SudokuStudio.ComponentModel;
using Windows.System;

namespace SudokuStudio.Views.Controls;

/// <summary>
/// Represents a dash array text box.
/// </summary>
[DependencyProperty<DashArray>("DashArray")]
public sealed partial class DashArrayTextBox : UserControl
{
	/// <summary>
	/// Initializes a <see cref="DashArrayTextBox"/> instance.
	/// </summary>
	public DashArrayTextBox() => InitializeComponent();


	/// <summary>
	/// Indicates the event triggered when the dash array is changed.
	/// </summary>
	public event EventHandler<DashArray>? DashArrayChanged;


	private void CoreBox_KeyDown(object sender, KeyRoutedEventArgs e)
	{
		if (e.Key != VirtualKey.Enter)
		{
			return;
		}

		var values = from element in CoreBox.Text.SplitBy([',']) select double.TryParse(element, out var r) && r is >= 0 and <= 10 ? r : 0;
		if (Array.FindAll(values, static value => value == 0).Length >= 2)
		{
			DashArrayChanged?.Invoke(this, DashArray = []);
			return;
		}

		if (values.Length >= 2 && Array.IndexOf(values, 0) != -1)
		{
			DashArrayChanged?.Invoke(this, DashArray = []);
			return;
		}

		DashArrayChanged?.Invoke(this, DashArray = [.. values]);
	}
}
