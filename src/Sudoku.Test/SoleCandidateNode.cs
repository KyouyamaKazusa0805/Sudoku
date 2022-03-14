namespace Sudoku.Test;

/// <summary>
/// Provides with the node that stores a sole candidate.
/// </summary>
public sealed class SoleCandidateNode :
	Node
#if FEATURE_GENERIC_MATH
	,
	IMaxGlobalId<SoleCandidateNode>
#endif
{
	/// <inheritdoc cref="IMaxGlobalId{T}.MaximumGlobalId"/>
	public const int MaximumGlobalId = 729;


	/// <summary>
	/// Initializes a <see cref="SoleCandidateNode"/> instance via the candidate and its current status.
	/// </summary>
	/// <param name="cell">Indicates the cell used.</param>
	/// <param name="digit">Indicates the digit used.</param>
	/// <param name="isOn">Indicates whether the node is on.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SoleCandidateNode(byte cell, byte digit) : base(NodeType.Sole, digit, new() { cell })
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

	/// <inheritdoc/>
	static int IMaxGlobalId<SoleCandidateNode>.MaximumGlobalId
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => MaximumGlobalId;
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => $"Sole candidate: {ToSimpleString()}";
}
