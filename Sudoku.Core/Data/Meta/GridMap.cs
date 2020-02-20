using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Sudoku.Data.Extensions;

namespace Sudoku.Data.Meta
{
	/// <summary>
	/// Encapsulates a binary series of cell status table consisting of 81 bits,
	/// where <see langword="true"/> bit (1) is for the cell having digit,
	/// and the <see langword="false"/> bit (0) is for empty cell. Sometimes for other usages.
	/// </summary>
	[DebuggerStepThrough]
	public partial struct GridMap : IEnumerable<bool>, IEquatable<GridMap>
	{
		/// <summary>
		/// Indicates an empty instance (making no changes).
		/// </summary>
		public static readonly GridMap Empty = new GridMap();


		/// <summary>
		/// The value used for shifting.
		/// </summary>
		private const int Shifting = 41;

		/// <summary>
		/// Inner binary representation values.
		/// </summary>
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
		public GridMap(int offset, bool setItself) : this((IEnumerable<int>)PeerTable[offset]) =>
			this[offset] = setItself;

		/// <summary>
		/// To copy an instance with the specified information.
		/// This initializer is only used for add other cell like
		/// <code>
		/// <see langword="var"/> y = <see langword="new"/> <see cref="GridMap"/>(x) { [i] = <see langword="true"/> };
		/// </code>
		/// </summary>
		/// <param name="another">Another instance.</param>
		public GridMap(GridMap another) =>
			(_high, _low, Count) = (another._high, another._low, another.Count);

		/// <summary>
		/// Initializes an instance with a series of cell offsets.
		/// </summary>
		/// <param name="offsets">cell offsets.</param>
		/// <remarks>
		/// Note that all offsets will be set <see langword="true"/>, but their own peers
		/// will not be set <see langword="true"/>.
		/// </remarks>
		public GridMap(Span<int> offsets)
		{
			(_low, _high, Count) = (0, 0, 0);
			ref long a = ref _low, b = ref _high;
			foreach (int offset in offsets)
			{
				switch (offset / Shifting)
				{
					case 0:
					{
						a |= 1L << offset % Shifting;
						Count++;
						break;
					}
					case 1:
					{
						b |= 1L << offset % Shifting;
						Count++;
						break;
					}
				}
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
			ref long a = ref _low, b = ref _high;
			foreach (int offset in offsets)
			{
				switch (offset / Shifting)
				{
					case 0:
					{
						a |= 1L << offset % Shifting;
						Count++;
						break;
					}
					case 1:
					{
						b |= 1L << offset % Shifting;
						Count++;
						break;
					}
				}
			}
		}

		/// <summary>
		/// Initializes an instance with three binary value.
		/// </summary>
		/// <param name="high">Higher 40 bits.</param>
		/// <param name="low">Lower 41 bits.</param>
		private GridMap(long high, long low)
		{
			(_high, _low) = (high, low);
			Count = _high.CountSet() + _low.CountSet();
		}


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
					if (this & CreateInstance(i))
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
					if (this & CreateInstance(i))
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
					if (this & CreateInstance(i))
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
		public int Count { get; private set; }

		/// <summary>
		/// <para>
		/// Indicates all cell offsets whose corresponding value
		/// are set <see langword="true"/>.
		/// </para>
		/// <para>
		/// If you want to make an array of them, please use method
		/// <see cref="ToArray"/> instead of code
		/// '<c><see cref="Offsets"/>.ToArray()</c>'.
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
			readonly get =>
				((stackalloc[] { _low, _high }[offset / Shifting] >> offset % Shifting) & 1) != 0;
			set
			{
				ref long a = ref _low, b = ref _high;
				switch (offset / Shifting)
				{
					case 0:
					{
						if (value)
						{
							a |= 1L << offset % Shifting;
							Count++;
						}
						else
						{
							a &= ~(1L << offset % Shifting);
							Count--;
						}
						break;
					}
					case 1:
					{
						if (value)
						{
							b |= 1L << offset % Shifting;
							Count++;
						}
						else
						{
							b &= ~(1L << offset % Shifting);
							Count--;
						}
						break;
					}
				}
				
			}
		}


