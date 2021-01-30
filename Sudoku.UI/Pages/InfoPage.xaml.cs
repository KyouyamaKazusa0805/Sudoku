#pragma warning disable IDE1006

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Sudoku.UI.Data;
using Sudoku.UI.Extensions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using static Sudoku.UI.Dictionaries.DictionaryResources;

namespace Sudoku.UI.Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class InfoPage : Page
	{
		/// <summary>
		/// Initializes a <see cref="InfoPage"/> instance using the default behavior.
		/// </summary>
		public InfoPage() => InitializeComponent();


		/// <summary>
		/// Triggers when <see cref="HyperlinkToGitHub"/> is clicked.
		/// </summary>
		/// <param name="sender">The object to trigger this event.</param>
		/// <param name="args">The event arguments provided.</param>
		private async void HyperlinkToGitHub_Click(Hyperlink sender, RoutedEventArgs e)
		{
			await i();

			static async Task i()
			{
				try
				{
					Process.Start(new ProcessStartInfo((string)LangSource["AboutMeRealGitHub"]));
				}
				catch (Exception ex)
				{
					const uint commandId = 0;
					await MessageDialogs.ExceptionMessage(ex)
						.WithCommand((string)LangSource["MessageDialogClose"], static _ => { }, commandId)
						.WithDefaultId(commandId)
						.ShowAsync();
				}
			}
		}
	}
}
