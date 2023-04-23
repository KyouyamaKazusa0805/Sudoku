namespace Sudoku.Rendering.Nodes;

/// <summary>
/// Defines a view node that highlights for a house.
/// </summary>
public sealed partial class HouseViewNode(ColorIdentifier identifier, House houseIndex) : BasicViewNode(identifier)
{
	/// <summary>
	/// Indicates the house highlighted.
	/// </summary>
	public House House { get; } = houseIndex;


	[DeconstructionMethod]
	public partial void Deconstruct(out ColorIdentifier identifier, out House house);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is HouseViewNode comparer && House == comparer.House;

	[GeneratedOverridingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(TypeIdentifier), nameof(House))]
	public override partial int GetHashCode();

	[GeneratedOverridingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(House))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override HouseViewNode Clone() => new(Identifier, House);
}
