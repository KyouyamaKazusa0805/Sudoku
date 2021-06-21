using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;

namespace Sudoku.Windows
{
	/// <summary>
	/// Interaction logic for <c>ThankForWindow.xaml</c>.
	/// </summary>
	public partial class SpecialThanksWindow : Window
	{
		/// <summary>
		/// Initializes a default <see cref="SpecialThanksWindow"/> instance.
		/// </summary>
		public SpecialThanksWindow() => InitializeComponent();


		private void Hyperlink_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Hyperlink link)
			{
				try
				{
					Process.Start("explorer.exe", link.NavigateUri.AbsoluteUri);
				}
				catch (Exception ex)
				{
					Messagings.ShowExceptionMessage(ex);
				}
			}
		}
	}
}
