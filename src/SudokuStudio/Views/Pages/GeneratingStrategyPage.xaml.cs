namespace SudokuStudio.Views.Pages;

/// <summary>
/// The generating strategy page.
/// </summary>
public sealed partial class GeneratingStrategyPage : Page
{
	/// <summary>
	/// Initializes a <see cref="GeneratingStrategyPage"/> instance.
	/// </summary>
	public GeneratingStrategyPage() => InitializeComponent();


	private void Grid_PointerEntered(object sender, PointerRoutedEventArgs e)
	{
		foreach (var control in ((GridLayout)sender).Children)
		{
			if (control is FrameworkElement { Tag: "Target" })
			{
				control.Opacity = 1;
				return;
			}
		}
	}

	private void Grid_PointerExited(object sender, PointerRoutedEventArgs e)
	{
		foreach (var control in ((GridLayout)sender).Children)
		{
			if (control is FrameworkElement { Tag: "Target" })
			{
				control.Opacity = 0;
				return;
			}
		}
	}
}
