using System;
using System.Collections;
using System.Collections.Generic;
using System.Extensions;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Sudoku.DocComments;
using static System.Numerics.BitOperations;
using static Sudoku.Constants.Tables;

namespace Sudoku.Data
{
	/// <summary>
	/// Encapsulates a binary series of cell status table.
	/// </summary>
	/// <remarks>
	/// The instance stores two <see cref="long"/> values, consisting of 81 bits,
	/// where <see langword="true"/> bit (1) is for the cell having that digit,
	/// and the <see langword="false"/> bit (0) is for the cell not containing
	/// the digit.
	/// </remarks>
	public partial struct Cells : IEnumerable<int>, IValueEquatable<Cells>, IFormattable
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
		/// Initializes an instance with the specified cell offset
		/// (Sets itself and all peers).
		/// </summary>
		/// <param name="offset">The cell offset.</param>
		/// <remarks>
		/// If you don't want to set itself, you may use <see cref="PeerMaps"/>
		/// instead.
		/// </remarks>
		/// <seealso cref="PeerMaps"/>
		public Cells(int offset) : this(offset, true)
		{
		}

		/// <summary>
		/// Same behavior of the constructor as <see cref="Cells(IEnumerable{int})"/>.
		/// </summary>
		/// <param name="offsets">All offsets.</param>
		/// <remarks>
		/// This constructor is defined after another constructor with
		/// <see cref="ReadOnlySpan{T}"/> had defined. Although this constructor
		/// doesn't initialize something (use the other one instead),
		/// while initializing with the type <see cref="int"/>[], the complier
		/// gives me an error without this constructor (ambiguity of two
		/// constructors). However, unfortunately, <see cref="ReadOnlySpan{T}"/>
		/// doesn't implemented the interface <see cref="IEnumerable{T}"/>.
		/// </remarks>
		/// <seealso cref="Cells(IEnumerable{int})"/>
		public Cells(int[] offsets) : this((IEnumerable<int>)offsets)
		{
		}

		/// <summary>
		/// Initializes an instance with a series of cell offsets.
		/// </summary>
		/// <param name="cells">(<see langword="in"/> parameter) cell offsets.</param>
		/// <remarks>
		/// <para>
		/// Note that all offsets will be set <see langword="true"/>, but their own peers
		/// won't be set <see langword="true"/>.
		/// </para>
		/// <para>
		/// In some case, you can use object initializer instead.
		/// You can use the code
		/// <code>
		/// var map = new Cells { 0, 3, 5 };
		/// </code>
		/// instead of the code
		/// <code>
		/// var map = new Cells(stackalloc[] { 0, 3, 5 });
		/// </code>
		/// </para>
		/// </remarks>
		public Cells(in ReadOnlySpan<int> cells) : this()
		{
			foreach (int offset in cells)
			{
				(offset / Shifting == 0 ? ref _low : ref _high) |= 1L << offset % Shifting;
				Count++;
			}
		}

		/// <summary>
		/// Initializes an instance with a series of cell offsets.
		/// </summary>
		/// <param name="offsets">cell offsets.</param>
		/// <remarks>
		/// Note that all offsets will be set <see langword="true"/>, but their own peers
		/// won't be set <see langword="true"/>.
		/// </remarks>
		public Cells(IEnumerable<int> offsets) : this()
		{
			foreach (int offset in offsets)
			{
				(offset / Shifting == 0 ? ref _low : ref _high) |= 1L << offset % Shifting;
				Count++;
			}
		}

		/// <summary>
		/// Initializes an instance with three binary values.
		/// </summary>
		/// <param name="high">Higher 27 bits.</param>
		/// <param name="mid">Medium 27 bits.</param>
		/// <param name="low">Lower 27 bits.</param>
		public Cells(int high, int mid, int low)
			: this((high & 0x7FFFFFFL) << 13 | (mid >> 14 & 0x1FFFL), (mid & 0x3FFFL) << 27 | (low & 0x7FFFFFFL))
		{
		}

