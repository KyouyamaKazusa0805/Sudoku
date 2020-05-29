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
using static Sudoku.Data.GridMap.InitializeOption;

namespace Sudoku.Data
{
	/// <summary>
	/// Encapsulates a binary series of cell status table.
	/// </summary>
	/// <remarks>
	/// The instance stores two <see cref="long"/> values, consisting of 81 bits,
	/// where <see langword="true"/> bit (1) is for the cell having that digit,
	/// and the <see langword="false"/> bit (0) is for the cell not containing
	/// the digit. Sometimes this type will be used for other cases.
	/// </remarks>
	[DebuggerStepThrough]
	public partial struct GridMap : IComparable<GridMap>, IEnumerable<bool>, IEquatable<GridMap>
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
		/// Indicates the instance that all bits are set <see langword="true"/> ahead of time.
		/// </summary>
		public static readonly GridMap Full;


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
		public GridMap(int offset, bool setItself) : this((IEnumerable<int>)Peers[offset]) => this[offset] = setItself;

		/// <summary>
		/// Initializes an instance with the specified cell offset
		/// with an initialize option.
		/// </summary>
		/// <param name="offset">The cell offset.</param>
		/// <param name="initializeOption">
		/// Indicates the behavior of the initialization.
		/// </param>
		/// <exception cref="ArgumentException">
		/// Throws when the specified initialize option is invalid.
		/// </exception>
		public GridMap(int offset, InitializeOption initializeOption) : this()
		{
			switch (initializeOption)
			{
				case Ordinary:
				{
					Add(offset);

					break;
				}
				case ProcessPeersAlso:
				case ProcessPeersWithoutItself:
				{
					foreach (int peer in Peers[offset])
					{
						Add(peer);
					}

					if (initializeOption == ProcessPeersAlso)
					{
						Add(offset);
					}

					break;
				}
				default:
				{
					throw new ArgumentException("The specified option does not exist.");
				}
			}
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
		/// This method is same behavior of <see cref="GridMap(IEnumerable{int}, InitializeOption)"/>
		/// </remarks>
		/// <seealso cref="GridMap(IEnumerable{int}, InitializeOption)"/>
		public GridMap(int[] offsets, InitializeOption initializeOption)
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
		public GridMap(ReadOnlySpan<int> offsets, InitializeOption initializeOption) : this()
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
		public GridMap(GridMap another) => (_high, _low, Count) = (another._high, another._low, another.Count);

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
		public GridMap(IEnumerable<int> offsets, InitializeOption initializeOption) : this()
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
		/// Initializes an instance with two binary values.
		/// </summary>
		/// <param name="high">Higher 40 bits.</param>
		/// <param name="low">Lower 41 bits.</param>
		private GridMap(long high, long low) => Count = (_high = high).CountSet() + (_low = low).CountSet();


		/// <include file='../../GlobalDocComments.xml' path='comments/staticConstructor[@aimTo="struct"]'/>
		static GridMap() => (Full._high, Full._low, Full.Count) = (-1L, -1L, 81);


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
		/// Indicates the map of cells, which is the peer intersections. For example,
		/// the code
		/// <code>
		/// var map = testMap.PeerIntersection;
		/// </code>
		/// is equivalent to the code
		/// <code>
		/// var map = new GridMap(testMap, InitializeOption.ProcessPeersWithoutItself);
		/// </code>
		/// </summary>
		public readonly GridMap PeerIntersection => new GridMap(Offsets, ProcessPeersWithoutItself);

		/// <summary>
		/// Indicates all regions covered.
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
		/// All regions that the map used.
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
		public readonly IEnumerable<int> Offsets
		{
			get
			{
				if (IsEmpty)
				{
					yield break;
				}

				long value;
				int i;
				for (value = _low, i = 0; i < Shifting; i++, value >>= 1)
				{
					if ((value & 1) != 0)
					{
						yield return i;
					}
				}
				for (value = _high; i < 81; i++, value >>= 1)
				{
					if ((value & 1) != 0)
					{
						yield return i;
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


		/// <include file='../../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="high">(<see langword="out"/> parameter) Higher 40 bits.</param>
		/// <param name="low">(<see langword="out"/> parameter) Lower 41 bits.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void Deconstruct(out long high, out long low) => (high, low) = (_high, _low);

		/// <include file='../../GlobalDocComments.xml' path='comments/method[@name="Equals" and @paramType="object"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly bool Equals(object? obj) => obj is GridMap comparer && Equals(comparer);

		/// <summary>
		/// Indicates whether the current object has the same value with the other one.
		/// </summary>
		/// <param name="other">The other value to compare.</param>
		/// <returns>
		/// The result of this comparison. <see langword="true"/> if two instances hold a same
		/// value; otherwise, <see langword="false"/>.
		/// </returns>
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
		/// Check whether the grid map is fully covered all cells in the specified region.
		/// </summary>
		/// <param name="regionOffset">The region offset.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool AllCellCovers(int regionOffset) => Count - (this - RegionMaps[regionOffset]).Count == 9;

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

		/// <inheritdoc/>
		public readonly IEnumerator<bool> GetEnumerator()
		{
			for (long l = _low; l != 0; l >>= 1)
			{
				yield return (l & 1) != 0;
			}
			for (long h = _high; h != 0; h >>= 1)
			{
				yield return (h & 1) != 0;
			}
		}

		/// <include file='../../GlobalDocComments.xml' path='comments/method[@name="GetHashCode"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly int GetHashCode() => GetType().GetHashCode() ^ (int)((_low ^ _high) & int.MaxValue);

		/// <include file='../../GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="__noparam"]'/>
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
		/// Set the specified cells as <see langword="true"/> value.
		/// </summary>
		/// <param name="offsets">The cells to add.</param>
		public void AddRange(ReadOnlySpan<int> offsets)
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
		public void AddRange(IEnumerable<int> offsets)
		{
			foreach (int cell in offsets)
			{
				Add(cell);
			}
		}

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


		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(GridMap left, GridMap right) => left.Equals(right);

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(GridMap left, GridMap right) => !(left == right);

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_GreaterThan"]'/>
		public static bool operator >(GridMap left, GridMap right) => left.CompareTo(right) > 0;

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_GreaterThanOrEqual"]'/>
		public static bool operator >=(GridMap left, GridMap right) => left.CompareTo(right) >= 0;

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_LessThan"]'/>
		public static bool operator <(GridMap left, GridMap right) => left.CompareTo(right) < 0;

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_LessThanOrEqual"]'/>
		public static bool operator <=(GridMap left, GridMap right) => left.CompareTo(right) <= 0;

		/// <summary>
		/// Reverse all cells' statuses, which means all <see langword="true"/> bits
		/// will be set <see langword="false"/>, and all <see langword="false"/> bits
		/// will be set <see langword="true"/>.
		/// </summary>
		/// <param name="gridMap">The instance to negate.</param>
		/// <returns>The negative result.</returns>
		public static GridMap operator ~(GridMap gridMap) => new GridMap(~gridMap._high, ~gridMap._low);

		/// <summary>
		/// Add a cell into the specified map.
		/// </summary>
		/// <param name="map">The map.</param>
		/// <param name="cell">The cell to remove.</param>
		/// <returns>The map after adding.</returns>
		/// <example>
		/// You can write code like this:
		/// <code>
		/// var map = new GridMap { 1 };
		/// var map2 = map + 3; // Is equivalent to 'map2 = map | new GridMap { 3 }'.
		/// </code>
		/// </example>
		public static GridMap operator +(GridMap map, int cell) => map | new GridMap { cell };

		/// <summary>
		/// Remove a cell from the specified map.
		/// </summary>
		/// <param name="map">The map.</param>
		/// <param name="cell">The cell to remove.</param>
		/// <returns>The map after removing.</returns>
		/// <example>
		/// You can write code like this:
		/// <code>
		/// var map = new GridMap { 1, 4 };
		/// var map2 = map - 4; // Is equivalent to 'map2 = map - new GridMap { 4 }'.
		/// </code>
		/// </example>
		public static GridMap operator -(GridMap map, int cell) => map - new GridMap { cell };

		/// <summary>
		/// Get a <see cref="GridMap"/> that contains all <paramref name="left"/> cells
		/// but not in <paramref name="right"/> cells.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>The result.</returns>
		public static GridMap operator -(GridMap left, GridMap right) => left & ~right;

		/// <summary>
		/// Get all cells that two <see cref="GridMap"/>s both contain.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>The intersection result.</returns>
		public static GridMap operator &(GridMap left, GridMap right) =>
			new GridMap(left._high & right._high, left._low & right._low);

		/// <summary>
		/// Get all cells from two <see cref="GridMap"/>s.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>The union result.</returns>
		public static GridMap operator |(GridMap left, GridMap right) =>
			new GridMap(left._high | right._high, left._low | right._low);

		/// <summary>
		/// Equivalent to code <c>(a - b) | (b - a)</c>, where the operator '<c>-</c>'
		/// is <see cref="operator -(GridMap, GridMap)"/>, and '<c>|</c>'
		/// is <see cref="operator |(GridMap, GridMap)"/>.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>The result.</returns>
		/// <seealso cref="operator -(GridMap, GridMap)"/>
		/// <seealso cref="operator |(GridMap, GridMap)"/>
		public static GridMap operator ^(GridMap left, GridMap right) =>
			new GridMap(left._high ^ right._high, left._low ^ right._low);
	}
}
