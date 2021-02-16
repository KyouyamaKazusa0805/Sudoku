using Sudoku.UI.ResourceDictionaries;
using Sudoku.UI.Windows;

namespace Sudoku.UI.Extensions
{
	/// <summary>
	/// Encapsulates a several of <see cref="MessageDialog"/>s to use.
	/// </summary>
	/// <seealso cref="MessageDialog"/>
	public static class MessageDialogs
	{
		/// <summary>
		/// Indicates the current application.
		/// </summary>
		private static readonly dynamic App = TextResources.Current;


		/// <summary>
		/// Indicates the message dialog that is used when the sudoku grid file is failed to open.
		/// </summary>
		public static MessageDialog PuzzleFileLoadFailed =>
			Create()
			.WithTitle((string)App.MessageDialogTitleInfo)
			.WithText((string)App.MessageDialogTextSudokuGridFileLoadFailed);


		/// <summary>
		/// Creates a new <see cref="MessageDialog"/> instance.
		/// </summary>
		/// <returns>The new <see cref="MessageDialog"/> instance.</returns>
		public static MessageDialog Create() => new();

		/// <summary>
		/// Sets the specified text into the message window.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The instance.</param>
		/// <param name="text">The text to set.</param>
		/// <returns>The reference that is same as <see langword="this"/>.</returns>
		public static MessageDialog WithText(this MessageDialog @this, string text)
		{
			@this.ViewModel.Text = text;
			return @this;
		}

		/// <summary>
		/// Sets the specified title into the message window.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The instance.</param>
		/// <param name="title">The title to set.</param>
		/// <returns>The reference that is same as <see langword="this"/>.</returns>
		public static MessageDialog WithTitle(this MessageDialog @this, string title)
		{
			@this.ViewModel.Title = title;
			return @this;
		}
	}
}
