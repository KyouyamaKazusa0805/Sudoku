namespace Sudoku.Drawing;

/// <summary>
/// Defines a view node.
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
[JsonDerivedType(typeof(CircleViewNode), 10)]
[JsonDerivedType(typeof(CrossViewNode), 11)]
[JsonDerivedType(typeof(TriangleViewNode), 12)]
[JsonDerivedType(typeof(DiamondViewNode), 13)]
[JsonDerivedType(typeof(StarViewNode), 14)]
[JsonDerivedType(typeof(HeartViewNode), 15)]
[TypeImpl(
	TypeImplFlag.AllObjectMethods | TypeImplFlag.EqualityOperators,
	GetHashCodeBehavior = GetHashCodeBehavior.MakeAbstract,
	ToStringBehavior = ToStringBehavior.MakeAbstract)]
public abstract partial class ViewNode(ColorIdentifier identifier) : IEquatable<ViewNode>, IEqualityOperators<ViewNode, ViewNode, bool>
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
	public ColorIdentifier Identifier { get; } = identifier;

	/// <summary>
	/// Indicates the inner identifier to distinct the different types that is derived from <see cref="ViewNode"/>.
	/// </summary>
	/// <seealso cref="ViewNode"/>
	[HashCodeMember]
	[StringMember("EqualityContract")]
	protected string TypeIdentifier => GetType().Name;


	/// <inheritdoc/>
	public abstract bool Equals([NotNullWhen(true)] ViewNode? other);

	/// <summary>
	/// Creates a new <see cref="View"/> instance with same values as the current instance, with independency.
	/// </summary>
	/// <returns>A new <see cref="View"/> instance with same values as the current instance.</returns>
	public abstract ViewNode Clone();
}
