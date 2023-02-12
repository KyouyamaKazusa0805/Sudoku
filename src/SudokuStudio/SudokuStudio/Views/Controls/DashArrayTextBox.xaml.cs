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


	private void CoreBox_KeyDown(object sender, KeyRoutedEventArgs e)
	{
		if (e.Key != VirtualKey.Enter)
		{
			return;
		}

		var values =
			from element in CoreBox.Text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			select double.TryParse(element, out var r) && r is >= 0 and <= 10 ? r : 0;
		if (Array.FindAll(values, static value => value == 0).Length >= 2)
		{
			DashArray = new();
			return;
		}

		if (values.Length >= 2 && Array.IndexOf(values, 0) != -1)
		{
			DashArray = new();
			return;
		}

		DashArray = new(values);
	}
}
