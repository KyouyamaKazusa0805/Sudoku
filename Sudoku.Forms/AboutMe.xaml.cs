using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;

namespace Sudoku.Forms
{
	/// <summary>
	/// Interaction logic for AboutMe.xaml
	/// </summary>
	public partial class AboutMe : Window
	{
		public AboutMe() => InitializeComponent();


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
