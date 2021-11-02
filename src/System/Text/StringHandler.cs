#define DECREASE_INITIALIZATION_MEMORY_ALLOCATION
#define DISCARD_INTERPOLATION_INFO
#define USE_NEWER_CONSTANT_VALUES

namespace System.Text;

/// <summary>
/// <para>
/// Provides a handler used by the language compiler to process interpolated strings
/// into <see cref="string"/> instances, or used as a <see cref="string"/> builder to
/// appending and catenating multiple <see cref="string"/>s to a whole one.
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
/// sb.AppendFormatted("Hello");
/// sb.AppendChar(',');
/// sb.AppendFormatted("World");
/// sb.AppendChar('!');
/// 
/// Console.WriteLine(sb.ToStringAndClear());
/// ]]></code>
/// </remarks>
/// <seealso cref="DefaultInterpolatedStringHandler"/>
/// <seealso cref="IFormatProvider"/>
[InterpolatedStringHandler]
[AutoGetEnumerator("@", MemberConversion = "new(@)", ReturnType = typeof(Enumerator))]
public ref partial struct StringHandler
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
			GetGreaterNearestPowerOf2(literalLength + holeCount * GuessedLengthPerHole)
#else
			Max(MinimumArrayPoolLength, literalLength + holeCount * GuessedLengthPerHole)
#endif
		);

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
	[EditorBrowsable(EditorBrowsableState.Never)]
	public StringHandler(
#if DISCARD_INTERPOLATION_INFO
		[Discard]
#endif
		int literalLength,
#if DISCARD_INTERPOLATION_INFO
		[Discard]
#endif
		int holeCount,
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
	public unsafe StringHandler(string initialString)
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
	public readonly unsafe void CopyTo(ref StringHandler handler)
	{
		fixed (char* old = _chars, @new = handler._chars)
		{
			Unsafe.CopyBlock(@new, old, (uint)(sizeof(char) * Length));
		}
	}

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
	/// Gets the built <see cref="string"/>.
	/// </summary>
	/// <returns>The built string.</returns>
	public override readonly string ToString() => new(Text);

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
	public unsafe void AppendLiteral(string? value)
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
			AppendFormatted(value);
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
		if (value is ISpanFormattable spanFormattable)
		{
			int charsWritten;

			// Constrained call avoiding boxing for value types.
			while (!spanFormattable.TryFormat(_chars[Length..], out charsWritten, null, null))
			{
				Grow();
			}

			Length += charsWritten;

			return;
		}

		if ((value is IFormattable f ? f.ToString(format: null, null) : value?.ToString()) is { } s)
		{
			AppendStringDirect(s);
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
		if (value is ISpanFormattable spanFormattable)
		{
			int charsWritten;

			// Constrained call avoiding boxing for value types.
			while (!spanFormattable.TryFormat(_chars[Length..], out charsWritten, format, null))
			{
				Grow();
			}

			Length += charsWritten;
			return;
		}

		if ((value is IFormattable f ? f.ToString(format, null) : value?.ToString()) is { } s)
		{
			AppendStringDirect(s);
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
	/// Get a pinnable reference to the builder.
	/// </summary>
	/// <param name="withTerminate">
	/// Ensures that the builder has a null character after <see cref="Length"/>.
	/// </param>
	/// <seealso cref="Length"/>
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
	/// Determines whether two instances has same values with the other instance.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[ProxyEquality]
	public static unsafe bool Equals(in StringHandler left, in StringHandler right)
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
	/// Gets the greater nearest value of power of 2 for the specified value.
	/// </summary>
	/// <param name="base">The base value.</param>
	/// <returns>The result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetGreaterNearestPowerOf2(int @base)
	{
		switch (@base)
		{
			case <= 0 or 1: { return 1; }
			case 2: { return 2; }
			case 3 or 4: { return 4; }
			case 5 or 6 or 7 or 8: { return 8; }
			default:
			{
				int n = @base - 1;
				n |= unsignedRightShift(n, 1);
				n |= unsignedRightShift(n, 2);
				n |= unsignedRightShift(n, 4);
				n |= unsignedRightShift(n, 8);
				n |= unsignedRightShift(n, 16);

				return n < 0 ? 1 : n + 1;


				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				static int unsignedRightShift(int v, int p)
				{
					if (p != 0)
					{
						v >>= 1;
						v &= int.MaxValue;
						v >>= p - 1;
					}

					return v;
				}
			}
		}
	}
}
