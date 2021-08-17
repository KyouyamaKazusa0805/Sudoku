namespace Sudoku.CodeGenerating.Collections;

/// <summary>
/// Encapsulates a lightweight data structure that is same as the collection type <c>Sudoku.Data.Cells</c>.
/// </summary>
internal ref struct Cells
{
	/// <summary>
	/// <para>Indicates an empty instance (all bits are 0).</para>
	/// <para>
	/// I strongly recommend you <b>should</b> use this instance instead of default constructor
	/// <see cref="Cells()"/> and <see langword="default"/>(<see cref="Cells"/>).
	/// </para>
	/// </summary>
	/// <seealso cref="Cells()"/>
	public static Cells Empty => new();


	/// <summary>
	/// The value used for shifting.
	/// </summary>
	private const int Shifting = 41;

	/// <summary>
	/// The value of offsets.
	/// </summary>
	private const int BlockOffset = 0, RowOffset = 9, ColumnOffset = 18, Limit = 27;


	/// <summary>
	/// Indicates the internal two <see cref="long"/> values,
	/// which represents 81 bits. <see cref="_high"/> represent the higher
	/// 40 bits and <see cref="_low"/> represents the lower 41 bits.
	/// </summary>
	private long _high, _low;


	/// <summary>
	/// Initializes an instance with the candidate list specified as a pointer.
	/// </summary>
	/// <param name="cells">The pointer points to an array of elements.</param>
	/// <param name="length">The length of the array.</param>
	public unsafe Cells(int* cells, int length) : this()
	{
		for (int i = 0; i < length; i++)
		{
			InternalAdd(cells[i], true);
		}
	}

	/// <summary>
	/// Initializes an instance with the specified array of cells.
	/// </summary>
	/// <param name="cells">All cells.</param>
	/// <remarks>
	/// This constructor is defined after another constructor with
	/// <see cref="ReadOnlySpan{T}"/> had defined. Although this constructor
	/// doesn't initialize something (use the other one instead),
	/// while initializing with the type <see cref="int"/>[], the compiler
	/// gives me an error without this constructor (ambiguity of two
	/// constructors). However, unfortunately, <see cref="ReadOnlySpan{T}"/>
	/// doesn't implemented the interface <see cref="IEnumerable{T}"/>.
	/// </remarks>
	public unsafe Cells(int[] cells) : this()
	{
		fixed (int* ptr = cells)
		{
			int i = 0;
			for (int* p = ptr; i < cells.Length; i++, p++)
			{
				InternalAdd(*p, true);
			}
		}
	}

	/// <summary>
	/// Initializes an instance with two binary values.
	/// </summary>
	/// <param name="high">Higher 40 bits.</param>
	/// <param name="low">Lower 41 bits.</param>
	private Cells(long high, long low)
	{
		_high = high;
		_low = low;

		int count = 0;
		for (; high != 0; high >>= 1) if ((high & 1) != 0) count++;
		for (; low != 0; low >>= 1) if ((low & 1) != 0) count++;
		Count = count;
	}


	/// <summary>
	/// Indicates the total number of cells where the corresponding
	/// value are set <see langword="true"/>.
	/// </summary>
	public int Count { get; private set; }

	/// <summary>
	/// Indicates all cell offsets whose corresponding value are set <see langword="true"/>.
	/// </summary>
	private readonly unsafe int[] Offsets
	{
		get
		{
			if (Count == 0)
			{
				return Array.Empty<int>();
			}

			long value;
			int i, pos = 0;
			int* result = stackalloc int[Count];
			if (_low != 0)
			{
				for (value = _low, i = 0; i < Shifting; i++, value >>= 1)
				{
					if ((value & 1) != 0)
					{
						result[pos++] = i;
					}
				}
			}
			if (_high != 0)
			{
				for (value = _high, i = Shifting; i < 81; i++, value >>= 1)
				{
					if ((value & 1) != 0)
					{
						result[pos++] = i;
					}
				}
			}

			int[] arr = new int[Count];
			fixed (int* ptr = arr)
			{
				Unsafe.CopyBlock(ptr, result, (uint)(sizeof(int) * Count));
			}

			return arr;
		}
	}


	/// <summary>
	/// Get the cell offset at the specified position index.
	/// </summary>
	/// <param name="index">The index of position of all set bits.</param>
	/// <returns>
	/// This cell offset at the specified position index. If the value is invalid,
	/// the return value will be <c>-1</c>.
	/// </returns>
	public readonly int this[int index]
	{
		get
		{
			if (Count == 0)
			{
				return -1;
			}

			long value;
			int i, pos = -1;
			if (_low != 0)
			{
				for (value = _low, i = 0; i < Shifting; i++, value >>= 1)
				{
					if ((value & 1) != 0 && ++pos == index)
					{
						return i;
					}
				}
			}
			if (_high != 0)
			{
				for (value = _high, i = Shifting; i < 81; i++, value >>= 1)
				{
					if ((value & 1) != 0 && ++pos == index)
					{
						return i;
					}
				}
			}

			return -1;
		}
	}


	/// <summary>
	/// Determine whether the map contains the specified cell.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Contains(int cell) =>
		((cell / Shifting == 0 ? _low : _high) >> cell % Shifting & 1) != 0;

	/// <summary>
	/// Get all set cell offsets and returns them as an array.
	/// </summary>
	/// <returns>An array of all set cell offsets.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly int[] ToArray() => Offsets;

	/// <inheritdoc cref="object.ToString"/>
	public override readonly string ToString()
	{
		StringBuilder sb = new(), innerSb = new();
		sb.Append("0b");

		int i;
		long value = _high;
		for (i = 0; i < 40; i++, value >>= 1)
		{
			innerSb.Append(value & 1);
		}
		reverse(innerSb);
		sb.Append(innerSb.ToString());
		innerSb.Clear();

		sb.Append(", 0b");
		for (value = _low; i < 81; i++, value >>= 1)
		{
			innerSb.Append(value & 1);
		}

		reverse(innerSb);
		sb.Append(innerSb.ToString());
		return sb.ToString();


		static void reverse(StringBuilder innerSb)
		{
			for (int i = 0, length = innerSb.Length >> 1; i < length; i++)
			{
				char t = innerSb[i];
				innerSb[i] = innerSb[innerSb.Length - 1 - i];
				innerSb[innerSb.Length - 1 - i] = t;
			}
		}
	}

	/// <summary>
	/// Set the specified cell as <see langword="true"/> or <see langword="false"/> value.
	/// </summary>
	/// <param name="offset">
	/// The cell offset. This value can be positive and negative. If 
	/// negative, the offset will be assigned <see langword="false"/>
	/// into the corresponding bit position of its absolute value.
	/// </param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void Add(int offset)
	{
		if (offset >= 0) // Positive or zero.
		{
			InternalAdd(offset, true);
		}
		else // Negative values.
		{
			InternalAdd(~offset, false);
		}
	}

	/// <summary>
	/// Set the specified cell as <see langword="true"/> value.
	/// </summary>
	/// <param name="offset">The cell offset.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddAnyway(int offset) => InternalAdd(offset, true);

	/// <summary>
	/// Set the specified cells as <see langword="true"/> value.
	/// </summary>
	/// <param name="offsets">The cells to add.</param>
	public void AddRange(in ReadOnlySpan<int> offsets)
	{
		foreach (int cell in offsets)
		{
			AddAnyway(cell);
		}
	}

	/// <summary>
	/// Set the specified cells as <see langword="true"/> value.
	/// </summary>
	/// <param name="offsets">The cells to add.</param>
	public void AddRange(IEnumerable<int> offsets)
	{
		foreach (int cell in offsets)
		{
			AddAnyway(cell);
		}
	}

	/// <summary>
	/// Set the specified cell as <see langword="false"/> value.
	/// </summary>
	/// <param name="offset">The cell offset.</param>
	/// <remarks>
	/// Different with <see cref="Add(int)"/>, this method <b>can't</b> receive
	/// the negative value as the parameter.
	/// </remarks>
	/// <seealso cref="Add(int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Remove(int offset) => InternalAdd(offset, false);

	/// <summary>
	/// The internal operation for adding a cell.
	/// </summary>
	/// <param name="cell">The cell to add into.</param>
	/// <param name="value">The value to add.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void InternalAdd(int cell, bool value)
	{
		if (cell is >= 0 and < 81)
		{
			ref long v = ref cell / Shifting == 0 ? ref _low : ref _high;
			bool older = Contains(cell);
			if (value)
			{
				v |= 1L << cell % Shifting;
				if (!older)
				{
					Count++;
				}
			}
			else
			{
				v &= ~(1L << cell % Shifting);
				if (older)
				{
					Count--;
				}
			}
		}
	}


	/// <summary>
	/// Reverse status for all cells, which means all <see langword="true"/> bits
	/// will be set <see langword="false"/>, and all <see langword="false"/> bits
	/// will be set <see langword="true"/>.
	/// </summary>
	/// <param name="gridMap">The instance to negate.</param>
	/// <returns>The negative result.</returns>
	/// <remarks>
	/// While reversing the higher 40 bits, the unused bits will be fixed and never be modified the state,
	/// that is why using the code "<c><![CDATA[higherBits & 0xFFFFFFFFFFL]]></c>".
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Cells operator ~(in Cells gridMap) =>
		new(~gridMap._high & 0xFFFFFFFFFFL, ~gridMap._low & 0x1FFFFFFFFFFL);

	/// <summary>
	/// Get a <see cref="Cells"/> that contains all <paramref name="left"/> instance
	/// but not in <paramref name="right"/> instance.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>The result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Cells operator -(in Cells left, in Cells right) => left & ~right;

	/// <summary>
	/// Get all cells that two <see cref="Cells"/> instances both contain.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>The intersection result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Cells operator &(in Cells left, in Cells right) =>
		new(left._high & right._high, left._low & right._low);

	/// <summary>
	/// Get all cells from two <see cref="Cells"/> instances.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>The union result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Cells operator |(in Cells left, in Cells right) =>
		new(left._high | right._high, left._low | right._low);

	/// <summary>
	/// Get all cells that only appears once in two <see cref="Cells"/> instances.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>The symmetrical difference result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Cells operator ^(in Cells left, in Cells right) =>
		new(left._high ^ right._high, left._low ^ right._low);


	/// <summary>
	/// Implicit cast from <see cref="int"/>[] to <see cref="Cells"/>.
	/// </summary>
	/// <param name="cells">The cells.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Cells(int[] cells) => new(cells);
}
