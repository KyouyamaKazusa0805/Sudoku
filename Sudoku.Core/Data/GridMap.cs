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
using static Sudoku.GridProcessings;

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
		public static readonly GridMap Empty = new GridMap();

		/// <summary>
		/// Indicates the instance that all bits are set <see langword="true"/> ahead of time.
		/// </summary>
		public static readonly GridMap Full;


		/// <summary>
		/// The value used for shifting.
		/// </summary>
		private const int Shifting = 41;


		/// <summary>
		/// <para>
		/// Indicates the internal two <see cref="long"/> values,
		/// which represents 81 bits. <see cref="_high"/> represent the higher
		/// 40 bits and <see cref="_low"/> represents the lower 41 bits.
		/// </para>
		/// <para>
		/// This data structure is mutable because of these two fields (these
		/// two fields are not <see langword="readonly"/>, but Roslyn lies).
		/// </para>
		/// </summary>
		[SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "<Pending>")]
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
		public GridMap(int offset, bool setItself) : this((IEnumerable<int>)Peers[offset]) =>
			this[offset] = setItself;

		/// <summary>
		/// To copy an instance with the specified information.
		/// This constructor is only used for adding or removing some extra cells like:
		/// <code>
		/// var y = new GridMap(x) { [i] = true };
		/// </code>
		/// or
		/// <code>
		/// var y = new GridMap(x) { i };
		/// </code>
		/// </summary>
		/// <param name="another">Another instance.</param>
		public GridMap(GridMap another) =>
			(_high, _low, Count) = (another._high, another._low, another.Count);

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
		/// Initializes an instance with a series of cell offsets.
		/// </summary>
		/// <param name="offsets">cell offsets.</param>
		/// <remarks>
		/// Note that all offsets will be set <see langword="true"/>, but their own peers
		/// will not be set <see langword="true"/>.
		/// </remarks>
		public GridMap(ReadOnlySpan<int> offsets)
		{
			(_low, _high, Count) = (0, 0, 0);
			foreach (int offset in offsets)
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
		/// will not be set <see langword="true"/>.
		/// </remarks>
		public GridMap(IEnumerable<int> offsets)
		{
			(_low, _high, Count) = (0, 0, 0);
			foreach (int offset in offsets)
			{
				(offset / Shifting == 0 ? ref _low : ref _high) |= 1L << offset % Shifting;
				Count++;
			}
		}

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
		public GridMap(int offset, InitializeOption initializeOption)
		{
			(_low, _high, Count) = (0, 0, 0);
			switch (initializeOption)
			{
				case InitializeOption.Ordinary:
				{
					Add(offset);

					break;
				}
				case InitializeOption.ProcessPeersAlso:
				case InitializeOption.ProcessPeersWithoutItself:
				{
					foreach (int peer in Peers[offset])
					{
						Add(peer);
					}

					if (initializeOption == InitializeOption.ProcessPeersAlso)
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
		/// Initializes an instance with cell offsets with an initialize option.
		/// </summary>
		/// <param name="offsets">The offsets to be processed.</param>
		/// <param name="initializeOption">
		/// Indicates the behavior of the initialization.
		/// </param>
		/// <exception cref="ArgumentException">
		/// Throws when the specified initialize option is invalid.
		/// </exception>
		public GridMap(ReadOnlySpan<int> offsets, InitializeOption initializeOption)
		{
			(_low, _high, Count) = (0, 0, 0);
			switch (initializeOption)
			{
				case InitializeOption.Ordinary:
				{
					foreach (int offset in offsets)
					{
						this[offset] = true;
					}

					break;
				}
				case InitializeOption.ProcessPeersAlso:
				case InitializeOption.ProcessPeersWithoutItself:
				{
					static void process(ref long low, ref long high, int peer) =>
						(peer / Shifting == 0 ? ref low : ref high) |= 1L << peer % Shifting;

					int i = 0;
					foreach (int offset in offsets)
					{
						long low = 0, high = 0;
						foreach (int peer in Peers[offset])
						{
							process(ref low, ref high, peer);
						}

						if (initializeOption == InitializeOption.ProcessPeersAlso)
						{
							process(ref low, ref high, offset);
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
		/// Initializes an instance with cell offsets with an initialize option.
		/// </summary>
		/// <param name="offsets">The offsets to be processed.</param>
		/// <param name="initializeOption">
		/// Indicates the behavior of the initialization.
		/// </param>
		/// <exception cref="ArgumentException">
		/// Throws when the specified initialize option is invalid.
		/// </exception>
		public GridMap(IEnumerable<int> offsets, InitializeOption initializeOption)
		{
			(_low, _high, Count) = (0, 0, 0);
			switch (initializeOption)
			{
				case InitializeOption.Ordinary:
				{
					foreach (int offset in offsets)
					{
						this[offset] = true;
					}

					break;
				}
				case InitializeOption.ProcessPeersAlso:
				case InitializeOption.ProcessPeersWithoutItself:
				{
					static void process(ref long low, ref long high, int peer) =>
						(peer / Shifting == 0 ? ref low : ref high) |= 1L << peer % Shifting;

					int i = 0;
					foreach (int offset in offsets)
					{
						long low = 0, high = 0;
						foreach (int peer in Peers[offset])
						{
							process(ref low, ref high, peer);
						}

						if (initializeOption == InitializeOption.ProcessPeersAlso)
						{
							process(ref low, ref high, offset);
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
		[SuppressMessage("", "IDE0004")]
		public GridMap(int high, int mid, int low)
			: this(
				  ((long)high & 134217727) << 13 | (long)(mid >> 14 & 8191),
				  ((long)mid & 16383) << 27 | (long)(low & 134217727))
		{
		}

		/// <summary>
		/// Initializes an instance with two binary values.
		/// </summary>
		/// <param name="high">Higher 40 bits.</param>
		/// <param name="low">Lower 41 bits.</param>
		private GridMap(long high, long low)
		{
			(_high, _low) = (high, low);
			Count = _high.CountSet() + _low.CountSet();
		}


		/// <include file='../../GlobalDocComments.xml' path='comments/staticConstructor[@aimTo="struct"]'/>
		static GridMap() => (Full._high, Full._low, Full.Count) = (-1, -1, 81);


		/// <summary>
		/// Indicates whether the map has no set bits.
		/// This property is equivalent to code '<c>!<see langword="this"/>.IsNotEmpty</c>'.
		/// </summary>
		/// <seealso cref="IsNotEmpty"/>
		public readonly bool IsEmpty => _high == 0 && _low == 0;

		/// <summary>
		/// Indicates whether the map has at least one set bit.
		/// This property is equivalent to code '<c>!<see langword="this"/>.IsEmpty</c>'.
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
					if ((this & CreateInstance(i)).IsNotEmpty)
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
					if ((this & CreateInstance(i)).IsNotEmpty)
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
					if ((this & CreateInstance(i)).IsNotEmpty)
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
		/// Indicates the total number of cells where the corresponding
		/// value are set <see langword="true"/>.
		/// </summary>
		public int Count { readonly get; private set; }

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
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get => ((stackalloc[] { _low, _high }[offset / Shifting] >> offset % Shifting) & 1) != 0;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				ref long d = ref offset / Shifting == 0 ? ref _low : ref _high;
				bool older = this[offset];
				if (value)
				{
					d |= 1L << offset % Shifting;
					if (!older)
					{
						Count++;
					}
				}
				else
				{
					d &= ~(1L << offset % Shifting);
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
		public readonly void Deconstruct(out long high, out long low) =>
			(high, low) = (_high, _low);

		/// <inheritdoc/>
		public override readonly bool Equals(object? obj) =>
			obj is GridMap comparer && Equals(comparer);

		/// <summary>
		/// Indicates whether the current object has the same value with the other one.
		/// </summary>
		/// <param name="other">The other value to compare.</param>
		/// <returns>
		/// The result of this comparison. <see langword="true"/> if two instances hold a same
		/// value; otherwise, <see langword="false"/>.
		/// </returns>
		public readonly bool Equals(GridMap other) =>
			_high == other._high && _low == other._low;

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
		public readonly bool AllCellCovers(int regionOffset) =>
			Count - (this - CreateInstance(regionOffset)).Count == 9;

		/// <summary>
		/// Indicates whether all cells in this instance are in one region.
		/// </summary>
		/// <param name="region">
		/// (<see langword="out"/> parameter) The region covered.
		/// </param>
		public readonly bool AllSetsAreInOneRegion([NotNullWhen(true)] out int? region)
		{
			for (int i = 0; i < 27; i++)
			{
				if ((_high & ~CoverTable[i, 0]) == 0 && (_low & ~CoverTable[i, 1]) == 0)
				{
					region = i;
					return true;
				}
			}

			region = null;
			return false;
		}

		/// <summary>
		/// Get a n-th index of the <see langword="true"/> bit in this instance.
		/// </summary>
		/// <param name="index">The true bit index order.</param>
		/// <returns>The real index.</returns>
		public readonly int SetAt(int index) => Offsets.ElementAt(index);

		/// <inheritdoc/>
		public readonly int CompareTo(GridMap other)
		{
			return ((new BigInteger(_high) << Shifting) + new BigInteger(_low)).CompareTo(
				(new BigInteger(other._high) << Shifting) + new BigInteger(other._low));
		}

		/// <summary>
		/// Get all cell offsets whose bits are set <see langword="true"/>.
		/// </summary>
		/// <returns>An array of cell offsets.</returns>
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

		/// <inheritdoc/>
		public override readonly int GetHashCode() =>
			GetType().GetHashCode() ^ (int)((_low ^ _high) & int.MaxValue);

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
		/// Create a <see cref="GridMap"/> instance with the specified region offset.
		/// This will set all bits <see langword="true"/> in this region.
		/// </summary>
		/// <param name="regionOffset">The region offset.</param>
		/// <returns>The grid map.</returns>
		public static GridMap CreateInstance(int regionOffset)
		{
			var result = Empty;
			foreach (int cell in RegionCells[regionOffset])
			{
				result.Add(cell);
			}

			return result;
		}

		/// <summary>
		/// Create a <see cref="GridMap"/> instance with the specified solution.
		/// If the puzzle has been solved, this method will create a grid map of
		/// distribution of a single digit in this solution.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="digit">The digit to search.</param>
		/// <returns>
		/// The grid map that contains all cells of a digit appearing
		/// in the solution.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Throws when the puzzle has not been solved.
		/// </exception>
		public static GridMap CreateInstance(IReadOnlyGrid grid, int digit)
		{
			if (!grid.HasSolved)
			{
				throw new ArgumentException($"The specified sudoku grid has not been solved.");
			}

			var result = Empty;
			for (int cell = 0; cell < 81; cell++)
			{
				if (grid[cell] == digit)
				{
					result.Add(cell);
				}
			}
			return result;
		}


		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(GridMap left, GridMap right) => left.Equals(right);

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(GridMap left, GridMap right) => !(left == right);

		/// <summary>
		/// Reverse all cells' statuses, which means all <see langword="true"/> bits
		/// will be set <see langword="false"/>, and all <see langword="false"/> bits
		/// will be set <see langword="true"/>.
		/// </summary>
		/// <param name="gridMap">The instance to negate.</param>
		/// <returns>The negative result.</returns>
		public static GridMap operator ~(GridMap gridMap) =>
			new GridMap(~gridMap._high, ~gridMap._low);

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
		/// Unions two sets of <paramref name="left"/> minus <paramref name="right"/>
		/// and <paramref name="right"/> minus <paramref name="left"/>, where the minus
		/// operator is <see cref="operator -(GridMap, GridMap)"/>.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>The result.</returns>
		/// <seealso cref="operator -(GridMap, GridMap)"/>
		public static GridMap operator ^(GridMap left, GridMap right) =>
			new GridMap(left._high ^ right._high, left._low ^ right._low);
	}
}
