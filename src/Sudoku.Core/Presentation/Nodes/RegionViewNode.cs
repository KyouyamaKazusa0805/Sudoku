namespace Sudoku.Presentation.Nodes;

/// <summary>
/// Defines a view node that highlights for a region.
/// </summary>
public sealed class RegionViewNode : ViewNode
{
	/// <summary>
	/// Initializes a <see cref="RegionViewNode"/> instance via the identifier and the highlight region.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="region">The region.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public RegionViewNode(Identifier identifier, int region) : base(identifier) => Region = region;


	/// <summary>
	/// Indicates the region highlighted.
	/// </summary>
	public int Region { get; }


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) =>
		other is RegionViewNode comparer
			&& Identifier == comparer.Identifier && Region == comparer.Region;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => HashCode.Combine(nameof(RegionViewNode), Identifier, Region);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() =>
		$"{nameof(RegionViewNode)} {{ {nameof(Identifier)} = {Identifier}, {nameof(Region)} = {new RegionCollection(Region).ToString()} }}";

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override RegionViewNode Clone() => new(Identifier, Region);
}
