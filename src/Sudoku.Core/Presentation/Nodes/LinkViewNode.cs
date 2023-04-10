namespace Sudoku.Presentation.Nodes;

/// <summary>
/// Defines a view node that highlights for a link.
/// </summary>
public sealed partial class LinkViewNode(
	Identifier identifier,
	scoped in LockedTarget startPoint,
	scoped in LockedTarget endPoint,
	Inference inference
) : BasicViewNode(identifier)
{
	/// <summary>
	/// Indicates the start point.
	/// </summary>
	public LockedTarget Start { get; } = startPoint;

	/// <summary>
	/// Indicates the end point.
	/// </summary>
	public LockedTarget End { get; } = endPoint;

	/// <summary>
	/// Indicates the inference type.
	/// </summary>
	public Inference Inference { get; } = inference;


	[DeconstructionMethod]
	public partial void Deconstruct(out Identifier identifier, out LockedTarget start, out LockedTarget end, out Inference inference);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is LinkViewNode comparer && Start == comparer.Start && End == comparer.End;

	[GeneratedOverridingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(TypeIdentifier), nameof(Start), nameof(End))]
	public override partial int GetHashCode();

	[GeneratedOverridingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(Start), nameof(End), nameof(Inference))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override LinkViewNode Clone() => new(Identifier, Start, End, Inference);
}
