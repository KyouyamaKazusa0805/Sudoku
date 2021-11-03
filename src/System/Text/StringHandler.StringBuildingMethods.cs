namespace System.Text;

partial struct StringHandler
{
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
	public unsafe void Append(char* value, int length)
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

	/// <summary>
	/// Writes the specified large-object value to the handler.
	/// </summary>
	/// <param name="value">The value to write.</param>
	/// <typeparam name="T">The type of the value to write.</typeparam>
	public void AppendLargeObjectFormatted<T>(in T value)
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
	/// Writes the specified large-object value to the handler.
	/// </summary>
	/// <param name="value">The value to write.</param>
	/// <param name="format">The format string.</param>
	/// <typeparam name="T">The type of the value to write.</typeparam>
	public void AppendLargeObjectFormatted<T>(in T value, string? format)
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
	/// Append a new line string <see cref="Environment.NewLine"/>.
	/// </summary>
	/// <seealso cref="Environment.NewLine"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AppendLine() => AppendFormatted(Environment.NewLine);

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
	public unsafe void AppendRange<T>(IEnumerable<T> list, delegate*<T, string?> converter)
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
	public unsafe void AppendRangeWithSeparator<T>(IEnumerable<T> list, delegate*<T, string?> converter, string separator)
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
	public unsafe void AppendRangeWithSeparatorUnsafe<TUnmanaged>(
		TUnmanaged* list,
		int length,
		delegate*<TUnmanaged, string?> converter,
		string separator
	)
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
	public unsafe void AppendRangeWithSeparatorUnsafe<TUnmanaged>(
		TUnmanaged* list,
		int length,
		Func<TUnmanaged, string?> converter,
		string separator
	)
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
	/// Reverse the instance. For example, if the handler holds a string <c>"Hello"</c>,
	/// after called this method, the string will be <c>"olleH"</c>.
	/// </summary>
	public unsafe void Reverse()
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
	public unsafe void Remove(int startIndex, int length)
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
}
