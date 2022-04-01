namespace Sudoku.Presentation;

/// <summary>
/// Defines an identifier that can differ colors.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public readonly struct Identifier :
	IEquatable<Identifier>
#if FEATURE_GENERIC_MATH
	,
	IEqualityOperators<Identifier, Identifier>
#endif
{
	/// <summary>
	/// Indicates the raw value of the color.
	/// </summary>
	[field: FieldOffset(4)]
	private readonly int _colorRawValue;


	/// <summary>
	/// Initializes an <see cref="Identifier"/> instance.
	/// </summary>
	/// <exception cref="NotSupportedException">Always throws.</exception>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("You cannot use the parameterless constructor to construct the data structure. Please use factory method instead.", true, DiagnosticId = "SUDOKULIB001")]
	public Identifier() => throw new NotSupportedException();

	/// <summary>
	/// Initializes an <see cref="Identifier"/> instance via the ID value.
	/// </summary>
	/// <param name="id">The ID value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Identifier(int id)
	{
		Unsafe.SkipInit(out this);

		UseId = true;
		Id = id;
	}

	/// <summary>
	/// Initializes an <see cref="Identifier"/> instance via the color value.
	/// </summary>
	/// <param name="a">The alpha value.</param>
	/// <param name="r">The red value.</param>
	/// <param name="g">The green value.</param>
	/// <param name="b">The blue value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Identifier(byte a, byte r, byte g, byte b)
	{
		Unsafe.SkipInit(out this);

		UseId = false;
		_colorRawValue = a << 24 | r << 16 | g << 8 | b;
	}


	/// <summary>
	/// Indicates whether the user uses the ID value.
	/// </summary>
	[field: FieldOffset(0)]
	public bool UseId { get; }

	/// <summary>
	/// Indicates the alpha value.
	/// </summary>
	/// <exception cref="InvalidOperationException">Throws when the current instance uses ID.</exception>
	public byte A =>
		UseId
			? throw new InvalidOperationException("Can't take the value because the current operation uses ID.")
			: (byte)(_colorRawValue >> 24 & byte.MaxValue);

	/// <summary>
	/// Indicates the red value.
	/// </summary>
	/// <exception cref="InvalidOperationException">Throws when the current instance uses ID.</exception>
	public byte R =>
		UseId
			? throw new InvalidOperationException("Can't take the value because the current operation uses ID.")
			: (byte)(_colorRawValue >> 16 & byte.MaxValue);

	/// <summary>
	/// Indicates the green value.
	/// </summary>
	/// <exception cref="InvalidOperationException">Throws when the current instance uses ID.</exception>
	public byte G =>
		UseId
			? throw new InvalidOperationException("Can't take the value because the current operation uses ID.")
			: (byte)(_colorRawValue >> 8 & byte.MaxValue);

	/// <summary>
	/// Indicates the blue value.
	/// </summary>
	/// <exception cref="InvalidOperationException">Throws when the current instance uses ID.</exception>
	public byte B =>
		UseId
			? throw new InvalidOperationException("Can't take the value because the current operation uses ID.")
			: (byte)(_colorRawValue & byte.MaxValue);

	/// <summary>
	/// Indicates the ID value used.
	/// </summary>
	[field: FieldOffset(4)]
	public int Id { get; }


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] object? obj) => base.Equals(obj);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Identifier other) => UseId == other.UseId && Id == other.Id;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => HashCode.Combine(UseId, _colorRawValue);

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() =>
		$"{nameof(Identifier)} {{ {(UseId ? $"ID = {Id}" : $"Color = #{A:X2}{R:X2}{G:X2}{B:X2}")} }}";

	/// <summary>
	/// Try to cast the current identifier instance into the result color value.
	/// </summary>
	/// <typeparam name="TColor">The type of the color.</typeparam>
	/// <param name="converter">The converter method.</param>
	/// <returns>The result color.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TColor GetColor<TColor>(Converter<Identifier, TColor> converter) => converter(this);


	/// <summary>
	/// Creates an <see cref="Identifier"/> instance via the specified ID value.
	/// </summary>
	/// <param name="id">The ID value.</param>
	/// <returns>The result <see cref="Identifier"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Identifier FromId(int id) => new(id);

	/// <summary>
	/// Creates an <see cref="Identifier"/> instance via the color value.
	/// </summary>
	/// <param name="r">The red value.</param>
	/// <param name="g">The green value.</param>
	/// <param name="b">The blue value.</param>
	/// <returns>The result <see cref="Identifier"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Identifier FromColor(byte r, byte g, byte b) => new(byte.MaxValue, r, g, b);

	/// <summary>
	/// Creates an <see cref="Identifier"/> instance via the color value.
	/// </summary>
	/// <param name="a">The alpha value.</param>
	/// <param name="r">The red value.</param>
	/// <param name="g">The green value.</param>
	/// <param name="b">The blue value.</param>
	/// <returns>The result <see cref="Identifier"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Identifier FromColor(byte a, byte r, byte g, byte b) => new(a, r, g, b);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Identifier left, Identifier right) => left.Equals(right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Identifier left, Identifier right) => !(left == right);


	/// <summary>
	/// Explicit cast from <see cref="Identifier"/> to <see cref="int"/> indicating the ID value.
	/// </summary>
	/// <param name="identifier">The <see cref="Identifier"/> instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator int(Identifier identifier) =>
		identifier.UseId
			? identifier.Id
			: throw new InvalidCastException("The instance cannot be converted to an 'int' due to invalid status.");

	/// <summary>
	/// Explicit cast from <see cref="Identifier"/> to (<see cref="byte"/>, <see cref="byte"/>,
	/// <see cref="byte"/>).
	/// </summary>
	/// <param name="identifier">The <see cref="Identifier"/> instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator (byte R, byte G, byte B)(Identifier identifier) =>
		identifier.UseId || identifier.A != byte.MaxValue
			? throw new InvalidCastException("The instance cannot be converted to a triple due to invalid status.")
			: (identifier.R, identifier.G, identifier.B);

	/// <summary>
	/// Explicit cast from <see cref="Identifier"/> to (<see cref="byte"/>, <see cref="byte"/>,
	/// <see cref="byte"/>, <see cref="byte"/>).
	/// </summary>
	/// <param name="identifier">The <see cref="Identifier"/> instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator (byte A, byte R, byte G, byte B)(Identifier identifier) =>
		identifier.UseId
			? throw new InvalidCastException("The instance cannot be converted to a quadruple due to invalid status.")
			: (identifier.A, identifier.R, identifier.G, identifier.B);

	/// <summary>
	/// Implicit cast from (<see cref="byte"/>, <see cref="byte"/>, <see cref="byte"/>, <see cref="byte"/>)
	/// to <see cref="Identifier"/>.
	/// </summary>
	/// <param name="colorQuadruple">
	/// The quadruple of element types <see cref="byte"/>, <see cref="byte"/>, <see cref="byte"/>
	/// and <see cref="byte"/>.
	/// </param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Identifier((byte A, byte R, byte G, byte B) colorQuadruple) =>
		new(colorQuadruple.A, colorQuadruple.R, colorQuadruple.G, colorQuadruple.B);

	/// <summary>
	/// Implicit cast from (<see cref="byte"/>, <see cref="byte"/>, <see cref="byte"/>)
	/// to <see cref="Identifier"/>.
	/// </summary>
	/// <param name="colorTriple">
	/// The quadruple of element types <see cref="byte"/>, <see cref="byte"/> and <see cref="byte"/>.
	/// </param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Identifier((byte R, byte G, byte B) colorTriple) =>
		new(byte.MaxValue, colorTriple.R, colorTriple.G, colorTriple.B);

	/// <summary>
	/// Implicit cast from <see cref="int"/> indicating the ID value to <see cref="Identifier"/>.
	/// </summary>
	/// <param name="id">The ID value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Identifier(int id) => new(id);
}
