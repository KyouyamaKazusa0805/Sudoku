using Utf8Char = System.Byte;

namespace System;

/// <summary>
/// Represents text as a sequence of UTF-8 code units.
/// </summary>
public readonly struct Utf8String :
	IAdditionOperators<Utf8String, Utf8String, Utf8String>,
	IComparable<Utf8String>,
	IComparisonOperators<Utf8String, Utf8String>,
	IDefaultable<Utf8String>,
	IEnumerable<Utf8Char>,
	IEqualityOperators<Utf8String, Utf8String>,
	IEquatable<Utf8String>,
	IReadOnlyCollection<Utf8Char>,
	IReadOnlyList<Utf8Char>
{
	/// <summary>
	/// Indicates the empty <see cref="Utf8String"/> instance.
	/// </summary>
	public static readonly Utf8String Empty = new(Array.Empty<Utf8Char>());


	/// <summary>
	/// Indicates the inner value.
	/// </summary>
	private readonly Utf8Char[] _value;


	/// <summary>
	/// Initializes a <see cref="Utf8String"/> instance via the specified array of <see cref="Utf8Char"/>s
	/// as the underlying array.
	/// </summary>
	/// <param name="underlyingArray">The underlying array.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Utf8String(Utf8Char[] underlyingArray) => _value = (Utf8Char[])underlyingArray.Clone();

	/// <summary>
	/// Initializes a <see cref="Utf8String"/> instance via the specified UTF-8 character and the specified
	/// times of the appearance.
	/// </summary>
	/// <param name="c">The character.</param>
	/// <param name="count">The times of the appearance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Utf8String(Utf8Char c, int count)
	{
		_value = new Utf8Char[count];
		Array.Fill(_value, c);
	}


	/// <inheritdoc/>
	public int Length
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _value.Length;
	}

	/// <summary>
	/// Indicates the underlying array.
	/// </summary>
	public ReadOnlySpan<Utf8Char> UnderlyingArray
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _value;
	}

	/// <inheritdoc/>
	bool IDefaultable<Utf8String>.IsDefault
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => CompareTo(Empty) == 0;
	}

	/// <inheritdoc/>
	int IReadOnlyCollection<Utf8Char>.Count
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Length;
	}

	/// <inheritdoc/>
	static Utf8String IDefaultable<Utf8String>.Default
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Empty;
	}


	/// <summary>
	/// Gets the reference of a character at the specified index in the current string.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <returns>The reference of the character.</returns>
	public ref Utf8Char this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ref _value[index];
	}

	/// <inheritdoc/>
	Utf8Char IReadOnlyList<Utf8Char>.this[int index] => _value[index];



	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] object? obj) => obj is Utf8String comparer && Equals(comparer);

	/// <inheritdoc/>
	public unsafe bool Equals(Utf8String other)
	{
		int length = _value.Length;
		if (length != other.Length)
		{
			return false;
		}

		if (_value == other._value)
		{
			return true;
		}

		fixed (Utf8Char* ap = _value, bp = other._value)
		{
			Utf8Char* a = ap, b = bp;
			while (length >= 4)
			{
				if (*(int*)a != *(int*)b)
				{
					return false;
				}

				a += 4;
				b += 4;
				length -= 4;
			}

			if (length >= 2)
			{
				if (*(short*)a != *(short*)b)
				{
					return false;
				}

				a += 2;
				b += 2;
				length -= 2;
			}

			return length <= 0 || *a == *b;
		}
	}

	/// <summary>
	/// Determines whether the current string contains the specified UTF-8 character.
	/// </summary>
	/// <param name="c">The character.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Contains(Utf8Char c) => IndexOf(c) != -1;

	/// <summary>
	/// Gets the pinnable reference of the current string, positioned at the zero index.
	/// </summary>
	/// <returns>The reference of the first character in this string.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ref Utf8Char GetPinnableReference() => ref MemoryMarshal.GetArrayDataReference(_value);

	/// <inheritdoc/>
	public override unsafe int GetHashCode()
	{
		int length = _value.Length;
		uint hash = (uint)length;
		fixed (Utf8Char* ap = _value)
		{
			Utf8Char* a = ap;

			while (length >= 4)
			{
				hash = (hash + RotateLeft(hash, 5)) ^ *(uint*)a;
				a += 4;
				length -= 4;
			}
			if (length >= 2)
			{
				hash = (hash + RotateLeft(hash, 5)) ^ *(ushort*)a;
				a += 2;
				length -= 2;
			}
			if (length > 0)
			{
				hash = (hash + RotateLeft(hash, 5)) ^ *a;
			}

			hash += RotateLeft(hash, 7);
			hash += RotateLeft(hash, 15);

			return (int)hash;
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(Utf8String other) => Compare(this, other);

	/// <summary>
	/// Reports the zero-based index of the first occurrence of the specified UTF-8 character in this string.
	/// </summary>
	/// <param name="c">The character.</param>
	/// <returns>
	/// The zero-based index position of <paramref name="c"/> if that character is found, or -1 if it is not.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int IndexOf(Utf8Char c) => Array.IndexOf(_value, c);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => Encoding.UTF8.GetString(_value);

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public OneDimensionalArrayRefEnumerator<Utf8Char> GetEnumerator() => _value.EnumerateRef();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	int IComparable.CompareTo([NotNullWhen(true)] object? obj) =>
		obj is Utf8String comparer
			? CompareTo(comparer)
			: throw new ArgumentException($"The target value must be of type '{nameof(Utf8String)}'.");

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator() => _value.GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator<Utf8Char> IEnumerable<Utf8Char>.GetEnumerator() => ((IEnumerable<Utf8Char>)_value).GetEnumerator();


	/// <summary>
	/// Compares two values.
	/// </summary>
	/// <param name="strA">The first string to be compared.</param>
	/// <param name="strB">The second string to be compared.</param>
	/// <returns>An <see cref="int"/> value indicating which one is greater.</returns>
	private static unsafe int Compare(Utf8String strA, Utf8String strB)
	{
		int length = Min(strA._value.Length, strB._value.Length);
		fixed (Utf8Char* ap = strA._value, bp = strB._value)
		{
			for (Utf8Char* a = ap, b = bp; length > 0; a++, b++, length--)
			{
				if (*a != *b)
				{
					return *a - *b;
				}
			}

			// At this point, we have compared all the characters in at least one string.
			// The longer string will be larger.
			// We could optimize and compare lengths before iterating strings, but we want
			// Foo and Foo1 to be sorted adjacent to eachother.
			return strA._value.Length - strB._value.Length;
		}
	}


	/// <inheritdoc cref="string.operator ==(string, string)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Utf8String left, Utf8String right) => left.Equals(right);

	/// <inheritdoc cref="string.operator !=(string, string)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Utf8String left, Utf8String right) => !(left == right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator >(Utf8String left, Utf8String right) => left.CompareTo(right) > 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator >=(Utf8String left, Utf8String right) => left.CompareTo(right) >= 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator <(Utf8String left, Utf8String right) => left.CompareTo(right) < 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator <=(Utf8String left, Utf8String right) => left.CompareTo(right) <= 0;

	/// <summary>
	/// Catenate two <see cref="Utf8String"/> instances.
	/// </summary>
	/// <param name="left">The left-side instance to be catenated.</param>
	/// <param name="right">The right-side instance to be catenated.</param>
	/// <returns>The final string.</returns>
	public static Utf8String operator +(Utf8String left, Utf8String right)
	{
		Unsafe.SkipInit(out Utf8Char[] targetBuffer);
		try
		{
			int totalLength = left._value.Length + right._value.Length;
			targetBuffer = ArrayPool<Utf8Char>.Shared.Rent(totalLength);
			Unsafe.CopyBlock(ref targetBuffer[0], ref left._value[0], (uint)(sizeof(Utf8Char) * left._value.Length));
			Unsafe.CopyBlock(ref targetBuffer[left._value.Length], ref right._value[0], (uint)(sizeof(Utf8Char) * right._value.Length));

			return targetBuffer[..totalLength];
		}
		finally
		{
			ArrayPool<Utf8Char>.Shared.Return(targetBuffer);
		}
	}


	/// <summary>
	/// Explicitly cast from <see cref="Utf8String"/> to <see cref="string"/>.
	/// </summary>
	/// <param name="s">The string.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator string(Utf8String s) => Encoding.UTF8.GetString(s._value);

	/// <summary>
	/// Explicitly cast from <see cref="Utf8String"/> to <see cref="Utf8Char"/>[].
	/// </summary>
	/// <param name="s">The string.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator Utf8Char[](Utf8String s) => (Utf8Char[])s._value.Clone();

	/// <summary>
	/// Implicitly cast from <see cref="Utf8Char"/>[] to <see cref="Utf8String"/>.
	/// </summary>
	/// <param name="underlyingArray">The underlying array.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Utf8String(Utf8Char[] underlyingArray) => new(underlyingArray);
}
