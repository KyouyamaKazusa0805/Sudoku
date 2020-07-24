using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using Sudoku.Windows.Constants;
using static Sudoku.Windows.Constants.Processings;

namespace Sudoku.Windows
{
	/// <summary>
	/// Interaction logic for <c>AboutMeWindow.xaml</c>.
	/// </summary>
	public partial class AboutMeWindow : Window
	{
		/// <include file='..\GlobalDocComments.xml' path='comments/defaultConstructor'/>
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
}
