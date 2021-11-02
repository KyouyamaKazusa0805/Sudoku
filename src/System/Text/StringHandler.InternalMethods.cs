namespace System.Text;

partial struct StringHandler
{
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
}
