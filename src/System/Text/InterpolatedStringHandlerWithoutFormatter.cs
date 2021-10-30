namespace System.Text;

/// <summary>
/// <para>
/// Provides a handler used by the language compiler to process interpolated strings
/// into <see cref="string"/> instances, whose code structure is similar
/// with the type <see cref="ValueStringBuilder"/>, because this type is a handler-typed
/// <see cref="string"/> builder.
/// </para>
/// <para>
/// Different with <see cref="DefaultInterpolatedStringHandler"/>, this type won't contain
/// any formatters, i.e. <see cref="IFormatProvider"/>.
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// (From the Microsoft documentation comments) Implementation note:
/// </para>
/// <para>
/// As this type lives in <see cref="Runtime.CompilerServices"/>
/// and is only intended to be targeted by the compiler,
/// public APIs eschew argument validation logic in a variety of places,
/// e.g. allowing a <see langword="null"/> input when one isn't expected to produce
/// a <see cref="NullReferenceException"/> rather than an <see cref="ArgumentNullException"/>.
/// </para>
/// </remarks>
/// <seealso cref="DefaultInterpolatedStringHandler"/>
/// <seealso cref="IFormatProvider"/>
#if false
[InterpolatedStringHandler]
#endif
public ref partial struct InterpolatedStringHandlerWithoutFormatter
{
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
	private const int GuessedLengthPerHole = 11;

	/// <summary>
	/// Minimum size array to rent from the pool.
	/// </summary>
	/// <remarks>
	/// Same as stack-allocation size used today by <see cref="string.Format(string, object?[])"/>.
	/// </remarks>
	/// <seealso cref="string.Format(string, object?[])"/>
	private const int MinimumArrayPoolLength = 256;


	/// <summary>
	/// Array rented from the array pool and used to back <see cref="_chars"/>.
	/// </summary>
	private char[]? _arrayToReturnToPool;

	/// <summary>
	/// Position at which to write the next character.
	/// </summary>
	private int _pos;

	/// <summary>
	/// The span to write into.
	/// </summary>
	private Span<char> _chars;


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
	public InterpolatedStringHandlerWithoutFormatter(int literalLength, int holeCount)
	{
		_chars = _arrayToReturnToPool = ArrayPool<char>.Shared.Rent(
			Max(
				MinimumArrayPoolLength,
				literalLength + holeCount * GuessedLengthPerHole
			)
		);

		_pos = 0;
	}

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
	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
	public InterpolatedStringHandlerWithoutFormatter(
		[Discard] int literalLength,
		[Discard] int holeCount,
		Span<char> initialBuffer
	)
	{
		_chars = initialBuffer;
		_arrayToReturnToPool = null;
		_pos = 0;
	}


	/// <summary>
	/// Gets a span of the written characters thus far.
	/// </summary>
	private readonly ReadOnlySpan<char> Text => _chars[.._pos];


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
	public unsafe void AppendLiteral(string value)
	{
		// TODO: https://github.com/dotnet/runtime/issues/41692#issuecomment-685192193

		// What we really want here is to be able to add a bunch of additional special-cases based on length,
		// e.g. a switch with a case for each length <= 8, not mark the method as AggressiveInlining, and have
		// it inlined when provided with a string literal such that all the other cases evaporate but not inlined
		// if called directly with something that doesn't enable pruning.  Even better, if "literal".TryCopyTo
		// could be unrolled based on the literal, a.k.a. https://github.com/dotnet/runtime/pull/46392, we might
		// be able to remove all special-casing here.

		switch (value.Length)
		{
			case 1:
			{
				var chars = _chars;
				int pos = _pos;
				if ((uint)pos < (uint)chars.Length)
				{
					chars[pos] = value[0];
					_pos = pos + 1;
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
				int pos = _pos;
				if ((uint)pos < chars.Length - 1)
				{
					fixed (char* pFirstChar = value)
					{
						Unsafe.WriteUnaligned(
							ref Unsafe.As<char, byte>(ref Unsafe.Add(ref MemoryMarshal.GetReference(chars), pos)),
							Unsafe.ReadUnaligned<int>(ref Unsafe.As<char, byte>(ref *pFirstChar))
						);
					}

					_pos = pos + 2;
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
	/// <typeparam name="T">The type of the value to write.</typeparam>
	public void AppendFormatted<T>(T value)
	{
		if (value is ISpanFormattable spanFormattable)
		{
			int charsWritten;

			// Constrained call avoiding boxing for value types.
			while (!spanFormattable.TryFormat(_chars[_pos..], out charsWritten, null, null))
			{
				Grow();
			}

			_pos += charsWritten;

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
			while (!spanFormattable.TryFormat(_chars[_pos..], out charsWritten, format, null))
			{
				Grow();
			}

			_pos += charsWritten;
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
		int startingPos = _pos;
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
		int startingPos = _pos;
		AppendFormatted(value, format);
		if (alignment != 0)
		{
			AppendOrInsertAlignmentIfNeeded(startingPos, alignment);
		}
	}

	/// <summary>
	/// Writes the specified character span to the handler.
	/// </summary>
	/// <param name="value">The span to write.</param>
	public void AppendFormatted(ReadOnlySpan<char> value)
	{
		// Fast path for when the value fits in the current buffer
		if (value.TryCopyTo(_chars[_pos..]))
		{
			_pos += value.Length;
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
		bool leftAlign = false;
		if (alignment < 0)
		{
			leftAlign = true;
			alignment = -alignment;
		}

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
			value.CopyTo(_chars[_pos..]);
			_pos += value.Length;
			_chars.Slice(_pos, paddingRequired).Fill(' ');
			_pos += paddingRequired;
		}
		else
		{
			_chars.Slice(_pos, paddingRequired).Fill(' ');
			_pos += paddingRequired;
			value.CopyTo(_chars[_pos..]);
			_pos += value.Length;
		}
	}

	/// <summary>
	/// Writes the specified value to the handler.
	/// </summary>
	/// <param name="value">The value to write.</param>
	public void AppendFormatted(string? value)
	{
		// Fast-path for no custom formatter and a non-null string that fits in the current destination buffer.
		if (value is not null && value.TryCopyTo(_chars[_pos..]))
		{
			_pos += value.Length;
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
	/// We have the overload simply to disambiguate between <c><![CDATA[ROS<char>]]></c> and <see cref="object"/>,
	/// just in case someone does specify a format, as <see cref="string"/> is implicitly convertible to both.
	/// Just delegate to the <c>T</c>-based implementation.
	/// </remarks>
	public void AppendFormatted(string? value, int alignment = 0, string? format = null) =>
		AppendFormatted<string?>(value, alignment, format);

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
	/// Gets the built <see cref="string"/>.
	/// </summary>
	/// <returns>The built string.</returns>
	public override readonly string ToString() => new(Text);

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
			// Clears the memory usage.
			char[]? toReturn = _arrayToReturnToPool;
			this = default;
			if (toReturn is not null)
			{
				ArrayPool<char>.Shared.Return(toReturn);
			}
		}
	}

	/// <summary>
	/// Writes the specified string to the handler.
	/// </summary>
	/// <param name="value">The string to write.</param>
	private void AppendStringDirect(string value)
	{
		if (value.TryCopyTo(_chars[_pos..]))
		{
			_pos += value.Length;
		}
		else
		{
			GrowThenCopyString(value);
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
			value.CopyTo(_chars[_pos..]);
			_pos += value.Length;
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
		Debug.Assert(startingPos >= 0 && startingPos <= _pos);
		Debug.Assert(alignment != 0);

		int charsWritten = _pos - startingPos;
		(alignment, bool leftAlign) = alignment < 0 ? (-alignment, true) : (alignment, false);
		if (alignment - charsWritten is var paddingNeeded and > 0)
		{
			EnsureCapacityForAdditionalChars(paddingNeeded);

			if (leftAlign)
			{
				_chars.Slice(_pos, paddingNeeded).Fill(' ');
			}
			else
			{
				_chars.Slice(startingPos, charsWritten).CopyTo(_chars[(startingPos + paddingNeeded)..]);
				_chars.Slice(startingPos, paddingNeeded).Fill(' ');
			}

			_pos += paddingNeeded;
		}
	}

	/// <summary>
	/// Ensures <see cref="_chars"/> has the capacity to store <paramref name="additionalChars"/>
	/// beyond <see cref="_pos"/>.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void EnsureCapacityForAdditionalChars(int additionalChars)
	{
		if (_chars.Length - _pos < additionalChars)
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
		value.CopyTo(_chars[_pos..]);
		_pos += value.Length;
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
		value.CopyTo(_chars[_pos..]);
		_pos += value.Length;
	}

	/// <summary>
	/// Grows <see cref="_chars"/> to have the capacity to store at least <paramref name="additionalChars"/>
	/// beyond <see cref="_pos"/>.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	private void Grow(int additionalChars)
	{
		// This method is called when the remaining space (_chars.Length - _pos) is
		// insufficient to store a specific number of additional characters.  Thus, we
		// need to grow to at least that new total. GrowCore will handle growing by more
		// than that if possible.
		Debug.Assert(additionalChars > _chars.Length - _pos);

		GrowCore((uint)_pos + (uint)additionalChars);
	}

	/// <summary>
	/// Grows the size of <see cref="_chars"/>.
	/// </summary>
	/// <remarks>
	/// This method is called when the remaining space in _chars isn't sufficient to continue
	/// the operation. Thus, we need at least one character beyond _chars.Length. <see cref="GrowCore(uint)"/>
	/// will handle growing by more than that if possible.
	/// </remarks>
	/// <seealso cref="GrowCore(uint)"/>
	[MethodImpl(MethodImplOptions.NoInlining)]
	private void Grow() => GrowCore((uint)_chars.Length + 1);

	/// <summary>
	/// Grow the size of <see cref="_chars"/> to at least the specified <paramref name="requiredMinCapacity"/>.
	/// </summary>
	/// <param name="requiredMinCapacity">The required minimum capacity.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void GrowCore(uint requiredMinCapacity)
	{
		// We want the max of how much space we actually required and doubling our capacity
		// (without going beyond the max allowed length).
		// We also want to avoid asking for small arrays, to reduce the number of times we need to grow,
		// and since we're working with unsigned integers that could technically overflow if someone tried to,
		// for example, append a huge string to a huge string, we also clamp to int.MaxValue.
		// Even if the array creation fails in such a case, we may later fail in ToStringAndClear.

		// 0x3FFFFFDF: string.MaxLength
		uint newCapacity = Max(requiredMinCapacity, Min((uint)_chars.Length * 2, 0x3FFFFFDF));
		int arraySize = (int)Clamp(newCapacity, MinimumArrayPoolLength, int.MaxValue);

		char[] newArray = ArrayPool<char>.Shared.Rent(arraySize);
		_chars[.._pos].CopyTo(newArray);

		char[]? toReturn = _arrayToReturnToPool;
		_chars = _arrayToReturnToPool = newArray;

		if (toReturn is not null)
		{
			ArrayPool<char>.Shared.Return(toReturn);
		}
	}
}
