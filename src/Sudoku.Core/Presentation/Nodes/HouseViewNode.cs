namespace Sudoku.Presentation.Nodes;

/// <summary>
/// Defines a view node that highlights for a house.
/// </summary>
public sealed class HouseViewNode : ViewNode
{
	/// <summary>
	/// Initializes a <see cref="HouseViewNode"/> instance via the identifier and the highlight house.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="houseIndex">The house index.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public HouseViewNode(Identifier identifier, int houseIndex) : base(identifier) => House = houseIndex;


	/// <summary>
	/// Indicates the house highlighted.
	/// </summary>
	public int House { get; }


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is HouseViewNode comparer && Identifier == comparer.Identifier && House == comparer.House;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => HashCode.Combine(nameof(HouseViewNode), Identifier, House);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
		=> $"{nameof(HouseViewNode)} {{ {nameof(Identifier)} = {Identifier}, {nameof(House)} = {new HouseCollection(House).ToString()} }}";

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override HouseViewNode Clone() => new(Identifier, House);
}
