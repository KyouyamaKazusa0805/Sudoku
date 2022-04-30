namespace Sudoku.Presentation.Nodes;

/// <summary>
/// Defines a view node that highlights for a candidate.
/// </summary>
[AutoOverridesToString(nameof(Identifier), nameof(Candidate))]
[AutoOverridesGetHashCode(nameof(TypeIdentifier), nameof(Identifier), nameof(Candidate))]
public sealed partial class CandidateViewNode : ViewNode
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
	protected override string TypeIdentifier => nameof(CandidateViewNode);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is CandidateViewNode comparer && Identifier == comparer.Identifier && Candidate == comparer.Candidate;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override CandidateViewNode Clone() => new(Identifier, Candidate);
}
