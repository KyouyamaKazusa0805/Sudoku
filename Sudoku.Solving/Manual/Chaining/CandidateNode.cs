namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Encapsulates a candidate node.
	/// </summary>
	public sealed class CandidateNode : Node
	{
		/// <summary>
		/// Initializes an instance with the specified candidate
		/// and a <see cref="bool"/> value.
		/// </summary>
		/// <param name="candidate">The candidate.</param>
		public CandidateNode(int candidate) : base(new[] { candidate })
		{
		}


		/// <inheritdoc/>
		public override NodeType NodeType => NodeType.Candidate;
	}
}
