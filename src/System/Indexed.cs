namespace System;

/// <summary>
/// Represents an object that is stored in a sequential memory, with an index to visit it.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
/// <param name="index">Indicates the index of the value.</param>
/// <param name="reference">Indicates the reference to the object.</param>
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.AllEqualityComparisonOperators, IsLargeStructure = true)]
public readonly unsafe ref partial struct Indexed<T>(
	[PrimaryConstructorParameter] int index,
	[PrimaryConstructorParameter(MemberKinds.Field, Accessibility = "public", NamingRule = ">@")] ref readonly T reference
) : IComparable<Indexed<T>>, IEquatable<Indexed<T>>, IFormattable
{
	/// <summary>
	/// Indicates the memory size of the element.
	/// </summary>
	public int Size => sizeof(T);

	/// <summary>
	/// Indicates the previous object.
	/// </summary>
	public Indexed<T> Previous => this - 1;

	/// <summary>
	/// Indicates the next object.
	/// </summary>
	public Indexed<T> Next => this + 1;

	/// <summary>
	/// Indicates an <see cref="Indexed{T}"/> object that matches the first element to the sequential memory.
	/// </summary>
	public Indexed<T> Aligned => this - Index;

	/// <summary>
	/// Indicates the reference to the previous object.
	/// </summary>
	public ref readonly T PreviousRef => ref Pointer[-1];

	/// <summary>
	/// Indicates the reference to the next object.
	/// </summary>
	public ref readonly T NextRef => ref Pointer[1];

	/// <summary>
	/// Indicates the backing pointer.
	/// </summary>
	private T* Pointer => (T*)Unsafe.AsPointer(ref Unsafe.AsRef(in Reference));


	/// <summary>
	/// Gets the element at the specified index, with start position aligned with the current instance.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <returns>The reference to the element at the specified index.</returns>
	public ref readonly T this[int index] => ref this[index, IndexingMode.Current];

	/// <summary>
	/// Gets the element at the specified index, with the specified start position.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <param name="mode">The mode to be checked.</param>
	/// <returns>The reference to the element at the specified index.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument <paramref name="mode"/> is not defined.</exception>
	public ref readonly T this[int index, IndexingMode mode]
		=> ref Unsafe.Add(
			ref Unsafe.AsRef(in Reference),
			mode switch
			{
				IndexingMode.Current => index,
				IndexingMode.MemoryStart => -Index + index,
				_ => throw new ArgumentOutOfRangeException(nameof(mode))
			}
		);


	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(ref readonly Indexed<T> other) => Pointer == other.Pointer;

	/// <summary>
	/// Compares the pointer with the value with the other one, determining which one holds a greater pointer value
	/// in decimal representation.
	/// </summary>
	/// <param name="other">The other <see cref="Indexed{T}"/> instance to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating whether the current object is greater.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool ReferenceGreaterThan(ref readonly Indexed<T> other) => Pointer > other.Pointer;

	/// <summary>
	/// Compares the pointer with the value with the other one, determining which one holds a less pointer value
	/// in decimal representation.
	/// </summary>
	/// <param name="other">The other <see cref="Indexed{T}"/> instance to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating whether the current object is less.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool ReferenceLessThan(ref readonly Indexed<T> other) => Pointer < other.Pointer;

	/// <inheritdoc cref="IComparable{T}.CompareTo(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(ref readonly Indexed<T> other) => ((nint)Pointer).CompareTo((nint)other.Pointer);

	/// <inheritdoc/>
	public override int GetHashCode() => (int)Pointer;

	/// <inheritdoc cref="object.ToString"/>
	public override string ToString() => ToString(null);

	/// <summary>
	/// Returns a string that represents the current object, with the specified format string.
	/// </summary>
	/// <param name="format">The format.</param>
	/// <returns>The string.</returns>
	/// <remarks>
	/// The format string is shown as follows
	/// (Suppose the value is 42, at the first element in the sequential memory, with pointer value <c>0xABC123</c>):
	/// <list type="table">
	/// <listheader>
	/// <term>Format string</term>
	/// <description>Meaning</description>
	/// </listheader>
	/// <item>
	/// <term><see langword="null"/>, <c>r</c> or <c>R</c></term>
	/// <description>Detail information (e.g. <c><![CDATA[42 (@0, 0x00ABC123)]]></c>)</description>
	/// </item>
	/// <item>
	/// <term><c>i</c> or <c>I</c></term>
	/// <description>Index value (e.g. <c><![CDATA[0]]></c>)</description>
	/// </item>
	/// <item>
	/// <term><c>v</c> or <c>V</c></term>
	/// <description>Value (e.g. <c><![CDATA[42]]></c>)</description>
	/// </item>
	/// <item>
	/// <term><c>p</c> or <c>P</c> or <c>p10</c> or <c>P10</c></term>
	/// <description>Pointer value (e.g. <c><![CDATA[11256099]]></c>)</description>
	/// </item>
	/// <item>
	/// <term><c>b</c> or <c>B</c> or <c>p2</c> or <c>P2</c></term>
	/// <description>Pointer value (e.g. <c><![CDATA[0b00000000101010111100000100100011]]></c>)</description>
	/// </item>
	/// <item>
	/// <term><c>o</c> or <c>O</c> or <c>p8</c> or <c>P8</c></term>
	/// <description>Pointer value (e.g. <c><![CDATA[0o00052740443]]></c>)</description>
	/// </item>
	/// <item>
	/// <term><c>x</c> or <c>X</c> or <c>p16</c> or <c>P16</c></term>
	/// <description>Pointer value (e.g. <c><![CDATA[0x00ABC123]]></c>)</description>
	/// </item>
	/// </list>
	/// </remarks>
	/// <exception cref="ArgumentException">Throws when the format is invalid.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(string? format)
		=> format switch
		{
			null or "r" or "R" => $"{Reference} (@{Index}, 0x{ToString("p16")})",
			"i" or "I" => Index.ToString(),
			"v" or "V" => Reference?.ToString() ?? "<null>",
			"iv" or "IV" => $"{ToString("i")}{ToString("v")}",
			"p" or "P" or "p10" or "P10" => ((nint)Pointer).ToString(),
			"b" or "B" or "p2" or "P2" => $"0b{Convert.ToString((nint)Pointer, 2).PadLeft(32, '0')}",
			"o" or "O" or "p8" or "P8" => $"0o{Convert.ToString((nint)Pointer, 8).PadLeft(11, '0')}",
			"x" or "X" or "p16" or "P16" => $"0x{Convert.ToString((nint)Pointer, 16).PadLeft(8, '0')}",
			_ => throw new ArgumentException(string.Empty, nameof(format))
		};

	/// <summary>
	/// Converts the current instance into a <see cref="ReadOnlySpan{T}"/>.
	/// </summary>
	/// <param name="length">The length.</param>
	/// <param name="mode">The mode.</param>
	/// <returns>A <see cref="ReadOnlySpan{T}"/> instance.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument <paramref name="mode"/> is not defined.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<T> ToSpan(int length, IndexingMode mode)
		=> new(
			mode switch
			{
				IndexingMode.MemoryStart => Pointer - Index,
				IndexingMode.Current => Pointer,
				_ => throw new ArgumentOutOfRangeException(nameof(mode))
			},
			length
		);

	/// <summary>
	/// Creates an <see cref="Enumerator"/> instance that can iterate on each element.
	/// </summary>
	/// <param name="length">The length of the whole sequence.</param>
	/// <returns>An <see cref="Enumerator"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Enumerator Enumerate(int length) => new(this, length);

	/// <summary>
	/// Creates an <see cref="EnumeratorReturningRef"/> instance that can iterate on each element with <see langword="ref"/>.
	/// </summary>
	/// <param name="length">The length of the whole sequence.</param>
	/// <returns>An <see cref="EnumeratorReturningRef"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public EnumeratorReturningRef EnumerateRef(int length) => new(this, length);

	/// <inheritdoc/>
	bool IEquatable<Indexed<T>>.Equals(Indexed<T> other) => Pointer == other.Pointer;

	/// <inheritdoc/>
	int IComparable<Indexed<T>>.CompareTo(Indexed<T> other) => CompareTo(in other);

	/// <inheritdoc/>
	string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString(format);


	/// <summary>
	/// Indicates the previous <see cref="Indexed{T}"/> object.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>The previous <see cref="Indexed{T}"/> object.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Indexed<T> operator --(scoped in Indexed<T> value) => value.Previous;

	/// <summary>
	/// Indicates the next <see cref="Indexed{T}"/> object.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>The next <see cref="Indexed{T}"/> object.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Indexed<T> operator ++(scoped in Indexed<T> value) => value.Next;

	/// <summary>
	/// Advances the pointer forwardly to the specified element.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="offset">The offset.</param>
	/// <returns>The new <see cref="Indexed{T}"/> object.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Indexed<T> operator +(scoped in Indexed<T> value, int offset)
		=> new(value.Index + offset, in value.Pointer[offset]);

	/// <summary>
	/// Advances the pointer backwardly to the specified element.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="offset">The offset.</param>
	/// <returns>The new <see cref="Indexed{T}"/> object.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Indexed<T> operator -(scoped in Indexed<T> value, int offset)
		=> new(value.Index - offset, in value.Pointer[-offset]);
}
