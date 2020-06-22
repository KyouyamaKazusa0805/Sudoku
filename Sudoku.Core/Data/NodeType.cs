namespace Sudoku.Data
{
	/// <summary>
	/// Indicates a node type in a chain.
	/// </summary>
	public enum NodeType : byte
	{
		/// <summary>
		/// Indicates a normal candidate.
		/// </summary>
		Candidate = 0,

		/// <summary>
		/// Indicates a locked candidates node, which contains at least two cells
		/// in a same box-row or box-column and contains the same digit.
		/// </summary>
		LockedCandidates,

		/// <summary>
		/// Indicates an ALS node.
		/// </summary>
		AlmostLockedSets,

		/// <summary>
		/// Indicates an AUR node.
		/// </summary>
		AlmostUniqueRectangle,
	}
}
