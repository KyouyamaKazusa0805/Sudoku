namespace SudokuStudio.Views.Pages;

/// <summary>
/// The generating strategy page.
/// </summary>
public sealed partial class GeneratingStrategyPage : Page
{
	/// <summary>
	/// The target item that the corresponding control's tag will set this value.
	/// </summary>
	private const string TargetTagName = "Target";


	/// <summary>
	/// Initializes a <see cref="GeneratingStrategyPage"/> instance.
	/// </summary>
	public GeneratingStrategyPage() => InitializeComponent();


	private void Grid_PointerEntered(object sender, PointerRoutedEventArgs e)
	{
		foreach (var control in ((GridLayout)sender).Children)
		{
			if (control is FrameworkElement { Tag: TargetTagName })
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
			if (control is FrameworkElement { Tag: TargetTagName })
			{
				control.Opacity = 0;
				return;
			}
		}
	}

	private void Button_Click(object sender, RoutedEventArgs e)
	{

	}
}
