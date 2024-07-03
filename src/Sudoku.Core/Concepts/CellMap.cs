namespace Sudoku.Concepts;

using static IBitStatusMap<CellMap, Cell, CellMap.Enumerator>;

/// <summary>
/// Encapsulates a binary series of cell state table.
/// </summary>
/// <remarks>
/// <include file="../../global-doc-comments.xml" path="/g/large-structure"/>
/// </remarks>
[JsonConverter(typeof(Converter))]
[StructLayout(LayoutKind.Auto)]
[CollectionBuilder(typeof(CellMap), nameof(Create))]
[DebuggerStepThrough]
[TypeImpl(TypeImplFlag.AllObjectMethods | TypeImplFlag.AllOperators, IsLargeStructure = true)]
public partial struct CellMap : IBitStatusMap<CellMap, Cell, CellMap.Enumerator>
{
	/// <inheritdoc cref="IBitStatusMap{TSelf, TElement, TEnumerator}.Shifting"/>
	private const int Shifting = 41;


	/// <inheritdoc cref="IBitStatusMap{TSelf, TElement, TEnumerator}.Empty"/>
	public static readonly CellMap Empty = [];

	/// <inheritdoc cref="IBitStatusMap{TSelf, TElement, TEnumerator}.Full"/>
	public static readonly CellMap Full = ~default(CellMap);

	/// <summary>
	/// Indicates the <see cref="Vector128{T}"/> instances to be used for checking shared houses.
	/// </summary>
	/// <seealso cref="Vector128{T}"/>
	private static readonly ReadOnlyMemory<Vector128<long>> SharedHouseConstants = (Vector128<long>[])[
		Vector128.Create(~     0x1C0E07L,            -1L),
		Vector128.Create(~     0xE07038L,            -1L),
		Vector128.Create(~    0x70381C0L,            -1L),
		Vector128.Create(~ 0x7038000000L, ~        0x70L),
		Vector128.Create(~0x181C0000000L, ~       0x381L),
		Vector128.Create(~  0xE00000000L, ~      0x1C0EL),
		Vector128.Create(            -1L, ~ 0x381C0E000L),
		Vector128.Create(            -1L, ~0x1C0E070000L),
		Vector128.Create(            -1L, ~0xE070380000L),
		Vector128.Create(~        0x1FFL,            -1L),
		Vector128.Create(~      0x3FE00L,            -1L),
		Vector128.Create(~    0x7FC0000L,            -1L),
		Vector128.Create(~  0xFF8000000L,            -1L),
		Vector128.Create(~0x1F000000000L, ~         0xFL),
		Vector128.Create(            -1L, ~      0x1FF0L),
		Vector128.Create(            -1L, ~    0x3FE000L),
		Vector128.Create(            -1L, ~  0x7FC00000L),
		Vector128.Create(            -1L, ~0xFF80000000L),
		Vector128.Create(~ 0x1008040201L, ~  0x80402010L),
		Vector128.Create(~ 0x2010080402L, ~ 0x100804020L),
		Vector128.Create(~ 0x4020100804L, ~ 0x201008040L),
		Vector128.Create(~ 0x8040201008L, ~ 0x402010080L),
		Vector128.Create(~0x10080402010L, ~ 0x804020100L),
		Vector128.Create(~  0x100804020L, ~0x1008040201L),
		Vector128.Create(~  0x201008040L, ~0x2010080402L),
		Vector128.Create(~  0x402010080L, ~0x4020100804L),
		Vector128.Create(~  0x804020100L, ~0x8040201008L)
	];

	/// <summary>
	/// Indicates the factor values for property <see cref="SharedHouses"/>.
	/// </summary>
	/// <seealso cref="SharedHouses"/>
	private static readonly HouseMask[] SharedHouseFactorValues = [
		0x1, 0x2, 0x4, 0x8, 0x10, 0x20, 0x40, 0x80, 0x100,
		0x200, 0x400, 0x800, 0x1000, 0x2000, 0x4000, 0x8000, 0x10000, 0x20000,
		0x40000, 0x80000, 0x100000, 0x200000, 0x400000, 0x800000, 0x1000000, 0x2000000, 0x4000000
	];


