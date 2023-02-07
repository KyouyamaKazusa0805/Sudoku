namespace SudokuStudio.Views.Controls;

/// <summary>
/// Represents a dash array text box.
/// </summary>
public sealed partial class DashArrayTextBox : UserControl
{
	/// <summary>
	/// Indicates the dependency property that binds with property <see cref="DashArray"/>.
	/// </summary>
	/// <seealso cref="DashArray"/>
	public static readonly DependencyProperty DashArrayProperty = RegisterDependency<DashArray, DashArrayTextBox>(nameof(DashArray));


	/// <summary>
	/// Initializes a <see cref="DashArrayTextBox"/> instance.
	/// </summary>
	public DashArrayTextBox() => InitializeComponent();


	/// <summary>
	/// Indicates the dash array.
	/// </summary>
	public DashArray DashArray
	{
		get => (DashArray)GetValue(DashArrayProperty);

		set => SetValue(DashArrayProperty, value);
	}


	/// <summary>
	/// Try to set to property <see cref="DashArray"/> via the current text box's text.
	/// </summary>
	/// <param name="text">The text input.</param>
	private void SetDashArrayViaString(string text)
	{
		var values =
			from element in text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
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


	private void CoreBox_KeyDown(object sender, KeyRoutedEventArgs e)
	{
		if (e.Key == VirtualKey.Enter)
		{
			SetDashArrayViaString(CoreBox.Text);
		}
	}
}
