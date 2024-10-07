namespace System;

/// <summary>
/// Represents a variant color type.
/// </summary>
[TypeImpl(TypeImplFlag.AllObjectMethods | TypeImplFlag.EqualityOperators | TypeImplFlag.Equatable)]
[method: JsonConstructor]
public readonly partial struct VariantColor(
	[Property, StringMember] byte a,
	[Property, StringMember] byte r,
	[Property, StringMember] byte g,
	[Property, StringMember] byte b
) :
	IEquatable<VariantColor>,
	IEqualityOperators<VariantColor, VariantColor, bool>,
	IFormattable
{
	/// <summary>
	/// Indicates the default value.
	/// </summary>
	public static readonly VariantColor Default = default;


	/// <summary>
	/// Initializes a <see cref="VariantColor"/> instance via red, green and blue value.
	/// </summary>
	/// <param name="r">The red value.</param>
	/// <param name="g">The green value.</param>
	/// <param name="b">The blue value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public VariantColor(byte r, byte g, byte b) : this(255, r, g, b)
	{
	}

	/// <summary>
	/// Initializes a <see cref="VariantColor"/> instance via a whole value of A, R, G, B values.
	/// </summary>
	/// <param name="argb">An integer that includes all A, R, G, B values.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public VariantColor(int argb) : this((byte)(argb >> 24), (byte)(argb >> 16 & 255), (byte)(argb >> 8 & 255), (byte)(argb & 255))
	{
	}


	[HashCodeMember]
	[EquatableMember]
	private int ArgbValue => A << 24 | R << 16 | G << 8 | B;


	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	/// <exception cref="FormatException">Throws when the format is invalid.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(string? format)
		=> format switch
		{
			null or "x" => ToHexString(true, false),
			"X" => ToHexString(false, false),
			"#x" => ToHexString(true, true),
			"#X" => ToHexString(false, true),
			"c" => ToConsoleColorString(true),
			"C" => ToConsoleColorString(false),
			_ => throw new FormatException()
		};

	/// <summary>
	/// Try to convert the color into hex string.
	/// </summary>
	/// <param name="alpha">Indicates whether the output string will include alpha value.</param>
	/// <param name="includeHashTag">Indicates whether the output string will prepend hash-tag character <c>#</c>.</param>
	/// <returns>The hex string.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToHexString(bool alpha, bool includeHashTag)
		=> alpha
			? $"{(includeHashTag ? "#" : string.Empty)}{A:X2}{R:X2}{G:X2}{B:X2}"
			: $"{(includeHashTag ? "#" : string.Empty)}{R:X2}{G:X2}{B:X2}";

	/// <summary>
	/// Try to convert the color into console color string, with a value indicating whether the color is used as foreground.
	/// </summary>
	/// <param name="isForeground">Indicates whether the color is used as foreground.</param>
	/// <returns>The console color string.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToConsoleColorString(bool isForeground)
		=> this == Default ? "\e[0m" : $"\e[{(isForeground ? 38 : 48)};2;{R};{G};{B}m";

	/// <inheritdoc/>
	string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString(format);
}
