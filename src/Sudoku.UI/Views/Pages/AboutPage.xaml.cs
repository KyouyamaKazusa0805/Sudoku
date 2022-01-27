namespace Sudoku.UI.Views.Pages;

/// <summary>
/// A page that can be used on its own or navigated to within a <see cref="Frame"/>.
/// </summary>
/// <seealso cref="Frame"/>
public sealed partial class AboutPage : Page
{
	/// <summary>
	/// Initializes an <see cref="AboutPage"/> instance.
	/// </summary>
	public AboutPage() => InitializeComponent();


	private void HyperlinkButton_Click([IsDiscard] object sender, RoutedEventArgs e)
	{
		if (e is not { OriginalSource: HyperlinkButton { Content: string link } })
		{
			return;
		}

		Website.Visit(link);
	}
}