		/// <summary>
		/// Initializes an instance with the specified cell offset.
		/// This will set all bits of all peers of this cell. Another
		/// <see cref="bool"/> value indicates whether this initialization
		/// will set the bit of itself.
		/// </summary>
		/// <param name="offset">The cell offset.</param>
		/// <param name="setItself">
		/// A <see cref="bool"/> value indicating whether this initialization
		/// will set the bit of itself.
		/// </param>
		/// <remarks>
		/// If you want to use this constructor, please use <see cref="PeerMaps"/> instead.
		/// </remarks>
		/// <seealso cref="PeerMaps"/>
		private Cells(int offset, bool setItself)
		{
			// Don't merge those two to one.
			this = PeerMaps[offset];
			InternalAdd(offset, setItself);
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
			Count = PopCount((ulong)_high) + PopCount((ulong)_low);
		}


		/// <summary>
		/// Indicates whether the map has no set bits.
		/// </summary>
		public readonly bool IsEmpty
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _high == 0 && _low == 0;
		}

		/// <summary>
		/// Same as <see cref="AllSetsAreInOneRegion(out int)"/>, but only contains
		/// the <see cref="bool"/> result.
		/// </summary>
		/// <seealso cref="AllSetsAreInOneRegion(out int)"/>
		public readonly bool InOneRegion
		{
			get
			{
				for (int i = BlockOffset; i < Limit; i++)
				{
					if ((_high & ~CoverTable[i, 0]) == 0 && (_low & ~CoverTable[i, 1]) == 0)
					{
						return true;
					}
				}

				return false;
			}
		}

		/// <summary>
		/// Indicates the mask of block.
		/// </summary>
		public readonly short BlockMask
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => GetFirst(BlockOffset, RowOffset);
		}

		/// <summary>
		/// Indicates the mask of row.
		/// </summary>
		public readonly short RowMask
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => GetFirst(RowOffset, ColumnOffset);
		}

		/// <summary>
		/// Indicates the mask of column.
		/// </summary>
		public readonly short ColumnMask
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => GetFirst(ColumnOffset, Limit);
		}

		/// <summary>
		/// Indicates the covered line.
		/// </summary>
		public readonly int CoveredLine
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => TrailingZeroCount(CoveredRegions & ~511);
		}

		/// <summary>
		/// Indicates the total number of cells where the corresponding
		/// value are set <see langword="true"/>.
		/// </summary>
		public int Count { readonly get; private set; }

		/// <summary>
		/// Indicates all regions covered. This property is used to check all regions that all cells
		/// of this instance covered. For example, if the cells are { 0, 1 }, the property
		/// <see cref="CoveredRegions"/> will return the region 0 (block 1) and region 9 (row 1);
		/// however, if cells spanned two regions or more (e.g. cells { 0, 1, 27 }), this property won't contain
		/// any regions.
		/// </summary>
		/// <remarks>
		/// The return value will be an <see cref="int"/> value indicating each regions. Bits set 1 are
		/// covered regions.
		/// </remarks>
		public readonly int CoveredRegions
		{
			get
			{
				int resultRegions = 0;
				for (int i = BlockOffset; i < Limit; i++)
				{
					if ((_high & ~CoverTable[i, 0]) == 0 && (_low & ~CoverTable[i, 1]) == 0)
					{
						resultRegions |= 1 << i;
					}
				}

				return resultRegions;
			}
		}

		/// <summary>
		/// All regions that the map spanned. This property is used to check all regions that all cells of
		/// this instance spanned. For example, if the cells are { 0, 1 }, the property
		/// <see cref="Regions"/> will return the region 0 (block 1), region 9 (row 1), region 18 (column 1)
		/// and the region 19 (column 2).
		/// </summary>
		public readonly int Regions
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (int)BlockMask | RowMask << RowOffset | ColumnMask << ColumnOffset;
		}

		/// <summary>
		/// Indicates the map of cells, which is the peer intersections.
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

		/// <summary>
		/// Indicates all cell offsets whose corresponding value are set <see langword="true"/>.
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
				var result = (stackalloc int[Count]);
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

				return result.ToArray();
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
		[IndexerName("SetOffset")]
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


		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="high">(<see langword="out"/> parameter) Higher 40 bits.</param>
		/// <param name="low">(<see langword="out"/> parameter) Lower 41 bits.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void Deconstruct(out long high, out long low)
		{
			high = _high;
			low = _low;
		}

		/// <inheritdoc cref="object.Equals(object?)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly bool Equals(object? obj) => obj is Cells comparer && Equals(comparer);

		/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
		[CLSCompliant(false)]
		public readonly bool Equals(in Cells other) => _high == other._high && _low == other._low;

		/// <summary>
		/// Indicates whether this map overlaps another one.
		/// </summary>
		/// <param name="other">(<see langword="in"/> parameter) The other map.</param>
		/// <returns>The <see cref="bool"/> value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Overlaps(in Cells other) => !(this & other).IsEmpty;

		/// <summary>
		/// Indicates whether all cells in this instance are in one region.
		/// </summary>
		/// <param name="region">
		/// (<see langword="out"/> parameter) The region covered. If the return value
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
			for (int i = BlockOffset; i < Limit; i++)
			{
				if ((_high & ~CoverTable[i, 0]) == 0 && (_low & ~CoverTable[i, 1]) == 0)
				{
					region = i;
					return true;
				}
			}

			region = -1;
			return false;
		}

		/// <summary>
		/// Determine whether the map contains the specified cell.
		/// </summary>
		/// <param name="cell">The cell.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly unsafe bool Contains(int cell)
		{
			long* ptr = stackalloc[] { _low, _high };
			return (ptr[cell / Shifting] >> cell % Shifting & 1) != 0;
		}

		/// <summary>
		/// Get the subview mask of this map.
		/// </summary>
		/// <param name="region">The region.</param>
		/// <returns>The mask.</returns>
		public readonly short GetSubviewMask(int region)
		{
			short p = 0, i = 0;
			foreach (int cell in RegionCells[region])
			{
				if (Contains(cell))
				{
					p |= (short)(1 << i);
				}

				i++;
			}

			return p;
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly IEnumerator<int> GetEnumerator() => ((IEnumerable<int>)Offsets).GetEnumerator();

		/// <inheritdoc cref="object.GetHashCode"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly int GetHashCode() => ToString("b").GetHashCode();

		/// <summary>
		/// Get all set cell offsets and returns them as an array.
		/// </summary>
		/// <returns>An array of all set cell offsets.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int[] ToArray() => Offsets;

		/// <inheritdoc cref="object.ToString"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly string ToString() => ToString(null);

		/// <inheritdoc cref="Formattable.ToString(string?)"/>
		/// <exception cref="FormatException">Throws when the format is invalid.</exception>
		public readonly string ToString(string? format)
		{
			return format switch
			{
				null or "N" or "n" => Count switch
				{
					0 => "{ }",
					1 when Offsets[0] is var cell => $"r{cell / 9 + 1}c{cell % 9 + 1}",
					_ => normalToString(this)
				},
				"B" or "b" => binaryToString(this, false),
				"T" or "t" => tableToString(this),
				_ => throw new FormatException("The specified format is invalid.")
			};

			static string tableToString(in Cells @this)
			{
				var sb = new StringBuilder();
				for (int i = 0; i < 3; i++)
				{
					for (int bandLine = 0; bandLine < 3; bandLine++)
					{
						for (int j = 0; j < 3; j++)
						{
							for (int columnLine = 0; columnLine < 3; columnLine++)
							{
								sb
									.Append(
										@this.Contains((i * 3 + bandLine) * 9 + j * 3 + columnLine) ? '*' : '.')
									.Append(' ');
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
						sb.AppendLine("------+-------+------");
					}
				}

				return sb.ToString();
			}

			static string normalToString(in Cells @this)
			{
				const string leftCurlyBrace = "{ ", rightCurlyBrace = " }", separator = ", ";
				var sbRow = new StringBuilder();
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
					sbRow
						.Append('r')
						.Append(row + 1)
						.Append('c')
						.AppendRange(dic[row], static v => (v + 1).ToString())
						.Append(separator);
				}
				sbRow.RemoveFromEnd(separator.Length);
				if (addCurlyBraces)
				{
					sbRow.Append(rightCurlyBrace);
				}

				dic.Clear();
				var sbColumn = new StringBuilder();
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
					sbColumn
						.Append('r')
						.AppendRange(dic[column], static v => (v + 1).ToString())
						.Append('c')
						.Append(column + 1)
						.Append(separator);
				}
				sbColumn.RemoveFromEnd(separator.Length);
				if (addCurlyBraces)
				{
					sbColumn.Append(rightCurlyBrace);
				}

				return (sbRow.Length > sbColumn.Length ? sbColumn : sbRow).ToString();
			}

			static string binaryToString(in Cells @this, bool withSeparator)
			{
				var sb = new StringBuilder();
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

				return sb.Reverse().ToString();
			}
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly string ToString(string? format, IFormatProvider? formatProvider) =>
			formatProvider.HasFormatted(this, format, out string? result) ? result : ToString(format);

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		/// <summary>
		/// Being called by <see cref="RowMask"/>, <see cref="ColumnMask"/> and <see cref="BlockMask"/>.
		/// </summary>
		/// <param name="start">The start index.</param>
		/// <param name="end">The end index.</param>
		/// <returns>The region mask.</returns>
		/// <seealso cref="RowMask"/>
		/// <seealso cref="ColumnMask"/>
		/// <seealso cref="BlockMask"/>
		private readonly short GetFirst(int start, int end)
		{
			short result = 0;
			for (int i = start; i < end; i++)
			{
				if (Overlaps(RegionMaps[i]))
				{
					result |= (short)(1 << i - start);
				}
			}

			return result;
		}

		/// <summary>
		/// Set the specified cell as <see langword="true"/> or <see langword="false"/> value.
		/// </summary>
		/// <param name="offset">
		/// The cell offset. This value can be positive and negative. If 
		/// negative, the offset will be assigned <see langword="false"/>
		/// into the corresponding bit position of its absolute value.
		/// </param>
		/// <remarks>
		/// <para>
		/// For example, if the offset is -2 (~1), the [1] will be assigned <see langword="false"/>:
		/// <code>
		/// var map = new Cells(xxx) { ~1 };
		/// </code>
		/// which is equivalent to:
		/// <code>
		/// var map = new Cells(xxx);
		/// map[1] = false;
		/// </code>
		/// </para>
		/// <para>
		/// Note: The argument <paramref name="offset"/> should be with the bit-complement operator <c>~</c>
		/// to describe the value is a negative one. As the belowing example, -2 is described as <c>~1</c>,
		/// so the offset is 1, rather than 2.
		/// </para>
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
		/// <remarks>
		/// Different with <see cref="Add(int)"/>, the method will process negative values,
		/// but this won't.
		/// </remarks>
		/// <seealso cref="Add(int)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddAnyway(int offset) => InternalAdd(offset, true);

		/// <summary>
		/// Set the specified cells as <see langword="true"/> value.
		/// </summary>
		/// <param name="offsets">(<see langword="in"/> parameter) The cells to add.</param>
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
		/// Clear all bits.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear() => _low = _high = Count = 0;

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


		/// <inheritdoc cref="Operators.operator =="/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(in Cells left, in Cells right) => left.Equals(right);

		/// <inheritdoc cref="Operators.operator !="/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(in Cells left, in Cells right) => !(left == right);

		/// <summary>
		/// The syntactic sugar for <c>!(<paramref name="left"/> - <paramref name="right"/>).IsEmpty</c>.
		/// </summary>
		/// <param name="left">(<see langword="in"/> parameter) The subtrahend.</param>
		/// <param name="right">(<see langword="in"/> parameter) The subtractor.</param>
		/// <returns>The <see cref="bool"/> value indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator >(in Cells left, in Cells right) => !(left - right).IsEmpty;

		/// <summary>
		/// The syntactic sugar for <c>(<paramref name="left"/> - <paramref name="right"/>).IsEmpty</c>.
		/// </summary>
		/// <param name="left">(<see langword="in"/> parameter) The subtrahend.</param>
		/// <param name="right">(<see langword="in"/> parameter) The subtractor.</param>
		/// <returns>The <see cref="bool"/> value indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator <(in Cells left, in Cells right) => !(left > right);

		/// <summary>
		/// The syntactic sugar for <c>new Cells(map) { cell }</c> (i.e. add a new cell into the current
		/// map, and return the new map).
		/// </summary>
		/// <param name="map">(<see langword="in"/> parameter) The map.</param>
		/// <param name="cell">The cell to add.</param>
		/// <returns>The result of the map.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Cells operator +(in Cells map, int cell) => new(map) { cell };

		/// <summary>
		/// The syntactic sugar for <c>new Cells(map) { ~cell }</c> (i.e. remove a cell from the current
		/// map, and return the new map).
		/// </summary>
		/// <param name="map">(<see langword="in"/> parameter) The map.</param>
		/// <param name="cell">The cell to remove.</param>
		/// <returns>The result of the map.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Cells operator -(in Cells map, int cell) => new(map) { ~cell };

		/// <summary>
		/// Reverse status for all cells, which means all <see langword="true"/> bits
		/// will be set <see langword="false"/>, and all <see langword="false"/> bits
		/// will be set <see langword="true"/>.
		/// </summary>
		/// <param name="gridMap">(<see langword="in"/> parameter) The instance to negate.</param>
		/// <returns>The negative result.</returns>
		/// <remarks>
		/// While reversing the higher 40 bits, the unused bits will be fixed and never be modified the state,
		/// that is why using the code "<c>higherBits &amp; 0xFFFFFFFFFFL</c>".
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Cells operator ~(in Cells gridMap) =>
			new(~gridMap._high & 0xFFFFFFFFFFL, ~gridMap._low & 0x1FFFFFFFFFFL);

		/// <summary>
		/// Get a <see cref="Cells"/> that contains all <paramref name="left"/> cells
		/// but not in <paramref name="right"/> cells.
		/// </summary>
		/// <param name="left">(<see langword="in"/> parameter) The left instance.</param>
		/// <param name="right">(<see langword="in"/> parameter) The right instance.</param>
		/// <returns>The result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Cells operator -(in Cells left, in Cells right) => left & ~right;

		/// <inheritdoc cref="operator -(in Cells, in Cells)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Cells operator -(IEnumerable<int> left, in Cells right) => new Cells(left) - right;

		/// <inheritdoc cref="operator -(in Cells, in Cells)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Cells operator -(in Cells left, IEnumerable<int> right) => left - new Cells(right);

		/// <summary>
		/// Get all cells that two <see cref="Cells"/>s both contain.
		/// </summary>
		/// <param name="left">(<see langword="in"/> parameter) The left instance.</param>
		/// <param name="right">(<see langword="in"/> parameter) The right instance.</param>
		/// <returns>The intersection result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Cells operator &(in Cells left, in Cells right) =>
			new(left._high & right._high, left._low & right._low);

		/// <summary>
		/// Get all cells from two <see cref="Cells"/>s.
		/// </summary>
		/// <param name="left">(<see langword="in"/> parameter) The left instance.</param>
		/// <param name="right">(<see langword="in"/> parameter) The right instance.</param>
		/// <returns>The union result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Cells operator |(in Cells left, in Cells right) =>
			new(left._high | right._high, left._low | right._low);

		/// <summary>
		/// Get all cells that only appears once in two <see cref="Cells"/>s.
		/// </summary>
		/// <param name="left">(<see langword="in"/> parameter) The left instance.</param>
		/// <param name="right">(<see langword="in"/> parameter) The right instance.</param>
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

		/// <summary>
		/// Implicit cast from <see cref="Span{T}"/> to <see cref="Cells"/>.
		/// </summary>
		/// <param name="cells">(<see langword="in"/> parameter) The cells.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Cells(in Span<int> cells) => new(cells);

		/// <summary>
		/// Implicit cast from <see cref="ReadOnlySpan{T}"/> to <see cref="Cells"/>.
		/// </summary>
		/// <param name="cells">(<see langword="in"/> parameter) The cells.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Cells(in ReadOnlySpan<int> cells) => new(cells);
	}
}
