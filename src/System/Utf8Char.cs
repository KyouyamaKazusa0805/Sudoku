namespace System;

/// <summary>
/// Represents a character as a UTF-8 code unit.
/// </summary>
[JsonConverter(typeof(JsonConverter))]
[Equals]
[GetHashCode]
[ToString]
[EqualityOperators]
[ComparisonOperators]
public readonly partial struct Utf8Char :
	IComparable,
	IComparable<Utf8Char>,
	IComparisonOperators<Utf8Char, Utf8Char, bool>,
	IEquatable<Utf8Char>,
	IMinMaxValue<Utf8Char>,
	IAdditionOperators<Utf8Char, byte, Utf8Char>,
	ISubtractionOperators<Utf8Char, byte, Utf8Char>,
	IIncrementOperators<Utf8Char>,
	IDecrementOperators<Utf8Char>
{
	/// <summary>
	/// Indicates the minimum-valued instance of the current type.
	/// </summary>
	public static readonly Utf8Char MinValue = (Utf8Char)'\0';

	/// <summary>
	/// Indicates the maximum-valued instance of the current type.
	/// </summary>
	public static readonly Utf8Char MaxValue = byte.MaxValue;


	/// <summary>
	/// Indicates the inner character.
	/// </summary>
	[HashCodeMember]
	private readonly byte _char;


	/// <summary>
	/// Initializes a <see cref="Utf8Char"/> instance via the specified <see cref="byte"/> value.
	/// </summary>
	/// <param name="value">The <see cref="byte"/> value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Utf8Char(byte value) => _char = value;


	/// <summary>
	/// Indicates the character string.
	/// </summary>
	[StringMember]
	private string CharString => ((char)_char).ToString();


	/// <inheritdoc/>
	static Utf8Char IMinMaxValue<Utf8Char>.MinValue => MinValue;

	/// <inheritdoc/>
	static Utf8Char IMinMaxValue<Utf8Char>.MaxValue => MaxValue;


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

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Utf8Char other) => _char == other._char;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(Utf8Char other) => _char.CompareTo(_char);

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
			: throw new ArgumentException(ResourceDictionary.ExceptionMessage("CharacterNotUtf8"), nameof(obj));


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Utf8Char operator +(Utf8Char @char, byte offset) => (byte)(@char._char + offset);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Utf8Char operator checked +(Utf8Char @char, byte offset) => checked((byte)(@char._char + offset));

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Utf8Char operator -(Utf8Char @char, byte offset) => (byte)(@char._char - offset);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Utf8Char operator checked -(Utf8Char @char, byte offset) => checked((byte)(@char._char - offset));

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Utf8Char operator ++(Utf8Char @char) => @char + 1;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Utf8Char operator checked ++(Utf8Char @char) => checked(@char + 1);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Utf8Char operator --(Utf8Char @char) => @char - 1;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Utf8Char operator checked --(Utf8Char @char) => checked(@char - 1);


	/// <summary>
	/// Explicitly cast from <see cref="char"/> instance to <see cref="Utf8Char"/> instance.
	/// </summary>
	/// <param name="utf16Char">The <see cref="char"/> instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator Utf8Char(char utf16Char) => new((byte)utf16Char);

	/// <summary>
	/// Explicitly cast from <see cref="char"/> instance to <see cref="Utf8Char"/> instance,
	/// with range check.
	/// </summary>
	/// <param name="utf16Char">The <see cref="char"/> instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator checked Utf8Char(char utf16Char) => new(checked((byte)utf16Char));

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

/// <summary>
/// The internal JSON converter type for <see cref="Utf8Char"/>.
/// </summary>
/// <seealso cref="Utf8Char"/>
file sealed class JsonConverter : JsonConverter<Utf8Char>
{
	/// <inheritdoc/>
	public override Utf8Char Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> (Utf8Char)(reader.GetString()?[0] ?? throw new JsonException());

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, Utf8Char value, JsonSerializerOptions options)
		=> writer.WriteStringValue(value.ToString());
}
