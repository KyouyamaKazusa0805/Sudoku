#pragma warning disable IDE0032, IDE0064
namespace Sudoku.Concepts;

/// <summary>
/// Encapsulates a binary series of cell status table.
/// </summary>
/// <remarks>
/// This type holds a <see langword="static readonly"/> field called <see cref="Empty"/>,
/// it is the only field provided to be used as the entry to create or update collection.
/// If you want to add elements into it, you can use <see cref="Add(int)"/>, <see cref="AddRange(IEnumerable{int})"/>
/// or just <see cref="op_Addition(in CellMap, int)"/> or <see cref="op_Addition(in CellMap, IEnumerable{int})"/>:
/// <code><![CDATA[
/// var cellMap = CellMap.Empty;
/// cellMap += 0; // Adds 'r1c1' into the collection.
/// cellMap.Add(1); // Adds 'r1c2' into the collection.
/// cellMap.AddRange(stackalloc[] { 2, 3, 4 }); // Adds 'r1c345' into the collection.
/// cellMap |= anotherCellMap; // Adds a list of another instance of type 'CellMap' into the current collection.
/// ]]></code>
/// If you want to learn more information about this type, please visit
/// <see href="https://sunnieshine.github.io/Sudoku/data-structures/cells">this wiki page</see>.
/// </remarks>
[IsLargeStruct]
[JsonConverter(typeof(Converter))]
[GeneratedOverloadingOperator(GeneratedOperator.EqualityOperators | GeneratedOperator.ComparisonOperators)]
public unsafe partial struct CellMap :
	IAdditionOperators<CellMap, int, CellMap>,
	IAdditionOperators<CellMap, IEnumerable<int>, CellMap>,
	IComparable,
	IComparable<CellMap>,
	IComparisonOperators<CellMap, CellMap, bool>,
	IDivisionOperators<CellMap, int, short>,
	IMultiplyOperators<CellMap, int, CandidateMap>,
	ISubtractionOperators<CellMap, int, CellMap>,
	ISubtractionOperators<CellMap, IEnumerable<int>, CellMap>,
	IBitStatusMap<CellMap>
{
	/// <inheritdoc cref="IBitStatusMap{TSelf}.Shifting"/>
	private const int Shifting = 41;


	/// <inheritdoc cref="IBitStatusMap{TSelf}.Empty"/>
	public static readonly CellMap Empty;

	/// <inheritdoc cref="IMinMaxValue{TSelf}.MaxValue"/>
	public static readonly CellMap MaxValue = ~default(CellMap);

	/// <inheritdoc cref="IMinMaxValue{TSelf}.MinValue"/>
	/// <remarks>
	/// This value is equivalent to <see cref="Empty"/>.
	/// </remarks>
	public static readonly CellMap MinValue;


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
	internal long _high, _low;

	/// <summary>
	/// The background field of the property <see cref="Count"/>.
	/// </summary>
	/// <remarks><b><i>This field is explicitly declared on purpose. Please don't use auto property.</i></b></remarks>
	/// <seealso cref="Count"/>
	private int _count;


	/// <summary>
	/// Initializes a <see cref="CellMap"/> instance via a list of offsets represented as a RxCy notation defined by <see cref="RxCyNotation"/>.
	/// </summary>
	/// <param name="segments">The cell offsets, represented as a RxCy notation.</param>
	/// <seealso cref="RxCyNotation"/>
	[DebuggerHidden]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[JsonConstructor]
	[Obsolete(RequiresJsonSerializerDynamicInvocationMessage.DynamicInvocationByJsonSerializerOnly, true, DiagnosticId = "SCA0103", UrlFormat = "https://sunnieshine.github.io/Sudoku/code-analysis/sca0103")]
	[RequiresUnreferencedCode(RequiresJsonSerializerDynamicInvocationMessage.DynamicInvocationByJsonSerializerOnly, Url = "https://sunnieshine.github.io/Sudoku/code-analysis/sca0103")]
	public CellMap(string[] segments)
	{
		this = Empty;
		foreach (var segment in segments)
		{
			this |= RxCyNotation.ParseCells(segment);
		}
	}


	/// <summary>
	/// Same as the method <see cref="AllSetsAreInOneHouse(out int)"/>, but this property doesn't contain
	/// the <see langword="out"/> argument, as the optimization.
	/// </summary>
	/// <seealso cref="AllSetsAreInOneHouse(out int)"/>
	public readonly bool InOneHouse
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
#pragma warning disable format
			if ((_high &            -1L) == 0 && (_low & ~     0x1C0E07L) == 0) return true;
			if ((_high &            -1L) == 0 && (_low & ~     0xE07038L) == 0) return true;
			if ((_high &            -1L) == 0 && (_low & ~    0x70381C0L) == 0) return true;
			if ((_high & ~        0x70L) == 0 && (_low & ~ 0x7038000000L) == 0) return true;
			if ((_high & ~       0x381L) == 0 && (_low & ~0x181C0000000L) == 0) return true;
			if ((_high & ~      0x1C0EL) == 0 && (_low & ~  0xE00000000L) == 0) return true;
			if ((_high & ~ 0x381C0E000L) == 0 && (_low &             -1L) == 0) return true;
			if ((_high & ~0x1C0E070000L) == 0 && (_low &             -1L) == 0) return true;
			if ((_high & ~0xE070380000L) == 0 && (_low &             -1L) == 0) return true;
			if ((_high &            -1L) == 0 && (_low & ~        0x1FFL) == 0) return true;
			if ((_high &            -1L) == 0 && (_low & ~      0x3FE00L) == 0) return true;
			if ((_high &            -1L) == 0 && (_low & ~    0x7FC0000L) == 0) return true;
			if ((_high &            -1L) == 0 && (_low & ~  0xFF8000000L) == 0) return true;
			if ((_high & ~         0xFL) == 0 && (_low & ~0x1F000000000L) == 0) return true;
			if ((_high & ~      0x1FF0L) == 0 && (_low &             -1L) == 0) return true;
			if ((_high & ~    0x3FE000L) == 0 && (_low &             -1L) == 0) return true;
			if ((_high & ~  0x7FC00000L) == 0 && (_low &             -1L) == 0) return true;
			if ((_high & ~0xFF80000000L) == 0 && (_low &             -1L) == 0) return true;
			if ((_high & ~  0x80402010L) == 0 && (_low & ~ 0x1008040201L) == 0) return true;
			if ((_high & ~ 0x100804020L) == 0 && (_low & ~ 0x2010080402L) == 0) return true;
			if ((_high & ~ 0x201008040L) == 0 && (_low & ~ 0x4020100804L) == 0) return true;
			if ((_high & ~ 0x402010080L) == 0 && (_low & ~ 0x8040201008L) == 0) return true;
			if ((_high & ~ 0x804020100L) == 0 && (_low & ~0x10080402010L) == 0) return true;
			if ((_high & ~0x1008040201L) == 0 && (_low & ~  0x100804020L) == 0) return true;
			if ((_high & ~0x2010080402L) == 0 && (_low & ~  0x201008040L) == 0) return true;
			if ((_high & ~0x4020100804L) == 0 && (_low & ~  0x402010080L) == 0) return true;
			if ((_high & ~0x8040201008L) == 0 && (_low & ~  0x804020100L) == 0) return true;
#pragma warning restore format

			return false;
		}
	}

	/// <summary>
	/// Determines whether the current list of cells are all lie in an intersection area,
	/// i.e. a locked candidates.
	/// </summary>
	public bool IsInIntersection
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _count <= 3 && PopCount((uint)CoveredHouses) == 2;
	}

	/// <summary>
	/// Indicates the mask of block that all cells in this collection spanned.
	/// </summary>
	/// <remarks>
	/// For example, if the cells are <c>{ 0, 1, 27, 28 }</c>, all spanned blocks are 0 and 3, so the return
	/// mask is <c>0b000001001</c> (i.e. 9).
	/// </remarks>
	public readonly short BlockMask
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			short result = 0;
			if (this && HousesMap[0]) result |= 1;
			if (this && HousesMap[1]) result |= 2;
			if (this && HousesMap[2]) result |= 4;
			if (this && HousesMap[3]) result |= 8;
			if (this && HousesMap[4]) result |= 16;
			if (this && HousesMap[5]) result |= 32;
			if (this && HousesMap[6]) result |= 64;
			if (this && HousesMap[7]) result |= 128;
			if (this && HousesMap[8]) result |= 256;

			return result;
		}
	}

	/// <summary>
	/// Indicates the mask of row that all cells in this collection spanned.
	/// </summary>
	/// <remarks>
	/// For example, if the cells are <c>{ 0, 1, 27, 28 }</c>, all spanned rows are 0 and 3, so the return mask is <c>0b000001001</c> (i.e. 9).
	/// </remarks>
	public readonly short RowMask
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			short result = 0;
			if (this && HousesMap[9]) result |= 1;
			if (this && HousesMap[10]) result |= 2;
			if (this && HousesMap[11]) result |= 4;
			if (this && HousesMap[12]) result |= 8;
			if (this && HousesMap[13]) result |= 16;
			if (this && HousesMap[14]) result |= 32;
			if (this && HousesMap[15]) result |= 64;
			if (this && HousesMap[16]) result |= 128;
			if (this && HousesMap[17]) result |= 256;

			return result;
		}
	}

	/// <summary>
	/// Indicates the mask of column that all cells in this collection spanned.
	/// </summary>
	/// <remarks>
	/// For example, if the cells are <c>{ 0, 1, 27, 28 }</c>, all spanned columns are 0 and 1, so the return mask is <c>0b000000011</c> (i.e. 3).
	/// </remarks>
	public readonly short ColumnMask
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			short result = 0;
			if (this && HousesMap[18]) result |= 1;
			if (this && HousesMap[19]) result |= 2;
			if (this && HousesMap[20]) result |= 4;
			if (this && HousesMap[21]) result |= 8;
			if (this && HousesMap[22]) result |= 16;
			if (this && HousesMap[23]) result |= 32;
			if (this && HousesMap[24]) result |= 64;
			if (this && HousesMap[25]) result |= 128;
			if (this && HousesMap[26]) result |= 256;

			return result;
		}
	}

	/// <summary>
	/// Indicates the covered line.
	/// </summary>
	/// <remarks>
	/// If the covered house can't be found, it'll return <see cref="InvalidValidOfTrailingZeroCountMethodFallback"/>.
	/// </remarks>
	/// <seealso cref="InvalidValidOfTrailingZeroCountMethodFallback"/>
	public readonly int CoveredLine
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => TrailingZeroCount(CoveredHouses & ~511);
	}

	/// <inheritdoc/>
	public readonly int Count
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _count;
	}

	/// <summary>
	/// Indicates all houses covered. This property is used to check all houses that all cells
	/// of this instance covered. For example, if the cells are <c>{ 0, 1 }</c>, the property
	/// <see cref="CoveredHouses"/> will return the house index 0 (block 1) and 9 (row 1);
	/// however, if cells spanned two houses or more (e.g. cells <c>{ 0, 1, 27 }</c>),
	/// this property won't contain any houses.
	/// </summary>
	/// <remarks>
	/// The return value will be an <see cref="int"/> value indicating each houses.
	/// Bits set 1 are covered houses.
	/// </remarks>
	public readonly int CoveredHouses
	{
		get
		{
			var z = 0;

#pragma warning disable format
			if ((_high &            -1L) == 0 && (_low & ~     0x1C0E07L) == 0) z |=       0x1;
			if ((_high &            -1L) == 0 && (_low & ~     0xE07038L) == 0) z |=       0x2;
			if ((_high &            -1L) == 0 && (_low & ~    0x70381C0L) == 0) z |=       0x4;
			if ((_high & ~        0x70L) == 0 && (_low & ~ 0x7038000000L) == 0) z |=       0x8;
			if ((_high & ~       0x381L) == 0 && (_low & ~0x181C0000000L) == 0) z |=      0x10;
			if ((_high & ~      0x1C0EL) == 0 && (_low & ~  0xE00000000L) == 0) z |=      0x20;
			if ((_high & ~ 0x381C0E000L) == 0 && (_low &             -1L) == 0) z |=      0x40;
			if ((_high & ~0x1C0E070000L) == 0 && (_low &             -1L) == 0) z |=      0x80;
			if ((_high & ~0xE070380000L) == 0 && (_low &             -1L) == 0) z |=     0x100;
			if ((_high &            -1L) == 0 && (_low & ~        0x1FFL) == 0) z |=     0x200;
			if ((_high &            -1L) == 0 && (_low & ~      0x3FE00L) == 0) z |=     0x400;
			if ((_high &            -1L) == 0 && (_low & ~    0x7FC0000L) == 0) z |=     0x800;
			if ((_high &            -1L) == 0 && (_low & ~  0xFF8000000L) == 0) z |=    0x1000;
			if ((_high & ~         0xFL) == 0 && (_low & ~0x1F000000000L) == 0) z |=    0x2000;
			if ((_high & ~      0x1FF0L) == 0 && (_low &             -1L) == 0) z |=    0x4000;
			if ((_high & ~    0x3FE000L) == 0 && (_low &             -1L) == 0) z |=    0x8000;
			if ((_high & ~  0x7FC00000L) == 0 && (_low &             -1L) == 0) z |=   0x10000;
			if ((_high & ~0xFF80000000L) == 0 && (_low &             -1L) == 0) z |=   0x20000;
			if ((_high & ~  0x80402010L) == 0 && (_low & ~ 0x1008040201L) == 0) z |=   0x40000;
			if ((_high & ~ 0x100804020L) == 0 && (_low & ~ 0x2010080402L) == 0) z |=   0x80000;
			if ((_high & ~ 0x201008040L) == 0 && (_low & ~ 0x4020100804L) == 0) z |=  0x100000;
			if ((_high & ~ 0x402010080L) == 0 && (_low & ~ 0x8040201008L) == 0) z |=  0x200000;
			if ((_high & ~ 0x804020100L) == 0 && (_low & ~0x10080402010L) == 0) z |=  0x400000;
			if ((_high & ~0x1008040201L) == 0 && (_low & ~  0x100804020L) == 0) z |=  0x800000;
			if ((_high & ~0x2010080402L) == 0 && (_low & ~  0x201008040L) == 0) z |= 0x1000000;
			if ((_high & ~0x4020100804L) == 0 && (_low & ~  0x402010080L) == 0) z |= 0x2000000;
			if ((_high & ~0x8040201008L) == 0 && (_low & ~  0x804020100L) == 0) z |= 0x4000000;
#pragma warning restore format

			return z;
		}
	}

	/// <summary>
	/// All houses that the map spanned. This property is used to check all houses that all cells of
	/// this instance spanned. For example, if the cells are <c>{ 0, 1 }</c>, the property
	/// <see cref="Houses"/> will return the house index 0 (block 1), 9 (row 1), 18 (column 1)
	/// and 19 (column 2).
	/// </summary>
	public readonly int Houses
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (int)BlockMask | RowMask << 9 | ColumnMask << 18;
	}

	/// <inheritdoc/>
	[JsonInclude]
	public readonly string[] StringChunks
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => this
			? ToString().Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			: Array.Empty<string>();
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
			var lowerBits = 0L;
			var higherBits = 0L;
			var i = 0;
			foreach (var offset in Offsets)
			{
				var low = 0L;
				var high = 0L;
				foreach (var peer in Peers[offset])
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

	/// <inheritdoc/>
	readonly int IBitStatusMap<CellMap>.Shifting => Shifting;

	/// <inheritdoc/>
	readonly int[] IBitStatusMap<CellMap>.Offsets => Offsets;

	/// <summary>
	/// Indicates the cell offsets in this collection.
	/// </summary>
	private readonly int[] Offsets
	{
		get
		{
			if (!this)
			{
				return Array.Empty<int>();
			}

			long value;
			int i, pos = 0;
			var arr = new int[_count];
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
	static CellMap IBitStatusMap<CellMap>.Empty => Empty;

	/// <inheritdoc/>
	static CellMap IMinMaxValue<CellMap>.MaxValue => MaxValue;

	/// <inheritdoc/>
	static CellMap IMinMaxValue<CellMap>.MinValue => MinValue;


	/// <inheritdoc/>
	public readonly int this[int index]
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
	public readonly void CopyTo(int* arr, int length)
	{
		ArgumentNullException.ThrowIfNull(arr);

		if (!this)
		{
			return;
		}

		Argument.ThrowIfInvalid(_count <= length, "The capacity is not enough.");

		long value;
		int i, pos = 0;
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
	}

	/// <inheritdoc/>
	public readonly void ForEach(Action<int> action)
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
	/// <remarks>
	/// If you don't want to use the <see langword="out"/> parameter value, please
	/// use the property <see cref="InOneHouse"/> to improve the performance.
	/// </remarks>
	/// <seealso cref="InOneHouse"/>
	public readonly bool AllSetsAreInOneHouse(out int houseIndex)
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
	public readonly bool Contains(int offset) => ((offset < Shifting ? _low : _high) >> offset % Shifting & 1) != 0;

	[GeneratedOverriddingMember(GeneratedEqualsBehavior.TypeCheckingAndCallingOverloading)]
	public override readonly partial bool Equals(object? obj);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(scoped in CellMap other) => _low == other._low && _high == other._high;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(_low), nameof(_high))]
	public override readonly partial int GetHashCode();

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
	public readonly int CompareTo(scoped in CellMap other)
		=> _count > other._count ? 1 : _count < other._count ? -1 : Sign($"{this:b}".CompareTo($"{other:b}"));

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly int[] ToArray() => Offsets;

	[GeneratedOverriddingMember(GeneratedToStringBehavior.CallOverloadWithNull)]
	public override readonly partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(string? format) => ToString(format, null);

	/// <summary>
	/// Gets <see cref="string"/> representation of the current <see cref="CellMap"/> instance, using pre-defined formatters.
	/// </summary>
	/// <param name="cellMapFormatter">
	/// The <see cref="CellMap"/> formatter instance to format the current instance.
	/// </param>
	/// <returns>The <see cref="string"/> result.</returns>
	/// <remarks>
	/// <para>
	/// The target and supported types are stored in namespace <see cref="Text.Formatting"/>.
	/// If you don't remember the full format strings, you can try this method instead by passing
	/// actual <see cref="ICellMapFormatter"/> instances.
	/// </para>
	/// <para>
	/// For example, by using Susser formatter <see cref="RxCyFormat"/> instances:
	/// <code><![CDATA[
	/// // Suppose the variable is of type 'CellMap'.
	/// var cells = ...;
	/// 
	/// // Creates a RxCyFormat-based formatter.
	/// var formatter = RxCyFormat.Default;
	/// 
	/// // Using this method to get the target string representation.
	/// string targetStr = cells.ToString(formatter);
	/// 
	/// // Output the result.
	/// Console.WriteLine(targetStr);
	/// ]]></code>
	/// </para>
	/// <para>
	/// In some cases we suggest you use this method instead of calling <see cref="ToString(string?)"/>
	/// and <see cref="ToString(string?, IFormatProvider?)"/> because you may not remember all possible string formats.
	/// </para>
	/// <para>
	/// In addition, the method <see cref="ToString(string?, IFormatProvider?)"/> is also compatible with this method.
	/// If you forget to call this one, you can also use that method to get the same target result by passing first argument
	/// named <c>format</c> with <see langword="null"/> value:
	/// <code><![CDATA[
	/// string targetStr = cells.ToString(null, formatter);
	/// ]]></code>
	/// </para>
	/// </remarks>
	/// <seealso cref="Text.Formatting"/>
	/// <seealso cref="ICellMapFormatter"/>
	/// <seealso cref="RxCyFormat"/>
	/// <seealso cref="ToString(string?)"/>
	/// <seealso cref="ToString(string?, IFormatProvider?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(ICellMapFormatter cellMapFormatter) => cellMapFormatter.ToString(this);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(string? format, IFormatProvider? formatProvider)
		=> (format, formatProvider) switch
		{
			(null, null) => ToString(RxCyFormat.Default),
			(not null, _) => ToString(format),
			(_, ICellMapFormatter formatter) => formatter.ToString(this),
			(_, ICustomFormatter formatter) => formatter.Format(format, this, formatProvider),
			(_, CultureInfo { Name: ['Z' or 'z', 'H' or 'h', ..] }) => K9Notation.ToCellsString(this),
			_ => ToString(RxCyFormat.Default)
		};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly OneDimensionalArrayEnumerator<int> GetEnumerator() => Offsets.EnumerateImmutable();

	/// <inheritdoc/>
	public readonly CellMap Slice(int start, int count)
	{
		var result = Empty;
		var offsets = Offsets;
		for (int i = start, end = start + count; i < end; i++)
		{
			result.Add(offsets[i]);
		}

		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Add(int item)
	{
		scoped ref var v = ref item / Shifting == 0 ? ref _low : ref _high;
		var older = Contains(item);
		v |= 1L << item % Shifting;
		if (!older)
		{
			_count++;
		}
	}

	/// <inheritdoc/>
	public void AddRange(IEnumerable<int> offsets)
	{
		foreach (var offset in offsets)
		{
			Add(offset);
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Remove(int offset)
	{
		scoped ref var v = ref offset / Shifting == 0 ? ref _low : ref _high;
		var older = Contains(offset);
		v &= ~(1L << offset % Shifting);
		if (older)
		{
			_count--;
		}
	}

	/// <inheritdoc/>
	public void RemoveRange(IEnumerable<int> offsets)
	{
		foreach (var offset in offsets)
		{
			Remove(offset);
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Clear() => this = default;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly int IComparable.CompareTo([NotNull] object? obj)
	{
		ArgumentNullException.ThrowIfNull(obj);

		return obj is CellMap other
			? CompareTo(other)
			: throw new ArgumentException($"The argument must be of type '{nameof(CellMap)}'.", nameof(obj));
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly int IComparable<CellMap>.CompareTo(CellMap other) => CompareTo(other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void IBitStatusMap<CellMap>.AddChecked(int offset)
		=> Add(offset is >= 0 and < 81 ? offset : throw new ArgumentOutOfRangeException(nameof(offset), "The cell offset is invalid."));

	/// <inheritdoc/>
	void IBitStatusMap<CellMap>.AddRangeChecked(IEnumerable<int> offsets)
	{
		foreach (var cell in offsets)
		{
			((IBitStatusMap<CellMap>)this).AddChecked(cell);
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void IBitStatusMap<CellMap>.RemoveChecked(int offset)
		=> Remove(offset is >= 0 and < 81 ? offset : throw new ArgumentOutOfRangeException(nameof(offset), "The cell offset is invalid."));

	/// <inheritdoc/>
	void IBitStatusMap<CellMap>.RemoveRangeChecked(IEnumerable<int> offsets)
	{
		foreach (var cell in offsets)
		{
			((IBitStatusMap<CellMap>)this).RemoveChecked(cell);
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void IBitStatusMap<CellMap>.ExceptWith(IEnumerable<int> other) => this -= other;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void IBitStatusMap<CellMap>.IntersectWith(IEnumerable<int> other) => this &= Empty + other;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void IBitStatusMap<CellMap>.SymmetricExceptWith(IEnumerable<int> other) => this = (this - other) | (Empty + other - this);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void IBitStatusMap<CellMap>.UnionWith(IEnumerable<int> other) => this += other;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParse(string str, out CellMap result) => RxCyNotation.TryParseCells(str, out result);

	/// <summary>
	/// Initializes an instance with two binary values.
	/// </summary>
	/// <param name="high">Higher 40 bits.</param>
	/// <param name="low">Lower 41 bits.</param>
	/// <returns>The result instance created.</returns>
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
	/// Initializes an instance with an <see cref="Int128"/> integer.
	/// </summary>
	/// <param name="int128">The <see cref="Int128"/> integer.</param>
	/// <returns>The result instance created.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap CreateByInt128(scoped in Int128 int128)
		=> CreateByBits((long)(ulong)(int128 >> 64), (long)(ulong)(int128 & ulong.MaxValue));

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap Parse(string str) => RxCyNotation.ParseCells(str);

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
		SkipInit(out result);
		return false;
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !(scoped in CellMap offsets) => offsets ? false : true;

	/// <inheritdoc/>
	public static bool operator true(scoped in CellMap value) => value._count != 0;

	/// <inheritdoc/>
	public static bool operator false(scoped in CellMap value) => value._count == 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap operator ~(scoped in CellMap offsets)
		=> CreateByBits(~offsets._high & 0xFF_FFFF_FFFFL, ~offsets._low & 0x1FF_FFFF_FFFFL);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap operator +(scoped in CellMap collection, int offset)
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
	public static CellMap operator +(scoped in CellMap collection, IEnumerable<int> offsets)
	{
		var result = collection;
		result.AddRange(offsets);

		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap operator -(scoped in CellMap collection, int offset)
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
	public static CellMap operator -(scoped in CellMap collection, IEnumerable<int> offsets)
	{
		var result = collection;
		foreach (var offset in offsets)
		{
			result.Remove(offset);
		}

		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap operator -(scoped in CellMap left, scoped in CellMap right) => left & ~right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap[] operator &(scoped in CellMap offsets, int subsetSize)
	{
		if (subsetSize == 0 || subsetSize > offsets._count)
		{
			return Array.Empty<CellMap>();
		}

		if (subsetSize == offsets._count)
		{
			return new[] { offsets };
		}

		var n = offsets._count;
		var buffer = stackalloc int[subsetSize];
		if (n <= 30 && subsetSize <= 30)
		{
			// Optimization: Use table to get the total number of result elements.
			var totalIndex = 0;
			var result = new CellMap[Combinatorial[n - 1, subsetSize - 1]];
			f(subsetSize, n, subsetSize, offsets.Offsets);
			return result;


			void f(int size, int last, int index, int[] offsets)
			{
				for (var i = last; i >= index; i--)
				{
					buffer[index - 1] = i - 1;
					if (index > 1)
					{
						f(size, i - 1, index - 1, offsets);
					}
					else
					{
						var temp = new int[size];
						for (var j = 0; j < size; j++)
						{
							temp[j] = offsets[buffer[j]];
						}

						result[totalIndex++] = (CellMap)temp;
					}
				}
			}
		}
		else if (n > 30 && subsetSize > 30)
		{
			throw new NotSupportedException(
				"""
				Both cells count and subset size is too large, which may cause potential out of memory exception. 
				This operator will throw this exception to calculate the result, in order to prevent any possible exceptions thrown.
				""".RemoveLineEndings()
			);
		}
		else
		{
			var result = new List<CellMap>();
			f(subsetSize, n, subsetSize, offsets.Offsets);
			return result.ToArray();


			void f(int size, int last, int index, int[] offsets)
			{
				for (var i = last; i >= index; i--)
				{
					buffer[index - 1] = i - 1;
					if (index > 1)
					{
						f(size, i - 1, index - 1, offsets);
					}
					else
					{
						var temp = new int[size];
						for (var j = 0; j < size; j++)
						{
							temp[j] = offsets[buffer[j]];
						}

						result.Add((CellMap)temp);
					}
				}
			}
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap operator &(scoped in CellMap left, scoped in CellMap right)
		=> CreateByBits(left._high & right._high, left._low & right._low);

	/// <inheritdoc/>
	public static CellMap[] operator |(scoped in CellMap offsets, int subsetSize)
	{
		if (subsetSize == 0 || offsets is [])
		{
			return Array.Empty<CellMap>();
		}

		var n = offsets._count;

		var desiredSize = 0;
		var length = Min(n, subsetSize);
		for (var i = 1; i <= length; i++)
		{
			var target = Combinatorial[n - 1, i - 1];
			desiredSize += target;
		}

		var result = new List<CellMap>(desiredSize);
		for (var i = 1; i <= length; i++)
		{
			result.AddRange(offsets & i);
		}

		return result.ToArray();
	}

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
	public static CandidateMap operator *(scoped in CellMap @base, int digit)
	{
		var result = CandidateMap.Empty;
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
	public static short operator /(scoped in CellMap map, int houseIndex)
	{
		short p = 0, i = 0;
		foreach (var cell in HouseCells[houseIndex])
		{
			if (map.Contains(cell))
			{
				p |= (short)(1 << i);
			}

			i++;
		}

		return p;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static short IDivisionOperators<CellMap, int, short>.operator /(CellMap left, int right) => left / right;

	/// <summary>
	/// Get the sub-view mask of this map. This operator will check the validity of the argument <paramref name="right"/>.
	/// </summary>
	/// <param name="left">The map.</param>
	/// <param name="right">The house index.</param>
	/// <returns>The mask.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static short IDivisionOperators<CellMap, int, short>.operator checked /(CellMap left, int right)
	{
		Argument.ThrowIfFalse(right is >= 0 and < 27);

		return left / right;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IComparisonOperators<CellMap, CellMap, bool>.operator >(CellMap left, CellMap right) => left.CompareTo(right) > 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IComparisonOperators<CellMap, CellMap, bool>.operator <(CellMap left, CellMap right) => left.CompareTo(right) < 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IComparisonOperators<CellMap, CellMap, bool>.operator >=(CellMap left, CellMap right) => left.CompareTo(right) >= 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IComparisonOperators<CellMap, CellMap, bool>.operator <=(CellMap left, CellMap right) => left.CompareTo(right) <= 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CellMap IAdditionOperators<CellMap, int, CellMap>.operator +(CellMap left, int right) => left + right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CellMap IAdditionOperators<CellMap, int, CellMap>.operator checked +(CellMap left, int right)
	{
		var copied = left;
		((IBitStatusMap<CellMap>)copied).AddChecked(right);

		return copied;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CellMap IAdditionOperators<CellMap, IEnumerable<int>, CellMap>.operator +(CellMap left, IEnumerable<int> right) => left + right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CellMap IAdditionOperators<CellMap, IEnumerable<int>, CellMap>.operator checked +(CellMap left, IEnumerable<int> right)
	{
		var copied = left;
		((IBitStatusMap<CellMap>)copied).AddRangeChecked(right);

		return copied;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CellMap ISubtractionOperators<CellMap, int, CellMap>.operator -(CellMap left, int right) => left - right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CellMap ISubtractionOperators<CellMap, int, CellMap>.operator checked -(CellMap left, int right)
	{
		var copied = left;
		((IBitStatusMap<CellMap>)left).RemoveChecked(right);

		return copied;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CellMap ISubtractionOperators<CellMap, IEnumerable<int>, CellMap>.operator -(CellMap left, IEnumerable<int> right) => left - right;

	/// <inheritdoc/>
	static CellMap ISubtractionOperators<CellMap, IEnumerable<int>, CellMap>.operator checked -(CellMap left, IEnumerable<int> right)
	{
		var copied = left;
		((IBitStatusMap<CellMap>)copied).RemoveRangeChecked(right);

		return copied;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CandidateMap IMultiplyOperators<CellMap, int, CandidateMap>.operator *(CellMap left, int right) => left * right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CandidateMap IMultiplyOperators<CellMap, int, CandidateMap>.operator checked *(CellMap left, int right)
		=> right is >= 0 and < 9 ? left * right : throw new ArgumentOutOfRangeException(nameof(right));

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CellMap IBitStatusMap<CellMap>.operator checked +(scoped in CellMap collection, int offset)
	{
		var copied = collection;
		((IBitStatusMap<CellMap>)copied).AddChecked(offset);

		return copied;
	}

	/// <inheritdoc/>
	static CellMap IBitStatusMap<CellMap>.operator checked +(scoped in CellMap collection, IEnumerable<int> offsets)
	{
		var copied = collection;
		((IBitStatusMap<CellMap>)copied).AddRangeChecked(offsets);

		return copied;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CellMap IBitStatusMap<CellMap>.operator checked -(scoped in CellMap collection, int offset)
	{
		var copied = collection;
		((IBitStatusMap<CellMap>)copied).RemoveChecked(offset);

		return copied;
	}

	/// <inheritdoc/>
	static CellMap IBitStatusMap<CellMap>.operator checked -(scoped in CellMap collection, IEnumerable<int> offsets)
	{
		var copied = collection;
		((IBitStatusMap<CellMap>)copied).RemoveRangeChecked(offsets);

		return copied;
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator int[](scoped in CellMap offsets) => offsets.ToArray();

	/// <inheritdoc/>
	public static implicit operator CellMap(scoped Span<int> offsets)
	{
		var result = Empty;
		foreach (var offset in offsets)
		{
			result.Add(offset);
		}

		return result;
	}

	/// <inheritdoc/>
	public static implicit operator CellMap(scoped ReadOnlySpan<int> offsets)
	{
		var result = Empty;
		foreach (var offset in offsets)
		{
			result.Add(offset);
		}

		return result;
	}

	/// <inheritdoc/>
	public static explicit operator CellMap(int[] offsets)
	{
		var result = Empty;
		foreach (var offset in offsets)
		{
			result.Add(offset);
		}

		return result;
	}

	/// <summary>
	/// Implicit cast from <see cref="CellMap"/> to <see cref="Int128"/>.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator Int128(scoped in CellMap offsets) => new((ulong)offsets._high, (ulong)offsets._low);

	/// <summary>
	/// Explicit cast from <see cref="Int128"/> to <see cref="CellMap"/>.
	/// </summary>
	/// <param name="int128">The <see cref="Int128"/> integer.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator CellMap(scoped in Int128 int128) => CreateByInt128(int128);

	/// <summary>
	/// Explicit cast from <see cref="Int128"/> to <see cref="CellMap"/>.
	/// </summary>
	/// <param name="int128">The <see cref="Int128"/> integer.</param>
	/// <exception cref="OverflowException">
	/// Throws when the base argument <paramref name="int128"/> is greater than the maximum value
	/// corresponding to <see cref="MaxValue"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator checked CellMap(scoped in Int128 int128)
		=> int128 >> 81 == 0
			? CreateByInt128(int128)
			: throw new OverflowException($"The base {nameof(Int128)} integer is greater than '{nameof(MaxValue)}'.");

	/// <inheritdoc/>
	static explicit IBitStatusMap<CellMap>.operator checked CellMap(int[] offsets)
	{
		var result = Empty;
		foreach (var offset in offsets)
		{
			((IBitStatusMap<CellMap>)result).AddChecked(offset);
		}

		return result;
	}

	/// <inheritdoc/>
	static implicit IBitStatusMap<CellMap>.operator CellMap(scoped Span<int> offsets)
	{
		var result = Empty;
		foreach (var offset in offsets)
		{
			result.Add(offset);
		}

		return result;
	}

	/// <inheritdoc/>
	static implicit IBitStatusMap<CellMap>.operator CellMap(scoped ReadOnlySpan<int> offsets)
	{
		var result = Empty;
		foreach (var offset in offsets)
		{
			result.Add(offset);
		}

		return result;
	}

	/// <inheritdoc/>
	static explicit IBitStatusMap<CellMap>.operator CellMap(int[] offsets)
	{
		var result = Empty;
		foreach (var offset in offsets)
		{
			result.Add(offset);
		}

		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static explicit IBitStatusMap<CellMap>.operator Span<int>(scoped in CellMap offsets) => offsets.Offsets;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static explicit IBitStatusMap<CellMap>.operator ReadOnlySpan<int>(scoped in CellMap offsets) => offsets.Offsets;
}

/// <summary>
/// Indicates the JSON converter of the current type.
/// </summary>
file sealed class Converter : JsonConverter<CellMap>
{
	/// <inheritdoc/>
	public override bool HandleNull => false;


	/// <inheritdoc/>
	public override CellMap Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var result = CellMap.Empty;
		var parts = JsonSerializer.Deserialize<string[]>(ref reader, options) ?? throw new JsonException("Unexpected token type.");
		foreach (var part in parts)
		{
			result |= RxCyNotation.ParseCells(part);
		}

		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override void Write(Utf8JsonWriter writer, CellMap value, JsonSerializerOptions options)
		=> writer.WriteArray(value.StringChunks, options);
}
