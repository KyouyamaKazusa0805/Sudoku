namespace Sudoku.Maui;

/// <summary>
/// The main page.
/// </summary>
public partial class MainPage : ContentPage
{
	/// <summary>
	/// The counter.
	/// </summary>
	private int _count = 0;


	/// <summary>
	/// Initializes a <see cref="MainPage"/> instance.
	/// </summary>
	public MainPage() => InitializeComponent();


	/// <summary>
	/// The event handler that is triggered when the button is clicked.
	/// </summary>
	private void OnCounterClicked(object sender, EventArgs e)
	{
		CounterBtn.Text = ++_count == 1 ? $"Clicked {_count} time" : $"Clicked {_count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}
}
