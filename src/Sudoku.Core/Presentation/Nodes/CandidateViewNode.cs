namespace Sudoku.Presentation.Nodes;

/// <summary>
/// Defines a view node that highlights for a candidate.
/// </summary>
public sealed partial class CandidateViewNode(Identifier identifier, int candidate) : BasicViewNode(identifier)
{
	/// <summary>
	/// Indicates the candidate highlighted.
	/// </summary>
	[JsonInclude]
	public int Candidate { get; } = candidate;

	/// <summary>
	/// Indicates the candidate string.
	/// </summary>
	[GeneratedDisplayName(nameof(Candidate))]
	private string CandidateString => RxCyNotation.ToCandidateString(Candidate);


	[DeconstructionMethod]
	public partial void Deconstruct(out Identifier identifier, out int candidate);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is CandidateViewNode comparer && Candidate == comparer.Candidate;

	[GeneratedOverridingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(TypeIdentifier), nameof(Candidate))]
	public override partial int GetHashCode();

	[GeneratedOverridingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(CandidateString))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override CandidateViewNode Clone() => new(Identifier, Candidate);
}
