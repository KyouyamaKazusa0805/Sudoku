namespace Sudoku.Drawing.Nodes;

/// <summary>
/// Defines a view node that highlights for a chute (i.e. 3 houses that is in a three blocks in a line).
/// </summary>
/// <param name="identifier"><inheritdoc/></param>
/// <param name="chuteIndex">Indicates the chute index. The value can be between 0 and 5.</param>
[TypeImpl(TypeImplFlag.Object_GetHashCode | TypeImplFlag.Object_ToString)]
[method: JsonConstructor]
public sealed partial class ChuteViewNode(ColorIdentifier identifier, [PrimaryConstructorParameter, HashCodeMember, StringMember] int chuteIndex) :
	BasicViewNode(identifier)
{
	/// <summary>
	/// Indicates whether the chute is in a row.
	/// </summary>
	[JsonIgnore]
	public bool IsRow => ChuteIndex < 3;

	/// <summary>
	/// <para>
	/// Indicates a <see cref="HouseMask"/> that represents for the houses used.
	/// The result mask is a 27-bit digit that represents every possible houses using cases.
	/// </para>
	/// <para>
	/// Please note that the first 9-bit always keep the zero value because they is reserved bits
	/// for block houses, but all chutes don't use them.
	/// </para>
	/// </summary>
	[JsonIgnore]
	public HouseMask HousesMask => Chutes[ChuteIndex] switch { var (_, _, _, rawMask) => rawMask };


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out ColorIdentifier identifier, out int chute) => (identifier, chute) = (Identifier, ChuteIndex);

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> base.Equals(other) && other is ChuteViewNode comparer && ChuteIndex == comparer.ChuteIndex;

	/// <inheritdoc/>
	public override ChuteViewNode Clone() => new(Identifier, ChuteIndex);
}
