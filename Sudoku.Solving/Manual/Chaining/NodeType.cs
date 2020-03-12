namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Indicates a node type in a chain.
	/// </summary>
	public enum NodeType
	{
		/// <summary>
		/// Indicates a normal candidate.
		/// </summary>
		Candidate = 1,

		/// <summary>
		/// Indicates a normal locked candidates.
		/// </summary>
		LockedCandidates,

		/// <summary>
		/// Indicates an ALS.
		/// </summary>
		AlmostLockedSets,

		/// <summary>
		/// Indicates a UR.
		/// </summary>
		UniqueRectangle,
	}
}
