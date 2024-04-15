namespace Sudoku.Rendering.Nodes;

/// <summary>
/// Defines a view node that highlights for a candidate.
/// </summary>
/// <param name="identifier"><inheritdoc/></param>
/// <param name="candidate">Indicates the candidate highlighted.</param>
[GetHashCode]
[ToString]
[method: JsonConstructor]
public sealed partial class CandidateViewNode(ColorIdentifier identifier, [PrimaryConstructorParameter, HashCodeMember] Candidate candidate) :
	BasicViewNode(identifier)
{
	/// <summary>
	/// Indicates the target cell.
	/// </summary>
	public Cell Cell => Candidate / 9;

	/// <summary>
	/// Indicates the candidate string.
	/// </summary>
	[StringMember(nameof(Candidate))]
	private string CandidateString => GlobalizedConverter.InvariantCultureConverter.CandidateConverter(Candidate);


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out ColorIdentifier identifier, out Candidate candidate) => (identifier, candidate) = (Identifier, Candidate);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is CandidateViewNode comparer && Candidate == comparer.Candidate;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override CandidateViewNode Clone() => new(Identifier, Candidate);
}
