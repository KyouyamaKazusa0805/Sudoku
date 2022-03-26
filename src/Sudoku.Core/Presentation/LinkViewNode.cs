namespace Sudoku.Presentation;

/// <summary>
/// Defines a view node that highlights for a link.
/// </summary>
public sealed class LinkViewNode : ViewNode
{
	/// <summary>
	/// Initializes a <see cref="LinkViewNode"/> instance via the specified identifier,
	/// the specified start and end point, and the link kind.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="startPoint">The start point of the link.</param>
	/// <param name="endPoint">The end point of the link.</param>
	/// <param name="linkKind">The kind of the link.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public LinkViewNode(
		Identifier identifier, in LockedTarget startPoint, in LockedTarget endPoint, LinkKind linkKind) :
		base(identifier)
	{
		Start = startPoint;
		End = endPoint;
		LinkKind = linkKind;
	}


	/// <summary>
	/// Indicates the start point.
	/// </summary>
	public LockedTarget Start { get; }

	/// <summary>
	/// Indicates the end point.
	/// </summary>
	public LockedTarget End { get; }

	/// <summary>
	/// Indicates the link kind.
	/// </summary>
	public LinkKind LinkKind { get; }


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) =>
		other is LinkViewNode comparer
			&& Start == comparer.Start && End == comparer.End && LinkKind == comparer.LinkKind;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() =>
		HashCode.Combine(nameof(LinkViewNode), Start.Cells, Start.Digit, End.Cells, End.Digit, LinkKind);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() =>
		$"{nameof(LinkViewNode)} {{ {nameof(Start)} = {Start}, {nameof(End)} = {End}, {nameof(LinkKind)} = {LinkKind} }}";

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override LinkViewNode Clone() => new(Identifier, Start, End, LinkKind);
}
