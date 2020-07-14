namespace Sudoku.Solving.Annotations
{
	/// <summary>
	/// Indicates a reason why the searcher is disabled.
	/// </summary>
	public enum DisabledReason : byte
	{
		/// <summary>
		/// Indicates the searcher is normal.
		/// </summary>
		None,

		/// <summary>
		/// Indicates the searcher searches for last resorts, which don't need to show.
		/// </summary>
		LastResort,

		/// <summary>
		/// Indicates the searcher has bugs while searching.
		/// </summary>
		HasBugs,
	}
}
