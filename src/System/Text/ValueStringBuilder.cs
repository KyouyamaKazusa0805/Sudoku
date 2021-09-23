namespace System.Text;

/// <summary>
/// Encapsulates a string builder implementation that is used via a <see langword="struct"/>.
/// </summary>
/// <remarks>
/// You shouldn't use the parameterless constructor <see cref="ValueStringBuilder()"/>.
/// </remarks>
/// <example>
/// You can use this struct like this:
/// <code><![CDATA[
/// var sb = new ValueStringBuilder(stackalloc char[100]);
/// 
/// // Appending operations...
/// sb.Append("Hello");
/// sb.Append(',');
/// sb.Append("World");
/// sb.Append('!');
/// 
/// Console.WriteLine(sb.ToString()); // Dispose method will be called here.
/// ]]></code>
/// </example>
/// <seealso cref="ValueStringBuilder()"/>
[AutoGetEnumerator("@", MemberConversion = "new(@)", ReturnType = typeof(Enumerator))]
public ref partial struct ValueStringBuilder
{
	/// <summary>
	/// Indicates the inner character series that is created by <see cref="ArrayPool{T}"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// In the general cases, this field always keeps the <see langword="null"/> value. This
	/// field is not <see langword="null"/> when you calls the constructor <see cref="ValueStringBuilder(int)"/>
	/// because that constructor will be called if you want to create a large buffer.
	/// </para>
	/// <para>
	/// The field
	/// is as the same reference as the rent buffer array segment. If called that constructor,
	/// the inner code will execute <see cref="ArrayPool{T}.Rent(int)"/> to rent the specified number
	/// of bytes of buffer to be used, which won't allocate any memory.
	/// </para>
	/// <para>
	/// If the field is not <see langword="null"/> and when we calls the method <see cref="ToString"/>,
	/// the buffer will be returned and the inner data will be released.
	/// </para>
	/// </remarks>
	/// <seealso cref="ArrayPool{T}"/>
	/// <seealso cref="ArrayPool{T}.Rent(int)"/>
	/// <seealso cref="ValueStringBuilder(int)"/>
	/// <seealso cref="ToString"/>
	private char[]? _chunk;

	/// <summary>
	/// Indicates the character pool.
	/// </summary>
	private Span<char> _chars;


	/// <summary>
	/// Initializes an instance with the specified string as the basic buffer.
	/// </summary>
	/// <param name="s">The string value.</param>
	/// <remarks>
	/// This constructor should be used when you know the maximum length of the return string. In addition,
	/// the string shouldn't be too long; below 300 (approximately) is okay.
	/// </remarks>
	public unsafe ValueStringBuilder(string s)
	{
		_chunk = null;
		Length = s.Length;

		fixed (char* p = s)
		{
			_chars = new(p, s.Length);
		}
	}

	/// <summary>
	/// Initializes an instance with the buffer specified as a <see cref="Span{T}"/>.
	/// </summary>
	/// <param name="buffer">The initial buffer.</param>
	/// <remarks>
	/// <para>
	/// For the buffer, you can use the nested <see langword="stackalloc"/> expression to create
	/// a serial of buffer, such as <c>stackalloc char[10]</c>, where the length 10 is the value
	/// that holds the approximate maximum number of characters when output from your evaluation.
	/// </para>
	/// <para>
	/// You can also use the constructor: <see cref="ValueStringBuilder(int)"/> like:
	/// <code><![CDATA[var sb = new ValueStringBuilder(10);]]></code>
	/// The code is nearly equivalent to
	/// <code><![CDATA[var sb = new ValueStringBuilder(stackalloc char[10]);]]></code>
	/// but uses shared array pool (i.e. the property <see cref="ArrayPool{T}.Shared"/>)
	/// to create the buffer rather than using <see cref="Span{T}"/>.
	/// </para>
	/// </remarks>
	/// <seealso cref="Span{T}"/>
	/// <seealso cref="ValueStringBuilder(int)"/>
	public ValueStringBuilder(in Span<char> buffer) : this() => _chars = buffer;

	/// <summary>
	/// Initializes an instance with the specified capacity.
	/// </summary>
	/// <param name="capacity">The capacity.</param>
	public ValueStringBuilder(int capacity)
	{
		_chunk = ArrayPool<char>.Shared.Rent(capacity);
		_chars = _chunk;
		Length = 0;
	}


	/// <summary>
	/// Indicates the length of the string builder.
	/// </summary>
	public int Length { get; private set; }

	/// <summary>
	/// Indicates the total capacity.
	/// </summary>
	public int Capacity => _chars.Length;


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
	public ref char this[int index] => ref _chars[index];


	/// <summary>
	/// Determines whether two instances has same values with the other instance.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[ProxyEquality]
	public static unsafe bool Equals(in ValueStringBuilder left, in ValueStringBuilder right)
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
	/// Try to copy the current instance to the specified builder.
	/// </summary>
	/// <param name="builder">The builder.</param>
	/// <exception cref="ArgumentException">
	/// Throws when the target argument doesn't contain the enough space.
	/// </exception>
	public readonly unsafe void CopyTo(ref ValueStringBuilder builder)
	{
		if (builder.Capacity < Length)
		{
			throw new ArgumentException(
				"The argument can't receive the value because the instance doesn't contain the enough space.",
				nameof(builder)
			);
		}

		fixed (char* pThis = _chars, pBuilder = builder._chars)
		{
			Unsafe.CopyBlock(pThis, pBuilder, (uint)(sizeof(char) * Length));
		}

		builder.Length = Length;
	}

	/// <summary>
	/// Try to copy the current instance to the specified builder.
	/// </summary>
	/// <param name="builder">The builder.</param>
	public readonly void CopyTo(StringBuilder builder)
	{
		builder.Length = Length;

		for (int i = 0; i < Length; i++)
		{
			builder[i] = _chars[i];
		}
	}

	/// <summary>
	/// <para>
	/// Get a pinnable reference to the builder.
	/// Does not ensure there is a null char after <see cref="Length"/>.
	/// </para>
	/// <para>
	/// This overload is pattern matched in the C# 7.3+ compiler so you can omit
	/// the explicit method call, and write eg <c>fixed (char* c = builder)</c>.
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
	public ref readonly char GetPinnableReference(bool withTerminate)
	{
		if (withTerminate)
		{
			EnsureCapacity(Length + 1);
			_chars[Length] = '\0';
		}

		return ref MemoryMarshal.GetReference(_chars);
	}

	/// <summary>
	/// To dispose the collection, all fields and properties will be cleared. In other words,
	/// this method is nearly equivalent to the code <c>this = default;</c>.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Dispose() => Dispose(true);

#if DEBUG
	/// <summary>
	/// To ensure the capacity in order to append characters into this collection.
	/// </summary>
	/// <param name="capacity">The capacity value to ensure.</param>
#else
		/// <summary>
		/// To ensure the capacity in order to append characters into this collection.
		/// </summary>
		/// <param name="capacity">The capacity value to ensure.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Throws when the argument <paramref name="capacity"/> is below 0.
		/// </exception>
#endif
	public void EnsureCapacity(int capacity)
	{
#if DEBUG
		// This is not expected to be called this with negative capacity
		Debug.Assert(capacity >= 0);
#else
			if (capacity < 0)
			{
				throw new ArgumentOutOfRange(nameof(capacity));
			}
#endif

		// If the caller has a bug and calls this with negative capacity,
		// make sure to call Grow to throw an exception.
		if (capacity > Capacity)
		{
			Grow(capacity - Length);
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
		s.AsSpan().CopyTo(_chars[index..]);
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
	public void RemoveFromEnd(int length) => Length -= length;

	/// <summary>
	/// To clear the builder.
	/// </summary>
	public void Clear() => Dispose(false);

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
			Grow(1);
			Append(c);
		}
	}

	/// <summary>
	/// Append a serial of same characters into the collection.
	/// </summary>
	/// <param name="c">The character.</param>
	/// <param name="count">The number of the character you want to append.</param>
	public void Append(char c, int count)
	{
		if (Length > _chars.Length - count)
		{
			Grow(count);
		}

		var dst = _chars[Length..(Length + count)];
		for (int i = 0, length = dst.Length; i < length; i++)
		{
			dst[i] = c;
		}

		Length += count;
	}

	/// <summary>
	/// Append a value.
	/// </summary>
	/// <typeparam name="TUnmanaged">The type of the value.</typeparam>
	/// <param name="value">The value.</param>
	public void Append<TUnmanaged>(TUnmanaged value) where TUnmanaged : unmanaged =>
		Append(value switch
		{
			sbyte s => s.ToString(),
			byte b => b.ToString(),
			short s => s.ToString(),
			ushort u => u.ToString(),
			nint n => n.ToString(),
			nuint n => n.ToString(),
			int i => i.ToString(),
			uint u => u.ToString(),
			long l => l.ToString(),
			ulong u => u.ToString(),
			char c => c.ToString(),
			bool b => b.ToString(),
			float f => f.ToString(),
			double d => d.ToString(),
			decimal d => d.ToString(),
			_ => value.ToString()
		});

	/// <summary>
	/// Append a string into the collection.
	/// </summary>
	/// <param name="s">The string.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void Append(string? s)
	{
		switch (s)
		{
			case not { Length: not 0 }:
			{
				return;
			}
			case { Length: 1 } when Length is var pos && (uint)pos < (uint)_chars.Length:
			{
				fixed (char* c = s)
				{
					_chars[pos] = *c;
				}

				Length = pos + 1;
				break;
			}
			default:
			{
				AppendSlow(s);

				break;
			}
		}
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

		var dst = _chars[Length..(Length + length)];
		for (int i = 0, iterationLength = dst.Length; i < iterationLength; i++)
		{
			dst[i] = *value++;
		}
		Length += length;
	}

	/// <summary>
	/// Append a serial of characters.
	/// </summary>
	/// <param name="value">The characters.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Append(in ReadOnlySpan<char> value)
	{
		int pos = Length;
		if (pos > _chars.Length - value.Length)
		{
			Grow(value.Length);
		}

		value.CopyTo(_chars[Length..]);
		Length += value.Length;
	}

	/// <summary>
	/// Append a new line string.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AppendLine() => Append(Environment.NewLine);

	/// <summary>
	/// Append a new line string through the specified unknown typed instance.
	/// </summary>
	/// <param name="instance">The instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AppendLine(object instance) => AppendLine(instance.ToString());

	/// <summary>
	/// Append a character, and then append a new line string.
	/// </summary>
	/// <param name="c">The character.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AppendLine(char c)
	{
		Append(c);
		AppendLine();
	}

	/// <summary>
	/// Append a series of same characters, whose length is specified
	/// as the argument <paramref name="count"/>.
	/// </summary>
	/// <param name="c">The character.</param>
	/// <param name="count">The number of characters.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AppendLine(char c, int count)
	{
		Append(c, count);
		AppendLine();
	}

	/// <summary>
	/// Append a string that represented as a pointer.
	/// </summary>
	/// <param name="s">The string.</param>
	/// <param name="length">The length.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void AppendLine(char* s, int length)
	{
		Append(s, length);
		AppendLine();
	}

	/// <summary>
	/// Append a string, and then append a new line.
	/// </summary>
	/// <param name="s">The string.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AppendLine(string? s)
	{
		Append(s);
		AppendLine();
	}

	/// <summary>
	/// Append a string representation of a specified instance, and then append a new line.
	/// </summary>
	/// <typeparam name="TUnmanaged">The type of the instance.</typeparam>
	/// <param name="value">The value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AppendLine<TUnmanaged>(TUnmanaged value) where TUnmanaged : unmanaged
	{
		Append(value);
		AppendLine();
	}

	/// <summary>
	/// Append a series of elements into the current collection.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="list">The list.</param>
	public void AppendLineRange<T>(IEnumerable<T?> list)
	{
		foreach (var element in list)
		{
			AppendLine(element?.ToString());
		}
	}

	/// <summary>
	/// Append a serial of strings from a serial of elements.
	/// </summary>
	/// <typeparam name="TUnmanaged">The type of each element.</typeparam>
	/// <param name="list">The list of elements.</param>
	/// <param name="separator">The separator when an element is finished to append.</param>
	public void AppendRange<TUnmanaged>(IEnumerable<TUnmanaged> list, string? separator = null)
	where TUnmanaged : unmanaged
	{
		foreach (var element in list)
		{
			Append(element);
			if (separator is not null)
			{
				Append(separator);
			}
		}

		if (separator is not null)
		{
			RemoveFromEnd(separator.Length);
		}
	}

	/// <summary>
	/// Append a serial of strings converted from a serial of elements.
	/// </summary>
	/// <typeparam name="TUnmanaged">The type of each element.</typeparam>
	/// <param name="list">The list of elements.</param>
	/// <param name="converter">The converter.</param>
	/// <param name="separator">The separator when an element is finished to append.</param>
	public unsafe void AppendRange<TUnmanaged>(
		IEnumerable<TUnmanaged> list,
		delegate*<TUnmanaged, string?> converter,
		string? separator = null
	) where TUnmanaged : unmanaged
	{
		foreach (var element in list)
		{
			Append(converter(element));
			if (separator is not null)
			{
				Append(separator);
			}
		}

		if (separator is not null)
		{
			RemoveFromEnd(separator.Length);
		}
	}

	/// <summary>
	/// Append a serial of strings from a serial of elements specified as a pointer.
	/// </summary>
	/// <typeparam name="TUnmanaged">The type of each element.</typeparam>
	/// <param name="list">The list of elements.</param>
	/// <param name="length">The length of the list.</param>
	/// <param name="separator">The separator when an element is finished to append.</param>
	public unsafe void AppendRange<TUnmanaged>(TUnmanaged* list, int length, string? separator = null)
	where TUnmanaged : unmanaged
	{
		int index = 0;
		for (var p = list; index < length; index++, p++)
		{
			Append(list[index]);
			if (separator is not null)
			{
				Append(separator);
			}
		}

		if (separator is not null)
		{
			RemoveFromEnd(separator.Length);
		}
	}

	/// <summary>
	/// Append a serial of strings converted from a serial of elements specified as a pointer.
	/// </summary>
	/// <typeparam name="TUnmanaged">The type of each element.</typeparam>
	/// <param name="list">The list of elements.</param>
	/// <param name="length">The length of the list.</param>
	/// <param name="converter">The converter.</param>
	/// <param name="separator">The separator when an element is finished to append.</param>
	public unsafe void AppendRange<TUnmanaged>(
		TUnmanaged* list,
		int length,
		delegate*<TUnmanaged, string?> converter,
		string? separator = null
	)
	where TUnmanaged : unmanaged
	{
		int index = 0;
		for (var p = list; index < length; index++, p++)
		{
			Append(converter(list[index]));
			if (separator is not null)
			{
				Append(separator);
			}
		}

		if (separator is not null)
		{
			RemoveFromEnd(separator.Length);
		}
	}

	/// <summary>
	/// Append a span.
	/// </summary>
	/// <param name="length">The length of the characters.</param>
	/// <returns>The span.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Span<char> AppendSpan(int length)
	{
		int originalPos = Length;
		if (originalPos > _chars.Length - length)
		{
			Grow(length);
		}

		Length = originalPos + length;
		return _chars[originalPos..Length];
	}

	/// <summary>
	/// Reverse the string builder instance. For example, if the list holds a string <c>"Hello"</c>,
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
	/// Returns the string result that is combined and constructed from the current instance,
	/// and then dispose the instance.
	/// </summary>
	/// <returns>The string representation.</returns>
	/// <remarks>
	/// <para>
	/// Please note that the dispose method will be invoked in the execution of this method <c>ToString</c>.
	/// Therefore, you can't or don't need to:
	/// <list type="bullet">
	/// <item>
	/// Use the keyword <see langword="using"/> onto the variable declaration, such as
	/// <c>using var sb = new ValueStringBuilder(stackalloc char[20]);</c>.
	/// </item>
	/// <item>
	/// Use the current string builder instance after had been called this method <c>ToString</c>.
	/// </item>
	/// </list>
	/// </para>
	/// <para>Because of the such behavior, this method isn't <see langword="readonly"/>.</para>
	/// </remarks>
	public override string ToString()
	{
		try
		{
			return _chars[..Length].ToString();
		}
		finally
		{
			Dispose();
		}
	}

	/// <summary>
	/// Resize the internal buffer either by doubling current buffer size or
	/// by adding <paramref name="additionalCapacityBeyondPos"/> to
	/// <see cref="Length"/> whichever is greater.
	/// </summary>
	/// <param name="additionalCapacityBeyondPos">Number of chars requested beyond current position.</param>
	/// <seealso cref="Length"/>
	[MethodImpl(MethodImplOptions.NoInlining)]
	private void Grow(int additionalCapacityBeyondPos)
	{
#if DEBUG
		Debug.Assert(additionalCapacityBeyondPos > 0);
		Debug.Assert(
			Length > _chars.Length - additionalCapacityBeyondPos,
			"Grow called incorrectly, no resize is needed."
		);
#endif

		// Make sure to let Rent throw an exception
		// if the caller has a bug and the desired capacity is negative.
		char[] poolArray = ArrayPool<char>.Shared.Rent(
			(int)Max((uint)(Length + additionalCapacityBeyondPos), (uint)_chars.Length * 2)
		);

		// If lack of space to store extra characters, just creates a new one,
		// and copy them into the new collection.
		_chars[..Length].CopyTo(poolArray);

		char[]? toReturn = _chunk;
		_chars = _chunk = poolArray;
		if (toReturn is not null)
		{
			ArrayPool<char>.Shared.Return(toReturn);
		}
	}

	/// <summary>
	/// Append a string.
	/// </summary>
	/// <param name="s">The string.</param>
	private void AppendSlow(string s)
	{
		int pos = Length;
		if (pos > _chars.Length - s.Length)
		{
			Grow(s.Length);
		}

		s.AsSpan().CopyTo(_chars[pos..]);
		Length += s.Length;
	}

	/// <summary>
	/// To dispose the collection. Although this method is <see langword="public"/>,
	/// you may not call this method, because this method will be called automatically when
	/// the method <see cref="ToString"/> is called.
	/// </summary>
	/// <param name="clearAll">Indicates whether we should return the buffer.</param>
	/// <seealso cref="ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void Dispose(bool clearAll)
	{
		char[]? toReturn = _chunk;

		if (clearAll)
		{
			// For safety, to avoid using pooled array if this instance is erroneously appended to again.
			this = default;
		}
		else
		{
			// Store the previous data, but clear the length value to 0.
			Length = 0;
			_chars.Clear();
		}

		// Returns the buffer memory.
		if (toReturn is not null)
		{
			ArrayPool<char>.Shared.Return(toReturn);
		}
	}
}
