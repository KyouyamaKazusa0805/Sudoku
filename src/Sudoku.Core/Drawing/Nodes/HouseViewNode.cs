namespace Sudoku.Drawing.Nodes;

/// <summary>
/// Defines a view node that highlights for a house.
/// </summary>
/// <param name="identifier"><inheritdoc/></param>
/// <param name="house">Indicates the house highlighted.</param>
[GetHashCode]
[ToString]
[method: JsonConstructor]
public sealed partial class HouseViewNode(ColorIdentifier identifier, [PrimaryConstructorParameter, HashCodeMember, StringMember] House house) :
	BasicViewNode(identifier)
{
	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out ColorIdentifier identifier, out House house) => (identifier, house) = (Identifier, House);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other) => other is HouseViewNode comparer && House == comparer.House;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override HouseViewNode Clone() => new(Identifier, House);
}
