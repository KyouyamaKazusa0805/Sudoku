namespace Sudoku.Presentation.Nodes;

/// <summary>
/// Defines a view node that highlights for a candidate.
/// </summary>
public sealed class CandidateViewNode : ViewNode
{
	/// <summary>
	/// Initializes a <see cref="CandidateViewNode"/> instance via the identifier and the highlight candidate.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="candidate">The candidate.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CandidateViewNode(Identifier identifier, int candidate) : base(identifier) => Candidate = candidate;


	/// <summary>
	/// Indicates the candidate highlighted.
	/// </summary>
	public int Candidate { get; }


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) =>
		other is CandidateViewNode comparer
			&& Identifier == comparer.Identifier && Candidate == comparer.Candidate;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => HashCode.Combine(nameof(CandidateViewNode), Identifier, Candidate);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() =>
		$"{nameof(CandidateViewNode)} {{ {nameof(Identifier)} = {Identifier}, {nameof(Candidate)} = {Candidates.Empty + Candidate} }}";

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override CandidateViewNode Clone() => new(Identifier, Candidate);
}
