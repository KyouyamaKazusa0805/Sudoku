namespace Sudoku.UI.Extensions
{
	/// <summary>
	/// Encapsulates some built-in <see cref="Filter"/>s in use.
	/// </summary>
	/// <seealso cref="Filter"/>
	public static class Filters
	{
		/// <summary>
		/// Indicates the filter that is used in puzzle loading.
		/// </summary>
		public static readonly Filter PuzzleLoading = new Filter().WithSudoku().WithAll();
	}
}
