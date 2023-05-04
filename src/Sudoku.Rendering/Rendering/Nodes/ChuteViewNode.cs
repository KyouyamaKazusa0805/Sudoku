namespace Sudoku.Rendering.Nodes;

/// <summary>
/// Defines a view node that highlights for a chute (i.e. 3 houses that is in a three blocks in a line).
/// </summary>
//[method: JsonConstructor]
public sealed partial class ChuteViewNode : BasicViewNode//(ColorIdentifier identifier, int chuteIndex) : BasicViewNode(identifier)
{
#pragma warning disable CS1591
	[JsonConstructor]
	public ChuteViewNode(ColorIdentifier identifier, int chuteIndex) : base(identifier) => ChuteIndex = chuteIndex;
#pragma warning restore CS1591


	/// <summary>
	/// Indicates whether the chute is in a row.
	/// </summary>
	[JsonIgnore]
	public bool IsRow => ChuteIndex < 3;

	/// <summary>
	/// Indicates the chute index. The value can be between 0 and 5.
	/// </summary>
	[JsonInclude]
	public int ChuteIndex { get; }// = chuteIndex;

	/// <summary>
	/// <para>
	/// Indicates a <see cref="int"/> bits that represents for the houses used.
	/// The result mask is a 27-bit digit that represents every possible houses using cases.
	/// </para>
	/// <para>
	/// Please note that the first 9-bit always keep the zero value because they is reserved bits
	/// for block houses, but all chutes don't use them.
	/// </para>
	/// </summary>
	[JsonIgnore]
	public HouseMask HousesMask => Chutes[ChuteIndex] switch { var (_, isRow, rawMask) => rawMask << (isRow ? 9 : 18) };


	[DeconstructionMethod]
	public partial void Deconstruct(out ColorIdentifier identifier, [DeconstructionMethodArgument(nameof(ChuteIndex))] out int chute);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is ChuteViewNode comparer && ChuteIndex == comparer.ChuteIndex;

	[GeneratedOverridingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(TypeIdentifier), nameof(ChuteIndex))]
	public override partial int GetHashCode();

	[GeneratedOverridingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(ChuteIndex))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override ChuteViewNode Clone() => new(Identifier, ChuteIndex);
}
