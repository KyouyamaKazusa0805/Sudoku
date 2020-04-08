using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;

namespace Sudoku.Windows
{
	/// <summary>
	/// Interaction logic for <c>AboutMeWindow.xaml</c>.
	/// </summary>
	public partial class AboutMeWindow : Window
	{
		/// <include file='../GlobalDocComments.xml' path='comments/defaultConstructor'/>
		public AboutMeWindow() => InitializeComponent();


		private void GitHubLink_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Hyperlink textBlock)
			{
				try
				{
					Process.Start(textBlock.NavigateUri.AbsoluteUri);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Warning");
				}
			}
		}
	}
}
