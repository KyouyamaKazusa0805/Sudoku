#pragma warning disable IDE0011

using System.ComponentModel;
using Sudoku.Presentation;
using static System.Numerics.BitOperations;
using static Sudoku.Constants;
using static Sudoku.Constants.Tables;

namespace Sudoku.Collections;

/// <summary>
/// Encapsulates a binary series of cell status table.
/// </summary>
/// <remarks>
/// The instance stores two <see cref="long"/> values, consisting of 81 bits,
/// where <see langword="true"/> bit (1) is for the cell having that digit,
/// and the <see langword="false"/> bit (0) is for the cell not containing
/// the digit.
/// </remarks>
public unsafe struct Cells :
	IDefaultable<Cells>,
	IEnumerable<int>,
	IEquatable<Cells>,
	ISimpleFormattable,
	ISimpleParseable<Cells>
#if FEATURE_GENERIC_MATH
	,
	IAdditionOperators<Cells, int, Cells>,
	ISubtractionOperators<Cells, int, Cells>,
	ISubtractionOperators<Cells, Cells, Cells>,
	IDivisionOperators<Cells, int, short>,
	IModulusOperators<Cells, Cells, Cells>,
	IBitwiseOperators<Cells, Cells, Cells>,
	IEqualityOperators<Cells, Cells>
#if FEATURE_GENEIC_MATH_IN_ARG
	,
	IValueAdditionOperators<Cells, int, Cells>,
	IValueSubtractionOperators<Cells, int, Cells>,
	IValueSubtractionOperators<Cells, Cells, Cells>,
	IValueDivisionOperators<Cells, int, short>,
	IValueModulusOperators<Cells, Cells, Cells>,
	IValueBitwiseAndOperators<Cells, Cells, Cells>,
	IValueBitwiseOrOperators<Cells, Cells, Cells>,
	IValueBitwiseNotOperators<Cells, Cells>,
	IValueBitwiseExclusiveOrOperators<Cells, Cells, Cells>,
	IValueEqualityOperators<Cells, Cells>,
	IValueGreaterThanOrLessThanOperators<Cells, Cells>,
	IValueLogicalNotOperators<Cells>
