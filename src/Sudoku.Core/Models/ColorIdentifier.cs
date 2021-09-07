namespace Sudoku.Models;

/// <summary>
/// Indicates the color identifier that used to identify the color.
/// </summary>
/// <param name="UseId">Indicates whether the current instance suggests an ID using.</param>
/// <param name="Id">
/// Indicates the ID label. The property has a useful value
/// if and only if the property <see cref="UseId"/> is <see langword="true"/>.
/// </param>
/// <param name="Color">
/// Indicates the color. The property has a useful value
/// if and only if the property <see cref="UseId"/> is <see langword="false"/>.
/// </param>
/// <seealso cref="UseId"/>
[StructLayout(LayoutKind.Explicit)]
public readonly record struct ColorIdentifier(
	[field: FieldOffset(0)] bool UseId,
	[field: FieldOffset(4)] int Id,
	[field: FieldOffset(4)] int Color
) : IValueEquatable<ColorIdentifier>
{
	/// <summary>
	/// Returns the <see cref="InvalidOperationException"/> instance to report the unexpected operation
	/// for getting the A, R, G, B value.
	/// </summary>
	private static readonly InvalidOperationException UnexpectedOperationException =
		new("Can't take the value because the current operation uses ID.");


	/// <summary>
	/// Initializes a <see cref="ColorIdentifier"/> with the specified R, G, B value.
	/// </summary>
	/// <param name="r">The red value.</param>
	/// <param name="g">The green value.</param>
	/// <param name="b">The blue value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ColorIdentifier(byte r, byte g, byte b) : this(255, r, g, b)
	{
	}

	/// <summary>
	/// Initializes a <see cref="ColorIdentifier"/> with the specified A, R, G, B value.
	/// </summary>
	/// <param name="a">The alpha value.</param>
	/// <param name="r">The red value.</param>
	/// <param name="g">The green value.</param>
	/// <param name="b">The blue value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ColorIdentifier(byte a, byte r, byte g, byte b) : this(false, default, a << 24 | r << 16 | g << 8 | b)
	{
	}


	/// <summary>
	/// Gets the alpha value.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	/// Throws when <see cref="UseId"/> is <see langword="true"/>.
	/// </exception>
	/// <seealso cref="UseId"/>
	public byte A
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => UseId ? throw UnexpectedOperationException : (byte)(Color >> 24 & 255);
	}

	/// <summary>
	/// Gets the red value.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	/// Throws when <see cref="UseId"/> is <see langword="true"/>.
	/// </exception>
	/// <seealso cref="UseId"/>
	public byte R
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => UseId ? throw UnexpectedOperationException : (byte)(Color >> 16 & 255);
	}

	/// <summary>
	/// Gets the green value.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	/// Throws when <see cref="UseId"/> is <see langword="true"/>.
	/// </exception>
	/// <seealso cref="UseId"/>
	public byte G
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => UseId ? throw UnexpectedOperationException : (byte)(Color >> 8 & 255);
	}

	/// <summary>
	/// Gets the blue value.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	/// Throws when <see cref="UseId"/> is <see langword="true"/>.
	/// </exception>
	/// <seealso cref="UseId"/>
	public byte B
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => UseId ? throw UnexpectedOperationException : (byte)(Color & 255);
	}


	/// <summary>
	/// Try to get the color value. If the property <see cref="UseId"/> is <see langword="true"/>,
	/// the return value will be <see langword="false"/>.
	/// </summary>
	/// <param name="a">The alpha value.</param>
	/// <param name="r">The red value.</param>
	/// <param name="g">The green value.</param>
	/// <param name="b">The blue value.</param>
	/// <returns>Returns whether the taking operation is successful.</returns>
	/// <seealso cref="UseId"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryGetColor(out byte a, out byte r, out byte g, out byte b)
	{
		(bool returnValue, a, r, g, b) = UseId ? (false, (byte)0, (byte)0, (byte)0, (byte)0) : (true, A, R, G, B);
		return returnValue;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(in ColorIdentifier other) => UseId && Id == other.Id || !UseId && Color == other.Color;

	/// <inheritdoc/>
	/// <remarks>
	/// We on purpose show the another property value. For example, if we uses ID
	/// as the instance representing, the result value of get hash code will be the color value,
	/// and vice versa.
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => UseId ? Color : Id;

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => UseId ? $"ID = {Id}" : $"Color = #{A:X2}{R:X2}{G:X2}{B:X2}";


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(in ColorIdentifier left, in ColorIdentifier right) => left.Equals(in right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(in ColorIdentifier left, in ColorIdentifier right) => !left.Equals(in right);


	/// <summary>
	/// Explicit cast from <see cref="int"/> to <see cref="ColorIdentifier"/>.
	/// The value is initialized as the ID value.
	/// </summary>
	/// <param name="id">The ID.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator ColorIdentifier(int id) => new() { UseId = true, Id = id };
}
