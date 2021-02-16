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
		public static Filter PuzzleLoading => Create().WithSudoku().WithAll();


		/// <summary>
		/// Creates a new <see cref="Filter"/> instance.
		/// </summary>
		/// <returns>The new <see cref="Filter"/> instance.</returns>
		public static Filter Create() => new();
	}
}
