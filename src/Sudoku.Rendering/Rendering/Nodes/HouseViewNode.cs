namespace Sudoku.Rendering.Nodes;

/// <summary>
/// Defines a view node that highlights for a house.
/// </summary>
/// <param name="identifier"><inheritdoc/></param>
/// <param name="house">Indicates the house highlighted.</param>
[GetHashCode]
[ToString]
[method: JsonConstructor]
public sealed partial class HouseViewNode(
	ColorIdentifier identifier,
	[PrimaryConstructorParameter, HashCodeMember, StringMember] House house
) : BasicViewNode(identifier)
{
	[DeconstructionMethod]
	public partial void Deconstruct(out ColorIdentifier identifier, out House house);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is HouseViewNode comparer && House == comparer.House;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override HouseViewNode Clone() => new(Identifier, House);
}
