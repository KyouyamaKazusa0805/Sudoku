using System.Collections.Generic;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Encapsulates a locked candidates node.
	/// </summary>
	public sealed class LockedCandidatesNode : Node
	{
		/// <inheritdoc/>
		public LockedCandidatesNode(IEnumerable<int> candidates) : base(candidates)
		{
		}


		/// <inheritdoc/>
		public override NodeType NodeType => NodeType.LockedCandidates;
	}
}
