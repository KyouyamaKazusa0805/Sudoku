using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using Sudoku.DocComments;
using static Sudoku.Windows.MainWindow;

namespace Sudoku.Windows
{
	/// <summary>
	/// Interaction logic for <c>AboutMeWindow.xaml</c>.
	/// </summary>
	public partial class AboutMeWindow : Window
	{
		/// <summary>
		/// Initializes an instance.
		/// </summary>
		public AboutMeWindow() => InitializeComponent();


		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
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
}
