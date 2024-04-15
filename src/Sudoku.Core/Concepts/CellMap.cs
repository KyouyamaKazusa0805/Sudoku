namespace Sudoku.Concepts;

/// <summary>
/// Encapsulates a binary series of cell state table.
/// </summary>
/// <remarks>
/// <include file="../../global-doc-comments.xml" path="/g/large-structure"/>
/// </remarks>
[JsonConverter(typeof(CellMapConverter))]
[StructLayout(LayoutKind.Auto)]
[CollectionBuilder(typeof(CellMap), nameof(Create))]
[DebuggerStepThrough]
[LargeStructure]
[Equals]
[GetHashCode]
[EqualityOperators]
[ComparisonOperators]
public partial struct CellMap :
	IAdditionOperators<CellMap, Cell, CellMap>,
	IBitStatusMap<CellMap, Cell, CellMap.Enumerator>,
	IComparable<CellMap>,
	ICoordinateObject<CellMap>,
	IComparisonOperators<CellMap, CellMap, bool>,
	IDivisionOperators<CellMap, House, Mask>,
	IMultiplyOperators<CellMap, Digit, CandidateMap>,
	ISubtractionOperators<CellMap, Cell, CellMap>,
	ITokenizable<CellMap>
{
	/// <inheritdoc cref="IBitStatusMap{TSelf, TElement, TEnumerator}.Shifting"/>
	private const int Shifting = 41;


	/// <inheritdoc cref="IMinMaxValue{TSelf}.MinValue"/>
	public static readonly CellMap MinValue = [];

	/// <inheritdoc cref="IMinMaxValue{TSelf}.MaxValue"/>
	public static readonly CellMap MaxValue = ~default(CellMap);

	/// <summary>
	/// The internal cell map parser.
	/// </summary>
	private static readonly BitStatusCellMapParser CellMapParser = new();

	/// <summary>
	/// The internal cell map converter.
	/// </summary>
	private static readonly BitStatusCellMapConverter CellMapConverter = new();


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
			this |= ParseExact(segment, new RxCyParser());
		}
	}


	/// <summary>
	/// Determines whether the current list of cells are all lie in an intersection area, i.e. a locked candidates.
	/// </summary>
	public readonly bool IsInIntersection
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _count <= 3 && PopCount((uint)SharedHouses) == 2;
	}

	/// <summary>
	/// Indicates whether every cell in the current collection cannot see each other.
	/// </summary>
	public readonly bool CanSeeEachOther
	{
		get
		{
			switch (_count)
			{
				case 0 or 1: { return false; }
				case 2: { return InOneHouse(out _); }
				default:
				{
					foreach (ref readonly var pair in GetSubsets(2))
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
	[ImplicitField(RequiredReadOnlyModifier = false)]
	public readonly int Count
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _count;
	}

	/// <inheritdoc/>
	public readonly string Token
	{
		get
		{
			var convertedString = CellMapConverter.Converter(in this);
			var bits = convertedString.CutOfLength(27);
			var sb = new StringBuilder(18);
			foreach (var z in (sextuple(getInteger(bits[2])), sextuple(getInteger(bits[1])), sextuple(getInteger(bits[0]))))
			{
				foreach (var element in z)
				{
					sb.Append(Grid.Base32CharSpan[element]);
				}
			}
			return sb.ToString();


			static int getInteger(string bits)
			{
				var result = 0;
				for (var i = 0; i < 27; i++)
				{
					if (bits[i] == '1')
					{
						result |= 1 << i;
					}
				}

				return result;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static int[] sextuple(int value)
				=> [value >> 25 & 3, value >> 20 & 31, value >> 15 & 31, value >> 10 & 31, value >> 5 & 31, value & 31];
		}
	}

	/// <inheritdoc/>
	[JsonInclude]
	public readonly string[] StringChunks => this ? ToString(GlobalizedConverter.InvariantCultureConverter).SplitBy(',', ' ') : [];

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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			var z = 0;

#pragma warning disable format
			if ((_high &            -1L) == 0 && (_low & ~     0x1C0E07L) == 0) { z |=       0x1; }
			if ((_high &            -1L) == 0 && (_low & ~     0xE07038L) == 0) { z |=       0x2; }
			if ((_high &            -1L) == 0 && (_low & ~    0x70381C0L) == 0) { z |=       0x4; }
			if ((_high & ~        0x70L) == 0 && (_low & ~ 0x7038000000L) == 0) { z |=       0x8; }
			if ((_high & ~       0x381L) == 0 && (_low & ~0x181C0000000L) == 0) { z |=      0x10; }
			if ((_high & ~      0x1C0EL) == 0 && (_low & ~  0xE00000000L) == 0) { z |=      0x20; }
			if ((_high & ~ 0x381C0E000L) == 0 && (_low &             -1L) == 0) { z |=      0x40; }
			if ((_high & ~0x1C0E070000L) == 0 && (_low &             -1L) == 0) { z |=      0x80; }
			if ((_high & ~0xE070380000L) == 0 && (_low &             -1L) == 0) { z |=     0x100; }
			if ((_high &            -1L) == 0 && (_low & ~        0x1FFL) == 0) { z |=     0x200; }
			if ((_high &            -1L) == 0 && (_low & ~      0x3FE00L) == 0) { z |=     0x400; }
			if ((_high &            -1L) == 0 && (_low & ~    0x7FC0000L) == 0) { z |=     0x800; }
			if ((_high &            -1L) == 0 && (_low & ~  0xFF8000000L) == 0) { z |=    0x1000; }
			if ((_high & ~         0xFL) == 0 && (_low & ~0x1F000000000L) == 0) { z |=    0x2000; }
			if ((_high & ~      0x1FF0L) == 0 && (_low &             -1L) == 0) { z |=    0x4000; }
			if ((_high & ~    0x3FE000L) == 0 && (_low &             -1L) == 0) { z |=    0x8000; }
			if ((_high & ~  0x7FC00000L) == 0 && (_low &             -1L) == 0) { z |=   0x10000; }
			if ((_high & ~0xFF80000000L) == 0 && (_low &             -1L) == 0) { z |=   0x20000; }
			if ((_high & ~  0x80402010L) == 0 && (_low & ~ 0x1008040201L) == 0) { z |=   0x40000; }
			if ((_high & ~ 0x100804020L) == 0 && (_low & ~ 0x2010080402L) == 0) { z |=   0x80000; }
			if ((_high & ~ 0x201008040L) == 0 && (_low & ~ 0x4020100804L) == 0) { z |=  0x100000; }
			if ((_high & ~ 0x402010080L) == 0 && (_low & ~ 0x8040201008L) == 0) { z |=  0x200000; }
			if ((_high & ~ 0x804020100L) == 0 && (_low & ~0x10080402010L) == 0) { z |=  0x400000; }
			if ((_high & ~0x1008040201L) == 0 && (_low & ~  0x100804020L) == 0) { z |=  0x800000; }
			if ((_high & ~0x2010080402L) == 0 && (_low & ~  0x201008040L) == 0) { z |= 0x1000000; }
			if ((_high & ~0x4020100804L) == 0 && (_low & ~  0x402010080L) == 0) { z |= 0x2000000; }
			if ((_high & ~0x8040201008L) == 0 && (_low & ~  0x804020100L) == 0) { z |= 0x4000000; }
#pragma warning restore format

			return z;
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
			var result = (CellMap)[];
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
	internal readonly Cell[] Offsets
	{
		get
		{
			if (!this)
			{
				return [];
			}

			long value;
			int i, pos = 0;
			var arr = new Cell[_count];
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
	public static ref readonly CellMap NullRef => ref Ref.MakeNullReference<CellMap>();

	/// <inheritdoc/>
	static Cell IBitStatusMap<CellMap, Cell, Enumerator>.MaxCount => 9 * 9;

	/// <inheritdoc/>
	static CellMap IMinMaxValue<CellMap>.MaxValue => MaxValue;

	/// <inheritdoc/>
	static CellMap IMinMaxValue<CellMap>.MinValue => MinValue;


	/// <inheritdoc/>
	[IndexerName("CellIndex")]
	public readonly Cell this[int index]
	{
		get
		{
			if (!this || index >= _count)
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


	/// <inheritdoc/>
	public readonly void CopyTo(scoped ref Cell sequence, int length)
	{
		ArgumentNullException.ThrowIfNull(sequence);

		if (!this)
		{
			return;
		}

		ArgumentOutOfRangeException.ThrowIfGreaterThan(_count, length);

		long value;
		int i, pos = 0;
		if (_low != 0)
		{
			for (value = _low, i = 0; i < Shifting; i++, value >>= 1)
			{
				if ((value & 1) != 0)
				{
					Unsafe.Add(ref sequence, pos++) = i;
				}
			}
		}
		if (_high != 0)
		{
			for (value = _high, i = Shifting; i < 81; i++, value >>= 1)
			{
				if ((value & 1) != 0)
				{
					Unsafe.Add(ref sequence, pos++) = i;
				}
			}
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
#pragma warning disable format
		if ((_high &            -1L) == 0 && (_low & ~     0x1C0E07L) == 0) { houseIndex =  0; return true; }
		if ((_high &            -1L) == 0 && (_low & ~     0xE07038L) == 0) { houseIndex =  1; return true; }
		if ((_high &            -1L) == 0 && (_low & ~    0x70381C0L) == 0) { houseIndex =  2; return true; }
		if ((_high & ~        0x70L) == 0 && (_low & ~ 0x7038000000L) == 0) { houseIndex =  3; return true; }
		if ((_high & ~       0x381L) == 0 && (_low & ~0x181C0000000L) == 0) { houseIndex =  4; return true; }
		if ((_high & ~      0x1C0EL) == 0 && (_low & ~  0xE00000000L) == 0) { houseIndex =  5; return true; }
		if ((_high & ~ 0x381C0E000L) == 0 && (_low &             -1L) == 0) { houseIndex =  6; return true; }
		if ((_high & ~0x1C0E070000L) == 0 && (_low &             -1L) == 0) { houseIndex =  7; return true; }
		if ((_high & ~0xE070380000L) == 0 && (_low &             -1L) == 0) { houseIndex =  8; return true; }
		if ((_high &            -1L) == 0 && (_low & ~        0x1FFL) == 0) { houseIndex =  9; return true; }
		if ((_high &            -1L) == 0 && (_low & ~      0x3FE00L) == 0) { houseIndex = 10; return true; }
		if ((_high &            -1L) == 0 && (_low & ~    0x7FC0000L) == 0) { houseIndex = 11; return true; }
		if ((_high &            -1L) == 0 && (_low & ~  0xFF8000000L) == 0) { houseIndex = 12; return true; }
		if ((_high & ~         0xFL) == 0 && (_low & ~0x1F000000000L) == 0) { houseIndex = 13; return true; }
		if ((_high & ~      0x1FF0L) == 0 && (_low &             -1L) == 0) { houseIndex = 14; return true; }
		if ((_high & ~    0x3FE000L) == 0 && (_low &             -1L) == 0) { houseIndex = 15; return true; }
		if ((_high & ~  0x7FC00000L) == 0 && (_low &             -1L) == 0) { houseIndex = 16; return true; }
		if ((_high & ~0xFF80000000L) == 0 && (_low &             -1L) == 0) { houseIndex = 17; return true; }
		if ((_high & ~  0x80402010L) == 0 && (_low & ~ 0x1008040201L) == 0) { houseIndex = 18; return true; }
		if ((_high & ~ 0x100804020L) == 0 && (_low & ~ 0x2010080402L) == 0) { houseIndex = 19; return true; }
		if ((_high & ~ 0x201008040L) == 0 && (_low & ~ 0x4020100804L) == 0) { houseIndex = 20; return true; }
		if ((_high & ~ 0x402010080L) == 0 && (_low & ~ 0x8040201008L) == 0) { houseIndex = 21; return true; }
		if ((_high & ~ 0x804020100L) == 0 && (_low & ~0x10080402010L) == 0) { houseIndex = 22; return true; }
		if ((_high & ~0x1008040201L) == 0 && (_low & ~  0x100804020L) == 0) { houseIndex = 23; return true; }
		if ((_high & ~0x2010080402L) == 0 && (_low & ~  0x201008040L) == 0) { houseIndex = 24; return true; }
		if ((_high & ~0x4020100804L) == 0 && (_low & ~  0x402010080L) == 0) { houseIndex = 25; return true; }
		if ((_high & ~0x8040201008L) == 0 && (_low & ~  0x804020100L) == 0) { houseIndex = 26; return true; }
#pragma warning restore format

		houseIndex = -1;
		return false;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Contains(Cell offset) => ((offset < Shifting ? _low : _high) >> offset % Shifting & 1) != 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[ExplicitInterfaceImpl(typeof(IEquatable<>))]
	public readonly bool Equals(scoped ref readonly CellMap other) => _low == other._low && _high == other._high;

	/// <summary>
	/// <inheritdoc cref="IComparable{TSelf}.CompareTo(TSelf)" path="/summary"/>
	/// </summary>
	/// <param name="other">
	/// <inheritdoc cref="IComparable{TSelf}.CompareTo(TSelf)" path="/param[@name='other']"/>
	/// </param>
	/// <returns>
	/// The result value only contains 3 possible values: 1, 0 and -1. The comparison rule is:
	/// <list type="number">
	/// <item>
	/// If <see langword="this"/> holds more cells than <paramref name="other"/>, then return 1
	/// indicating <see langword="this"/> is greater.
	/// </item>
	/// <item>
	/// If <see langword="this"/> holds less cells than <paramref name="other"/>, then return -1
	/// indicating <paramref name="other"/> is greater.
	/// </item>
	/// <item>
	/// If they two hold same cells, then checks for indices held:
	/// <list type="bullet">
	/// <item>
	/// If <see langword="this"/> holds a cell whose index is greater than all cells appeared in <paramref name="other"/>,
	/// then return 1 indicating <see langword="this"/> is greater.
	/// </item>
	/// <item>
	/// If <paramref name="other"/> holds a cell whose index is greater than all cells
	/// appeared in <paramref name="other"/>, then return -1 indicating <paramref name="other"/> is greater.
	/// </item>
	/// </list>
	/// </item>
	/// </list>
	/// If all rules are compared, but they are still considered equal, then return 0.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[ExplicitInterfaceImpl(typeof(IComparable<>))]
	public readonly int CompareTo(scoped ref readonly CellMap other)
	{
		var b = new BitStatusCellMapConverter().Converter;
		return _count > other._count ? 1 : _count < other._count ? -1 : Math.Sign($"{b(in this)}".CompareTo($"{b(in other)}"));
	}

	/// <inheritdoc/>
	public readonly int IndexOf(Cell offset)
	{
		for (var index = 0; index < _count; index++)
		{
			if (this[index] == offset)
			{
				return index;
			}
		}

		return -1;
	}

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly string ToString() => ToString(GlobalizedConverter.InvariantCultureConverter);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(CultureInfo? culture = null) => ToString(GlobalizedConverter.GetConverter(culture ?? CultureInfo.CurrentUICulture));

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(CoordinateConverter converter) => converter.CellConverter(this);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Cell[] ToArray() => Offsets;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Enumerator GetEnumerator() => new(Offsets);

	/// <inheritdoc/>
	public readonly CellMap Slice(int start, int count)
	{
		var (result, offsets) = ((CellMap)[], Offsets);
		for (int i = start, end = start + count; i < end; i++)
		{
			result.Add(offsets[i]);
		}

		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly CellMap RandomSelect(int count)
	{
		var result = Offsets[..];
		Random.Shared.Shuffle(result);
		return [.. result[..count]];
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly unsafe ReadOnlySpan<CellMap> GetSubsets(int subsetSize)
	{
		if (subsetSize == 0 || subsetSize > _count)
		{
			return [];
		}

		if (subsetSize == _count)
		{
			return (CellMap[])[this];
		}

		var n = _count;
		var buffer = stackalloc int[subsetSize];
		if (n <= 30 && subsetSize <= 30)
		{
			// Optimization: Use table to get the total number of result elements.
			var totalIndex = 0;
			var result = new CellMap[PascalTriangle[n - 1][subsetSize - 1]];
			enumerateWithLimit(subsetSize, n, subsetSize, Offsets);
			return result;


			void enumerateWithLimit(int size, int last, int index, Cell[] offsets)
			{
				for (var i = last; i >= index; i--)
				{
					buffer[index - 1] = i - 1;
					if (index > 1)
					{
						enumerateWithLimit(size, i - 1, index - 1, offsets);
					}
					else
					{
						var temp = new Cell[size];
						for (var j = 0; j < size; j++)
						{
							temp[j] = offsets[buffer[j]];
						}

						result[totalIndex++] = (CellMap)temp;
					}
				}
			}
		}
		else
		{
			if (n > 30 && subsetSize > 30)
			{
				throw new NotSupportedException(ResourceDictionary.ExceptionMessage("SubsetsExceeded"));
			}
			var result = new List<CellMap>();
			enumerateWithoutLimit(subsetSize, n, subsetSize, Offsets);
			return result.AsReadOnlySpan();


			void enumerateWithoutLimit(int size, int last, int index, Cell[] offsets)
			{
				for (var i = last; i >= index; i--)
				{
					buffer[index - 1] = i - 1;
					if (index > 1)
					{
						enumerateWithoutLimit(size, i - 1, index - 1, offsets);
					}
					else
					{
						var temp = new Cell[size];
						for (var j = 0; j < size; j++)
						{
							temp[j] = offsets[buffer[j]];
						}

						result.AddRef((CellMap)temp);
					}
				}
			}
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly ReadOnlySpan<CellMap> GetSubsetsAll() => GetSubsetsAllBelow(_count);

	/// <inheritdoc/>
	public readonly ReadOnlySpan<CellMap> GetSubsetsAllBelow(int limitSubsetSize)
	{
		if (limitSubsetSize == 0 || !this)
		{
			return [];
		}

		var (n, desiredSize) = (_count, 0);
		var length = Math.Min(n, limitSubsetSize);
		for (var i = 1; i <= length; i++)
		{
			desiredSize += PascalTriangle[n - 1][i - 1];
		}

		var result = new List<CellMap>(desiredSize);
		for (var i = 1; i <= length; i++)
		{
			result.AddRangeRef(GetSubsets(i));
		}

		return result.AsReadOnlySpan();
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Add(Cell offset)
	{
		scoped ref var v = ref offset / Shifting == 0 ? ref _low : ref _high;
		var older = Contains(offset);
		v |= 1L << offset % Shifting;
		if (!older)
		{
			_count++;
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

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Remove(Cell offset)
	{
		scoped ref var v = ref offset / Shifting == 0 ? ref _low : ref _high;
		var older = Contains(offset);
		v &= ~(1L << offset % Shifting);
		if (older)
		{
			_count--;
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
	public void Clear() => this = default;

	/// <inheritdoc/>
	void IBitStatusMap<CellMap, Cell, Enumerator>.ExceptWith(IEnumerable<Cell> other)
	{
		foreach (var element in other)
		{
			Remove(element);
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void IBitStatusMap<CellMap, Cell, Enumerator>.IntersectWith(IEnumerable<Cell> other) => this &= [.. other];

	/// <inheritdoc/>
	void IBitStatusMap<CellMap, Cell, Enumerator>.SymmetricExceptWith(IEnumerable<Cell> other)
	{
		var left = this;
		foreach (var element in other)
		{
			left.Remove(element);
		}

		var right = [.. other] - this;
		this = left | right;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void IBitStatusMap<CellMap, Cell, Enumerator>.UnionWith(IEnumerable<Cell> other) => this |= [.. other];


	/// <inheritdoc/>
	public static bool TryParse(string str, out CellMap result)
	{
		try
		{
			result = Parse(str);
			return true;
		}
		catch
		{
			result = [];
			return false;
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap Create(string token)
		=> token.Length switch
		{
			18 => CellMapParser.Parser(
				string.Concat(
					from i in Digits[..3]
					let segment = Grid.GetDigitViaToken(token[(i * 6)..((i + 1) * 6)]).ToString()
					let binary = Convert.ToString(int.Parse(segment), 2)
					select binary.PadLeft(27, '0')
				)
			),
			_ => throw new FormatException(string.Format(ResourceDictionary.ExceptionMessage("LengthMustBeMatched"), 18))
		};

	/// <summary>
	/// Creates a <see cref="CellMap"/> instance via the specified cells.
	/// </summary>
	/// <param name="cells">The cells.</param>
	/// <returns>A <see cref="CellMap"/> instance.</returns>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static CellMap Create(scoped ReadOnlySpan<Cell> cells)
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
		CellMap result;
		(result._high, result._low, result._count) = (high, low, PopCount((ulong)high) + PopCount((ulong)low));

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
	public static CellMap CreateByInt128(scoped ref readonly llong llong)
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

		throw new FormatException(ResourceDictionary.ExceptionMessage("StringValueInvalidToBeParsed"));
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap ParseExact(string str, CoordinateParser parser) => parser.CellParser(str);

	/// <inheritdoc/>
	static bool IParsable<CellMap>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out CellMap result)
	{
		try
		{
			if (s is null)
			{
				goto ReturnFalse;
			}

			return TryParse(s, out result);
		}
		catch
		{
		}

	ReturnFalse:
		Unsafe.SkipInit(out result);
		return false;
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !(scoped in CellMap offsets) => offsets._count == 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator true(scoped in CellMap value) => value._count != 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator false(scoped in CellMap value) => value._count == 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap operator ~(scoped in CellMap offsets)
		=> CreateByBits(~offsets._high & 0xFF_FFFF_FFFFL, ~offsets._low & 0x1FF_FFFF_FFFFL);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap operator +(scoped in CellMap collection, Cell offset)
	{
		var result = collection;
		if (result.Contains(offset))
		{
			return result;
		}

		(offset / Shifting == 0 ? ref result._low : ref result._high) |= 1L << offset % Shifting;
		result._count++;
		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap operator -(scoped in CellMap collection, Cell offset)
	{
		var result = collection;
		if (!result.Contains(offset))
		{
			return collection;
		}

		(offset / Shifting == 0 ? ref result._low : ref result._high) &= ~(1L << offset % Shifting);
		result._count--;
		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap operator -(scoped in CellMap left, scoped in CellMap right) => left & ~right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap operator &(scoped in CellMap left, scoped in CellMap right)
		=> CreateByBits(left._high & right._high, left._low & right._low);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap operator |(scoped in CellMap left, scoped in CellMap right)
		=> CreateByBits(left._high | right._high, left._low | right._low);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap operator ^(scoped in CellMap left, scoped in CellMap right)
		=> CreateByBits(left._high ^ right._high, left._low ^ right._low);

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
	public static CellMap operator %(scoped in CellMap @base, scoped in CellMap template) => (@base & template).PeerIntersection & template;

	/// <summary>
	/// Expands via the specified digit.
	/// </summary>
	/// <param name="base">The base map.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>The result instance.</returns>
	[ExplicitInterfaceImpl(typeof(IMultiplyOperators<,,>))]
	public static CandidateMap operator *(scoped in CellMap @base, Digit digit)
	{
		var result = (CandidateMap)[];
		foreach (var cell in @base.Offsets)
		{
			result.Add(cell * 9 + digit);
		}

		return result;
	}

	/// <summary>
	/// Get the sub-view mask of this map.
	/// </summary>
	/// <param name="map">The map.</param>
	/// <param name="houseIndex">The house index.</param>
	/// <returns>The mask.</returns>
	[ExplicitInterfaceImpl(typeof(IDivisionOperators<,,>))]
	public static Mask operator /(scoped in CellMap map, House houseIndex)
	{
		var (p, i) = ((Mask)0, 0);
		foreach (var cell in HousesCells[houseIndex])
		{
			if (map.Contains(cell))
			{
				p |= (Mask)(1 << i);
			}

			i++;
		}

		return p;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CellMap IAdditionOperators<CellMap, Cell, CellMap>.operator +(CellMap left, Cell right) => left + right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CellMap ISubtractionOperators<CellMap, Cell, CellMap>.operator -(CellMap left, Cell right) => left - right;


	/// <summary>
	/// Implicit cast from a <see cref="CellMap"/> instance into a <see cref="llong"/> result.
	/// </summary>
	/// <param name="this">A <see cref="CellMap"/> instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator llong(scoped in CellMap @this) => new((ulong)@this._high, (ulong)@this._low);

	/// <summary>
	/// Implicit cast from a <see cref="llong"/> value into a <see cref="CellMap"/> instance.
	/// </summary>
	/// <param name="value">A <see cref="llong"/> value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator CellMap(llong value) => CreateByInt128(in value);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator CellMap(Cell[] offsets) => [.. offsets];

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator CellMap(scoped ReadOnlySpan<Cell> offsets) => [.. offsets];
}
