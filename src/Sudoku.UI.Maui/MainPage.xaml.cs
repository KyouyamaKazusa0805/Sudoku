namespace Sudoku.UI.Maui;

public partial class MainPage : ContentPage
{
	private int _count = 0;

	public MainPage() => InitializeComponent();

	private void OnCounterClicked(object sender, EventArgs e)
	{
		_count++;

		CounterBtn.Text = _count == 1 ? $"Clicked {_count} time" : $"Clicked {_count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}
}
