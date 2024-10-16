namespace Sudoku.Drawing;

/// <summary>
/// Represents an item that can be drawn by GDI+ graphics module or UI shape controls.
/// </summary>
/// <param name="identifier"><inheritdoc cref="Identifier" path="/summary"/></param>
[JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToBaseType, TypeDiscriminatorPropertyName = "$typeid")]
[JsonDerivedType(typeof(CellViewNode), 0)]
[JsonDerivedType(typeof(CandidateViewNode), 1)]
[JsonDerivedType(typeof(HouseViewNode), 2)]
[JsonDerivedType(typeof(ChuteViewNode), 3)]
[JsonDerivedType(typeof(BabaGroupViewNode), 4)]
[JsonDerivedType(typeof(ChainLinkViewNode), 5)]
[JsonDerivedType(typeof(CellLinkViewNode), 6)]
[JsonDerivedType(typeof(ConjugateLinkViewNode), 7)]
[JsonDerivedType(typeof(CircleViewNode), 10)]
[JsonDerivedType(typeof(CrossViewNode), 11)]
[JsonDerivedType(typeof(TriangleViewNode), 12)]
[JsonDerivedType(typeof(DiamondViewNode), 13)]
[JsonDerivedType(typeof(StarViewNode), 14)]
[JsonDerivedType(typeof(SquareViewNode), 15)]
[JsonDerivedType(typeof(HeartViewNode), 16)]
[TypeImpl(
	TypeImplFlags.AllObjectMethods | TypeImplFlags.EqualityOperators | TypeImplFlags.Equatable,
	GetHashCodeBehavior = GetHashCodeBehavior.MakeAbstract,
	ToStringBehavior = ToStringBehavior.MakeAbstract,
	OtherModifiersOnEquatableEquals = "virtual")]
public abstract partial class ViewNode(ColorIdentifier identifier) :
	ICloneable,
	IDrawableItem,
	IEquatable<ViewNode>,
	IEqualityOperators<ViewNode, ViewNode, bool>
{
	/// <summary>
	/// Indicates an instance providing with data for describing coloring.
	/// </summary>
	/// <remarks><b>
	/// We cannot change this property into a primary constructor parameter because here attribute <c>[StringMember]</c>
	/// is not supported by derived types, meaning derived types cannot detect this attribute
	/// because it's in primary constructor declaration by a base type.
	/// </b></remarks>
	[StringMember]
	[EquatableMember]
	public ColorIdentifier Identifier { get; } = identifier;

	/// <summary>
	/// Indicates the inner identifier to distinct the different types that is derived from <see cref="ViewNode"/>.
	/// </summary>
	/// <seealso cref="ViewNode"/>
	[HashCodeMember]
	[StringMember("EqualityContract")]
	protected string TypeIdentifier => GetType().Name;


	/// <inheritdoc cref="ICloneable.Clone"/>
	public abstract ViewNode Clone();

	/// <inheritdoc/>
	object ICloneable.Clone() => Clone();
}
