namespace Sudoku.Presentation.Nodes;

/// <summary>
/// Defines a view node that highlights for a house.
/// </summary>
/// <param name="identifier"><inheritdoc cref="BasicViewNode(Identifier)" path="/param[@name='identifier']"/></param>
/// <param name="houseIndex">The house index.</param>
public sealed partial class HouseViewNode(Identifier identifier, int houseIndex) : BasicViewNode(identifier)
{
	/// <summary>
	/// Indicates the house highlighted.
	/// </summary>
	public int House { get; } = houseIndex;


	[DeconstructionMethod]
	public partial void Deconstruct(out Identifier identifier, out int house);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is HouseViewNode comparer && Identifier == comparer.Identifier && House == comparer.House;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(TypeIdentifier), nameof(Identifier), nameof(House))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(House))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override HouseViewNode Clone() => new(Identifier, House);
}
