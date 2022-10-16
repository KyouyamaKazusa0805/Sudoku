namespace Sudoku.Presentation.Nodes;

/// <summary>
/// Defines a view node that highlights for a link.
/// </summary>
public sealed class LinkViewNode : ViewNode
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
	public LinkViewNode(
		Identifier identifier,
		scoped in LockedTarget startPoint,
		scoped in LockedTarget endPoint,
		Inference inference
	) : base(identifier) => (Start, End, Inference) = (startPoint, endPoint, inference);


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

	/// <inheritdoc/>
	protected override string TypeIdentifier => nameof(LinkViewNode);


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out LockedTarget start, out LockedTarget end, out Inference inference)
		=> (start, end, inference) = (Start, End, Inference);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is LinkViewNode comparer
		&& Start == comparer.Start && End == comparer.End && Inference == comparer.Inference;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => HashCode.Combine(TypeIdentifier, Identifier, Start, End);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() =>
		$$"""{{nameof(LinkViewNode)}} { {{nameof(Identifier)}} = {{Identifier}}, {{nameof(Start)}} = {{Start}}, {{nameof(End)}} = {{End}}, {{nameof(Inference)}} = {{Inference}} }""";

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override LinkViewNode Clone() => new(Identifier, Start, End, Inference);
}
