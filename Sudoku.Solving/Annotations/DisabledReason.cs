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

		/// <summary>
		/// Indicates the searcher runs so slowly that the author himself cannot stand to use it.
		/// </summary>
		TooSlow,

		/// <summary>
		/// Indicates the searcher can get correct <see cref="TechniqueInfo"/>s, but the difference
		/// of the difficulty among them are too big.
		/// </summary>
		Unstable,
	}
}
