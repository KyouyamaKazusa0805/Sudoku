namespace Sudoku.UI;

/// <summary>
/// Indicates the main page.
/// </summary>
public partial class MainPage : ContentPage
{
	private int _count = 0;


#nullable disable
	/// <summary>
	/// Initializes a <see cref="MainPage"/> instance.
	/// </summary>
	public MainPage() => InitializeComponent();
#nullable enable


	private void OnCounterClicked(object sender, EventArgs e)
	{
		_count++;
		CounterLabel.Text = $"Current count: {_count}";

		SemanticScreenReader.Announce(CounterLabel.Text);
	}
}
