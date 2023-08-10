namespace Sudoku.Rendering.Nodes;

/// <summary>
/// Defines a view node that highlights for a candidate.
/// </summary>
/// <param name="identifier"><inheritdoc/></param>
/// <param name="candidate">Indicates the candidate highlighted.</param>
[GetHashCode]
[ToString]
[method: JsonConstructor]
public sealed partial class CandidateViewNode(ColorIdentifier identifier, [DataMember, HashCodeMember] Candidate candidate) :
	BasicViewNode(identifier)
{
	/// <summary>
	/// Indicates the candidate string.
	/// </summary>
	[StringMember(nameof(Candidate))]
	private string CandidateString => CandidateNotation.ToString(Candidate);


	[DeconstructionMethod]
	public partial void Deconstruct(out ColorIdentifier identifier, out Candidate candidate);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is CandidateViewNode comparer && Candidate == comparer.Candidate;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override CandidateViewNode Clone() => new(Identifier, Candidate);
}
