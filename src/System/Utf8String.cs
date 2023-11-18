// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// https://github.com/dotnet/runtime/blob/4aadfea70082ae23e6c54a449268341e9429434e/src/libraries/System.Utf8String.Experimental/src/System/Utf8String.Portable.cs

using System.Buffers;
using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.SourceGeneration;
using System.Text;
using static System.Numerics.BitOperations;

namespace System;

/// <summary>
/// Represents text as a sequence of UTF-8 code units.
/// </summary>
[Equals]
[EqualityOperators]
[ComparisonOperators]
public readonly unsafe partial struct Utf8String :
	IAdditionOperators<Utf8String, Utf8String, Utf8String>,
	IComparable<Utf8String>,
	IComparisonOperators<Utf8String, Utf8String, bool>,
	IEquatable<Utf8String>,
	IReadOnlyList<Utf8Char>
{
	/// <summary>
	/// Indicates the default instance.
	/// </summary>
	public static readonly Utf8String Empty = new((Utf8Char[])[]);


	/// <summary>
	/// Indicates the inner value.
	/// </summary>
	private readonly Utf8Char[] _value;


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

	/// <summary>
	/// Initializes a new instance of the <see cref="Utf8String"/> struct to the value
	/// indicated by a specified pointer to an array of UTF-8 characters.
	/// </summary>
	/// <param name="value">A pointer to a <see langword="null"/>-terminated array of UTF-8 characters.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Utf8String(Utf8Char* value)
	{
		var length = StringLengthOf(value);
		_value = new Utf8Char[length];
		Unsafe.CopyBlock(
			ref Unsafe.As<Utf8Char, byte>(ref _value[0]),
			in Unsafe.As<Utf8Char, byte>(ref value[0]),
			(uint)(sizeof(Utf8Char) * length)
		);
	}

	/// <summary>
	/// Initializes a <see cref="Utf8String"/> instance via the specified array of <see cref="Utf8Char"/>s
	/// as the underlying array.
	/// </summary>
	/// <param name="underlyingArray">The underlying array.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Utf8String(Utf8Char[] underlyingArray) => _value = (Utf8Char[])underlyingArray.Clone();

	/// <summary>
	/// Initializes a <see cref="Utf8String"/> instance via the specified array of <see cref="byte"/>s
	/// as the underlying values.
	/// </summary>
	/// <param name="array">The array of <see cref="byte"/>s.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Utf8String(byte[] array)
	{
		_value = new Utf8Char[array.Length];
		Unsafe.CopyBlock(ref Ref.AsByteRef(ref _value[0]), in array[0], (uint)(sizeof(byte) * array.Length));
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
	int IReadOnlyCollection<Utf8Char>.Count
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Length;
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
	public bool Equals(Utf8String other)
	{
		var length = _value.Length;
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
	/// Determines whether the current string contains the specified UTF-8 string.
	/// </summary>
	/// <param name="s">The string.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Contains(Utf8String s) => IndexOf(s) != -1;

	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='custom-fixed']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ref Utf8Char GetPinnableReference() => ref MemoryMarshal.GetArrayDataReference(_value);

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		var length = _value.Length;
		var hash = (uint)length;
		fixed (Utf8Char* ap = _value)
		{
			var a = ap;

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
				hash = (hash + RotateLeft(hash, 5)) ^ (byte)*a;
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

	/// <summary>
	/// Reports the zero-based index of the first occurrence of the specified UTF-8 string in this string.
	/// </summary>
	/// <param name="s">The character.</param>
	/// <returns>
	/// The zero-based index position of <paramref name="s"/> if that character is found, or -1 if it is not.
	/// </returns>
	public int IndexOf(Utf8String s)
	{
		var next = getNext(s);
		var (i, j) = (0, 0);

		while (i < Length && j < s.Length)
		{
			if (j == -1 || this[i] == s[j])
			{
				i++;
				j++;
			}
			else
			{
				j = next[j];
			}
		}
		return j == s.Length ? i - j : -1;


		static int[] getNext(Utf8String s)
		{
			var (i, j) = (0, -1);
			var next = new int[s.Length];
			next[0] = -1;

			while (i < next.Length - 1)
			{
				if (j == -1 || s[i] == s[j])
				{
					next[++i] = ++j;
				}
				else
				{
					j = next[j];
				}
			}

			return next;
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
	{
		var array = new byte[_value.Length];
		Unsafe.CopyBlock(ref array[0], in Ref.AsReadOnlyByteRef(in _value[0]), (uint)(sizeof(byte) * _value.Length));

		return Encoding.UTF8.GetString(array);
	}

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Span<Utf8Char>.Enumerator GetEnumerator() => _value.AsSpan().GetEnumerator();

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
	private static int Compare(Utf8String strA, Utf8String strB)
	{
		var length = Math.Min(strA._value.Length, strB._value.Length);
		fixed (Utf8Char* ap = strA._value, bp = strB._value)
		{
			for (Utf8Char* a = ap, b = bp; length > 0; a++, b++, length--)
			{
				if (*a != *b)
				{
					return (byte)*a - (byte)*b;
				}
			}

			// At this point, we have compared all the characters in at least one string.
			// The longer string will be larger.
			// We could optimize and compare lengths before iterating strings, but we want
			// Foo and Foo1 to be sorted adjacent to each other.
			return strA._value.Length - strB._value.Length;
		}
	}


	/// <summary>
	/// Concatenate two <see cref="Utf8String"/> instances.
	/// </summary>
	/// <param name="left">The left-side instance to be concatenated.</param>
	/// <param name="right">The right-side instance to be concatenated.</param>
	/// <returns>The final string.</returns>
	public static Utf8String operator +(Utf8String left, Utf8String right)
	{
		Unsafe.SkipInit(out Utf8Char[] targetBuffer);

		try
		{
			var totalLength = left._value.Length + right._value.Length;
			targetBuffer = ArrayPool<Utf8Char>.Shared.Rent(totalLength);
			Unsafe.CopyBlock(
				ref Unsafe.As<Utf8Char, byte>(ref targetBuffer[0]),
				in Unsafe.As<Utf8Char, byte>(ref left._value[0]),
				(uint)(sizeof(byte) * left._value.Length)
			);
			Unsafe.CopyBlock(
				ref Unsafe.As<Utf8Char, byte>(ref targetBuffer[left._value.Length]),
				in Unsafe.As<Utf8Char, byte>(ref right._value[0]),
				(uint)(sizeof(byte) * right._value.Length)
			);

			return targetBuffer[..totalLength];
		}
		finally
		{
			ArrayPool<Utf8Char>.Shared.Return(targetBuffer);
		}
	}

	/// <summary>
	/// Get the length of the specified string which is represented by a <see cref="Utf8Char"/>*.
	/// </summary>
	/// <param name="ptr">The pointer.</param>
	/// <returns>The total length.</returns>
	/// <exception cref="ArgumentNullException">
	/// Throws when the argument <paramref name="ptr"/> is <see langword="null"/>.
	/// </exception>
	/// <remarks>
	/// In C#, this function is unsafe because the implementation of
	/// <see cref="Utf8String"/> types between C and C# is totally different.
	/// In C, <see cref="Utf8String"/> is like a <see cref="Utf8Char"/>* or a
	/// <see cref="Utf8Char"/>[], they ends with the terminator symbol <c>'\0'</c>.
	/// However, C# not.
	/// </remarks>
	private static unsafe int StringLengthOf(Utf8Char* ptr)
	{
		ArgumentNullException.ThrowIfNull(ptr);

		var result = 0;
		for (var p = ptr; *p != (Utf8Char)'\0'; p++)
		{
			result++;
		}

		return result;
	}


	/// <summary>
	/// Explicitly cast from <see cref="Utf8String"/> to <see cref="Utf8Char"/>[].
	/// </summary>
	/// <param name="s">The string.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator Utf8Char[](Utf8String s) => (Utf8Char[])s._value.Clone();

	/// <summary>
	/// Explicitly cast from <see cref="string"/> to <see cref="Utf8String"/> instance.
	/// </summary>
	/// <param name="s">The string.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator Utf8String(string s) => new(Encoding.Default.GetBytes(s));

	/// <summary>
	/// Explicitly cast from <see cref="string"/> to <see cref="Utf8String"/> instance,
	/// with character range check.
	/// </summary>
	/// <param name="s">The string.</param>
	/// <exception cref="ArithmeticException">
	/// Throws when a certain character in the sequence cannot convert to a UTF-8 formatted character.
	/// </exception>
	public static explicit operator checked Utf8String(string s)
	{
		try
		{
			return new(Encoding.Default.GetBytes(s));
		}
		catch (EncoderFallbackException ex)
		when (ex is { Index: var index, CharUnknown: var @char, CharUnknownHigh: var high, CharUnknownLow: var low })
		{
			throw new ArithmeticException(
				$"Cannot convert the value at {nameof(index)} {index} (character '{@char}', range '{high}' to '{low}').",
				ex
			);
		}
	}

	/// <summary>
	/// Implicitly cast from <see cref="Utf8String"/> to <see cref="string"/>.
	/// </summary>
	/// <param name="s">The string.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator string(Utf8String s) => s.ToString();

	/// <summary>
	/// Implicitly cast from <see cref="Utf8Char"/>[]? to <see cref="Utf8String"/>.
	/// </summary>
	/// <param name="underlyingArray">The underlying array.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Utf8String(Utf8Char[]? underlyingArray) => new(underlyingArray ?? []);

	/// <summary>
	/// Implicitly cast from <see cref="byte"/>[] to <see cref="Utf8String"/>.
	/// </summary>
	/// <param name="underlyingArray">The underlying array.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Utf8String(byte[] underlyingArray) => (Utf8String)Encoding.UTF8.GetString(underlyingArray);

	/// <summary>
	/// Implicitly cast from <see cref="ReadOnlySpan{T}"/> of <see cref="byte"/> to <see cref="Utf8String"/>.
	/// </summary>
	/// <param name="underlyingArray">The underlying array.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Utf8String(scoped ReadOnlySpan<byte> underlyingArray) => (Utf8String)Encoding.UTF8.GetString(underlyingArray);
}
