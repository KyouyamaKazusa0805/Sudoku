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
	[field: FieldOffset(0)] bool UseId, [field: FieldOffset(4)] int Id, [field: FieldOffset(4)] int Color
) : IValueEquatable<ColorIdentifier>
{
	/// <summary>
	/// Gets the alpha value.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	/// Throws when <see cref="UseId"/> is <see langword="true"/>.
	/// </exception>
	/// <seealso cref="UseId"/>
	public byte A => UseId
		? throw new InvalidOperationException("Can't take the value because the current operation uses ID.")
		: (byte)(Color >> 24 & 255);

	/// <summary>
	/// Gets the red value.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	/// Throws when <see cref="UseId"/> is <see langword="true"/>.
	/// </exception>
	/// <seealso cref="UseId"/>
	public byte R => UseId
		? throw new InvalidOperationException("Can't take the value because the current operation uses ID.")
		: (byte)(Color >> 16 & 255);

	/// <summary>
	/// Gets the green value.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	/// Throws when <see cref="UseId"/> is <see langword="true"/>.
	/// </exception>
	/// <seealso cref="UseId"/>
	public byte G => UseId
		? throw new InvalidOperationException("Can't take the value because the current operation uses ID.")
		: (byte)(Color >> 8 & 255);

	/// <summary>
	/// Gets the blue value.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	/// Throws when <see cref="UseId"/> is <see langword="true"/>.
	/// </exception>
	/// <seealso cref="UseId"/>
	public byte B => UseId
		? throw new InvalidOperationException("Can't take the value because the current operation uses ID.")
		: (byte)(Color & 255);


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
	public bool TryGetColor(out byte a, out byte r, out byte g, out byte b)
	{
		(bool returnValue, a, r, g, b) = UseId ? (false, (byte)0, (byte)0, (byte)0, (byte)0) : (true, A, R, G, B);
		return returnValue;
	}

	/// <inheritdoc/>
	public bool Equals(in ColorIdentifier other) => UseId && Id == other.Id || !UseId && Color == other.Color;

	/// <inheritdoc/>
	/// <remarks>
	/// We on purpose show the another property value. For example, if we uses ID
	/// as the instance representing, the result value of get hash code will be the color value,
	/// and vice versa.
	/// </remarks>
	public override int GetHashCode() => UseId ? Color : Id;

	/// <inheritdoc cref="object.ToString"/>
	public override string ToString() => UseId ? $"ID = {Id}" : $"Color = #{A:X2}{R:X2}{G:X2}{B:X2}";


	/// <inheritdoc/>
	public static bool operator ==(in ColorIdentifier left, in ColorIdentifier right) => left.Equals(in right);

	/// <inheritdoc/>
	public static bool operator !=(in ColorIdentifier left, in ColorIdentifier right) => !left.Equals(in right);
}
