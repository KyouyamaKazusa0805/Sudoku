namespace Sudoku.Presentation.Nodes;

/// <summary>
/// Defines a view node that highlights for a link.
/// </summary>
public sealed partial class LinkViewNode : BasicViewNode
{
	/// <summary>
	/// Initializes a <see cref="LinkViewNode"/> instance via the specified identifier,
	/// the specified start and end point, and the inference type.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="startPoint">The start point of the link.</param>
	/// <param name="endPoint">The end point of the link.</param>
	/// <param name="inference">The inference type.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public LinkViewNode(Identifier identifier, scoped in LockedTarget startPoint, scoped in LockedTarget endPoint, Inference inference) :
		base(identifier) => (Start, End, Inference) = (startPoint, endPoint, inference);


	/// <summary>
	/// Indicates the start point.
	/// </summary>
	public LockedTarget Start { get; }

	/// <summary>
	/// Indicates the end point.
	/// </summary>
	public LockedTarget End { get; }

	/// <summary>
	/// Indicates the inference type.
	/// </summary>
	public Inference Inference { get; }


	[GeneratedDeconstruction]
	public partial void Deconstruct(out LockedTarget start, out LockedTarget end, out Inference inference);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is LinkViewNode comparer && Start == comparer.Start && End == comparer.End && Inference == comparer.Inference;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(TypeIdentifier), nameof(Identifier), nameof(Start), nameof(End))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(Start), nameof(End), nameof(Inference))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override LinkViewNode Clone() => new(Identifier, Start, End, Inference);
}
