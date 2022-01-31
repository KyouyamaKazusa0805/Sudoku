#define DECREASE_INITIALIZATION_MEMORY_ALLOCATION
#define DISCARD_INTERPOLATION_INFO
#define USE_NEWER_CONSTANT_VALUES

using System.Buffers;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Sudoku.Diagnostics.CodeAnalysis;
using static System.Math;
using static System.Numerics.BitOperations;

namespace System.Text;

/// <summary>
/// <para>
/// Provides a handler used on two cases:
/// <list type="bullet">
/// <item>By the language compiler to process interpolated strings into <see cref="string"/> instances.</item>
/// <item>
/// As a <see cref="string"/> builder to append and catenate multiple <see cref="string"/>s to a whole one.
/// </item>
/// </list>
/// </para>
/// <para>
/// Different with <see cref="DefaultInterpolatedStringHandler"/>, this type won't contain
/// any formatters to construct any custom format operations, i.e. using <see cref="IFormatProvider"/>.
/// </para>
/// </summary>
/// <remarks>
/// You can use this type like this:
/// <code><![CDATA[
/// var sb = new StringHandler(initialCapacity: 100);
/// 
/// sb.Append("Hello");
/// sb.Append(',');
/// sb.Append("World");
/// sb.Append('!');
/// 
/// Console.WriteLine(sb.ToStringAndClear());
/// ]]></code>
/// </remarks>
/// <seealso cref="DefaultInterpolatedStringHandler"/>
/// <seealso cref="IFormatProvider"/>
[InterpolatedStringHandler]
public unsafe ref partial struct StringHandler
{
#if USE_NEWER_CONSTANT_VALUES
	/// <summary>
	/// Expected average length of formatted data used for an individual interpolation expression result.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This is inherited from <see cref="string.Format(string, object?[])"/>,
	/// and could be changed based on further data.
	/// </para>
	/// <para>
	/// <see cref="string.Format(string, object?[])"/> actually uses <c>format.Length + args.Length * 8</c>,
	/// but <c>format.Length</c> includes the format items themselves, e.g. <c>"{0}"</c>,
	/// and since it's rare to have double-digit numbers of items, we bump the 8 up to 11 to account
	/// for the three extra characters in <c>"{d}"</c>, since the compiler-provided base length won't include
	/// the equivalent character count.
	/// </para>
	/// <para><i>The original value implemented by .NET foundation is 11, but I change it to 8.</i></para>
	/// </remarks>
	/// <seealso cref="string.Format(string, object?[])"/>
#else
	/// <summary>
	/// Expected average length of formatted data used for an individual interpolation expression result.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This is inherited from <see cref="string.Format(string, object?[])"/>,
	/// and could be changed based on further data.
	/// </para>
	/// <para>
	/// <see cref="string.Format(string, object?[])"/> actually uses <c>format.Length + args.Length * 8</c>,
	/// but <c>format.Length</c> includes the format items themselves, e.g. <c>"{0}"</c>,
	/// and since it's rare to have double-digit numbers of items, we bump the 8 up to 11 to account
	/// for the three extra characters in <c>"{d}"</c>, since the compiler-provided base length won't include
	/// the equivalent character count.
	/// </para>
	/// </remarks>
	/// <seealso cref="string.Format(string, object?[])"/>
#endif
	private const int GuessedLengthPerHole =
#if USE_NEWER_CONSTANT_VALUES
		8;
#else
		11;
#endif

	/// <summary>
	/// Minimum size array to rent from the pool.
	/// </summary>
	/// <remarks>
	/// Same as stack-allocation size used today by <see cref="string.Format(string, object?[])"/>.
	/// </remarks>
	/// <seealso cref="string.Format(string, object?[])"/>
	private const int MinimumArrayPoolLength = 256;


#if !DISCARD_INTERPOLATION_INFO
	/// <summary>
	/// The number of constant characters outside of interpolation expressions in the interpolated string.
	/// </summary>
	private readonly int _literalLength = 0;

	/// <summary>
	/// The number of interpolation expressions in the interpolated string.
	/// </summary>
	private readonly int _holeCount = 0;
#endif

	/// <summary>
	/// Array rented from the array pool and used to back <see cref="_chars"/>.
	/// </summary>
	private char[]? _arrayToReturnToPool;

	/// <summary>
	/// The span to write into.
	/// </summary>
	private Span<char> _chars;


	/// <summary>
	/// Creates a handler used to translate an interpolated string into a <see cref="string"/>,
	/// with the default-sized buffer 256.
	/// </summary>
	/// <remarks>
	/// <include
	///     file='../../global-doc-comments.xml'
	///     path='g/csharp9/feature[@name="parameterless-struct-constructor"]/target[@name="constructor"]' />
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public StringHandler() : this(MinimumArrayPoolLength)
	{
	}

	/// <summary>
	/// Creates a handler used to translate an interpolated string into a <see cref="string"/>.
	/// </summary>
	/// <param name="initialCapacity">
	/// The number of constant characters as the default memory to initialize.
	/// </param>
	public StringHandler(int initialCapacity = MinimumArrayPoolLength) =>
		_chars = _arrayToReturnToPool = ArrayPool<char>.Shared.Rent(initialCapacity);

	/// <summary>
	/// Creates a handler used to translate an interpolated string into a <see cref="string"/>.
	/// </summary>
	/// <param name="literalLength">
	/// The number of constant characters outside of interpolation expressions in the interpolated string.
	/// </param>
	/// <param name="holeCount">The number of interpolation expressions in the interpolated string.</param>
	/// <remarks>
	/// This is intended to be called only by compiler-generated code.
	/// Arguments aren't validated as they'd otherwise be for members intended to be used directly.
	/// </remarks>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public StringHandler(int literalLength, int holeCount) =>
		_chars = _arrayToReturnToPool = ArrayPool<char>.Shared.Rent(
#if DECREASE_INITIALIZATION_MEMORY_ALLOCATION
			(int)RoundUpToPowerOf2((uint)(literalLength + holeCount * GuessedLengthPerHole))
#else
			Max(MinimumArrayPoolLength, literalLength + holeCount * GuessedLengthPerHole)
#endif
		);

#if DISCARD_INTERPOLATION_INFO
	/// <summary>
	/// Creates a handler used to translate an interpolated string into a <see cref="string"/>.
	/// </summary>
	/// <param name="_"/>
	/// <param name="__"/>
	/// <param name="initialBuffer">
	/// A buffer temporarily transferred to the handler for use as part of its formatting.
	/// Contents may be overwritten.
	/// </param>
	/// <remarks>
	/// This is intended to be called only by compiler-generated code.
	/// Arguments are not validated as they'd otherwise be for members intended to be used directly.
	/// </remarks>
#else
	/// <summary>
	/// Creates a handler used to translate an interpolated string into a <see cref="string"/>.
	/// </summary>
	/// <param name="literalLength">
	/// The number of constant characters outside of interpolation expressions in the interpolated string.
	/// </param>
	/// <param name="holeCount">The number of interpolation expressions in the interpolated string.</param>
	/// <param name="initialBuffer">
	/// A buffer temporarily transferred to the handler for use as part of its formatting.
	/// Contents may be overwritten.
	/// </param>
	/// <remarks>
	/// This is intended to be called only by compiler-generated code.
	/// Arguments are not validated as they'd otherwise be for members intended to be used directly.
	/// </remarks>
#endif
	[EditorBrowsable(EditorBrowsableState.Never)]
	public StringHandler(
#if DISCARD_INTERPOLATION_INFO
		[IsDiscard] int _,
#else
		int literalLength,
#endif
#if DISCARD_INTERPOLATION_INFO
		[IsDiscard] int __,
#else
		int holeCount,
#endif

		Span<char> initialBuffer
	)
	{
#if !DISCARD_INTERPOLATION_INFO
		_literalLength = literalLength;
		_holeCount = holeCount;
#endif

		_chars = initialBuffer;
		_arrayToReturnToPool = null;
	}

	/// <summary>
	/// Creates a handler used to translate an interpolated string into a <see cref="string"/>
	/// that is initialized by a string value.
	/// </summary>
	/// <param name="initialString">The initialized string.</param>
	public StringHandler(string initialString)
	{
		fixed (char* pChars = _chars, pInitialString = initialString)
		{
			Unsafe.CopyBlock(pChars, pInitialString, (uint)(sizeof(char) * initialString.Length));
		}

		_arrayToReturnToPool = null;
	}


	/// <summary>
	/// Gets a span of the written characters thus far.
	/// </summary>
	private readonly ReadOnlySpan<char> Text => _chars[..Length];

	/// <summary>
	/// Position at which to write the next character.
	/// </summary>
	public int Length { get; private set; } = 0;


	/// <summary>
	/// Gets the reference of a character at the specified index.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <returns>The reference of the character.</returns>
	/// <remarks>
	/// This property returns a <see langword="ref"/> <see cref="char"/>, which
	/// means you can use the return value to re-assign a new value, as the same behavior
	/// as the <see langword="set"/> accessor.
	/// </remarks>
	public readonly ref char this[int index] => ref _chars[index];


	/// <summary>
	/// Copies the current colletion into the specified collection.
	/// </summary>
	/// <param name="handler">The collection.</param>
	public readonly void CopyTo(ref StringHandler handler)
	{
		fixed (char* old = _chars, @new = handler._chars)
		{
			Unsafe.CopyBlock(@new, old, (uint)(sizeof(char) * Length));
		}
	}

	/// <summary>
	/// Determine whether the specified <see cref="StringHandler"/> instance hold a same character set
	/// as the current instance.
	/// </summary>
	/// <param name="other">The instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(in StringHandler other) => Equals(this, other);

	/// <summary>
	/// <para>
	/// Get a pinnable reference to the handler.
	/// The operation does not ensure there is a null char after <see cref="Length"/>.
	/// </para>
	/// <para>
	/// This overload is pattern matched in the C# 7.3+ compiler so you can omit
	/// the explicit method call, and write eg <c>fixed (char* c = handler)</c>.
	/// </para>
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public readonly ref readonly char GetPinnableReference() => ref MemoryMarshal.GetReference(_chars);

	/// <summary>
	/// Get a pinnable reference to the builder.
	/// </summary>
	/// <param name="withTerminate">
	/// Ensures that the builder has a null character after <see cref="Length"/>.
	/// </param>
	/// <seealso cref="Length"/>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public ref readonly char GetPinnableReference(bool withTerminate)
	{
		if (withTerminate)
		{
			EnsureCapacityForAdditionalChars(1);
			_chars[Length] = '\0';
		}

		return ref MemoryMarshal.GetReference(_chars);
	}

	/// <summary>
	/// Gets the built <see cref="string"/>.
	/// </summary>
	/// <returns>The built string.</returns>
	public override readonly string ToString() => new(Text);

	/// <summary>
	/// Gets the enumerator of the current instance in order to use <see langword="foreach"/> loop.
	/// </summary>
	/// <returns>The enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Enumerator GetEnumerator() => new(this);

	/// <summary>
	/// Writes the specified string to the handler.
	/// </summary>
	/// <param name="value">The string to write.</param>
	/// <remarks>
	/// <para>
	/// <see cref="AppendLiteral(string)"/> is expected to always be called by compiler-generated code
	/// with a literal string. By inlining it, the method body is exposed to the constant length of that literal,
	/// allowing the JIT to prune away the irrelevant cases.
	/// This effectively enables multiple implementations of <see cref="AppendLiteral(string)"/>,
	/// special-cased on and optimized for the literal's length.
	/// </para>
	/// <para>
	/// We special-case lengths 1 and 2 because they're very common, e.g.
	/// <list type="number">
	/// <item><c>' '</c>, <c>'.'</c>, <c>'-'</c>, <c>'\t'</c>, etc.</item>
	/// <item><c>", "</c>, <c>"0x"</c>, <c>"=>"</c>, <c>": "</c>, etc.</item>
	/// </list>
	/// but we refrain from adding more because, in the rare case where <see cref="AppendLiteral(string)"/>
	/// is called with a non-literal, there is a lot of code here to be inlined.
	/// </para>
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void AppendLiteral(string? value)
	{
		// TODO: https://github.com/dotnet/runtime/issues/41692#issuecomment-685192193

		// What we really want here is to be able to add a bunch of additional special-cases based on length,
		// e.g. a switch with a case for each length <= 8, not mark the method as AggressiveInlining, and have
		// it inlined when provided with a string literal such that all the other cases evaporate but not inlined
		// if called directly with something that doesn't enable pruning.  Even better, if "literal".TryCopyTo
		// could be unrolled based on the literal, a.k.a. https://github.com/dotnet/runtime/pull/46392, we might
		// be able to remove all special-casing here.

		if (value is null)
		{
			return;
		}

		switch (value.Length)
		{
			case 0:
			{
				return;
			}
			case 1:
			{
				var chars = _chars;
				int pos = Length;
				if ((uint)pos < (uint)chars.Length)
				{
					chars[pos] = value[0];
					Length = pos + 1;
				}
				else
				{
					GrowThenCopyString(value);
				}

				return;
			}
			case 2:
			{
				var chars = _chars;
				int pos = Length;
				if ((uint)pos < chars.Length - 1)
				{
					fixed (char* pFirstChar = value)
					{
						Unsafe.WriteUnaligned(
							ref Unsafe.As<char, byte>(ref Unsafe.Add(ref MemoryMarshal.GetReference(chars), pos)),
							Unsafe.ReadUnaligned<int>(ref Unsafe.As<char, byte>(ref *pFirstChar))
						);
					}

					Length = pos + 2;
				}
				else
				{
					GrowThenCopyString(value);
				}

				return;
			}
			default:
			{
				AppendStringDirect(value);

				break;
			}
		}
	}

	/// <inheritdoc cref="AppendFormatted(object?, int, string?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Append(object? value, int alignment = 0, string? format = null) =>
		AppendFormatted<object?>(value, alignment, format);

	/// <summary>
	/// Append a character at the tail of the collection.
	/// </summary>
	/// <param name="c">The character.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Append(char c)
	{
		int pos = Length;
		if ((uint)pos < (uint)_chars.Length)
		{
			_chars[pos] = c;
			Length = pos + 1;
		}
		else
		{
			Grow();
			Append(c);
		}
	}

	/// <summary>
	/// Append a serial of same characters into the collection.
	/// </summary>
	/// <param name="c">The character.</param>
	/// <param name="count">The number of the character you want to append.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Append(char c, int count)
	{
		if (Length > _chars.Length - count)
		{
			Grow(count);
		}

		var dst = _chars.Slice(Length, count);
		for (int i = 0, length = dst.Length; i < length; i++)
		{
			dst[i] = c;
		}

		Length += count;
	}

	/// <summary>
	/// Append a string that is represented as a <see cref="char"/>*.
	/// </summary>
	/// <param name="value">The string.</param>
	/// <param name="length">The length of the string.</param>
	public void Append(char* value, int length)
	{
		int pos = Length;
		if (pos > _chars.Length - length)
		{
			Grow(length);
		}

		var dst = _chars.Slice(Length, length);
		for (int i = 0, iterationLength = dst.Length; i < iterationLength; i++)
		{
			dst[i] = *value++;
		}

		Length += length;
	}

	/// <inheritdoc cref="AppendFormatted(string?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Append(string value) => AppendFormatted<string>(value);

	/// <inheritdoc cref="AppendFormatted(string?, int, string?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Append(string value, int alignment, string? format = null) =>
		AppendFormatted<string>(value, alignment, format);

	/// <inheritdoc cref="AppendFormatted(ReadOnlySpan{char})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Append(ReadOnlySpan<char> value) => AppendFormatted(value);

	/// <inheritdoc cref="AppendFormatted(ReadOnlySpan{char}, int, string?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Append(ReadOnlySpan<char> value, int alignment, string? format = null) =>
		AppendFormatted(value, alignment, format);

	/// <inheritdoc cref="AppendFormatted(in StringHandler)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Append([InterpolatedStringHandlerArgument] in StringHandler handler) => AppendFormatted(handler);

	/// <summary>
	/// Writes the specified interpolated string with the specified format provider into the handler.
	/// </summary>
	/// <param name="provider">The format provider used.</param>
	/// <param name="handler">The handler that holds the interpolated string processings.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Append(
		IFormatProvider? provider,
		[InterpolatedStringHandlerArgument("provider")] ref DefaultInterpolatedStringHandler handler
	) => AppendFormatted(string.Create(provider, ref handler));

	/// <summary>
	/// Writes the specified interpolated string with the specified format provider into the handler.
	/// </summary>
	/// <param name="provider">The format provider used.</param>
	/// <param name="initialBuffer">The initial buffer used.</param>
	/// <param name="handler">The handler that holds the interpolated string processings.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Append(
		IFormatProvider? provider,
		Span<char> initialBuffer,
		[InterpolatedStringHandlerArgument("provider", "initialBuffer")] ref DefaultInterpolatedStringHandler handler
	) => AppendFormatted(string.Create(provider, initialBuffer, ref handler));

	/// <inheritdoc cref="AppendFormatted{T}(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Append<T>(T value) => AppendFormatted(value);

	/// <inheritdoc cref="AppendFormatted{T}(T, int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Append<T>(T value, int alignment) => AppendFormatted(value, alignment);

	/// <inheritdoc cref="AppendFormatted{T}(T, string?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Append<T>(T value, string? format) => AppendFormatted(value, format);

	/// <inheritdoc cref="AppendFormatted{T}(T, int, string?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Append<T>(T value, int alignment, string? format = null) =>
		AppendFormatted(value, alignment, format);

	/// <summary>
	/// Append a new line string <see cref="Environment.NewLine"/>.
	/// </summary>
	/// <seealso cref="Environment.NewLine"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AppendLine() => AppendFormatted(Environment.NewLine);

	/// <summary>
	/// Append a serial of characters at the tail of the collection.
	/// </summary>
	/// <param name="chars">The serial of characters.</param>
	public void AppendCharacters(IEnumerable<char> chars)
	{
		foreach (char @char in chars)
		{
			Append(@char);
		}
	}

	/// <summary>
	/// Append a serial of strings from a serial of elements.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="list">The list of elements.</param>
	public void AppendRange<T>(IEnumerable<T> list)
	{
		foreach (var element in list)
		{
			AppendFormatted(element);
		}
	}

	/// <summary>
	/// Append a serial of strings converted from a serial of elements.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="list">The list of elements.</param>
	/// <param name="converter">
	/// The converter that allows the instance to convert into the <see cref="string"/> representation,
	/// whose the rule is defined as a method specified as the function pointer as this argument.
	/// </param>
	public void AppendRange<T>(IEnumerable<T> list, delegate*<T, string?> converter)
	{
		foreach (var element in list)
		{
			AppendFormatted(converter(element));
		}
	}

	/// <summary>
	/// Append a serial of strings converted from a serial of elements.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="list">The list of elements.</param>
	/// <param name="converter">
	/// The converter that allows the instance to convert into the <see cref="string"/> representation,
	/// whose the rule is defined as a method specified as the delegate instance as this argument.
	/// </param>
	public void AppendRange<T>(IEnumerable<T> list, Func<T, string?> converter)
	{
		foreach (var element in list)
		{
			AppendFormatted(converter(element));
		}
	}

	/// <summary>
	/// Append a serial of strings from a serial of elements.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="list">The list of elements.</param>
	/// <param name="separator">The separator to append when an element is finished to append.</param>
	public void AppendRangeWithSeparator<T>(IEnumerable<T> list, string separator)
	{
		foreach (var element in list)
		{
			AppendFormatted(element);
			AppendFormatted(separator);
		}

		Length -= separator.Length;
	}

	/// <summary>
	/// Append a serial of strings converted from a serial of elements.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="list">The list of elements.</param>
	/// <param name="converter">
	/// The converter that allows the instance to convert into the <see cref="string"/> representation,
	/// whose the rule is defined as a method specified as the function pointer as this argument.
	/// </param>
	/// <param name="separator">The separator to append when an element is finished to append.</param>
	public void AppendRangeWithSeparator<T>(IEnumerable<T> list, delegate*<T, string?> converter, string separator)
	{
		foreach (var element in list)
		{
			AppendFormatted(converter(element));
			AppendFormatted(separator);
		}

		Length -= separator.Length;
	}

	/// <summary>
	/// Append a serial of strings converted from a serial of elements.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="list">The list of elements.</param>
	/// <param name="converter">
	/// The converter that allows the instance to convert into the <see cref="string"/> representation,
	/// whose the rule is defined as a method specified as the delegate instance as this argument.
	/// </param>
	/// <param name="separator">The separator to append when an element is finished to append.</param>
	public void AppendRangeWithSeparator<T>(IEnumerable<T> list, Func<T, string?> converter, string separator)
	{
		foreach (var element in list)
		{
			AppendFormatted(converter(element));
			AppendFormatted(separator);
		}

		Length -= separator.Length;
	}

	/// <summary>
	/// Append a serial of strings converted from a serial of elements.
	/// </summary>
	/// <typeparam name="TUnmanaged">The type of each element.</typeparam>
	/// <param name="list">The list of elements that is represented as a pointer.</param>
	/// <param name="length">The length of the list.</param>
	/// <param name="converter">
	/// The converter that allows the instance to convert into the <see cref="string"/> representation,
	/// whose the rule is defined as a method specified as the delegate instance as this argument.
	/// </param>
	/// <param name="separator">The separator to append when an element is finished to append.</param>
	public void AppendRangeWithSeparatorUnsafe<TUnmanaged>(
		TUnmanaged* list, int length, delegate*<TUnmanaged, string?> converter, string separator)
		where TUnmanaged : unmanaged
	{
		for (int i = 0; i < length; i++)
		{
			var element = list[i];
			AppendFormatted(converter(element));
			AppendFormatted(separator);
		}

		Length -= separator.Length;
	}

	/// <summary>
	/// Append a serial of strings converted from a serial of elements.
	/// </summary>
	/// <typeparam name="TUnmanaged">The type of each element.</typeparam>
	/// <param name="list">The list of elements that is represented as a pointer.</param>
	/// <param name="length">The length of the list.</param>
	/// <param name="converter">
	/// The converter that allows the instance to convert into the <see cref="string"/> representation,
	/// whose the rule is defined as a method specified as the delegate instance as this argument.
	/// </param>
	/// <param name="separator">The separator to append when an element is finished to append.</param>
	public void AppendRangeWithSeparatorUnsafe<TUnmanaged>(
		TUnmanaged* list, int length, Func<TUnmanaged, string?> converter, string separator)
		where TUnmanaged : unmanaged
	{
		for (int i = 0; i < length; i++)
		{
			var element = list[i];
			AppendFormatted(converter(element));
			AppendFormatted(separator);
		}

		Length -= separator.Length;
	}

	/// <summary>
	/// Append a series of elements into the current collection.
	/// In addition, new line characters will be inserted after each element.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="list">The list.</param>
	public void AppendRangeWithLines<T>(IEnumerable<T?> list)
	{
		foreach (var element in list)
		{
			AppendFormatted(element?.ToString());
			AppendLine();
		}
	}

	/// <summary>
	/// Writes the specified large-object value to the handler.
	/// </summary>
	/// <param name="value">The value to write.</param>
	/// <typeparam name="T">The type of the value to write.</typeparam>
	public void AppendLargeObjectFormatted<T>(in T value)
	{
		switch (value)
		{
			case ISpanFormattable spanFormattable:
			{
				int charsWritten;

				// Constrained call avoiding boxing for value types.
				while (!spanFormattable.TryFormat(_chars[Length..], out charsWritten, null, null))
				{
					Grow();
				}

				Length += charsWritten;

				break;
			}
			case ISimpleFormattable s:
			{
				AppendStringDirect(s.ToString(null));

				break;
			}
			case IFormattable f:
			{
				AppendStringDirect(f.ToString(null, null));

				break;
			}
			case var _ when value?.ToString() is { } s:
			{
				AppendStringDirect(s);

				break;
			}
		}
	}

	/// <summary>
	/// Writes the specified large-object value to the handler.
	/// </summary>
	/// <param name="value">The value to write.</param>
	/// <param name="format">The format string.</param>
	/// <typeparam name="T">The type of the value to write.</typeparam>
	public void AppendLargeObjectFormatted<T>(in T value, string? format)
	{
		switch (value)
		{
			case ISpanFormattable spanFormattable:
			{
				int charsWritten;

				// Constrained call avoiding boxing for value types.
				while (!spanFormattable.TryFormat(_chars[Length..], out charsWritten, format, null))
				{
					Grow();
				}

				Length += charsWritten;

				break;
			}
			case ISimpleFormattable s:
			{
				AppendStringDirect(s.ToString(format));

				break;
			}
			case IFormattable f:
			{
				AppendStringDirect(f.ToString(format, null));

				break;
			}
			case var _ when value?.ToString() is { } s:
			{
				AppendStringDirect(s);

				break;
			}
		}
	}

	/// <summary>
	/// Writes the specified value to the handler.
	/// </summary>
	/// <param name="value">The value to write.</param>
	/// <param name="alignment">
	/// Minimum number of characters that should be written for this value.
	/// If the value is negative, it indicates left-aligned and the required minimum is the absolute value.
	/// </param>
	/// <typeparam name="T">The type of the value to write.</typeparam>
	public void AppendLargeObjectFormatted<T>(in T value, int alignment)
	{
		int startingPos = Length;
		AppendFormatted(value);
		if (alignment != 0)
		{
			AppendOrInsertAlignmentIfNeeded(startingPos, alignment);
		}
	}

	/// <summary>
	/// Writes the specified large-object value to the handler.
	/// </summary>
	/// <param name="value">The value to write.</param>
	/// <param name="format">The format string.</param>
	/// <param name="alignment">
	/// Minimum number of characters that should be written for this value.
	/// If the value is negative, it indicates left-aligned and the required minimum is the absolute value.
	/// </param>
	/// <typeparam name="T">The type of the value to write.</typeparam>
	public void AppendLargeObjectFormatted<T>(T value, int alignment, string? format)
	{
		int startingPos = Length;
		AppendFormatted(value, format);
		if (alignment != 0)
		{
			AppendOrInsertAlignmentIfNeeded(startingPos, alignment);
		}
	}

	/// <summary>
	/// Writes the specified value to the handler.
	/// </summary>
	/// <param name="value">The value to write.</param>
	/// <param name="alignment">
	/// Minimum number of characters that should be written for this value.
	/// If the value is negative, it indicates left-aligned and the required minimum is the absolute value.
	/// </param>
	/// <param name="format">The format string.</param>
	/// <remarks>
	/// This overload is expected to be used rarely, only if either:
	/// <list type="bullet">
	/// <item>
	/// a) something strongly typed as <see cref="object"/> is formatted with both an alignment and a format.
	/// </item>
	/// <item>
	/// b) the compiler is unable to target type to <c>T</c>.
	/// </item>
	/// </list>
	/// It exists purely to help make cases from (b) compile. Just delegate to the <c>T</c>-based implementation.
	/// </remarks>
	public void AppendFormatted(object? value, int alignment = 0, string? format = null) =>
		AppendFormatted<object?>(value, alignment, format);

	/// <summary>
	/// Writes the specified value to the handler.
	/// </summary>
	/// <param name="value">The value to write.</param>
	public void AppendFormatted(string? value)
	{
		// Fast-path for no custom formatter and a non-null string that fits in the current destination buffer.
		if (value is not null && value.TryCopyTo(_chars[Length..]))
		{
			Length += value.Length;
		}
		else
		{
			AppendFormattedSlow(value);
		}
	}

	/// <summary>
	/// Writes the specified value to the handler.
	/// </summary>
	/// <param name="value">The value to write.</param>
	/// <param name="alignment">
	/// Minimum number of characters that should be written for this value.
	/// If the value is negative, it indicates left-aligned and the required minimum is the absolute value.
	/// </param>
	/// <param name="format">The format string.</param>
	/// <remarks>
	/// Format is meaningless for strings and doesn't make sense for someone to specify.
	/// We have the overload simply to disambiguate between <c><![CDATA[ReadOnlySpan<char>]]></c>
	/// and <see cref="object"/>, just in case someone does specify a format,
	/// as <see cref="string"/> is implicitly convertible to both.
	/// Just delegate to the <c>T</c>-based implementation.
	/// </remarks>
	public void AppendFormatted(string? value, int alignment = 0, string? format = null) =>
		AppendFormatted<string?>(value, alignment, format);

	/// <summary>
	/// Writes the specified character span to the handler.
	/// </summary>
	/// <param name="value">The span to write.</param>
	public void AppendFormatted(ReadOnlySpan<char> value)
	{
		// Fast path for when the value fits in the current buffer
		if (value.TryCopyTo(_chars[Length..]))
		{
			Length += value.Length;
		}
		else
		{
			GrowThenCopySpan(value);
		}
	}

	/// <summary>
	/// Writes the specified string of chars to the handler.
	/// </summary>
	/// <param name="value">The span to write.</param>
	/// <param name="alignment">
	/// Minimum number of characters that should be written for this value.
	/// If the value is negative, it indicates left-aligned and the required minimum is the absolute value.
	/// </param>
	/// <param name="format">The format string.</param>
	public void AppendFormatted(ReadOnlySpan<char> value, int alignment = 0, string? format = null)
	{
		(bool leftAlign, alignment) = alignment < 0 ? (true, -alignment) : (false, alignment);

		int paddingRequired = alignment - value.Length;
		if (paddingRequired <= 0)
		{
			// The value is as large or larger than the required amount of padding,
			// so just write the value.
			AppendFormatted(value, format: format);
			return;
		}

		// Write the value along with the appropriate padding.
		EnsureCapacityForAdditionalChars(value.Length + paddingRequired);
		if (leftAlign)
		{
			value.CopyTo(_chars[Length..]);
			Length += value.Length;
			_chars.Slice(Length, paddingRequired).Fill(' ');
			Length += paddingRequired;
		}
		else
		{
			_chars.Slice(Length, paddingRequired).Fill(' ');
			Length += paddingRequired;
			value.CopyTo(_chars[Length..]);
			Length += value.Length;
		}
	}

	/// <summary>
	/// Writes the specified interpolated string into the handler.
	/// </summary>
	/// <param name="handler">The handler that creates the interpolated string as this argument.</param>
	/// <remarks><b>
	/// Don't use <see langword="ref"/> keyword instead of here <see langword="in"/> <paramref name="handler"/>;
	/// otherwise, the compiler error CS8751 (internal compiler erorr) will be raised.
	/// </b></remarks>
	public void AppendFormatted([InterpolatedStringHandlerArgument] in StringHandler handler)
	{
		string result = handler.ToStringAndClear();
		if (result.TryCopyTo(_chars[Length..]))
		{
			Length += result.Length;
		}
		else
		{
			AppendFormattedSlow(result);
		}
	}

	/// <summary>
	/// Writes the specified value to the handler.
	/// </summary>
	/// <param name="value">The value to write.</param>
	/// <typeparam name="T">The type of the value to write.</typeparam>
	public void AppendFormatted<T>(T value)
	{
		switch (value)
		{
			case ISpanFormattable spanFormattable:
			{
				int charsWritten;

				// Constrained call avoiding boxing for value types.
				while (!spanFormattable.TryFormat(_chars[Length..], out charsWritten, null, null))
				{
					Grow();
				}

				Length += charsWritten;

				break;
			}
			case ISimpleFormattable s:
			{
				AppendStringDirect(s.ToString(null));

				break;
			}
			case IFormattable f:
			{
				AppendStringDirect(f.ToString(null, null));

				break;
			}
			case var _ when value?.ToString() is { } s:
			{
				AppendStringDirect(s);

				break;
			}
		}
	}

	/// <summary>
	/// Writes the specified value to the handler.
	/// </summary>
	/// <param name="value">The value to write.</param>
	/// <param name="format">The format string.</param>
	/// <typeparam name="T">The type of the value to write.</typeparam>
	public void AppendFormatted<T>(T value, string? format)
	{
		switch (value)
		{
			case ISpanFormattable spanFormattable:
			{
				int charsWritten;

				// Constrained call avoiding boxing for value types.
				while (!spanFormattable.TryFormat(_chars[Length..], out charsWritten, format, null))
				{
					Grow();
				}

				Length += charsWritten;

				break;
			}
			case ISimpleFormattable s:
			{
				AppendStringDirect(s.ToString(format));

				break;
			}
			case IFormattable f:
			{
				AppendStringDirect(f.ToString(format, null));

				break;
			}
			case var _ when value?.ToString() is { } s:
			{
				AppendStringDirect(s);

				break;
			}
		}
	}

	/// <summary>
	/// Writes the specified value to the handler.
	/// </summary>
	/// <param name="value">The value to write.</param>
	/// <param name="alignment">
	/// Minimum number of characters that should be written for this value.
	/// If the value is negative, it indicates left-aligned and the required minimum is the absolute value.
	/// </param>
	/// <typeparam name="T">The type of the value to write.</typeparam>
	public void AppendFormatted<T>(T value, int alignment)
	{
		int startingPos = Length;
		AppendFormatted(value);
		if (alignment != 0)
		{
			AppendOrInsertAlignmentIfNeeded(startingPos, alignment);
		}
	}

	/// <summary>
	/// Writes the specified value to the handler.
	/// </summary>
	/// <param name="value">The value to write.</param>
	/// <param name="format">The format string.</param>
	/// <param name="alignment">
	/// Minimum number of characters that should be written for this value.
	/// If the value is negative, it indicates left-aligned and the required minimum is the absolute value.
	/// </param>
	/// <typeparam name="T">The type of the value to write.</typeparam>
	public void AppendFormatted<T>(T value, int alignment, string? format)
	{
		int startingPos = Length;
		AppendFormatted(value, format);
		if (alignment != 0)
		{
			AppendOrInsertAlignmentIfNeeded(startingPos, alignment);
		}
	}

	/// <summary>
	/// Reverse the instance. For example, if the handler holds a string <c>"Hello"</c>,
	/// after called this method, the string will be <c>"olleH"</c>.
	/// </summary>
	public void Reverse()
	{
		fixed (char* p = _chars)
		{
			for (int i = 0, iterationLength = Length >> 1; i < iterationLength; i++)
			{
				char c = p[i];
				p[i] = p[Length - 1 - i];
				p[Length - 1 - i] = c;
			}
		}
	}

	/// <summary>
	/// Inserts a new character into the collection at the specified index.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <param name="value">The character you want to insert into the collection.</param>
	/// <param name="count">The number.</param>
	public void Insert(int index, char value, int count)
	{
		if (Length > _chars.Length - count)
		{
			Grow(count);
		}

		_chars[index..Length].CopyTo(_chars[(index + count)..]);
		_chars[index..(index + count)].Fill(value);
		Length += count;
	}

	/// <summary>
	/// Inserts a new string into the collection at the specified index.
	/// </summary>
	/// <param name="index">The index you want to insert.</param>
	/// <param name="s">The string you want to insert.</param>
	public void Insert(int index, string s)
	{
		int count = s.Length;
		if (Length > _chars.Length - count)
		{
			Grow(count);
		}

		_chars[index..Length].CopyTo(_chars[(index + count)..]);
		s.CopyTo(_chars[index..]);
		Length += count;
	}

	/// <summary>
	/// Remove a serial of characters from the specified index, with the specified length.
	/// </summary>
	/// <param name="startIndex">The start index.</param>
	/// <param name="length">The length you want to remove.</param>
	/// <remarks>
	/// This method will be costly (move a lot of elements), so you shouldn't call this method usually.
	/// </remarks>
	public void Remove(int startIndex, int length)
	{
		fixed (char* pThis = _chars)
		{
			int i = startIndex;
			for (char* p = pThis + startIndex; i < Length; i++, p++)
			{
				// Assign the value.
				// Please note that 'p[a]' is equivalent to '*(p + a)'.
				*p = p[length];
			}
		}

		Length -= length;
	}

	/// <summary>
	/// Removes the specified number of characters from the end of the collection.
	/// </summary>
	/// <param name="length">The number of characters you want to remove.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RemoveFromEnd(int length) => Length -= length;

	/// <summary>
	/// Gets the built <see cref="string"/> and clears the handler.
	/// </summary>
	/// <returns>The built string.</returns>
	/// <remarks>
	/// This releases any resources used by the handler. The method should be invoked only
	/// once and as the last thing performed on the handler. Subsequent use is erroneous, ill-defined,
	/// and may destabilize the process, as may using any other copies of the handler after
	/// <see cref="ToStringAndClear"/> is called on any one of them.
	/// </remarks>
	public string ToStringAndClear()
	{
		try
		{
			return new(Text);
		}
		finally
		{
			Clear();
		}
	}

	/// <summary>
	/// Writes the specified string to the handler.
	/// </summary>
	/// <param name="value">The string to write.</param>
	internal void AppendStringDirect(string value)
	{
		if (value.TryCopyTo(_chars[Length..]))
		{
			Length += value.Length;
		}
		else
		{
			GrowThenCopyString(value);
		}
	}

	/// <summary>
	/// Clears the handler, returning any rented array to the pool.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void Clear()
	{
		char[]? toReturn = _arrayToReturnToPool;
		this = default;
		if (toReturn is not null)
		{
			ArrayPool<char>.Shared.Return(toReturn);
		}
	}

	/// <summary>Writes the specified value to the handler.</summary>
	/// <param name="value">The value to write.</param>
	/// <remarks>
	/// Slow path to handle a custom formatter, potentially null value,
	/// or a string that doesn't fit in the current buffer.
	/// </remarks>
	[MethodImpl(MethodImplOptions.NoInlining)]
	private void AppendFormattedSlow(string? value)
	{
		if (value is not null)
		{
			EnsureCapacityForAdditionalChars(value.Length);
			value.CopyTo(_chars[Length..]);
			Length += value.Length;
		}
	}

	/// <summary>
	/// Handles adding any padding required for aligning a formatted value in an interpolation expression.
	/// </summary>
	/// <param name="startingPos">The position at which the written value started.</param>
	/// <param name="alignment">
	/// Non-zero minimum number of characters that should be written for this value.
	/// If the value is negative, it indicates left-aligned and the required minimum is the absolute value.
	/// </param>
	private void AppendOrInsertAlignmentIfNeeded(int startingPos, int alignment)
	{
		Debug.Assert(startingPos >= 0 && startingPos <= Length);
		Debug.Assert(alignment != 0);

		int charsWritten = Length - startingPos;
		(alignment, bool leftAlign) = alignment < 0 ? (-alignment, true) : (alignment, false);
		if (alignment - charsWritten is var paddingNeeded and > 0)
		{
			EnsureCapacityForAdditionalChars(paddingNeeded);

			if (leftAlign)
			{
				_chars.Slice(Length, paddingNeeded).Fill(' ');
			}
			else
			{
				_chars.Slice(startingPos, charsWritten).CopyTo(_chars[(startingPos + paddingNeeded)..]);
				_chars.Slice(startingPos, paddingNeeded).Fill(' ');
			}

			Length += paddingNeeded;
		}
	}

	/// <summary>
	/// Ensures <see cref="_chars"/> has the capacity to store <paramref name="additionalChars"/>
	/// beyond <see cref="Length"/>.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void EnsureCapacityForAdditionalChars(int additionalChars)
	{
		if (_chars.Length - Length < additionalChars)
		{
			Grow(additionalChars);
		}
	}

	/// <summary>
	/// Fallback for fast path in <see cref="AppendStringDirect"/>
	/// when there's not enough space in the destination.
	/// </summary>
	/// <param name="value">The string to write.</param>
	[MethodImpl(MethodImplOptions.NoInlining)]
	private void GrowThenCopyString(string value)
	{
		Grow(value.Length);
		value.CopyTo(_chars[Length..]);
		Length += value.Length;
	}

	/// <summary>
	/// Fallback for <see cref="AppendFormatted(ReadOnlySpan{char})"/> for when not enough space exists
	/// in the current buffer.
	/// </summary>
	/// <param name="value">The span to write.</param>
	[MethodImpl(MethodImplOptions.NoInlining)]
	private void GrowThenCopySpan(ReadOnlySpan<char> value)
	{
		Grow(value.Length);
		value.CopyTo(_chars[Length..]);
		Length += value.Length;
	}

	/// <summary>
	/// Grows <see cref="_chars"/> to have the capacity to store at least <paramref name="additionalChars"/>
	/// beyond <see cref="Length"/>.
	/// </summary>
	/// <remarks>
	/// This method is called when the remaining space <c>_chars.Length - _pos</c> is
	/// insufficient to store a specific number of additional characters.
	/// Thus, we need to grow to at least that new total. <see cref="GrowCore(uint)"/>
	/// will handle growing by more than that if possible.
	/// </remarks>
	/// <seealso cref="GrowCore(uint)"/>
	[MethodImpl(MethodImplOptions.NoInlining)]
	private void Grow(int additionalChars)
	{
		Debug.Assert(additionalChars > _chars.Length - Length);

		GrowCore((uint)Length + (uint)additionalChars);
	}

	/// <summary>
	/// Grows the size of <see cref="_chars"/>.
	/// </summary>
	/// <remarks>
	/// This method is called when the remaining space in <c>_chars</c> isn't sufficient to continue
	/// the operation. Thus, we need at least one character beyond <c>_chars.Length</c>.
	/// <see cref="GrowCore(uint)"/> will handle growing by more than that if possible.
	/// </remarks>
	/// <seealso cref="GrowCore(uint)"/>
	[MethodImpl(MethodImplOptions.NoInlining)]
	private void Grow() => GrowCore((uint)_chars.Length + 1);

	/// <summary>
	/// Grow the size of <see cref="_chars"/> to at least the specified <paramref name="requiredMinCapacity"/>.
	/// </summary>
	/// <param name="requiredMinCapacity">The required minimum capacity.</param>
	/// <remarks>
	/// <para>Design notes:</para>
	/// <para>
	/// We want the max of how much space we actually required and doubling our capacity (without going
	/// beyond the max allowed length). We also want to avoid asking for small arrays,
	/// to reduce the number of times we need to grow, and since we're working with unsigned integers
	/// that could technically overflow if someone tried to, for example, append a huge string
	/// to a huge string, we also clamp to <see cref="int.MaxValue"/>. Even if the array creation
	/// fails in such a case, we may later fail in <see cref="ToStringAndClear"/>.
	/// </para>
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void GrowCore(uint requiredMinCapacity)
	{
		// 0x3FFFFFDF: string.MaxLength
		uint newCapacity = Max(requiredMinCapacity, Min((uint)_chars.Length * 2, 0x3FFFFFDF));
		int arraySize = (int)Clamp(newCapacity, MinimumArrayPoolLength, int.MaxValue);

		char[] newArray = ArrayPool<char>.Shared.Rent(arraySize);
		_chars[..Length].CopyTo(newArray);

		char[]? toReturn = _arrayToReturnToPool;
		_chars = _arrayToReturnToPool = newArray;

		if (toReturn is not null)
		{
			ArrayPool<char>.Shared.Return(toReturn);
		}
	}


	/// <summary>
	/// Determines whether two instances has same values with the other instance.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public static bool Equals(in StringHandler left, in StringHandler right)
	{
		if (left.Length != right.Length)
		{
			return false;
		}

		fixed (char* pThis = left._chars, pOther = right._chars)
		{
			int i = 0;
			char* p = pThis, q = pOther;
			for (int length = left.Length; i < length; i++)
			{
				if (*p++ != *q++)
				{
					return false;
				}
			}
		}

		return true;
	}


	/// <summary>
	/// Determine whether two <see cref="StringHandler"/>s hold a same character set.
	/// </summary>
	/// <param name="left">The left-side instance to compare.</param>
	/// <param name="right">The right-side instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(in StringHandler left, in StringHandler right) => Equals(left, right);

	/// <summary>
	/// Determine whether two <see cref="StringHandler"/>s don't hold a same character set.
	/// </summary>
	/// <param name="left">The left-side instance to compare.</param>
	/// <param name="right">The right-side instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(in StringHandler left, in StringHandler right) => !(left == right);
}
