namespace Sudoku.Concepts.Solving.ChainNodes;

/// <summary>
/// Provides with the node that stores a sole candidate.
/// </summary>
public sealed class SoleCandidateNode : Node
{
	/// <summary>
	/// Initializes a <see cref="SoleCandidateNode"/> instance via the candidate and its current status.
	/// </summary>
	/// <param name="cell">Indicates the cell used.</param>
	/// <param name="digit">Indicates the digit used.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SoleCandidateNode(byte cell, byte digit) : base(NodeType.Sole, digit, Cells.Empty + cell)
	{
	}


	/// <summary>
	/// Indicates the cell used.
	/// </summary>
	public byte Cell { get; }

	/// <summary>
	/// Indicates the candidate used.
	/// </summary>
	public int Candidate
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Cell * 9 + Digit;
	}
}
