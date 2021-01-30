using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using static Sudoku.UI.Dictionaries.DictionaryResources;

namespace Sudoku.UI.Data
{
	/// <summary>
	/// Provides a series of built-in <see cref="MessageDialog"/> instances.
	/// </summary>
	internal static class MessageDialogs
	{
		/// <summary>
		/// To show the exception details.
		/// </summary>
		/// <param name="ex">The exception instance.</param>
		public static async Task ShowExceptionMessageAsync(Exception ex)
		{
			await new MessageDialog(
				content: ex.Message,
				title: (string)LangSource["MessageDialogTitleInfo"]
			).ShowAsync();
		}
	}
}