#endif
#endif
{
	/// <summary>
	/// <para>Indicates an empty instance (all bits are 0).</para>
	/// <para>
	/// I strongly recommend you <b>should</b> use this instance instead of default constructor
	/// <see cref="Cells()"/> and <see langword="default"/>(<see cref="Cells"/>).
	/// </para>
	/// </summary>
	/// <seealso cref="Cells()"/>
	public static readonly Cells Empty;

	/// <summary>
	/// Indicates the regular expression for matching a cell or cell-list.
	/// </summary>
	private static readonly Regex CellOrCellListRegex = new(
		RegularExpressions.CellOrCellList,
		RegexOptions.ExplicitCapture,
		TimeSpan.FromSeconds(5)
	);


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
	private long _high = 0, _low = 0;


	/// <summary>
	/// Initializes a <see cref="Cells"/> instance via the read-only field <see cref="Empty"/>.
	/// </summary>
	/// <remarks>
	/// <include
	///     file='../../global-doc-comments.xml'
	///     path='g/csharp9/feature[@name="parameterless-struct-constructor"]/target[@name="constructor"]' />
	/// </remarks>
	/// <seealso cref="Empty"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public Cells() => this = Empty;

	/// <summary>
	/// Initializes an instance with the specified cell offset
	/// (Sets itself and all peers).
	/// </summary>
	/// <param name="cell">The cell offset.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Cells(int cell) : this(cell, true)
	{
	}

	/// <summary>
	/// Initializes an instance with the candidate list specified as a pointer.
	/// </summary>
	/// <param name="cells">The pointer points to an array of elements.</param>
	/// <param name="length">The length of the array.</param>
	public Cells(int* cells, int length) : this(in *cells, length)
	{
	}

	/// <summary>
	/// Same behavior of the constructor as <see cref="Cells(IEnumerable{int})"/>:
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
	/// <seealso cref="Cells(IEnumerable{int})"/>
	public Cells(int[] cells) : this(in cells[0], cells.Length)
	{
	}

	/// <summary>
	/// Initializes an instance with the cell offset specified as an <see cref="Index"/>
	/// (Sets itself and all peers).
	/// </summary>
	/// <param name="cellIndex">The cell offset specified as an <see cref="Index"/>.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Cells(Index cellIndex) : this(cellIndex.GetOffset(81))
	{
	}

	/// <summary>
	/// Initializes an instance with a series of cell offsets.
	/// </summary>
	/// <param name="cells">cell offsets.</param>
	public Cells(Span<int> cells) : this(in cells.GetPinnableReference(), cells.Length)
	{
	}

	/// <summary>
	/// Initializes an instance with a series of cell offsets.
	/// </summary>
	/// <param name="cells">cell offsets.</param>
	public Cells(ReadOnlySpan<int> cells) : this(in cells.GetPinnableReference(), cells.Length)
	{
	}

	/// <summary>
	/// Initializes an instance with a series of cell offsets.
	/// </summary>
	/// <param name="cells">cell offsets.</param>
	/// <remarks>
	/// Note that all offsets will be set <see langword="true"/>, but their own peers
	/// won't be set <see langword="true"/>.
	/// </remarks>
	public Cells(IEnumerable<int> cells) : this()
	{
		foreach (int offset in cells)
		{
			InternalAdd(offset, true);
		}
	}

	/// <summary>
	/// Initializes an instance with the specified range.
	/// </summary>
	/// <param name="range">The range.</param>
	public Cells(Range range)
	{
		int start = range.Start.GetOffset(81);
		int end = range.End.GetOffset(81);
		for (int i = start; i < end; i++)
		{
			InternalAdd(i, true);
		}
	}

	/// <summary>
	/// Initializes an instance with two binary values.
	/// </summary>
	/// <param name="high">Higher 40 bits.</param>
	/// <param name="low">Lower 41 bits.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Cells(long high, long low)
	{
		_high = high;
		_low = low;
		Count = PopCount((ulong)_high) + PopCount((ulong)_low);
	}

	/// <summary>
	/// Initializes an instance with three binary values.
	/// </summary>
	/// <param name="high">Higher 27 bits.</param>
	/// <param name="mid">Medium 27 bits.</param>
	/// <param name="low">Lower 27 bits.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Cells(int high, int mid, int low) :
		this((high & 0x7FFFFFFL) << 13 | (mid >> 14 & 0x1FFFL), (mid & 0x3FFFL) << 27 | (low & 0x7FFFFFFL))
	{
	}

	/// <summary>
	/// Initializes a <see cref="Cells"/> instance using the specified reference for the array of cells.
	/// </summary>
	/// <param name="cell">
	/// <para>The reference to the array of cells.</para>
	/// <para>
	/// <include
	///     file='../../global-doc-comments.xml'
	///     path='g/csharp7/feature[@name="ref-returns"]/target[@name="in-parameter"]'/>
	/// </para>
	/// </param>
	/// <param name="length">The length of the array.</param>
	/// <remarks>
	/// <include
	///     file='../../global-doc-comments.xml'
	///     path='g/csharp7/feature[@name="ref-returns"]/target[@name="method"]'/>
	/// </remarks>
	private Cells(in int cell, int length)
	{
		fixed (int* p = &cell)
		{
			for (int i = 0; i < length; i++)
			{
				InternalAdd(p[i], true);
			}
		}
	}

	/// <summary>
	/// Initializes an instance with the specified cell offset.
	/// This will set all bits of all peers of this cell. Another
	/// <see cref="bool"/> value indicates whether this initialization
	/// will set the bit of itself.
	/// </summary>
	/// <param name="cell">The cell offset.</param>
	/// <param name="setItself">
	/// A <see cref="bool"/> value indicating whether this initialization
	/// will set the bit of itself.
	/// </param>
	/// <remarks>
	/// If you want to use this constructor, please use <see cref="PeerMaps"/> instead.
	/// </remarks>
	/// <seealso cref="PeerMaps"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Cells(int cell, bool setItself)
	{
		// Don't merge those two to one.
		//(this = PeerMaps[cell]).InternalAdd(cell, setItself);
		// This is a bug for the compiler, see sharplab http://bitly.ws/fRdf
		this = PeerMaps[cell];
		InternalAdd(cell, setItself);
	}


	/// <summary>
	/// Indicates whether the collection is empty.
	/// </summary>
	public readonly bool IsEmpty
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Count == 0;
	}

	/// <summary>
	/// Same as <see cref="AllSetsAreInOneRegion(out int)"/>, but only contains
	/// the <see cref="bool"/> result.
	/// </summary>
	/// <seealso cref="AllSetsAreInOneRegion(out int)"/>
	public readonly bool InOneRegion
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
#pragma warning disable IDE0055
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
#pragma warning restore IDE0055

			return false;
		}
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
			if (!(this & RegionMaps[0]).IsEmpty) result |= 1;
			if (!(this & RegionMaps[1]).IsEmpty) result |= 2;
			if (!(this & RegionMaps[2]).IsEmpty) result |= 4;
			if (!(this & RegionMaps[3]).IsEmpty) result |= 8;
			if (!(this & RegionMaps[4]).IsEmpty) result |= 16;
			if (!(this & RegionMaps[5]).IsEmpty) result |= 32;
			if (!(this & RegionMaps[6]).IsEmpty) result |= 64;
			if (!(this & RegionMaps[7]).IsEmpty) result |= 128;
			if (!(this & RegionMaps[8]).IsEmpty) result |= 256;

			return result;
		}
	}

	/// <summary>
	/// Indicates the mask of row that all cells in this collection spanned.
	/// </summary>
	/// <remarks>
	/// For example, if the cells are <c>{ 0, 1, 27, 28 }</c>, all spanned rows are 0 and 3, so the return
	/// mask is <c>0b000001001</c> (i.e. 9).
	/// </remarks>
	public readonly short RowMask
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			short result = 0;
			if (!(this & RegionMaps[9]).IsEmpty) result |= 1;
			if (!(this & RegionMaps[10]).IsEmpty) result |= 2;
			if (!(this & RegionMaps[11]).IsEmpty) result |= 4;
			if (!(this & RegionMaps[12]).IsEmpty) result |= 8;
			if (!(this & RegionMaps[13]).IsEmpty) result |= 16;
			if (!(this & RegionMaps[14]).IsEmpty) result |= 32;
			if (!(this & RegionMaps[15]).IsEmpty) result |= 64;
			if (!(this & RegionMaps[16]).IsEmpty) result |= 128;
			if (!(this & RegionMaps[17]).IsEmpty) result |= 256;

			return result;
		}
	}

	/// <summary>
	/// Indicates the mask of column that all cells in this collection spanned.
	/// </summary>
	/// <remarks>
	/// For example, if the cells are <c>{ 0, 1, 27, 28 }</c>, all spanned columns are 0 and 1, so the return
	/// mask is <c>0b000000011</c> (i.e. 3).
	/// </remarks>
	public readonly short ColumnMask
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			short result = 0;
			if (!(this & RegionMaps[18]).IsEmpty) result |= 1;
			if (!(this & RegionMaps[19]).IsEmpty) result |= 2;
			if (!(this & RegionMaps[20]).IsEmpty) result |= 4;
			if (!(this & RegionMaps[21]).IsEmpty) result |= 8;
			if (!(this & RegionMaps[22]).IsEmpty) result |= 16;
			if (!(this & RegionMaps[23]).IsEmpty) result |= 32;
			if (!(this & RegionMaps[24]).IsEmpty) result |= 64;
			if (!(this & RegionMaps[25]).IsEmpty) result |= 128;
			if (!(this & RegionMaps[26]).IsEmpty) result |= 256;

			return result;
		}
	}

	/// <summary>
	/// Indicates the covered line.
	/// </summary>
	/// <remarks>
	/// When the covered region can't be found, it'll return <see cref="InvalidFirstSet"/>
	/// (i.e. 32) always.
	/// </remarks>
	/// <seealso cref="InvalidFirstSet"/>
	public readonly int CoveredLine
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => TrailingZeroCount(CoveredRegions & ~511);
	}

	/// <summary>
	/// Indicates the number of the values stored in this collection.
	/// </summary>
	public int Count { get; private set; } = 0;

	/// <summary>
	/// Indicates all regions covered. This property is used to check all regions that all cells
	/// of this instance covered. For example, if the cells are <c>{ 0, 1 }</c>, the property
	/// <see cref="CoveredRegions"/> will return the region 0 (block 1) and region 9 (row 1);
	/// however, if cells spanned two regions or more (e.g. cells <c>{ 0, 1, 27 }</c>),
	/// this property won't contain any regions.
	/// </summary>
	/// <remarks>
	/// The return value will be an <see cref="int"/> value indicating each regions.
	/// Bits set 1 are covered regions.
	/// </remarks>
	public readonly int CoveredRegions
	{
		get
		{
			int z = 0;

#pragma warning disable IDE0055
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
#pragma warning restore IDE0055

			return z;
		}
	}

	/// <summary>
	/// All regions that the map spanned. This property is used to check all regions that all cells of
	/// this instance spanned. For example, if the cells are <c>{ 0, 1 }</c>, the property
	/// <see cref="Regions"/> will return the region 0 (block 1), region 9 (row 1), region 18 (column 1)
	/// and the region 19 (column 2).
	/// </summary>
	public readonly int Regions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (int)BlockMask | RowMask << RowOffset | ColumnMask << ColumnOffset;
	}

	/// <summary>
	/// Indicates the <see cref="Cells"/> of intersections.
	/// </summary>
	public readonly Cells PeerIntersection
	{
		get
		{
			long lowerBits = 0, higherBits = 0;
			int i = 0;
			foreach (int offset in Offsets)
			{
				long low = 0, high = 0;
				foreach (int peer in Peers[offset])
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

			return new(higherBits, lowerBits);
		}
	}

	/// <inheritdoc/>
	readonly bool IDefaultable<Cells>.IsDefault => IsEmpty;

	/// <inheritdoc/>
	static Cells IDefaultable<Cells>.Default => Empty;

	/// <summary>
	/// Indicates the cell offsets in this collection.
	/// </summary>
	private readonly int[] Offsets
	{
		get
		{
			if (IsEmpty)
			{
				return Array.Empty<int>();
			}

			long value;
			int i, pos = 0;
			int[] arr = new int[Count];
			fixed (int* pArr = arr)
			{
				if (_low != 0)
				{
					for (value = _low, i = 0; i < Shifting; i++, value >>= 1)
					{
						if ((value & 1) != 0)
						{
							pArr[pos++] = i;
						}
					}
				}
				if (_high != 0)
				{
					for (value = _high, i = Shifting; i < 81; i++, value >>= 1)
					{
						if ((value & 1) != 0)
						{
							pArr[pos++] = i;
						}
					}
				}
			}

			return arr;
		}
	}


	/// <summary>
	/// Get the offset at the specified position index.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <returns>
	/// The offset at the specified position index. If the value is invalid, the return value will be <c>-1</c>.
	/// </returns>
	public readonly int this[int index]
	{
		get
		{
			if (IsEmpty)
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
	/// Copies the current instance to the target array specified as an <see cref="int"/>*.
	/// </summary>
	/// <param name="arr">The pointer that points to an array of type <see cref="int"/>.</param>
	/// <param name="length">The length of that array.</param>
	/// <exception cref="InvalidOperationException">
	/// Throws when the capacity isn't enough to store all values.
	/// </exception>
	public readonly void CopyTo(int* arr, int length)
	{
		if (IsEmpty)
		{
			return;
		}

		if (Count > length)
		{
			throw new InvalidOperationException("The capacity is not enough.");
		}

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

	/// <summary>
	/// Copies the current instance to the target <see cref="Span{T}"/> instance.
	/// </summary>
	/// <param name="span">
	/// The target <see cref="Span{T}"/> instance.
	/// </param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly void CopyTo(ref Span<int> span)
	{
		fixed (int* arr = span)
		{
			CopyTo(arr, span.Length);
		}
	}

	/// <summary>
	/// Indicates whether all cells in this instance are in one region.
	/// </summary>
	/// <param name="region">
	/// The region covered. If the return value
	/// is false, this value will be the constant -1.
	/// </param>
	/// <returns>A <see cref="bool"/> result.</returns>
	/// <remarks>
	/// If you don't want to use the <see langword="out"/> parameter value, please
	/// use the property <see cref="InOneRegion"/> to improve the performance.
	/// </remarks>
	/// <seealso cref="InOneRegion"/>
	public readonly bool AllSetsAreInOneRegion(out int region)
	{
#pragma warning disable IDE0055
		if ((_high &            -1L) == 0 && (_low & ~     0x1C0E07L) == 0) { region =  0; return true; }
		if ((_high &            -1L) == 0 && (_low & ~     0xE07038L) == 0) { region =  1; return true; }
		if ((_high &            -1L) == 0 && (_low & ~    0x70381C0L) == 0) { region =  2; return true; }
		if ((_high & ~        0x70L) == 0 && (_low & ~ 0x7038000000L) == 0) { region =  3; return true; }
		if ((_high & ~       0x381L) == 0 && (_low & ~0x181C0000000L) == 0) { region =  4; return true; }
		if ((_high & ~      0x1C0EL) == 0 && (_low & ~  0xE00000000L) == 0) { region =  5; return true; }
		if ((_high & ~ 0x381C0E000L) == 0 && (_low &             -1L) == 0) { region =  6; return true; }
		if ((_high & ~0x1C0E070000L) == 0 && (_low &             -1L) == 0) { region =  7; return true; }
		if ((_high & ~0xE070380000L) == 0 && (_low &             -1L) == 0) { region =  8; return true; }
		if ((_high &            -1L) == 0 && (_low & ~        0x1FFL) == 0) { region =  9; return true; }
		if ((_high &            -1L) == 0 && (_low & ~      0x3FE00L) == 0) { region = 10; return true; }
		if ((_high &            -1L) == 0 && (_low & ~    0x7FC0000L) == 0) { region = 11; return true; }
		if ((_high &            -1L) == 0 && (_low & ~  0xFF8000000L) == 0) { region = 12; return true; }
		if ((_high & ~         0xFL) == 0 && (_low & ~0x1F000000000L) == 0) { region = 13; return true; }
		if ((_high & ~      0x1FF0L) == 0 && (_low &             -1L) == 0) { region = 14; return true; }
		if ((_high & ~    0x3FE000L) == 0 && (_low &             -1L) == 0) { region = 15; return true; }
		if ((_high & ~  0x7FC00000L) == 0 && (_low &             -1L) == 0) { region = 16; return true; }
		if ((_high & ~0xFF80000000L) == 0 && (_low &             -1L) == 0) { region = 17; return true; }
		if ((_high & ~  0x80402010L) == 0 && (_low & ~ 0x1008040201L) == 0) { region = 18; return true; }
		if ((_high & ~ 0x100804020L) == 0 && (_low & ~ 0x2010080402L) == 0) { region = 19; return true; }
		if ((_high & ~ 0x201008040L) == 0 && (_low & ~ 0x4020100804L) == 0) { region = 20; return true; }
		if ((_high & ~ 0x402010080L) == 0 && (_low & ~ 0x8040201008L) == 0) { region = 21; return true; }
		if ((_high & ~ 0x804020100L) == 0 && (_low & ~0x10080402010L) == 0) { region = 22; return true; }
		if ((_high & ~0x1008040201L) == 0 && (_low & ~  0x100804020L) == 0) { region = 23; return true; }
		if ((_high & ~0x2010080402L) == 0 && (_low & ~  0x201008040L) == 0) { region = 24; return true; }
		if ((_high & ~0x4020100804L) == 0 && (_low & ~  0x402010080L) == 0) { region = 25; return true; }
		if ((_high & ~0x8040201008L) == 0 && (_low & ~  0x804020100L) == 0) { region = 26; return true; }
#pragma warning restore IDE0055

		region = -1;
		return false;
	}

	/// <summary>
	/// Determine whether the map contains the specified offset.
	/// </summary>
	/// <param name="offset">The offset.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Contains(int offset) =>
		((offset < Shifting ? _low : _high) >> offset % Shifting & 1) != 0;

	/// <inheritdoc cref="object.Equals(object?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] object? obj) => obj is Cells comparer && Equals(comparer);

	/// <summary>
	/// Determine whether the two elements are equal.
	/// </summary>
	/// <param name="other">The object to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(in Cells other) => _low == other._low && _high == other._high;

	/// <summary>
	/// Get the subview mask of this map.
	/// </summary>
	/// <param name="region">The region.</param>
	/// <returns>The mask.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly short GetSubviewMask(int region) => this / region;

	/// <inheritdoc cref="object.GetHashCode"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly int GetHashCode() => ToString("b").GetHashCode();

	/// <summary>
	/// Get all offsets whose bits are set <see langword="true"/>.
	/// </summary>
	/// <returns>An array of offsets.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly int[] ToArray() => Offsets;

	/// <inheritdoc cref="object.ToString"/>
	public override readonly string ToString() => ToString(null);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(string? format)
	{
		return format switch
		{
			null or "N" or "n" => this switch
			{
				[] => "{ }",
				[var cell] => $"r{cell / 9 + 1}c{cell % 9 + 1}",
				_ => normalToString(this)
			},
			"B" or "b" => binaryToString(this, false),
			"T" or "t" => tableToString(this),
			_ => throw new FormatException("The specified format is invalid.")
		};


		static string tableToString(in Cells @this)
		{
			var sb = new StringHandler(initialCapacity: (3 * 7 + 2) * 13);
			for (int i = 0; i < 3; i++)
			{
				for (int bandLn = 0; bandLn < 3; bandLn++)
				{
					for (int j = 0; j < 3; j++)
					{
						for (int columnLn = 0; columnLn < 3; columnLn++)
						{
							sb.Append(@this.Contains((i * 3 + bandLn) * 9 + j * 3 + columnLn) ? '*' : '.');
							sb.Append(' ');
						}

						if (j != 2)
						{
							sb.Append("| ");
						}
						else
						{
							sb.AppendLine();
						}
					}
				}

				if (i != 2)
				{
					sb.Append("------+-------+------");
					sb.AppendLine();
				}
			}

			return sb.ToStringAndClear();
		}

		static string normalToString(in Cells @this)
		{
			const string leftCurlyBrace = "{ ", rightCurlyBrace = " }", separator = ", ";
			var sbRow = new StringHandler(initialCapacity: 50);
			var dic = new Dictionary<int, ICollection<int>>();
			foreach (int cell in @this)
			{
				if (!dic.ContainsKey(cell / 9))
				{
					dic.Add(cell / 9, new List<int>());
				}

				dic[cell / 9].Add(cell % 9);
			}
			bool addCurlyBraces = dic.Count > 1;
			if (addCurlyBraces)
			{
				sbRow.Append(leftCurlyBrace);
			}
			foreach (int row in dic.Keys)
			{
				sbRow.Append('r');
				sbRow.Append(row + 1);
				sbRow.Append('c');
				sbRow.AppendRange(dic[row], static v => (v + 1).ToString());
				sbRow.Append(separator);
			}
			sbRow.RemoveFromEnd(separator.Length);
			if (addCurlyBraces)
			{
				sbRow.Append(rightCurlyBrace);
			}

			dic.Clear();
			var sbColumn = new StringHandler(initialCapacity: 50);
			foreach (int cell in @this)
			{
				if (!dic.ContainsKey(cell % 9))
				{
					dic.Add(cell % 9, new List<int>());
				}

				dic[cell % 9].Add(cell / 9);
			}
			addCurlyBraces = dic.Count > 1;
			if (addCurlyBraces)
			{
				sbColumn.Append(leftCurlyBrace);
			}

			foreach (int column in dic.Keys)
			{
				sbColumn.Append('r');
				sbColumn.AppendRange(dic[column], static v => (v + 1).ToString());
				sbColumn.Append('c');
				sbColumn.Append(column + 1);
				sbColumn.Append(separator);
			}
			sbColumn.RemoveFromEnd(separator.Length);
			if (addCurlyBraces)
			{
				sbColumn.Append(rightCurlyBrace);
			}

			return (sbRow.Length > sbColumn.Length ? sbColumn : sbRow).ToStringAndClear();
		}

		static string binaryToString(in Cells @this, bool withSeparator)
		{
			var sb = new StringHandler(initialCapacity: 81);
			int i;
			long value = @this._low;
			for (i = 0; i < 27; i++, value >>= 1)
			{
				sb.Append(value & 1);
			}
			if (withSeparator)
			{
				sb.Append(' ');
			}
			for (; i < 41; i++, value >>= 1)
			{
				sb.Append(value & 1);
			}
			for (value = @this._high; i < 54; i++, value >>= 1)
			{
				sb.Append(value & 1);
			}
			if (withSeparator)
			{
				sb.Append(' ');
			}
			for (; i < 81; i++, value >>= 1)
			{
				sb.Append(value & 1);
			}

			sb.Reverse();
			return sb.ToStringAndClear();
		}
	}

	/// <summary>
	/// To gets the cells that is in the cells that both <see langword="this"/>
	/// and <paramref name="limit"/> see (i.e. peer intersection of <c><![CDATA[this & limit]]></c>),
	/// and gets the result map that is in the map above, and only lies in <paramref name="limit"/>.
	/// </summary>
	/// <param name="limit">
	/// The map to limit the result peer intersection.
	/// </param>
	/// <returns>The result map.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Cells PeerIntersectionLimitsWith(in Cells limit) => this % limit;

	/// <summary>
	/// Converts the current instance to a <see cref="Span{T}"/> of type <see cref="int"/>.
	/// </summary>
	/// <returns>The <see cref="Span{T}"/> of <see cref="int"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Span<int> ToSpan() => Offsets.AsSpan();

	/// <summary>
	/// Converts the current instance to a <see cref="ReadOnlySpan{T}"/> of type <see cref="int"/>.
	/// </summary>
	/// <returns>The <see cref="ReadOnlySpan{T}"/> of <see cref="int"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly ReadOnlySpan<int> ToReadOnlySpan() => Offsets.AsSpan();

	/// <summary>
	/// Gets the enumerator of the current instance in order to use <see langword="foreach"/> loop.
	/// </summary>
	/// <returns>The enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly OneDimensionalArrayEnumerator<int> GetEnumerator() => Offsets.EnumerateImmutable();

	/// <summary>
	/// Expands the current instance, using the specified digit.
	/// </summary>
	/// <param name="digit">The digit.</param>
	/// <returns>The candidate list.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Candidates Expand(int digit) => this * digit;

	/// <summary>
	/// Being called by <see cref="RowMask"/>, <see cref="ColumnMask"/> and <see cref="BlockMask"/>.
	/// </summary>
	/// <param name="start">The start index.</param>
	/// <param name="end">The end index.</param>
	/// <returns>The region mask.</returns>
	/// <seealso cref="RowMask"/>
	/// <seealso cref="ColumnMask"/>
	/// <seealso cref="BlockMask"/>
	private readonly short CreateMask(int start, int end)
	{
		short result = 0;
		for (int i = start; i < end; i++)
		{
			if (!(this & RegionMaps[i]).IsEmpty)
			{
				result |= (short)(1 << i - start);
			}
		}

		return result;
	}

	/// <summary>
	/// Set the specified offset as <see langword="true"/> or <see langword="false"/> value.
	/// </summary>
	/// <param name="offset">
	/// The offset. This value can be positive and negative. If 
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
	/// <param name="offset">The cell to add, represented as a <see cref="string"/> value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void Add(string offset)
	{
		if (Coordinate.TryParse(offset, out var result))
		{
			InternalAdd(result, true);
		}
	}

	/// <summary>
	/// Set the specified offset as <see langword="true"/> value.
	/// </summary>
	/// <param name="offset">The offset.</param>
	/// <remarks>
	/// Different with <see cref="Add(int)"/>, the method will process negative values,
	/// but this won't.
	/// </remarks>
	/// <seealso cref="Add(int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddAnyway(int offset) => InternalAdd(offset, true);

	/// <summary>
	/// Set the specified offsets as <see langword="true"/> value.
	/// </summary>
	/// <param name="offsets">The offsets to add.</param>
	public void AddRange(in ReadOnlySpan<int> offsets)
	{
		foreach (int cell in offsets)
		{
			AddAnyway(cell);
		}
	}

	/// <inheritdoc cref="AddRange(in ReadOnlySpan{int})"/>
	public void AddRange(IEnumerable<int> offsets)
	{
		foreach (int cell in offsets)
		{
			AddAnyway(cell);
		}
	}

	/// <summary>
	/// Set the specified offset as <see langword="false"/> value.
	/// </summary>
	/// <param name="offset">The offset.</param>
	/// <remarks>
	/// Different with <see cref="Add(int)"/>, this method <b>can't</b> receive the negative value as the parameter.
	/// </remarks>
	/// <seealso cref="Add(int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Remove(int offset) => InternalAdd(offset, false);

	/// <summary>
	/// Clear all bits.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Clear() => _low = _high = Count = 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly bool IEquatable<Cells>.Equals(Cells other) => Equals(other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly IEnumerator<int> IEnumerable<int>.GetEnumerator() => ((IEnumerable<int>)Offsets).GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly IEnumerator IEnumerable.GetEnumerator() => Offsets.GetEnumerator();

	/// <summary>
	/// The internal operation for adding an offset into the current collection.
	/// </summary>
	/// <param name="offset">The cell to add into.</param>
	/// <param name="value">The value to add.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void InternalAdd(int offset, bool value)
	{
		if (offset is >= 0 and < 81)
		{
			ref long v = ref offset / Shifting == 0 ? ref _low : ref _high;
			bool older = Contains(offset);
			if (value)
			{
				v |= 1L << offset % Shifting;
				if (!older)
				{
					Count++;
				}
			}
			else
			{
				v &= ~(1L << offset % Shifting);
				if (older)
				{
					Count--;
				}
			}
		}
	}


	/// <inheritdoc/>
	public static Cells Parse(string str)
	{
		// Check whether the match is successful.
		if (CellOrCellListRegex.Matches(str) is not [_, ..] matches)
		{
			throw new FormatException("The specified string can't match any cell instance.");
		}

		// Declare the buffer.
		int* bufferRows = stackalloc int[9], bufferColumns = stackalloc int[9];

		// Declare the result variable.
		var result = Empty;

		// Iterate on each match instance.
		foreach (Match match in matches)
		{
			string value = match.Value;
			char* anchorR, anchorC;
			fixed (char* pValue = value)
			{
				anchorR = anchorC = pValue;

				// Find the index of the character 'C'.
				// The regular expression guaranteed the string must contain the character 'C' or 'c',
				// so we don't need to check '*p != '\0''.
				while (*anchorC is not ('C' or 'c'/* or '\0'*/))
				{
					anchorC++;
				}
			}

			// Stores the possible values into the buffer.
			int rIndex = 0, cIndex = 0;
			for (char* p = anchorR + 1; *p is not ('C' or 'c'); p++, rIndex++)
			{
				bufferRows[rIndex] = *p - '1';
			}
			for (char* p = anchorC + 1; *p != '\0'; p++, cIndex++)
			{
				bufferColumns[cIndex] = *p - '1';
			}

			// Now combine two buffers.
			for (int i = 0; i < rIndex; i++)
			{
				for (int j = 0; j < cIndex; j++)
				{
					result.Add(bufferRows[i] * 9 + bufferColumns[j]);
				}
			}
		}

		// Returns the result.
		return result;
	}

	/// <inheritdoc/>
	public static bool TryParse(string str, out Cells result)
	{
		try
		{
			result = Parse(str);
			return true;
		}
		catch (Exception ex) when (ex is ArgumentNullException or FormatException)
		{
			result = Empty;
			return false;
		}
	}


	/// <summary>
	/// Gets the peer intersection of the current instance, which simply calls the property
	/// <see cref="PeerIntersection"/>.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	/// <returns>The result list that is the peer intersection of the current instance.</returns>
	/// <remarks>
	/// A <b>Peer Intersection</b> is a set of cells that all cells
	/// from the base collection can be seen.
	/// </remarks>
	/// <seealso cref="PeerIntersection"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Cells operator !(in Cells offsets) => offsets.PeerIntersection;

	/// <summary>
	/// Reverse status for all offsets, which means all <see langword="true"/> bits
	/// will be set <see langword="false"/>, and all <see langword="false"/> bits
	/// will be set <see langword="true"/>.
	/// </summary>
	/// <param name="offsets">The instance to negate.</param>
	/// <returns>The negative result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Cells operator ~(in Cells offsets) =>
		new(~offsets._high & 0xFF_FFFF_FFFFL, ~offsets._low & 0x1FF_FFFF_FFFFL);

	/// <summary>
	/// The syntactic sugar for <c>!(<paramref name="left"/> - <paramref name="right"/>).IsEmpty</c>.
	/// </summary>
	/// <param name="left">The subtrahend.</param>
	/// <param name="right">The subtractor.</param>
	/// <returns>The <see cref="bool"/> value indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator >(in Cells left, in Cells right) => !(left - right).IsEmpty;

	/// <summary>
	/// The syntactic sugar for <c>(<paramref name="left"/> - <paramref name="right"/>).IsEmpty</c>.
	/// </summary>
	/// <param name="left">The subtrahend.</param>
	/// <param name="right">The subtractor.</param>
	/// <returns>The <see cref="bool"/> value indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator <(in Cells left, in Cells right) => (left - right).IsEmpty;

	/// <summary>
	/// Adds the specified <paramref name="offset"/> to the <paramref name="collection"/>,
	/// and returns the added result.
	/// </summary>
	/// <param name="collection">The collection.</param>
	/// <param name="offset">The offset to be removed.</param>
	/// <returns>The result collection.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Cells operator +(in Cells collection, int offset)
	{
		var result = collection;
		if (result.Contains(offset))
		{
			return result;
		}

		(offset / Shifting == 0 ? ref result._low : ref result._high) |= 1L << offset % Shifting;
		result.Count++;
		return result;
	}

	/// <summary>
	/// Removes the specified <paramref name="offset"/> from the <paramref name="collection"/>,
	/// and returns the removed result.
	/// </summary>
	/// <param name="collection">The collection.</param>
	/// <param name="offset">The offset to be removed.</param>
	/// <returns>The result collection.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Cells operator -(in Cells collection, int offset)
	{
		var result = collection;
		if (!result.Contains(offset))
		{
			return collection;
		}

		(offset / Shifting == 0 ? ref result._low : ref result._high) &= ~(1L << offset % Shifting);
		result.Count--;
		return result;
	}

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
	/// Gets the subsets of the current collection via the specified size
	/// indicating the number of elements of the each subset.
	/// </summary>
	/// <param name="cell">Indicates the base template cells.</param>
	/// <param name="subsetSize">The size to get.</param>
	/// <returns>All possible subsets.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Cells[] operator &(in Cells cell, int subsetSize)
	{
		int totalIndex = 0, n = cell.Count;
		int* buffer = stackalloc int[subsetSize];
		var result = new Cells[Combinatorials[n - 1, subsetSize - 1]];
		f(subsetSize, n, subsetSize, cell.Offsets);
		return result;


		void f(int size, int last, int index, int[] offsets)
		{
			for (int i = last; i >= index; i--)
			{
				buffer[index - 1] = i - 1;
				if (index > 1)
				{
					f(size, i - 1, index - 1, offsets);
				}
				else
				{
					int[] temp = new int[size];
					for (int j = 0; j < size; j++)
					{
						temp[j] = offsets[buffer[j]];
					}

					result[totalIndex++] = temp;
				}
			}
		}
	}

	/// <summary>
	/// Get the elements that both <paramref name="left"/> and <paramref name="right"/> contain.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>The result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Cells operator &(in Cells left, in Cells right) =>
		new(left._high & right._high, left._low & right._low);

	/// <summary>
	/// Combine the elements from <paramref name="left"/> and <paramref name="right"/>,
	/// and return the merged result.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>The result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Cells operator |(in Cells left, in Cells right) =>
		new(left._high | right._high, left._low | right._low);

	/// <summary>
	/// Get the elements that either <paramref name="left"/> or <paramref name="right"/> contains.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>The result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Cells operator ^(in Cells left, in Cells right) =>
		new(left._high ^ right._high, left._low ^ right._low);

	/// <summary>
	/// <para>
	/// Simply expands the code to <c><![CDATA[(a & b).PeerIntersection & b]]></c>,
	/// where <c>PeerIntersection</c> corresponds to the property <see cref="PeerIntersection"/>.
	/// </para>
	/// <para>The operator is used for searching for and checking eliminations.</para>
	/// </summary>
	/// <param name="base">The base map.</param>
	/// <param name="template">The template map that the base map to check and cover.</param>
	/// <returns>The result map.</returns>
	/// <seealso cref="PeerIntersection"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Cells operator %(in Cells @base, in Cells template) =>
		(@base & template).PeerIntersection & template;

	/// <summary>
	/// Expands via the specified digit.
	/// </summary>
	/// <param name="base">The base map.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>The result instance.</returns>
	public static Candidates operator *(in Cells @base, int digit)
	{
		var result = Candidates.Empty;
		int[] cells = @base.Offsets;
		fixed (int* p = cells)
		{
			int* ptr = p;
			for (int i = 0, length = cells.Length; i < length; ptr++)
			{
				result.AddAnyway(*ptr * 9 + digit);
			}
		}

		return result;
	}

	/// <summary>
	/// Get the subview mask of this map.
	/// </summary>
	/// <param name="map">The map.</param>
	/// <param name="region">The region.</param>
	/// <returns>The mask.</returns>
	public static short operator /(in Cells map, int region)
	{
		short p = 0, i = 0;
		foreach (int cell in RegionCells[region])
		{
			if (map.Contains(cell))
			{
				p |= (short)(1 << i);
			}

			i++;
		}

		return p;
	}

	/// <summary>
	/// Indicates whether the two <see cref="Cells"/> collection hold a same set of cells.
	/// </summary>
	/// <param name="left">The left-side instance to compare.</param>
	/// <param name="right">The right-side instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(in Cells left, in Cells right) =>
		left._low == right._low && left._high == right._high;

	/// <summary>
	/// Indicates whether the two <see cref="Cells"/> collection don't hold a same set of cells.
	/// </summary>
	/// <param name="left">The left-side instance to compare.</param>
	/// <param name="right">The right-side instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(in Cells left, in Cells right) => !(left == right);

#if FEATURE_GENERIC_MATH
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Cells IAdditionOperators<Cells, int, Cells>.operator +(Cells left, int right) => left + right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Cells ISubtractionOperators<Cells, int, Cells>.operator -(Cells left, int right) => left - right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Cells ISubtractionOperators<Cells, Cells, Cells>.operator -(Cells left, Cells right) =>
		left - right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static short IDivisionOperators<Cells, int, short>.operator /(Cells left, int right) => left / right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Cells IModulusOperators<Cells, Cells, Cells>.operator %(Cells left, Cells right) => left % right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Cells IBitwiseOperators<Cells, Cells, Cells>.operator ~(Cells value) => ~value;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Cells IBitwiseOperators<Cells, Cells, Cells>.operator &(Cells left, Cells right) => left & right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Cells IBitwiseOperators<Cells, Cells, Cells>.operator |(Cells left, Cells right) => left | right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Cells IBitwiseOperators<Cells, Cells, Cells>.operator ^(Cells left, Cells right) => left ^ right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEqualityOperators<Cells, Cells>.operator ==(Cells left, Cells right) => left == right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEqualityOperators<Cells, Cells>.operator !=(Cells left, Cells right) => left != right;

#if FEATURE_GENERIC_MATH_IN_ARG
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Cells IValueAdditionOperators<Cells, int, Cells>.operator +(Cells left, in int right) =>
		left + right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Cells IValueAdditionOperators<Cells, int, Cells>.operator +(in Cells left, in int right) =>
		left + right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Cells IValueSubtractionOperators<Cells, int, Cells>.operator -(Cells left, in int right) =>
		left - right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Cells IValueSubtractionOperators<Cells, int, Cells>.operator -(in Cells left, in int right) =>
		left - right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Cells IValueSubtractionOperators<Cells, Cells, Cells>.operator -(Cells left, in Cells right) =>
		left - right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Cells IValueSubtractionOperators<Cells, Cells, Cells>.operator -(in Cells left, Cells right) =>
		left - right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static short IValueDivisionOperators<Cells, int, short>.operator /(Cells left, in int right) =>
		left / right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static short IValueDivisionOperators<Cells, int, short>.operator /(in Cells left, in int right) =>
		left / right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Cells IValueModulusOperators<Cells, Cells, Cells>.operator %(Cells left, in Cells right) =>
		left % right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Cells IValueModulusOperators<Cells, Cells, Cells>.operator %(in Cells left, Cells right) =>
		left % right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IValueEqualityOperators<Cells, Cells>.operator ==(Cells left, in Cells right) => left == right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IValueEqualityOperators<Cells, Cells>.operator ==(in Cells left, Cells right) => left == right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IValueEqualityOperators<Cells, Cells>.operator !=(Cells left, in Cells right) => left != right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IValueEqualityOperators<Cells, Cells>.operator !=(in Cells left, Cells right) => left != right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Cells IValueBitwiseAndOperators<Cells, Cells, Cells>.operator &(Cells left, in Cells right) =>
		left & right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Cells IValueBitwiseAndOperators<Cells, Cells, Cells>.operator &(in Cells left, Cells right) =>
		left & right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Cells IValueBitwiseOrOperators<Cells, Cells, Cells>.operator |(Cells left, in Cells right) =>
		left | right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Cells IValueBitwiseOrOperators<Cells, Cells, Cells>.operator |(in Cells left, Cells right) =>
		left | right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Cells IValueBitwiseExclusiveOrOperators<Cells, Cells, Cells>.operator ^(Cells left, in Cells right) =>
		left ^ right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Cells IValueBitwiseExclusiveOrOperators<Cells, Cells, Cells>.operator ^(in Cells left, Cells right) =>
		left ^ right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IValueGreaterThanOrLessThanOperators<Cells, Cells>.operator >(Cells left, in Cells right) =>
		left > right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IValueGreaterThanOrLessThanOperators<Cells, Cells>.operator >(in Cells left, Cells right) =>
		left > right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IValueGreaterThanOrLessThanOperators<Cells, Cells>.operator <(Cells left, in Cells right) =>
		left < right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IValueGreaterThanOrLessThanOperators<Cells, Cells>.operator <(in Cells left, Cells right) =>
		left < right;
#endif
#endif


	/// <summary>
	/// Implicit cast from <see cref="int"/>[] to <see cref="Cells"/>.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Cells(int[] offsets) => new(offsets);

	/// <summary>
	/// Implicit cast from <see cref="Span{T}"/> to <see cref="Cells"/>.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Cells(in Span<int> offsets) => new(offsets);

	/// <summary>
	/// Implicit cast from <see cref="ReadOnlySpan{T}"/> to <see cref="Cells"/>.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Cells(in ReadOnlySpan<int> offsets) => new(offsets);

	/// <summary>
	/// Explicit cast from <see cref="Range"/> to <see cref="Cells"/>.
	/// </summary>
	/// <param name="range">The range.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator Cells(Range range) => new(range);

	/// <summary>
	/// Explicit cast from <see cref="Cells"/> to <see cref="Range"/>?.
	/// Returns <see langword="null"/> if the cells are discontinuous,
	/// which cannot be converted to a valid <see cref="Range"/> result.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator Range?(in Cells offsets)
	{
		int first = offsets[0];
		int lastCell = first;
		foreach (int cell in offsets)
		{
			if (cell == first)
			{
				continue;
			}

			if (cell - lastCell != 1)
			{
				return null;
			}

			lastCell = cell;
		}

		return (81 - lastCell) switch
		{
			1 => first..,
			var indexFromLast and (2 or 3) => first..^(indexFromLast - 1),
			_ => first..lastCell
		};
	}

	/// <summary>
	/// Explicit cast from <see cref="Cells"/> to <see cref="int"/>[].
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator int[](in Cells offsets) => offsets.ToArray();

	/// <summary>
	/// Explicit cast from <see cref="Cells"/> to <see cref="Span{T}"/>.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator Span<int>(in Cells offsets) => offsets.ToSpan();

	/// <summary>
	/// Explicit cast from <see cref="Cells"/> to <see cref="ReadOnlySpan{T}"/>.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator ReadOnlySpan<int>(in Cells offsets) => offsets.ToReadOnlySpan();
}
