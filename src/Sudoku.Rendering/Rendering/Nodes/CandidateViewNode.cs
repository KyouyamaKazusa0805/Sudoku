namespace Sudoku.Rendering.Nodes;

/// <summary>
/// Defines a view node that highlights for a candidate.
/// </summary>
/// <param name="identifier"><inheritdoc/></param>
/// <param name="candidate">Indicates the candidate highlighted.</param>
[method: JsonConstructor]
public sealed partial class CandidateViewNode(ColorIdentifier identifier, [PrimaryConstructorParameter] Candidate candidate) :
	BasicViewNode(identifier)
{
	/// <summary>
	/// Indicates the candidate string.
	/// </summary>
	[ToStringIdentifier(nameof(Candidate))]
	private string CandidateString => RxCyNotation.ToCandidateString(Candidate);


	[DeconstructionMethod]
	public partial void Deconstruct(out ColorIdentifier identifier, out Candidate candidate);

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
