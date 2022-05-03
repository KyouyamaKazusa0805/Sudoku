namespace System;

/// <summary>
/// Represents a character as a UTF-8 code unit.
/// </summary>
[AutoOverridesGetHashCode(nameof(_char))]
[AutoOverridesEquals(nameof(_char))]
[AutoImplementsComparable(nameof(_char))]
[AutoOverridesToString(nameof(_char), Pattern = "{((char)[0]).*}")]
[AutoOverloadsEqualityOperators]
[AutoOverloadsComparisonOperators]
public readonly partial struct Utf8Char :
	IComparable,
	IComparable<Utf8Char>,
	IComparisonOperators<Utf8Char, Utf8Char>,
	IDefaultable<Utf8Char>,
	IEquatable<Utf8Char>,
	IEqualityOperators<Utf8Char, Utf8Char>,
	IMinMaxValue<Utf8Char>
{
	/// <summary>
	/// Indicates the minimum-valued instance of the current type.
	/// </summary>
	public static readonly Utf8Char MinValue = (Utf8Char)'\0';

	/// <summary>
	/// Indicates the maximum-valued instance of the current type.
	/// </summary>
	public static readonly Utf8Char MaxValue = (Utf8Char)byte.MaxValue;


	/// <summary>
	/// Indicates the inner character.
	/// </summary>
	private readonly byte _char;


	/// <summary>
	/// Initializes a <see cref="Utf8Char"/> instance via the specified <see cref="byte"/> value.
	/// </summary>
	/// <param name="value">The <see cref="byte"/> value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Utf8Char(byte value) => _char = value;


	/// <inheritdoc/>
	bool IDefaultable<Utf8Char>.IsDefault => Equals(MinValue);

	/// <inheritdoc/>
	static Utf8Char IMinMaxValue<Utf8Char>.MinValue => MinValue;

	/// <inheritdoc/>
	static Utf8Char IMinMaxValue<Utf8Char>.MaxValue => MaxValue;

	/// <inheritdoc/>
	static Utf8Char IDefaultable<Utf8Char>.Default => MinValue;


	/// <summary>
	/// Determines whether the current character is a digit.
	/// </summary>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsDigit() => _char is >= (byte)'0' and <= (byte)'9';

	/// <summary>
	/// Determines whether the current character is a letter.
	/// </summary>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsLetter() => _char is >= (byte)'A' and <= (byte)'Z' or >= (byte)'a' and <= (byte)'z';

	/// <summary>
	/// Determines whether the current character is a upper-casing letter.
	/// </summary>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsUpper() => _char is >= (byte)'A' and <= (byte)'Z';

	/// <summary>
	/// Determines whether the current character is a lower-casing letter.
	/// </summary>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsLower() => _char is >= (byte)'a' and <= (byte)'z';

	/// <summary>
	/// Determines whether a character is a letter or a digit.
	/// </summary>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsLetterOrDigit() => IsLetter() || IsDigit();

	/// <summary>
	/// Converts the current character to the upper-casing letter.
	/// </summary>
	/// <returns>The result character.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Utf8Char ToUpper() => _char - 32 is var resultChar and >= 'A' and <= 'Z' ? (Utf8Char)(char)resultChar : _char;

	/// <summary>
	/// Converts the current character to the lower-casing letter.
	/// </summary>
	/// <returns>The result character.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Utf8Char ToLower() => _char + 32 is var resultChar and >= 'a' and <= 'z' ? (Utf8Char)(char)resultChar : _char;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	int IComparable.CompareTo(object? obj)
		=> obj is Utf8Char comparer
			? CompareTo(comparer)
			: throw new ArgumentException("Cannot operate because the argument is not a UTF-8 formatted character.", nameof(obj));
	

	/// <summary>
	/// Explicitly cast from <see cref="char"/> instance to <see cref="Utf8Char"/> instance.
	/// </summary>
	/// <param name="utf16char">The <see cref="char"/> instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator Utf8Char(char utf16char) => new((byte)utf16char);

	/// <summary>
	/// Implicitly cast from <see cref="Utf8Char"/> instance to <see cref="byte"/> instance.
	/// </summary>
	/// <param name="utf8Char">The <see cref="Utf8Char"/> instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator byte(Utf8Char utf8Char) => utf8Char._char;

	/// <summary>
	/// Implicitly cast from <see cref="Utf8Char"/> instance to <see cref="char"/> instance.
	/// </summary>
	/// <param name="utf8Char">The <see cref="Utf8Char"/> instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator char(Utf8Char utf8Char) => (char)utf8Char._char;

	/// <summary>
	/// Implicitly cast from <see cref="byte"/> instance to <see cref="Utf8Char"/> instance.
	/// </summary>
	/// <param name="byteValue">The <see cref="byte"/> instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Utf8Char(byte byteValue) => new(byteValue);
}
