namespace Sudoku.Rendering;

/// <summary>
/// Represents an identifier that is used for describing target rendering item.
/// </summary>
/// <completionlist cref="WellKnownColorIdentifier"/>
[JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToBaseType, TypeDiscriminatorPropertyName = "$typeid")]
[JsonDerivedType(typeof(ColorColorIdentifier), 0)]
[JsonDerivedType(typeof(WellKnownColorIdentifier), 1)]
[JsonDerivedType(typeof(PaletteIdColorIdentifier), 2)]
public abstract partial class ColorIdentifier : IEquatable<ColorIdentifier>, IEqualityOperators<ColorIdentifier, ColorIdentifier, bool>
{
	[GeneratedOverridingMember(GeneratedEqualsBehavior.AsCastAndCallingOverloading)]
	public sealed override partial bool Equals(object? obj);

	/// <inheritdoc/>
	public abstract bool Equals([NotNullWhen(true)] ColorIdentifier? other);

	/// <inheritdoc/>
	public abstract override int GetHashCode();

	/// <inheritdoc/>
	public abstract override string ToString();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(ColorIdentifier? left, ColorIdentifier? right)
		=> (left, right) switch { (null, null) => true, (not null, not null) => left.Equals(right), _ => false };

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(ColorIdentifier? left, ColorIdentifier? right) => !(left == right);


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
