namespace Sudoku.Concepts.Solving.ChainNodes;

/// <summary>
/// Defines a chain node that provides with the data for a locked candidates.
/// </summary>
public sealed class LockedCandidatesNode : Node
{
	/// <summary>
	/// Initializes a <see cref="LockedCandidatesNode"/> instance via the digit used,
	/// and two cells used.
	/// </summary>
	/// <param name="digit">The digit used.</param>
	/// <param name="cell1">The cell 1.</param>
	/// <param name="cell2">The cell 2.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public LockedCandidatesNode(byte digit, byte cell1, byte cell2) :
		base(NodeType.LockedCandidates, digit, Cells.Empty + cell1 + cell2)
	{
	}

	/// <summary>
	/// Initializes a <see cref="LockedCandidatesNode"/> instance via the digit used,
	/// and three cells used.
	/// </summary>
	/// <param name="digit">The digit used.</param>
	/// <param name="cell1">The cell 1.</param>
	/// <param name="cell2">The cell 2.</param>
	/// <param name="cell3">The cell 3.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public LockedCandidatesNode(byte digit, byte cell1, byte cell2, byte cell3) :
		base(NodeType.LockedCandidates, digit, Cells.Empty + cell1 + cell2 + cell3)
	{
	}

	/// <summary>
	/// Initializes a <see cref="LockedCandidatesNode"/> instance via the digit used and cells used.
	/// </summary>
	/// <param name="digit">The digit used.</param>
	/// <param name="cells">The cells used.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public LockedCandidatesNode(byte digit, in Cells cells) : base(NodeType.LockedCandidates, digit, cells)
	{
	}
}
