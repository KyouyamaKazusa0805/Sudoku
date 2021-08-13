namespace Sudoku.Windows;

/// <summary>
/// Interaction logic for <c>AboutMeWindow.xaml</c>.
/// </summary>
public partial class AboutMeWindow : Window
{
	/// <summary>
	/// Initializes an instance.
	/// </summary>
	public AboutMeWindow() => InitializeComponent();


	private void GitHubLink_Click(object sender, RoutedEventArgs e)
	{
		if (sender is Hyperlink)
		{
			try
			{
				Process.Start(new ProcessStartInfo((string)LangSource["AboutMeRealGitHub"]));
			}
			catch (Exception ex)
			{
				Messagings.ShowExceptionMessage(ex);
			}
		}
	}
}
