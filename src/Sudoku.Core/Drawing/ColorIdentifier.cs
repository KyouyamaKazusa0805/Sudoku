namespace Sudoku.Drawing;

/// <summary>
/// Represents an identifier that is used for describing target rendering item.
/// </summary>
/// <completionlist cref="ColorIdentifier"/>
[JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToBaseType, TypeDiscriminatorPropertyName = "$typeid")]
[JsonDerivedType(typeof(ColorColorIdentifier), 0)]
[JsonDerivedType(typeof(WellKnownColorIdentifier), 1)]
[JsonDerivedType(typeof(PaletteIdColorIdentifier), 2)]
[TypeImpl(
	TypeImplFlags.AllObjectMethods | TypeImplFlags.EqualityOperators,
	GetHashCodeBehavior = GetHashCodeBehavior.MakeAbstract,
	ToStringBehavior = ToStringBehavior.MakeAbstract)]
public abstract partial class ColorIdentifier : IEquatable<ColorIdentifier>, IEqualityOperators<ColorIdentifier, ColorIdentifier, bool>
{
	/// <inheritdoc cref="WellKnownColorIdentifierKind.Normal"/>
	public static readonly ColorIdentifier Normal = WellKnownColorIdentifierKind.Normal;

	/// <inheritdoc cref="WellKnownColorIdentifierKind.Auxiliary1"/>
	public static readonly ColorIdentifier Auxiliary1 = WellKnownColorIdentifierKind.Auxiliary1;

	/// <inheritdoc cref="WellKnownColorIdentifierKind.Auxiliary2"/>
	public static readonly ColorIdentifier Auxiliary2 = WellKnownColorIdentifierKind.Auxiliary2;

	/// <inheritdoc cref="WellKnownColorIdentifierKind.Auxiliary3"/>
	public static readonly ColorIdentifier Auxiliary3 = WellKnownColorIdentifierKind.Auxiliary3;

	/// <inheritdoc cref="WellKnownColorIdentifierKind.Assignment"/>
	public static readonly ColorIdentifier Assignment = WellKnownColorIdentifierKind.Assignment;

	/// <inheritdoc cref="WellKnownColorIdentifierKind.OverlappedAssignment"/>
	public static readonly ColorIdentifier OverlappedAssignment = WellKnownColorIdentifierKind.OverlappedAssignment;

	/// <inheritdoc cref="WellKnownColorIdentifierKind.Elimination"/>
	public static readonly ColorIdentifier Elimination = WellKnownColorIdentifierKind.Elimination;

	/// <inheritdoc cref="WellKnownColorIdentifierKind.Cannibalism"/>
	public static readonly ColorIdentifier Cannibalism = WellKnownColorIdentifierKind.Cannibalism;

	/// <inheritdoc cref="WellKnownColorIdentifierKind.Exofin"/>
	public static readonly ColorIdentifier Exofin = WellKnownColorIdentifierKind.Exofin;

	/// <inheritdoc cref="WellKnownColorIdentifierKind.Endofin"/>
	public static readonly ColorIdentifier Endofin = WellKnownColorIdentifierKind.Endofin;

	/// <inheritdoc cref="WellKnownColorIdentifierKind.Link"/>
	public static readonly ColorIdentifier Link = WellKnownColorIdentifierKind.Link;

	/// <inheritdoc cref="WellKnownColorIdentifierKind.AlmostLockedSet1"/>
	public static readonly ColorIdentifier AlmostLockedSet1 = WellKnownColorIdentifierKind.AlmostLockedSet1;

	/// <inheritdoc cref="WellKnownColorIdentifierKind.AlmostLockedSet2"/>
	public static readonly ColorIdentifier AlmostLockedSet2 = WellKnownColorIdentifierKind.AlmostLockedSet2;

	/// <inheritdoc cref="WellKnownColorIdentifierKind.AlmostLockedSet3"/>
	public static readonly ColorIdentifier AlmostLockedSet3 = WellKnownColorIdentifierKind.AlmostLockedSet3;

	/// <inheritdoc cref="WellKnownColorIdentifierKind.AlmostLockedSet4"/>
	public static readonly ColorIdentifier AlmostLockedSet4 = WellKnownColorIdentifierKind.AlmostLockedSet4;

	/// <inheritdoc cref="WellKnownColorIdentifierKind.AlmostLockedSet5"/>
	public static readonly ColorIdentifier AlmostLockedSet5 = WellKnownColorIdentifierKind.AlmostLockedSet5;

	/// <inheritdoc cref="WellKnownColorIdentifierKind.Rectangle1"/>
	public static readonly ColorIdentifier Rectangle1 = WellKnownColorIdentifierKind.Rectangle1;

	/// <inheritdoc cref="WellKnownColorIdentifierKind.Rectangle2"/>
	public static readonly ColorIdentifier Rectangle2 = WellKnownColorIdentifierKind.Rectangle2;

	/// <inheritdoc cref="WellKnownColorIdentifierKind.Rectangle3"/>
	public static readonly ColorIdentifier Rectangle3 = WellKnownColorIdentifierKind.Rectangle3;


	/// <inheritdoc/>
	public abstract bool Equals([NotNullWhen(true)] ColorIdentifier? other);


	/// <summary>
	/// Implicit cast from <see cref="int"/> to <see cref="ColorIdentifier"/>.
	/// </summary>
	/// <param name="paletteId">The <see cref="int"/> instance indicating the palette ID.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator ColorIdentifier(int paletteId) => new PaletteIdColorIdentifier(paletteId);

	/// <summary>
	/// Implicit cast from <see cref="WellKnownColorIdentifierKind"/> to <see cref="ColorIdentifier"/>.
	/// </summary>
	/// <param name="kind">The <see cref="WellKnownColorIdentifierKind"/> instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator ColorIdentifier(WellKnownColorIdentifierKind kind) => new WellKnownColorIdentifier(kind);
}
