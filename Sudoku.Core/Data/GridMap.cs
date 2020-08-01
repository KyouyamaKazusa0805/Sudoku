using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Sudoku.Extensions;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.GridMap.InitializationOption;

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
	[DebuggerStepThrough]
	public partial struct GridMap : IComparable<GridMap>, IEnumerable<int>, IEquatable<GridMap>
	{
		/// <summary>
		/// <para>Indicates an empty instance (all bits are 0).</para>
		/// <para>
		/// I strongly recommend you <b>should</b> use this instance instead of default constructor
		/// <see cref="GridMap()"/>.
		/// </para>
		/// </summary>
		/// <seealso cref="GridMap()"/>
		public static readonly GridMap Empty = default;


		/// <summary>
		/// The value used for shifting.
		/// </summary>
		private const int Shifting = 41;


		/// <summary>
		/// Indicates the internal two <see cref="long"/> values,
		/// which represents 81 bits. <see cref="_high"/> represent the higher
		/// 40 bits and <see cref="_low"/> represents the lower 41 bits.
		/// </summary>
		/// <seealso cref="_low"/>
		/// <seealso cref="_high"/>
		private long _high, _low;


		/// <summary>
		/// Initializes an instance with the specified cell offset
		/// (Sets itself and all peers).
		/// </summary>
		/// <param name="offset">The cell offset.</param>
		public GridMap(int offset) : this(offset, true)
		{
		}

		/// <summary>
		/// Same behavior of the constructor as <see cref="GridMap(IEnumerable{int})"/>.
		/// </summary>
		/// <param name="offsets">All offsets.</param>
		/// <remarks>
		/// This constructor is defined after another constructor with
		/// <see cref="ReadOnlySpan{T}"/> had defined. Although this constructor
		/// does not initialize something (use the other one instead),
		/// while initializing with the type <see cref="int"/>[], the complier
		/// gives me an error without this constructor (ambiguity of two
		/// constructors). However, unfortunately, <see cref="ReadOnlySpan{T}"/>
		/// does not implemented the interface <see cref="IEnumerable{T}"/>...
		/// </remarks>
		/// <seealso cref="GridMap(IEnumerable{int})"/>
		public GridMap(int[] offsets) : this((IEnumerable<int>)offsets)
		{
		}

		/// <summary>
		/// Initializes an instance with cell offsets with an initialize option.
		/// </summary>
		/// <param name="offsets">The offsets to be processed.</param>
		/// <param name="initializeOption">
		/// Indicates the behavior of the initialization.
		/// </param>
		/// <remarks>
		/// This method is same behavior of <see cref="GridMap(IEnumerable{int}, InitializationOption)"/>
		/// </remarks>
		/// <seealso cref="GridMap(IEnumerable{int}, InitializationOption)"/>
		public GridMap(int[] offsets, InitializationOption initializeOption)
			: this((IEnumerable<int>)offsets, initializeOption)
		{
		}

		/// <summary>
		/// Initializes an instance with a series of cell offsets.
		/// </summary>
		/// <param name="offsets">cell offsets.</param>
		/// <remarks>
		/// <para>
		/// Note that all offsets will be set <see langword="true"/>, but their own peers
		/// will not be set <see langword="true"/>.
		/// </para>
		/// <para>
		/// In some case, you can use object initializer instead.
		/// You can use the code
		/// <code>
		/// var map = new GridMap { 0, 3, 5 };
		/// </code>
		/// instead of the code
		/// <code>
		/// var map = new GridMap(stackalloc[] { 0, 3, 5 });
		/// </code>
		/// </para>
		/// </remarks>
		public GridMap(ReadOnlySpan<int> offsets) : this()
		{
			foreach (int offset in offsets)
			{
				(offset / Shifting == 0 ? ref _low : ref _high) |= 1L << offset % Shifting;
				Count++;
			}
		}

		/// <summary>
		/// Initializes an instance with cell offsets with an initialize option.
		/// </summary>
		/// <param name="offsets">The offsets to be processed.</param>
		/// <param name="initializeOption">
		/// Indicates the behavior of the initialization.
		/// </param>
		/// <exception cref="ArgumentException">
		/// Throws when the specified initialize option is invalid.
		/// </exception>
		public GridMap(ReadOnlySpan<int> offsets, InitializationOption initializeOption) : this()
		{
			switch (initializeOption)
			{
				case Ordinary:
				{
					foreach (int offset in offsets)
					{
						this[offset] = true;
					}

					break;
				}
				case ProcessPeersAlso:
				case ProcessPeersWithoutItself:
				{
					int i = 0;
					foreach (int offset in offsets)
					{
						long low = 0, high = 0;
						foreach (int peer in Peers[offset])
						{
							(peer / Shifting == 0 ? ref low : ref high) |= 1L << peer % Shifting;
						}

						if (initializeOption == ProcessPeersAlso)
						{
							(offset / Shifting == 0 ? ref low : ref high) |= 1L << offset % Shifting;
						}

						(_low, _high) = i++ == 0 ? (low, high) : (_low & low, _high & high);
					}

					Count = _low.CountSet() + _high.CountSet();

					break;
				}
				default:
				{
					throw new ArgumentException("The specified option does not exist.");
				}
			}
		}

		/// <summary>
		/// To copy an instance with the specified information.
		/// </summary>
		/// <param name="another">Another instance.</param>
		/// <remarks>
		/// <para>
		/// This constructor is only used for adding or removing some extra cells like:
		/// <code>
		/// var y = new GridMap(x) { [i] = true };
		/// </code>
		/// or
		/// <code>
		/// var y = new GridMap(x) { i };
		/// </code>
		/// </para>
		/// </remarks>
		public GridMap(GridMap another) => this = another;

		/// <summary>
		/// Initializes an instance with a series of cell offsets.
		/// </summary>
		/// <param name="offsets">cell offsets.</param>
		/// <remarks>
		/// Note that all offsets will be set <see langword="true"/>, but their own peers
		/// will not be set <see langword="true"/>.
		/// </remarks>
		public GridMap(IEnumerable<int> offsets) : this()
		{
			foreach (int offset in offsets)
			{
				(offset / Shifting == 0 ? ref _low : ref _high) |= 1L << offset % Shifting;
				Count++;
			}
		}

		/// <summary>
		/// Initializes an instance with cell offsets with an initialize option.
		/// </summary>
		/// <param name="offsets">The offsets to be processed.</param>
		/// <param name="initializeOption">
		/// Indicates the behavior of the initialization.
		/// </param>
		/// <exception cref="ArgumentException">
		/// Throws when the specified initialize option is invalid.
		/// </exception>
		public GridMap(IEnumerable<int> offsets, InitializationOption initializeOption) : this()
		{
			switch (initializeOption)
			{
				case Ordinary:
				{
					foreach (int offset in offsets)
					{
						this[offset] = true;
					}

					break;
				}
				case ProcessPeersAlso:
				case ProcessPeersWithoutItself:
				{
					int i = 0;
					foreach (int offset in offsets)
					{
						long low = 0, high = 0;
						foreach (int peer in Peers[offset])
						{
							(peer / Shifting == 0 ? ref low : ref high) |= 1L << peer % Shifting;
						}

						if (initializeOption == ProcessPeersAlso)
						{
							(offset / Shifting == 0 ? ref low : ref high) |= 1L << offset % Shifting;
						}

						(_low, _high) = i++ == 0 ? (low, high) : (_low & low, _high & high);
					}

					Count = _low.CountSet() + _high.CountSet();

					break;
				}
				default:
				{
					throw new ArgumentException("The specified option does not exist.");
				}
			}
		}

		/// <summary>
		/// Initializes an instance with three binary values.
		/// </summary>
		/// <param name="high">Higher 27 bits.</param>
		/// <param name="mid">Medium 27 bits.</param>
		/// <param name="low">Lower 27 bits.</param>
		public GridMap(int high, int mid, int low)
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
		/// If the value is <see langword="false"/>, it will be equivalent
		/// to below:
		/// <code>
		/// var map = new GridMap(offset) { [offset] = false };
		/// </code>
		/// </param>
		private GridMap(int offset, bool setItself)
		{
			this = PeerMaps[offset];
			this[offset] = setItself;
		}

		/// <summary>
		/// Initializes an instance with two binary values.
		/// </summary>
		/// <param name="high">Higher 40 bits.</param>
		/// <param name="low">Lower 41 bits.</param>
		private GridMap(long high, long low) => Count = (_high = high).CountSet() + (_low = low).CountSet();


		/// <summary>
		/// Indicates whether the map has no set bits.
		/// This property is equivalent to code '<c>!this.IsNotEmpty</c>'.
		/// </summary>
		/// <seealso cref="IsNotEmpty"/>
		public readonly bool IsEmpty => _high == 0 && _low == 0;

		/// <summary>
		/// Indicates whether the map has at least one set bit.
		/// This property is equivalent to code '<c>!this.IsEmpty</c>'.
		/// </summary>
		/// <seealso cref="IsEmpty"/>
		public readonly bool IsNotEmpty => _high != 0 || _low != 0;

		/// <summary>
		/// Indicates the mask of block.
		/// </summary>
		public readonly short BlockMask
		{
			get
			{
				short result = 0;
				for (int i = 0; i < 9; i++)
				{
					if (Overlaps(RegionMaps[i]))
					{
						result |= (short)(1 << i);
					}
				}

				return result;
			}
		}

		/// <summary>
		/// Indicates the mask of row.
		/// </summary>
		public readonly short RowMask
		{
			get
			{
				short result = 0;
				for (int i = 9; i < 18; i++)
				{
					if (Overlaps(RegionMaps[i]))
					{
						result |= (short)(1 << i - 9);
					}
				}

				return result;
			}
		}

		/// <summary>
		/// Indicates the mask of column.
		/// </summary>
		public readonly short ColumnMask
		{
			get
			{
				short result = 0;
				for (int i = 18; i < 27; i++)
				{
					if (Overlaps(RegionMaps[i]))
					{
						result |= (short)(1 << i - 18);
					}
				}

				return result;
			}
		}

		/// <summary>
		/// Indicates the covered line.
		/// </summary>
		public readonly int CoveredLine
		{
			get
			{
				for (int i = 9; i < 27; i++)
				{
					if ((_high & ~CoverTable[i, 0]) == 0 && (_low & ~CoverTable[i, 1]) == 0)
					{
						return i;
					}
				}

				return -1;
			}
		}

		/// <summary>
		/// Indicates the total number of cells where the corresponding
		/// value are set <see langword="true"/>.
		/// </summary>
		public int Count { readonly get; private set; }

		/// <summary>
		/// Indicates the map of cells, which is the peer intersections.
		/// </summary>
		/// <example>
		/// For example, the code
		/// <code>
		/// var map = testMap.PeerIntersection;
		/// </code>
		/// is equivalent to the code
		/// <code>
		/// var map = new GridMap(testMap, InitializeOption.ProcessPeersWithoutItself);
		/// </code>
		/// </example>
		public readonly GridMap PeerIntersection => new GridMap(Offsets, ProcessPeersWithoutItself);

		/// <summary>
		/// Indicates all regions covered. This property is used to check all regions that all cells
		/// of this instance covered. For examp;le, if the cells are { 0, 1 }, the property
		/// <see cref="CoveredRegions"/> will return the region 0 (block 1) and region 9 (row 1);
		/// however, if cells spanned two regions or more (e.g. cells { 0, 1, 27 }), this property will not contain
		/// any regions.
		/// </summary>
		public readonly IEnumerable<int> CoveredRegions
		{
			get
			{
				for (int i = 0; i < 27; i++)
				{
					if ((_high & ~CoverTable[i, 0]) == 0 && (_low & ~CoverTable[i, 1]) == 0)
					{
						yield return i;
					}
				}
			}
		}

		/// <summary>
		/// All regions that the map spanned. This property is used to check all regions that all cells of
		/// this instance spanned. For example, if the cells are { 0, 1 }, the property
		/// <see cref="Regions"/> will return the region 0 (block 1), region 9 (row 1), region 18 (column 1)
		/// and the region 19 (column 2).
		/// </summary>
		[SuppressMessage("", "IDE0004:Remove redundant cast")]
		public readonly IEnumerable<int> Regions => ((int)BlockMask | RowMask << 9 | ColumnMask << 18).GetAllSets();

		/// <summary>
		/// <para>
		/// Indicates all cell offsets whose corresponding value
		/// are set <see langword="true"/>.
		/// </para>
		/// <para>
		/// If you want to make an array of them, please use method
		/// <see cref="ToArray"/> instead of code
		/// '<c>Offsets.ToArray()</c>'.
		/// </para>
		/// </summary>
		/// <seealso cref="ToArray"/>
		private readonly IEnumerable<int> Offsets
		{
			get
			{
				if (IsEmpty)
				{
					yield break;
				}

				long value;
				int i;
				if (_low != 0)
				{
					for (value = _low, i = 0; i < Shifting; i++, value >>= 1)
					{
						if ((value & 1) != 0)
						{
							yield return i;
						}
					}
				}
				if (_high != 0)
				{
					for (value = _high, i = Shifting; i < 81; i++, value >>= 1)
					{
						if ((value & 1) != 0)
						{
							yield return i;
						}
					}
				}
			}
		}


		/// <summary>
		/// Gets or sets a <see cref="bool"/> value on the specified cell
		/// offset.
		/// </summary>
		/// <param name="offset">The cell offset.</param>
		/// <value>
		/// A <see cref="bool"/> value on assignment.
		/// </value>
		/// <returns>
		/// A <see cref="bool"/> value indicating whether the cell has digit.
		/// </returns>
		public bool this[int offset]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get => ((stackalloc[] { _low, _high }[offset / Shifting] >> offset % Shifting) & 1) != 0;
			set
			{
				ref long v = ref offset / Shifting == 0 ? ref _low : ref _high;
				bool older = this[offset];
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


		/// <include file='..\GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="high">(<see langword="out"/> parameter) Higher 40 bits.</param>
		/// <param name="low">(<see langword="out"/> parameter) Lower 41 bits.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void Deconstruct(out long high, out long low) => (high, low) = (_high, _low);

		/// <include file='..\GlobalDocComments.xml' path='comments/method[@name="Equals" and @paramType="object"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly bool Equals(object? obj) => obj is GridMap comparer && Equals(comparer);

		/// <include file='..\GlobalDocComments.xml' path='comments/method[@name="Equals" and @paramType="__any"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(GridMap other) => _high == other._high && _low == other._low;

		/// <summary>
		/// Indicates whether this map overlaps another one.
		/// </summary>
		/// <param name="other">The other map.</param>
		/// <returns>The <see cref="bool"/> value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Overlaps(GridMap other) => (this & other).IsNotEmpty;

		/// <summary>
		/// Indicates whether all cells in this instance are in one region.
		/// </summary>
		/// <param name="region">
		/// (<see langword="out"/> parameter) The region covered. If the return value
		/// is false, this value will be the constant -1.
		/// </param>
		public readonly bool AllSetsAreInOneRegion(out int region)
		{
			for (int i = 0; i < 27; i++)
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
		/// Get a n-th index of the <see langword="true"/> bit in this instance.
		/// </summary>
		/// <param name="index">The true bit index order.</param>
		/// <returns>The real index.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int SetAt(int index) => Offsets.ElementAt(index);

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int CompareTo(GridMap other) =>
			((new BigInteger(_high) << Shifting) + new BigInteger(_low)).CompareTo(
				(new BigInteger(other._high) << Shifting) + new BigInteger(other._low));

		/// <summary>
		/// Get all cell offsets whose bits are set <see langword="true"/>.
		/// </summary>
		/// <returns>An array of cell offsets.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int[] ToArray() => Offsets.ToArray();

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
				if (this[cell])
				{
					p |= (short)(1 << i);
				}

				i++;
			}

			return p;
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly IEnumerator<int> GetEnumerator() => Offsets.GetEnumerator();

		/// <include file='..\GlobalDocComments.xml' path='comments/method[@name="GetHashCode"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly int GetHashCode() => GetType().GetHashCode() ^ (int)((_low ^ _high) & int.MaxValue);

		/// <include file='..\GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="__noparam"]'/>
		public override readonly string ToString()
		{
			var sb = new StringBuilder();
			int i;
			long value = _low;
			for (i = 0; i < 27; i++, value >>= 1)
			{
				sb.Append(value & 1);
			}
			sb.Append(" ");
			for (; i < 41; i++, value >>= 1)
			{
				sb.Append(value & 1);
			}
			for (value = _high; i < 54; i++, value >>= 1)
			{
				sb.Append(value & 1);
			}
			sb.Append(" ");
			for (; i < 81; i++, value >>= 1)
			{
				sb.Append(value & 1);
			}

			return sb.Reverse().ToString();
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		/// <summary>
		/// Set the specified cell as <see langword="true"/> value.
		/// </summary>
		/// <param name="offset">The cell offset.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(int offset) => this[offset] = true;

		/// <summary>
		/// Set the specified cell as <see langword="false"/> value.
		/// </summary>
		/// <param name="offset">The cell offset.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Remove(int offset) => this[offset] = false;

		/// <summary>
		/// Clear all bits.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear() => _low = _high = Count = 0;

		/// <summary>
		/// Set the specified cells as <see langword="true"/> value.
		/// </summary>
		/// <param name="offsets">The cells to add.</param>
		internal void AddRange(ReadOnlySpan<int> offsets)
		{
			foreach (int cell in offsets)
			{
				Add(cell);
			}
		}

		/// <summary>
		/// Set the specified cells as <see langword="true"/> value.
		/// </summary>
		/// <param name="offsets">The cells to add.</param>
		internal void AddRange(IEnumerable<int> offsets)
		{
			foreach (int cell in offsets)
			{
				Add(cell);
			}
		}


		/// <include file='..\GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(GridMap left, GridMap right) => left.Equals(right);

		/// <include file='..\GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(GridMap left, GridMap right) => !(left == right);

		/// <include file='..\GlobalDocComments.xml' path='comments/operator[@name="op_GreaterThan"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator >(GridMap left, GridMap right) => left.CompareTo(right) > 0;

		/// <include file='..\GlobalDocComments.xml' path='comments/operator[@name="op_GreaterThanOrEqual"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator >=(GridMap left, GridMap right) => left.CompareTo(right) >= 0;

		/// <include file='..\GlobalDocComments.xml' path='comments/operator[@name="op_LessThan"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator <(GridMap left, GridMap right) => left.CompareTo(right) < 0;

		/// <include file='..\GlobalDocComments.xml' path='comments/operator[@name="op_LessThanOrEqual"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator <=(GridMap left, GridMap right) => left.CompareTo(right) <= 0;

		/// <summary>
		/// Reverse status for all cells, which means all <see langword="true"/> bits
		/// will be set <see langword="false"/>, and all <see langword="false"/> bits
		/// will be set <see langword="true"/>.
		/// </summary>
		/// <param name="gridMap">The instance to negate.</param>
		/// <returns>The negative result.</returns>
		/// <remarks>
		/// While reversing the higher 40 bits, the unused bits will be fixed and never be modified the state,
		/// that is why using the code "<c>higherBits &amp; 0xFFFFFFFFFFL</c>".
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GridMap operator ~(GridMap gridMap) => new GridMap(~gridMap._high & 0xFFFFFFFFFFL, ~gridMap._low);

		/// <summary>
		/// Add a cell into the specified map.
		/// </summary>
		/// <param name="map">The map.</param>
		/// <param name="cell">The cell to remove.</param>
		/// <returns>The map after adding.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GridMap operator +(GridMap map, int cell)
		{
			var result = map;
			result.Add(cell);

			return result;
		}

		/// <summary>
		/// Add a cell into the specified map.
		/// </summary>
		/// <param name="map">The map.</param>
		/// <param name="cell">The cell to remove.</param>
		/// <returns>The map after adding.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GridMap operator +(int cell, GridMap map) => map + cell;

		/// <summary>
		/// Remove a cell from the specified map.
		/// </summary>
		/// <param name="map">The map.</param>
		/// <param name="cell">The cell to remove.</param>
		/// <returns>The map after removing.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GridMap operator -(GridMap map, int cell)
		{
			var result = map;
			result.Remove(cell);

			return result;
		}

		/// <summary>
		/// Get a <see cref="GridMap"/> that contains all <paramref name="left"/> cells
		/// but not in <paramref name="right"/> cells.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>The result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GridMap operator -(GridMap left, GridMap right) => left & ~right;

		/// <summary>
		/// Get all cells that two <see cref="GridMap"/>s both contain.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>The intersection result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GridMap operator &(GridMap left, GridMap right) =>
			new GridMap(left._high & right._high, left._low & right._low);

		/// <summary>
		/// Get all cells from two <see cref="GridMap"/>s.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>The union result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GridMap operator |(GridMap left, GridMap right) =>
			new GridMap(left._high | right._high, left._low | right._low);

		/// <summary>
		/// Get all cells that only appears once in two <see cref="GridMap"/>s.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>The symmetrical difference result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GridMap operator ^(GridMap left, GridMap right) =>
			new GridMap(left._high ^ right._high, left._low ^ right._low);


		/// <summary>
		/// Implicite cast from <see cref="int"/>[] to <see cref="GridMap"/>.
		/// </summary>
		/// <param name="cells">The cells.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator GridMap(int[] cells) => new GridMap(cells);

		/// <summary>
		/// Implicit cast from <see cref="ReadOnlySpan{T}"/> to <see cref="GridMap"/>.
		/// </summary>
		/// <param name="cells">The cells.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator GridMap(ReadOnlySpan<int> cells) => new GridMap(cells);

		/// <summary>
		/// Explicit cast from <see cref="GridMap"/> to <see cref="int"/>[].
		/// </summary>
		/// <param name="map">The map.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int[](GridMap map) => map.ToArray();
	}
}