	/// <summary>
	/// Indicates the internal two <see cref="long"/> values,
	/// which represents 81 bits. <see cref="_high"/> represent the higher
	/// 40 bits and <see cref="_low"/> represents the lower 41 bits, where each bit is:
	/// <list type="table">
	/// <item>
	/// <term><see langword="true"/> bit (1)</term>
	/// <description>The corresponding cell is contained in this collection</description>
	/// </item>
	/// <item>
	/// <term><see langword="false"/> bit (0)</term>
	/// <description>The corresponding cell is not contained in this collection</description>
	/// </item>
	/// </list>
	/// </summary>
	[HashCodeMember]
	private long _high, _low;


	/// <summary>
	/// Initializes a <see cref="CellMap"/> instance via a list of offsets represented as a RxCy notation.
	/// </summary>
	/// <param name="segments">The cell offsets, represented as a RxCy notation.</param>
	[JsonConstructor]
	public CellMap(string[] segments)
	{
		this = [];
		foreach (var segment in segments)
		{
			this |= Parse(segment, new RxCyParser());
		}
	}


	/// <summary>
	/// Determines whether the current list of cells are all lie in an intersection area, i.e. a locked candidates.
	/// </summary>
	public readonly bool IsInIntersection
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Count == 1 || Count <= 3 && PopCount((uint)SharedHouses) == 2;
	}

	/// <summary>
	/// Indicates whether every cell in the current collection cannot see each other.
	/// </summary>
	public readonly bool CanSeeEachOther
	{
		get
		{
			switch (Count)
			{
				case 0 or 1: { return false; }
				case 2: { return InOneHouse(out _); }
				default:
				{
					foreach (ref readonly var pair in this & 2)
					{
						if (pair.InOneHouse(out _))
						{
							return true;
						}
					}
					return false;
				}
			}
		}
	}

	/// <inheritdoc/>
	public int Count { get; private set; }

	/// <inheritdoc/>
	[JsonInclude]
	public readonly string[] StringChunks => this ? ToString().SplitBy(',', ' ') : [];

	/// <summary>
	/// Indicates the mask of block that all cells in this collection spanned.
	/// </summary>
	/// <remarks>
	/// For example, if the cells are <c>[0, 1, 27, 28]</c>, all spanned blocks are 0 and 3, so the return mask is <c>0b000001001</c> (i.e. 9).
	/// </remarks>
	public readonly Mask BlockMask
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			var result = (Mask)0;
			if (this && HousesMap[0]) { result |= 1; }
			if (this && HousesMap[1]) { result |= 2; }
			if (this && HousesMap[2]) { result |= 4; }
			if (this && HousesMap[3]) { result |= 8; }
			if (this && HousesMap[4]) { result |= 16; }
			if (this && HousesMap[5]) { result |= 32; }
			if (this && HousesMap[6]) { result |= 64; }
			if (this && HousesMap[7]) { result |= 128; }
			if (this && HousesMap[8]) { result |= 256; }
			return result;
		}
	}

	/// <summary>
	/// Indicates the mask of row that all cells in this collection spanned.
	/// </summary>
	/// <remarks>
	/// For example, if the cells are <c>[0, 1, 27, 28]</c>, all spanned rows are 0 and 3, so the return mask is <c>0b000001001</c> (i.e. 9).
	/// </remarks>
	public readonly Mask RowMask
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			var result = (Mask)0;
			if (this && HousesMap[9]) { result |= 1; }
			if (this && HousesMap[10]) { result |= 2; }
			if (this && HousesMap[11]) { result |= 4; }
			if (this && HousesMap[12]) { result |= 8; }
			if (this && HousesMap[13]) { result |= 16; }
			if (this && HousesMap[14]) { result |= 32; }
			if (this && HousesMap[15]) { result |= 64; }
			if (this && HousesMap[16]) { result |= 128; }
			if (this && HousesMap[17]) { result |= 256; }
			return result;
		}
	}

	/// <summary>
	/// Indicates the mask of column that all cells in this collection spanned.
	/// </summary>
	/// <remarks>
	/// For example, if the cells are <c>[0, 1, 27, 28]</c>, all spanned columns are 0 and 1, so the return mask is <c>0b000000011</c> (i.e. 3).
	/// </remarks>
	public readonly Mask ColumnMask
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			var result = (Mask)0;
			if (this && HousesMap[18]) { result |= 1; }
			if (this && HousesMap[19]) { result |= 2; }
			if (this && HousesMap[20]) { result |= 4; }
			if (this && HousesMap[21]) { result |= 8; }
			if (this && HousesMap[22]) { result |= 16; }
			if (this && HousesMap[23]) { result |= 32; }
			if (this && HousesMap[24]) { result |= 64; }
			if (this && HousesMap[25]) { result |= 128; }
			if (this && HousesMap[26]) { result |= 256; }
			return result;
		}
	}

	/// <summary>
	/// Indicates the shared line. In other words, the line will contain all cells in this collection.
	/// </summary>
	/// <remarks>
	/// If no shared houses can be found (i.e. return value of property <see cref="SharedHouses"/> is 0),
	/// this property will return <see cref="TrailingZeroCountFallback"/>.
	/// </remarks>
	/// <seealso cref="TrailingZeroCountFallback"/>
	/// <seealso cref="SharedHouses"/>
	public readonly House SharedLine
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => TrailingZeroCount(SharedHouses & ~Grid.MaxCandidatesMask);
	}

	/// <summary>
	/// Indicates all houses shared. This property is used to check all houses that all cells of this instance shared.
	/// For example, if the cells are <c>[0, 1]</c>, the property <see cref="SharedHouses"/> will return
	/// house indices 0 (block 1) and 9 (row 1); however, if cells span two houses or more (e.g. cells <c>[0, 1, 27]</c>),
	/// this property won't contain any houses.
	/// </summary>
	/// <remarks>
	/// The return value will be a <see cref="HouseMask"/> value indicating each houses. Bits set 1 are shared houses.
	/// </remarks>
	public readonly HouseMask SharedHouses
	{
		get
		{
			var result = 0;
			var tempSpan = SharedHouseConstants.Span;
			var bits = Vector128.Create(_low, _high);
			for (var i = 0; i < 27; i++)
			{
				if ((bits & tempSpan[i]) == Vector128<long>.Zero)
				{
					result |= SharedHouseFactorValues[i];
				}
			}
			return result;
		}
	}

	/// <summary>
	/// All houses that the map spanned. This property is used to check all houses that all cells of
	/// this instance spanned. For example, if the cells are <c>[0, 1]</c>, the property
	/// <see cref="Houses"/> will return the house index 0 (block 1), 9 (row 1), 18 (column 1)
	/// and 19 (column 2).
	/// </summary>
	public readonly HouseMask Houses
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (HouseMask)BlockMask | RowMask << 9 | ColumnMask << 18;
	}

	/// <summary>
	/// Try to get the symmetric type of the pattern.
	/// </summary>
	public readonly SymmetricType Symmetry
	{
		get
		{
			foreach (var symmetry in Enum.GetValues<SymmetricType>().AsReadOnlySpan()[1..].EnumerateReversely())
			{
				var isThisSymmetry = true;
				foreach (var cell in this)
				{
					var symmetricCells = symmetry.GetCells(cell);
					if ((this & symmetricCells) != symmetricCells)
					{
						isThisSymmetry = false;
						break;
					}
				}
				if (!isThisSymmetry)
				{
					continue;
				}
				return symmetry;
			}
			return SymmetricType.None;
		}
	}

	/// <summary>
	/// Gets the expanded peers of the current map.
	/// </summary>
	/// <remarks>
	/// An <b>Expanded Peers</b> is a list of cells that contains all peer cells of each cell
	/// appeared in the current collection. For example, if a collection contains cells <c>r1c123</c>,
	/// this collection will be the result of the expression <c>PeersMap[r1c1] | PeersMap[r1c2] | PeersMap[r1c3]</c>,
	/// where the member <c>PeersMap</c> corresponds to the array <see cref="PeersMap"/>.
	/// </remarks>
	/// <seealso cref="PeersMap"/>
	public readonly CellMap ExpandedPeers
	{
		get
		{
			var result = Empty;
			foreach (var cell in Offsets)
			{
				result |= PeersMap[cell];
			}
			return result;
		}
	}

	/// <inheritdoc/>
	public readonly CellMap PeerIntersection
	{
		get
		{
			var (lowerBits, higherBits, i) = (0L, 0L, 0);
			foreach (var offset in Offsets)
			{
				var (low, high) = (0L, 0L);
				foreach (var peer in PeersMap[offset])
				{
					(peer / Shifting == 0 ? ref low : ref high) |= 1L << peer % Shifting;
				}

				if (i++ == 0)
				{
					lowerBits = low;
					higherBits = high;
				}
				else
				{
					lowerBits &= low;
					higherBits &= high;
				}
			}
			return CreateByBits(higherBits, lowerBits);
		}
	}

	/// <summary>
	/// Indicates the cell offsets in this collection.
	/// </summary>
	internal readonly unsafe Cell[] Offsets
	{
		get
		{
			if (!this)
			{
				return [];
			}

			long value;
			int i, pos = 0;
			var arr = new Cell[Count];
			if (_low != 0)
			{
				for (value = _low, i = 0; i < Shifting; i++, value >>= 1)
				{
					if ((value & 1) != 0)
					{
						arr[pos++] = i;
					}
				}
			}
			if (_high != 0)
			{
				for (value = _high, i = Shifting; i < 81; i++, value >>= 1)
				{
					if ((value & 1) != 0)
					{
						arr[pos++] = i;
					}
				}
			}
			return arr;
		}
	}

	/// <inheritdoc/>
	readonly int IBitStatusMap<CellMap, Cell, Enumerator>.Shifting => Shifting;

	/// <inheritdoc/>
	readonly Cell[] IBitStatusMap<CellMap, Cell, Enumerator>.Offsets => Offsets;


	/// <inheritdoc/>
	static Cell IBitStatusMap<CellMap, Cell, Enumerator>.MaxCount => 9 * 9;

	/// <inheritdoc/>
	static ref readonly CellMap IBitStatusMap<CellMap, Cell, Enumerator>.Empty => ref Empty;

	/// <inheritdoc/>
	static ref readonly CellMap IBitStatusMap<CellMap, Cell, Enumerator>.Full => ref Full;


	/// <summary>
	/// Get the offset at the specified position index.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <returns>
	/// The offset at the specified position index. If the value is invalid, the return value will be <c>-1</c>.
	/// </returns>
	public readonly Cell this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			if (!this || index >= Count)
			{
				return -1;
			}

			if (Bmi2.X64.IsSupported)
			{
				// https://stackoverflow.com/questions/7669057/find-nth-set-bit-in-an-int

				return TrailingZeroCount(Bmi2.X64.ParallelBitDeposit(1UL << index, (ulong)_low)) switch
				{
					var low and not TrailingZeroCountFallbackLong => low,
					_ => TrailingZeroCount(Bmi2.X64.ParallelBitDeposit(1UL << index - PopCount((ulong)_low), (ulong)_high)) switch
					{
						var high and not TrailingZeroCountFallbackLong => high + Shifting,
						_ => -1
					}
				};
			}

			return PopCount((ulong)_low) is var popCountLow && popCountLow == index
				? 63 - LeadingZeroCount((ulong)_low)
				: popCountLow > index
					? _low.SetAt(index)
					: _high.SetAt(index - popCountLow) is var z and not TrailingZeroCountFallbackLong ? z + Shifting : -1;
		}
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly unsafe void CopyTo(ref Cell sequence, int length)
	{
		@ref.ThrowIfNullRef(in sequence);

		if (!this)
		{
			return;
		}

		ArgumentOutOfRangeException.ThrowIfGreaterThan(Count, length);

		fixed (Cell* pSequence = &sequence)
		{
			Offsets[..length].AsReadOnlySpan().CopyTo(new(pSequence, length));
		}
	}

	/// <inheritdoc/>
	public readonly void ForEach(Action<Cell> action)
	{
		foreach (var element in this)
		{
			action(element);
		}
	}

	/// <summary>
	/// Indicates whether all cells in this instance are in one house.
	/// </summary>
	/// <param name="houseIndex">
	/// The house index whose corresponding house covered.
	/// If the return value is <see langword="false"/>, this value will be the constant -1.
	/// </param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public readonly bool InOneHouse(out House houseIndex)
	{
		var tempSpan = SharedHouseConstants.Span;
		var bits = Vector128.Create(_low, _high);
		for (var i = 0; i < 27; i++)
		{
			if ((bits & tempSpan[i]) == Vector128<long>.Zero)
			{
				houseIndex = i;
				return true;
			}
		}

		houseIndex = -1;
		return false;
	}

	/// <summary>
	/// Determine whether the map contains the specified offset.
	/// </summary>
	/// <param name="item">The offset.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Contains(Cell item) => ((item < Shifting ? _low : _high) >> item % Shifting & 1) != 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(ref readonly CellMap other) => _low == other._low && _high == other._high;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
	{
		var targetString = ToString(provider);
		if (destination.Length < targetString.Length)
		{
			goto ReturnFalse;
		}

		if (targetString.TryCopyTo(destination))
		{
			charsWritten = targetString.Length;
			return true;
		}

	ReturnFalse:
		charsWritten = 0;
		return false;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly int CompareTo(ref readonly CellMap other)
	{
		return Count > other.Count ? 1 : Count < other.Count ? -1 : -Math.Sign($"{b(in this)}".CompareTo($"{b(in other)}"));


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static string b(ref readonly CellMap f) => f.ToString(new BitmapCellMapFormatInfo());
	}

	/// <inheritdoc/>
	public readonly int IndexOf(Cell offset)
	{
		for (var index = 0; index < Count; index++)
		{
			if (this[index] == offset)
			{
				return index;
			}
		}
		return -1;
	}

	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(IFormatProvider? formatProvider)
		=> formatProvider switch
		{
			CellMapFormatInfo i => i.FormatMap(in this),
			_ => CoordinateConverter.GetConverter(formatProvider).CellConverter(this)
		};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Cell[] ToArray() => Offsets;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Enumerator GetEnumerator() => new(Offsets);

	/// <inheritdoc/>
	public readonly CellMap Slice(int start, int count)
	{
		var (result, offsets) = (Empty, Offsets);
		for (int i = start, end = start + count; i < end; i++)
		{
			result.Add(offsets[i]);
		}
		return result;
	}

	/// <summary>
	/// Add a new <see cref="Cell"/> into the collection.
	/// </summary>
	/// <param name="item">The offset to be added.</param>
	/// <returns>A <see cref="bool"/> value indicating whether the collection has already contained the offset.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Add(Cell item)
	{
		ref var v = ref item / Shifting == 0 ? ref _low : ref _high;
		var older = Contains(item);
		v |= 1L << item % Shifting;
		if (!older)
		{
			Count++;
			return true;
		}
		return false;
	}

	/// <inheritdoc/>
	public int AddRange(scoped ReadOnlySpan<Cell> offsets)
	{
		var result = 0;
		foreach (var offset in offsets)
		{
			if (Add(offset))
			{
				result++;
			}
		}
		return result;
	}

	/// <summary>
	/// Removes the specified offset from the current collection.
	/// </summary>
	/// <param name="item">An offset to be removed.</param>
	/// <returns>A <see cref="bool"/> value indicating whether the collection has already contained the specified offset.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Remove(Cell item)
	{
		ref var v = ref item / Shifting == 0 ? ref _low : ref _high;
		var older = Contains(item);
		v &= ~(1L << item % Shifting);
		if (older)
		{
			Count--;
			return true;
		}
		return false;
	}

	/// <inheritdoc/>
	public int RemoveRange(scoped ReadOnlySpan<Cell> offsets)
	{
		var result = 0;
		foreach (var offset in offsets)
		{
			if (Remove(offset))
			{
				result++;
			}
		}
		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Toggle(Cell offset) => _ = Contains(offset) ? Remove(offset) : Add(offset);

	/// <summary>
	/// Remove all elements stored in the current collection, and set the property <see cref="Count"/> to zero.
	/// </summary>
	/// <seealso cref="Count"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Clear() => this = Empty;

	/// <inheritdoc/>
	readonly bool IEquatable<CellMap>.Equals(CellMap other) => Equals(in other);

	/// <inheritdoc/>
	readonly bool IAnyAllMethod<CellMap, Cell>.Any() => Count != 0;

	/// <inheritdoc/>
	readonly bool IAnyAllMethod<CellMap, Cell>.Any(Func<Cell, bool> predicate) => this.Any(predicate);

	/// <inheritdoc/>
	readonly bool IAnyAllMethod<CellMap, Cell>.All(Func<Cell, bool> predicate) => this.All(predicate);

	/// <inheritdoc/>
	readonly string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString(formatProvider);

	/// <inheritdoc/>
	readonly Cell IFirstLastMethod<CellMap, Cell>.First() => this[0];

	/// <inheritdoc/>
	readonly Cell IFirstLastMethod<CellMap, Cell>.First(Func<Cell, bool> predicate) => this.First(predicate);

	/// <inheritdoc/>
	readonly IEnumerable<Cell> IWhereMethod<CellMap, Cell>.Where(Func<Cell, bool> predicate) => this.Where(predicate);

	/// <inheritdoc/>
	readonly IEnumerable<IGrouping<TKey, Cell>> IGroupByMethod<CellMap, Cell>.GroupBy<TKey>(Func<Cell, TKey> keySelector)
		=> this.GroupBy(keySelector).ToArray().Select(static element => (IGrouping<TKey, Cell>)element);

	/// <inheritdoc/>
	readonly IEnumerable<TResult> ISelectMethod<CellMap, Cell>.Select<TResult>(Func<Cell, TResult> selector) => this.Select(selector).ToArray();

	/// <inheritdoc cref="IParsable{TSelf}.TryParse(string, IFormatProvider?, out TSelf)"/>
	public static bool TryParse(string str, out CellMap result)
	{
		try
		{
			result = Parse(str);
			return true;
		}
		catch (FormatException)
		{
			result = default;
			return false;
		}
	}

	/// <inheritdoc/>
	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out CellMap result)
	{
		try
		{
			if (s is null)
			{
				result = [];
				return false;
			}

			result = Parse(s, provider);
			return true;
		}
		catch (FormatException)
		{
			result = [];
			return false;
		}
	}

	/// <inheritdoc cref="TryParse(ReadOnlySpan{char}, IFormatProvider?, out CellMap)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParse(ReadOnlySpan<char> s, out CellMap result) => TryParse(s, null, out result);

	/// <inheritdoc/>
	public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out CellMap result)
	{
		try
		{
			result = Parse(s, provider);
			return true;
		}
		catch (FormatException)
		{
			result = Empty;
			return false;
		}
	}

	/// <summary>
	/// Creates a <see cref="CellMap"/> instance via the specified cells.
	/// </summary>
	/// <param name="cells">The cells.</param>
	/// <returns>A <see cref="CellMap"/> instance.</returns>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static CellMap Create(ReadOnlySpan<Cell> cells)
	{
		if (cells.IsEmpty)
		{
			return default;
		}

		var result = default(CellMap);
		foreach (var cell in cells)
		{
			result.Add(cell);
		}
		return result;
	}

	/// <summary>
	/// Initializes an instance with two binary values.
	/// </summary>
	/// <param name="high">Higher 40 bits.</param>
	/// <param name="low">Lower 41 bits.</param>
	/// <returns>The result instance created.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap CreateByBits(long high, long low)
	{
		Unsafe.SkipInit<CellMap>(out var result);
		(result._high, result._low, result.Count) = (high, low, PopCount((ulong)high) + PopCount((ulong)low));
		return result;
	}

	/// <summary>
	/// Initializes an instance with three binary values.
	/// </summary>
	/// <param name="high">Higher 27 bits.</param>
	/// <param name="mid">Medium 27 bits.</param>
	/// <param name="low">Lower 27 bits.</param>
	/// <returns>The result instance created.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap CreateByBits(int high, int mid, int low)
		=> CreateByBits((high & 0x7FFFFFFL) << 13 | mid >> 14 & 0x1FFFL, (mid & 0x3FFFL) << 27 | low & 0x7FFFFFFL);

	/// <summary>
	/// Initializes an instance with an <see cref="llong"/> integer.
	/// </summary>
	/// <param name="llong">The <see cref="llong"/> integer.</param>
	/// <returns>The result instance created.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap CreateByInt128(ref readonly llong llong)
		=> CreateByBits((long)(ulong)(llong >> 64), (long)(ulong)(llong & ulong.MaxValue));

	/// <inheritdoc/>
	public static CellMap Parse(string str)
	{
		foreach (var parser in
			from element in Enum.GetValues<CoordinateType>()
			let parser = element.GetParser()
			where parser is not null
			select parser)
		{
			if (parser.CellParser(str) is { Count: not 0 } result)
			{
				return result;
			}
		}
		throw new FormatException(SR.ExceptionMessage("StringValueInvalidToBeParsed"));
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap Parse(string s, IFormatProvider? provider)
		=> provider switch
		{
			CellMapFormatInfo i => i.ParseMap(s),
			_ => CoordinateParser.GetParser(provider).CellParser(s)
		};

	/// <inheritdoc cref="Parse(ReadOnlySpan{char}, IFormatProvider?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap Parse(ReadOnlySpan<char> s) => Parse(s, null);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s.ToString(), provider);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !(in CellMap offsets) => offsets.Count == 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator true(in CellMap value) => value.Count != 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator false(in CellMap value) => value.Count == 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap operator ~(in CellMap offsets)
		=> CreateByBits(~offsets._high & 0xFF_FFFF_FFFFL, ~offsets._low & 0x1FF_FFFF_FFFFL);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap operator +(in CellMap collection, Cell offset)
	{
		var result = collection;
		result.Add(offset);
		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap operator -(in CellMap collection, Cell offset)
	{
		var result = collection;
		result.Remove(offset);
		return result;
	}

	/// <summary>
	/// <inheritdoc cref="CandidateMap.op_Modulus(in CandidateMap, in CandidateMap)" path="/summary"/>
	/// </summary>
	/// <param name="base">
	/// <inheritdoc cref="CandidateMap.op_Modulus(in CandidateMap, in CandidateMap)" path="/param[@name='base']"/>
	/// </param>
	/// <param name="template">
	/// <inheritdoc cref="CandidateMap.op_Modulus(in CandidateMap, in CandidateMap)" path="/param[@name='template']"/>
	/// </param>
	/// <returns><inheritdoc cref="CandidateMap.op_Modulus(in CandidateMap, in CandidateMap)" path="/returns"/></returns>
	/// <remarks>
	/// <para>
	/// The operator is commonly used for checking eliminations, especially in type 2 of deadly patterns. 
	/// </para>
	/// <para>
	/// For example, if we should check the eliminations
	/// of digit <c>d</c>, we may use the expression
	/// <code><![CDATA[
	/// (urCells & grid.CandidatesMap[d]).PeerIntersection & grid.CandidatesMap[d]
	/// ]]></code>
	/// to express the eliminations are the peer intersection of cells of digit <c>d</c>
	/// appeared in <c>urCells</c>. This expression can be simplified to
	/// <code><![CDATA[
	/// urCells % grid.CandidatesMap[d]
	/// ]]></code>
	/// </para>
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap operator %(in CellMap @base, in CellMap template) => (@base & template).PeerIntersection & template;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap operator &(in CellMap left, in CellMap right)
		=> CreateByBits(left._high & right._high, left._low & right._low);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap operator |(in CellMap left, in CellMap right)
		=> CreateByBits(left._high | right._high, left._low | right._low);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap operator ^(in CellMap left, in CellMap right)
		=> CreateByBits(left._high ^ right._high, left._low ^ right._low);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static unsafe ReadOnlySpan<CellMap> operator &(in CellMap map, int subsetSize)
	{
		if (subsetSize == 0 || subsetSize > map.Count)
		{
			return [];
		}

		if (subsetSize == map.Count)
		{
			return (CellMap[])[map];
		}

		var n = map.Count;
		var buffer = stackalloc int[subsetSize];
		if (n <= MaxLimit && subsetSize <= MaxLimit)
		{
			// Optimization: Use table to get the total number of result elements.
			var totalIndex = 0;
			var result = new CellMap[Combinatorial.PascalTriangle[n - 1][subsetSize - 1]];
			enumerate(result, subsetSize, n, subsetSize, map.Offsets, (r, c) => r[totalIndex++] = c.AsCellMap());
			return result;
		}
		else
		{
			if (n > MaxLimit && subsetSize > MaxLimit)
			{
				throw new NotSupportedException(SR.ExceptionMessage("SubsetsExceeded"));
			}
			var result = new List<CellMap>();
			enumerate(result, subsetSize, n, subsetSize, map.Offsets, (r, c) => r.AddRef(c.AsCellMap()));
			return result.AsReadOnlySpan();
		}


		void enumerate<T>(T result, int size, int last, int index, Cell[] offsets, CollectionAddingHandler<T> addingAction)
		{
			for (var i = last; i >= index; i--)
			{
				buffer[index - 1] = i - 1;
				if (index > 1)
				{
					enumerate(result, size, i - 1, index - 1, offsets, addingAction);
				}
				else
				{
					var temp = new Cell[size];
					for (var j = 0; j < size; j++)
					{
						temp[j] = offsets[buffer[j]];
					}
					addingAction(result, temp);
				}
			}
		}
	}

	/// <inheritdoc/>
	public static ReadOnlySpan<CellMap> operator |(in CellMap map, int subsetSize)
	{
		if (subsetSize == 0 || !map)
		{
			return [];
		}

		var (n, desiredSize) = (map.Count, 0);
		var length = Math.Min(n, subsetSize);
		for (var i = 1; i <= length; i++)
		{
			desiredSize += Combinatorial.PascalTriangle[n - 1][i - 1];
		}

		var result = new List<CellMap>(desiredSize);
		for (var i = 1; i <= length; i++)
		{
			result.AddRangeRef(map & i);
		}
		return result.AsReadOnlySpan();
	}

	/// <inheritdoc/>
	public static ReadOnlySpan<CellMap> operator |(in CellMap map, Range subsetSizeRange)
	{
		if (!map)
		{
			return [];
		}

		var (s, e) = subsetSizeRange;
		var result = new List<CellMap>();
		for (var i = s.GetOffset(map.Count); i <= e.GetOffset(map.Count); i++)
		{
			result.AddRangeRef(map & i);
		}
		return result.AsReadOnlySpan();
	}

	/// <summary>
	/// Expands the current <see cref="CellMap"/> instance, inserting into a <see cref="CandidateMap"/> instance by specified digit.
	/// </summary>
	/// <param name="cells">The cells to be checked.</param>
	/// <param name="digit">The digit to be checked.</param>
	/// <returns>A <see cref="CandidateMap"/> instance.</returns>
	public static CandidateMap operator *(in CellMap cells, Digit digit)
	{
		var result = CandidateMap.Empty;
		foreach (var cell in cells.Offsets)
		{
			result.Add(cell * 9 + digit);
		}
		return result;
	}

	/// <summary>
	/// Reduces the <see cref="CellMap"/> instances, only checks for cells in the specified cells, and merge into a <see cref="Mask"/> value.
	/// </summary>
	/// <param name="cells">The cells to be checked.</param>
	/// <param name="house">The house to be checked.</param>
	/// <returns>A <see cref="Mask"/> instance.</returns>
	public static Mask operator /(in CellMap cells, House house)
	{
		var (result, i) = ((Mask)0, 0);
		foreach (var cell in HousesCells[house])
		{
			if (cells.Contains(cell))
			{
				result |= (Mask)(1 << i);
			}
			i++;
		}
		return result;
	}


	/// <summary>
	/// Implicit cast from a <see cref="CellMap"/> instance into a <see cref="llong"/> result.
	/// </summary>
	/// <param name="this">A <see cref="CellMap"/> instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator llong(in CellMap @this) => new((ulong)@this._high, (ulong)@this._low);

	/// <summary>
	/// Implicit cast from a <see cref="llong"/> value into a <see cref="CellMap"/> instance.
	/// </summary>
	/// <param name="value">A <see cref="llong"/> value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator CellMap(llong value) => CreateByInt128(in value);


	/// <summary>
	/// Explicit cast from a <see cref="Cell"/> array into a <see cref="CellMap"/> instance.
	/// </summary>
	/// <param name="cells">An array of <see cref="Cell"/> values.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator CellMap(Cell[] cells) => cells.AsCellMap();
}

/// <summary>
/// Indicates the JSON converter of <see cref="CellMap"/> instance.
/// </summary>
/// <seealso cref="CellMap"/>
file sealed class Converter : JsonConverter<CellMap>
{
	/// <inheritdoc/>
	public override bool HandleNull => false;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override CellMap Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> new(JsonSerializer.Deserialize<string[]>(ref reader, options)!);

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, CellMap value, JsonSerializerOptions options)
	{
		writer.WriteStartArray();
		foreach (var element in value.StringChunks)
		{
			writer.WriteStringValue(element);
		}
		writer.WriteEndArray();
	}
}
