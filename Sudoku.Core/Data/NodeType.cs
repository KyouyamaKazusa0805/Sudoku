using System;

namespace Sudoku.Data
{
	/// <summary>
	/// Indicates a node type in a chain.
	/// </summary>
	[Obsolete("Need rewrite.")]
	public enum NodeType : byte
	{
		/// <summary>
		/// Indicates an empty node, which is only used for
		/// keep away from the throwing of exceptions.
		/// </summary>
		[Obsolete]
		Empty = 0,

		/// <summary>
		/// Indicates a normal candidate.
		/// </summary>
		Candidate,

		/// <summary>
		/// Indicates a locked candidates node, which contains at least two cells
		/// in a same box-row or box-column and contains the same digit.
		/// </summary>
		LockedCandidates,
	}
}
