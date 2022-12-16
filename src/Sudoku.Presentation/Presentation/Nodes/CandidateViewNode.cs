namespace Sudoku.Presentation.Nodes;

/// <summary>
/// Defines a view node that highlights for a candidate.
/// </summary>
public sealed partial class CandidateViewNode : BasicViewNode
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
	[JsonInclude]
	public int Candidate { get; }

	/// <summary>
	/// Indicates the candidate string.
	/// </summary>
	[DebuggerHidden]
	[GeneratedDisplayName(nameof(Candidate))]
	private string CandidateString => (Candidates.Empty + Candidate).ToString();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is CandidateViewNode comparer && Identifier == comparer.Identifier && Candidate == comparer.Candidate;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(TypeIdentifier), nameof(Identifier), nameof(Candidate))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(CandidateString))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override CandidateViewNode Clone() => new(Identifier, Candidate);
}
