namespace Sudoku.UI;

partial class Constants
{
	/// <summary>
	/// Provides with the file format strings.
	/// </summary>
	/// <remarks>
	/// <para>The strings can be used in the file type checking in <see cref="DragEventArgs"/>.</para>
	/// <para>
	/// The code example:
	/// <code><![CDATA[
	/// if (e.DataView is var view && view.Contains(StandardDataFormats.StorageItems))
	/// {
	///     var item = await view.GetStorageItemsAsync();
	///     if (item is [StorageFile { FileType: TextFormat, Path: var path }, ..])
	///     {
	///         string content = await File.ReadAllTextAsync(path, cancellationToken);
	///         
	///         // Code using the variable 'content'.
	///     }
	/// }
	/// ]]></code>
	/// </para>
	/// </remarks>
	/// <seealso cref="DragEventArgs"/>
	public static class Formats
	{
		/// <summary>
		/// Indicates the text file format (<c>*.txt</c>).
		/// </summary>
		public const string TextFormat = ".txt";

		/// <summary>
		/// Indicates the sudoku file format (<c>*.sudoku</c>).
		/// </summary>
		public const string SudokuFormat = ".sudoku";
	}
}