		/// <summary>
		/// Deconstruct this instance.
		/// </summary>
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
		/// The result of this comparsion. <see langword="true"/> if two instances hold a same
		/// value; otherwise, <see langword="false"/>.
		/// </returns>
		public readonly bool Equals(GridMap other) =>
			_high == other._high && _low == other._low;

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
		public readonly bool IsCoveredOneRegion([NotNullWhen(true)] out int? region)
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
		/// Get the specified index of <see langword="true"/> bits in this instance.
		/// </summary>
		/// <param name="index">The true bit index order.</param>
		/// <returns>The real index.</returns>
		public readonly int ElementAt(int index) => Offsets.ElementAt(index);

		/// <summary>
		/// Get all cell offsets whose bits are set <see langword="true"/>.
		/// </summary>
		/// <returns>An array of cell offsets.</returns>
		public readonly int[] ToArray() => Offsets.ToArray();

		/// <inheritdoc/>
		public readonly IEnumerator<bool> GetEnumerator()
		{
			long h = _high, l = _low;
			while (l != 0)
			{
				yield return (l & 1) != 0;
				l >>= 1;
			}

			while (h != 0)
			{
				yield return (h & 1) != 0;
				h >>= 1;
			}
		}

		/// <inheritdoc/>
		public override readonly int GetHashCode() =>
			GetType().GetHashCode() ^ (int)((_low ^ _high) & int.MaxValue);

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
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
		/// Get all cell offsets in the specified region.
		/// </summary>
		/// <param name="regionOffset">The region offset.</param>
		/// <returns>All cell offsets.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int[] GetCellsIn(int regionOffset) => RegionTable[regionOffset];

		/// <summary>
		/// Create a <see cref="GridMap"/> instance with the specified region offset.
		/// This will set all bits <see langword="true"/> in this region.
		/// </summary>
		/// <param name="regionOffset">The region offset.</param>
		/// <returns>The grid map.</returns>
		public static GridMap CreateInstance(int regionOffset)
		{
			var result = Empty;
			foreach (int cell in RegionTable[regionOffset])
			{
				result[cell] = true;
			}

			return result;
		}

		/// <summary>
		/// Create a <see cref="GridMap"/> instance with the specified solution.
		/// If the puzzle has been solved, this method will create a gridmap of
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
		public static GridMap CreateInstance(Grid grid, int digit)
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
					result[cell] = true;
				}
			}
			return result;
		}


		/// <summary>
		/// Check whether the specified map has at least one <see langword="true"/> bits.
		/// </summary>
		/// <param name="map">The grid map.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		public static bool operator true(GridMap map) => !(map._low == 0 && map._high == 0);

		/// <summary>
		/// Check whether the specified grid map has no <see langword="true"/> bits.
		/// </summary>
		/// <param name="map">The grid map.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		public static bool operator false(GridMap map) => map._low == 0 && map._high == 0;

		/// <summary>
		/// Check whether the specified grid map has no <see langword="true"/> bits.
		/// Same as <see cref="operator false(GridMap)"/>.
		/// </summary>
		/// <param name="map">The grid map.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		/// <seealso cref="operator false(GridMap)"/>.
		public static bool operator !(GridMap map) => map._low == 0 && map._high == 0;

		/// <summary>
		/// Indicates whether two instances have a same value.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		public static bool operator ==(GridMap left, GridMap right) => left.Equals(right);

		/// <summary>
		/// Indicates whether two instances have two different values.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		public static bool operator !=(GridMap left, GridMap right) => !(left == right);

		/// <summary>
		/// Negate all bits.
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
		/// Intersect two <see cref="GridMap"/>s.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>The intersection result.</returns>
		public static GridMap operator &(GridMap left, GridMap right) =>
			new GridMap(left._high & right._high, left._low & right._low);

		/// <summary>
		/// Union two <see cref="GridMap"/>s.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>The union result.</returns>
		public static GridMap operator |(GridMap left, GridMap right) =>
			new GridMap(left._high | right._high, left._low | right._low);

		/// <summary>
		/// Symmetrical except two <see cref="GridMap"/>s.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>The result.</returns>
		public static GridMap operator ^(GridMap left, GridMap right) =>
			new GridMap(left._high ^ right._high, left._low ^ right._low);
	}
}
