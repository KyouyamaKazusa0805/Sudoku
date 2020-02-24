using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Sudoku.Data.Meta
{
	/// <summary>
	/// Encapsulates a binary series of candidate status table consisting of 729 bits,
	/// where <see langword="true"/> bit (1) is for the cell having digit,
	/// and the <see langword="false"/> bit (0) is for empty cell. Sometimes for other usages.
	/// </summary>
	/// <remarks>
	/// This data structure use at least 144 bytes, which has outnumbered the number of
	/// the threshold of value types via Microsoft C# language designation team (Every instance
	/// of value type should be no more than 64 bytes). Therefore, we should use
	/// <see cref="GridMap"/> instead of this data structure as much as possible.
	/// </remarks>
	[DebuggerStepThrough]
	public struct FullGridMap : IEquatable<FullGridMap>
	{
		/// <summary>
		/// Indicates an empty instance (making no changes).
		/// </summary>
		public static readonly FullGridMap Empty = new FullGridMap();


		/// <summary>
		/// All bits map.
		/// </summary>
		private GridMap _line1, _line2, _line3, _line4, _line5, _line6, _line7, _line8, _line9;


		/// <summary>
		/// Initializes an instance with the specified candidate offset
		/// (Sets itself and all peers).
		/// </summary>
		/// <param name="offset">The candidate offset.</param>
		public FullGridMap(int offset) : this(offset, true)
		{
		}

		/// <summary>
		/// Initializes an instance with the specified candidate offset.
		/// This will set all bits of all peers of this candidate. Another
		/// <see cref="bool"/> value indicates whether this initialization
		/// will set the bit of itself.
		/// </summary>
		/// <param name="offset">The candidate offset.</param>
		/// <param name="setItself">
		/// A <see cref="bool"/> value indicating whether this initialization
		/// will set the bit of itself.
		/// If the value is <see langword="false"/>, it will be equivalent
		/// to below:
		/// <code>
		/// var map = new FullGridMap(offset) { [offset] = false };
		/// </code>
		/// </param>
		public FullGridMap(int offset, bool setItself)
		{
			_line1 = GridMap.Empty; _line2 = GridMap.Empty; _line3 = GridMap.Empty;
			_line4 = GridMap.Empty; _line5 = GridMap.Empty; _line6 = GridMap.Empty;
			_line7 = GridMap.Empty; _line8 = GridMap.Empty; _line9 = GridMap.Empty;
			int digit = offset % 9;
			int cell = offset / 9;
			unsafe
			{
				fixed (GridMap*
					a = &_line1, b = &_line2, c = &_line3,
					d = &_line4, e = &_line5, f = &_line6,
					g = &_line7, h = &_line8, i = &_line9)
				{
					var series = stackalloc[] { a, b, c, d, e, f, g, h, i };
					foreach (int z in GridMap.PeerTable[cell])
					{
						(*series[z / 9])[z % 9 * 9 + digit] = true;
					}
				}
			}

			for (int i = 0; i < 9; i++)
			{
				this[cell * 9 + i] = i == digit ? setItself : true;
			}
		}

		/// <summary>
		/// Initializes an instance with three binary value.
		/// </summary>
		/// <param name="line1">Line 1 grid map.</param>
		/// <param name="line2">Line 2 grid map.</param>
		/// <param name="line3">Line 3 grid map.</param>
		/// <param name="line4">Line 4 grid map.</param>
		/// <param name="line5">Line 5 grid map.</param>
		/// <param name="line6">Line 6 grid map.</param>
		/// <param name="line7">Line 7 grid map.</param>
		/// <param name="line8">Line 8 grid map.</param>
		/// <param name="line9">Line 9 grid map.</param>
		private FullGridMap(
			GridMap line1, GridMap line2, GridMap line3,
			GridMap line4, GridMap line5, GridMap line6,
			GridMap line7, GridMap line8, GridMap line9)
		{
			_line1 = line1; _line2 = line2; _line3 = line3;
			_line4 = line4; _line5 = line5; _line6 = line6;
			_line7 = line7; _line8 = line8; _line9 = line9;
		}


		/// <summary>
		/// Indicates the total number of cells where the corresponding
		/// value are set <see langword="true"/>.
		/// </summary>
		public readonly int Count
		{
			get
			{
				return _line1.Count + _line2.Count + _line3.Count
					+ _line4.Count + _line5.Count + _line6.Count
					+ _line7.Count + _line8.Count + _line9.Count;
			}
		}

		/// <summary>
		/// Indicates all candidate offsets whose corresponding value
		/// are set <see langword="true"/>.
		/// </summary>
		public readonly IEnumerable<int> Offsets
		{
			get
			{
				var lines = new[]
				{
					_line1, _line2, _line3, _line4, _line5, _line6, _line7, _line8, _line9
				};

				for (int i = 0; i < 9; i++)
				{
					foreach (int offset in lines[i].Offsets)
					{
						yield return i * 81 + offset;
					}
				}
			}
		}


		/// <summary>
		/// Gets or sets a <see cref="bool"/> value on the specified candidate
		/// offset.
		/// </summary>
		/// <param name="offset">The candidate offset.</param>
		/// <value>
		/// A <see cref="bool"/> value on assignment.
		/// </value>
		/// <returns>
		/// A <see cref="bool"/> value indicating whether the candidate has digit.
		/// </returns>
		public bool this[int offset]
		{
			readonly get
			{
				return stackalloc[]
				{
					_line1, _line2, _line3, _line4, _line5, _line6, _line7, _line8, _line9
				}[offset / 81][offset / 9 % 9 * 9 + offset % 9];
			}
			set
			{
				// We should get along with pointers extremely carefully.
				if (offset < 0 || offset >= 729)
				{
					throw new ArgumentOutOfRangeException(nameof(offset));
				}

				unsafe
				{
					fixed (GridMap*
						a = &_line1, b = &_line2, c = &_line3,
						d = &_line4, e = &_line5, f = &_line6,
						g = &_line7, h = &_line8, i = &_line9)
					{
						var series = stackalloc[] { a, b, c, d, e, f, g, h, i };
						(*series[offset / 81])[offset / 9 % 9 * 9 + offset % 9] = value;
					}
				}
			}
		}


		/// <inheritdoc/>
		public override bool Equals(object? obj) =>
			obj is FullGridMap comparer && Equals(comparer);

		/// <summary>
		/// Indicates whether the current object has the same value with the other one.
		/// </summary>
		/// <param name="other">The other value to compare.</param>
		/// <returns>
		/// The result of this comparison. <see langword="true"/> if two instances hold a same
		/// value; otherwise, <see langword="false"/>.
		/// </returns>
		public readonly bool Equals(FullGridMap other)
		{
			var left = (Span<GridMap>)stackalloc[]
			{
				_line1, _line2, _line3, _line4, _line5, _line6, _line7, _line8, _line9
			};
			var right = (Span<GridMap>)stackalloc[]
			{
				other._line1, other._line2, other._line3, other._line4,
				other._line5, other._line6, other._line7, other._line8, other._line9
			};

			for (int i = 0; i < 9; i++)
			{
				if (left[i] != right[i])
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Get all candidate offsets whose bits are set <see langword="true"/>.
		/// </summary>
		/// <returns>An array of candidate offsets.</returns>
		public readonly int[] ToArray() => Offsets.ToArray();

		/// <inheritdoc/>
		public override readonly int GetHashCode()
		{
			int result = GetType().GetHashCode();
			foreach (var map in stackalloc[]
			{
				_line1, _line2, _line3, _line4, _line5, _line6, _line7, _line8, _line9
			})
			{
				result ^= map.GetHashCode();
			}

			return result;
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		public override readonly string ToString() => "...";


		/// <summary>
		/// Check whether the specified map has at least one <see langword="true"/> bits.
		/// </summary>
		/// <param name="map">The grid map.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		public static bool operator true(FullGridMap map) => map.Count != 0;

		/// <summary>
		/// Check whether the specified grid map has no <see langword="true"/> bits.
		/// </summary>
		/// <param name="map">The grid map.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		public static bool operator false(FullGridMap map) => map.Count == 0;

		/// <summary>
		/// Check whether the specified grid map has no <see langword="true"/> bits.
		/// Same as <see cref="operator false(FullGridMap)"/>.
		/// </summary>
		/// <param name="map">The grid map.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		/// <seealso cref="operator false(FullGridMap)"/>
		public static bool operator !(FullGridMap map) => map.Count == 0;

		/// <summary>
		/// Indicates whether two instances have a same value.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		public static bool operator ==(FullGridMap left, FullGridMap right) =>
			left.Equals(right);

		/// <summary>
		/// Indicates whether two instances have two different values.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		public static bool operator !=(FullGridMap left, FullGridMap right) =>
			!(left == right);

		/// <summary>
		/// Negate all bits.
		/// </summary>
		/// <param name="map">The instance to negate.</param>
		/// <returns>The negative result.</returns>
		public static FullGridMap operator ~(FullGridMap map)
		{
			return new FullGridMap(
				~map._line1, ~map._line2, ~map._line3,
				~map._line4, ~map._line5, ~map._line6,
				~map._line7, ~map._line8, ~map._line9);
		}

		/// <summary>
		/// Get a <see cref="FullGridMap"/> that contains all <paramref name="left"/> cells
		/// but not in <paramref name="right"/> cells.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>The result.</returns>
		public static FullGridMap operator -(FullGridMap left, FullGridMap right) =>
			left & ~right;

		/// <summary>
		/// Intersect two <see cref="FullGridMap"/>s.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>The intersection result.</returns>
		public static FullGridMap operator &(FullGridMap left, FullGridMap right)
		{
			return new FullGridMap(
				left._line1 & right._line1, left._line2 & right._line2, left._line3 & right._line3,
				left._line4 & right._line4, left._line5 & right._line5, left._line6 & right._line6,
				left._line7 & right._line7, left._line8 & right._line8, left._line9 & right._line9);
		}

		/// <summary>
		/// Union two <see cref="FullGridMap"/>s.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>The union result.</returns>
		public static FullGridMap operator |(FullGridMap left, FullGridMap right)
		{
			return new FullGridMap(
				left._line1 | right._line1, left._line2 | right._line2, left._line3 | right._line3,
				left._line4 | right._line4, left._line5 | right._line5, left._line6 | right._line6,
				left._line7 | right._line7, left._line8 | right._line8, left._line9 | right._line9);
		}

		/// <summary>
		/// Symmetrical except two <see cref="FullGridMap"/>s.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>The result.</returns>
		public static FullGridMap operator ^(FullGridMap left, FullGridMap right)
		{
			return new FullGridMap(
				left._line1 ^ right._line1, left._line2 ^ right._line2, left._line3 ^ right._line3,
				left._line4 ^ right._line4, left._line5 ^ right._line5, left._line6 ^ right._line6,
				left._line7 ^ right._line7, left._line8 ^ right._line8, left._line9 ^ right._line9);
		}
	}
}
