using Microsoft.Win32;
using Sudoku.UI.ResourceDictionaries;

namespace Sudoku.UI.Extensions
{
	/// <summary>
	/// Provides the extension methods on <see cref="OpenFileDialog"/>.
	/// </summary>
	/// <seealso cref="OpenFileDialog"/>
	public static class OpenFileDialogs
	{
		/// <summary>
		/// Indicates the open file dialog used in opening a sudoku grid file.
		/// </summary>
		public static OpenFileDialog PuzzleLoading =>
			Create()
			.WithDefaultExtension("*.sudoku")
			.WithFilter(Filters.PuzzleLoading)
			.WithMultiselect(false)
			.WithTitle((string)TextResources.Current.FileDialogTitlePuzzleLoading);


		/// <summary>
		/// Create an empty open file dialog.
		/// </summary>
		/// <returns>The instance.</returns>
		public static OpenFileDialog Create() => new();

		/// <summary>
		/// Modify the current instance, to change the default extension property
		/// <see cref="FileDialog.DefaultExt"/>.
		/// </summary>
		/// <param name="this">The instance.</param>
		/// <param name="extension">The extension to change.</param>
		/// <returns>The current instance.</returns>
		/// <seealso cref="FileDialog.DefaultExt"/>
		public static OpenFileDialog WithDefaultExtension(this OpenFileDialog @this, string extension)
		{
			@this.DefaultExt = extension;
			return @this;
		}

		/// <summary>
		/// Modify the current instance, to change the filter property
		/// <see cref="FileDialog.Filter"/>.
		/// </summary>
		/// <param name="this">The instance.</param>
		/// <param name="filter">The filter to change.</param>
		/// <returns>The current instance.</returns>
		/// <seealso cref="FileDialog.Filter"/>
		public static OpenFileDialog WithFilter(this OpenFileDialog @this, string filter)
		{
			@this.Filter = filter;
			return @this;
		}

		/// <summary>
		/// Modify the current instance, to change the multi-select property
		/// <see cref="OpenFileDialog.Multiselect"/>.
		/// </summary>
		/// <param name="this">The instance.</param>
		/// <param name="multiselect">The multi-select to change.</param>
		/// <returns>The current instance.</returns>
		/// <seealso cref="OpenFileDialog.Multiselect"/>
		public static OpenFileDialog WithMultiselect(this OpenFileDialog @this, bool multiselect)
		{
			@this.Multiselect = multiselect;
			return @this;
		}

		/// <summary>
		/// Modify the current instance, to change the title property
		/// <see cref="FileDialog.Title"/>.
		/// </summary>
		/// <param name="this">The instance.</param>
		/// <param name="title">The title to change.</param>
		/// <returns>The current instance.</returns>
		/// <seealso cref="FileDialog.Title"/>
		public static OpenFileDialog WithTitle(this OpenFileDialog @this, string title)
		{
			@this.Title = title;
			return @this;
		}
	}
}
