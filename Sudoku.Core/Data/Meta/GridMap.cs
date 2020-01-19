using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Sudoku.Data.Extensions;
using Sudoku.Diagnostics.CodeAnalysis;

namespace Sudoku.Data.Meta
{
	/// <summary>
	/// Encapsulates a binary series of cell status table.
	/// <c>true</c> is for the cell having digit, and the <c>false</c>
	/// value is for empty cell.
	/// </summary>
	public partial struct GridMap : IEnumerable<bool>, IEquatable<GridMap>
	{
		/// <summary>
		/// Inner binary representation values.
		/// </summary>
		private int _low, _mid, _high;


		/// <summary>
		/// Initializes an instance with the specified cell offset
		/// (Sets itself and all peers).
		/// </summary>
		/// <param name="offset">The cell offset.</param>
		public GridMap(int offset) : this(PeerTable[offset])
		{
		}

		/// <summary>
		/// Initializes an instance with a series of cell offsets.
		/// </summary>
		/// <param name="offsets">cell offsets.</param>
		/// <remarks>
		/// Note that all offsets will be set <c>true</c>, but their own peers
		/// will not be set <c>true</c>.
		/// </remarks>
		public GridMap(IEnumerable<int> offsets)
		{
			(_low, _mid, _high) = (0, 0, 0);

#if I_DONT_KNOW_WHY_GENERATING_BUG
			var series = (Span<int>)stackalloc[] { _low, _mid, _high };
			foreach (int offset in offsets)
			{
				ref int valueToModify = ref series[offset / 27];
				valueToModify |= 1 << offset % 27;
			}
#else
			// We should get along with pointers extremely carefully.
			unsafe
			{
				fixed (int* a = &_low, b = &_mid, c = &_high)
				{
					int** series = stackalloc[] { a, b, c };
					foreach (int offset in offsets)
					{
						*series[offset / 27] |= 1 << offset % 27;
					}
				}
			}
#endif
		}

		/// <summary>
		/// Initializes an instance with three binary value.
		/// </summary>
		/// <param name="high">Higher 32 bits.</param>
		/// <param name="mid">Middle 32 bits.</param>
		/// <param name="low">Lower 32 bits.</param>
		private GridMap(int high, int mid, int low) =>
			(_high, _mid, _low) = (high, mid, low);


		/// <summary>
		/// Indicates the total number of cells where the corresponding
		/// value are set <c>true</c>.
		/// </summary>
		public readonly int Count => _low.CountSet() + _mid.CountSet() + _high.CountSet();

		/// <summary>
		/// Indicates all cell offsets whose corresponding value
		/// are set <c>true</c>.
		/// </summary>
		public readonly IEnumerable<int> Offsets
		{
			get
			{
				int value, i;
				for (value = _low, i = 0; i < 27; i++, value >>= 1)
				{
					if ((value & 1) != 0)
					{
						yield return i;
					}
				}
				for (value = _mid; i < 54; i++, value >>= 1)
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
				(((stackalloc[] { _low, _mid, _high })[offset / 27] >> offset % 27) & 1) != 0;
			set
			{
#if I_DONT_KNOW_WHY_GENERATING_BUG
				ref int valueToModify = ref (stackalloc[] { _low, _mid, _high })[offset / 27];
				if (value)
				{
					valueToModify |= 1 << offset % 27;
				}
				else
				{
					valueToModify &= ~(1 << offset % 27);
				}
#else
				// We should get along with pointers extremely carefully.
				unsafe
				{
					if (offset < 0 || offset >= 81)
					{
						throw new ArgumentOutOfRangeException(nameof(offset));
					}

					fixed (int* a = &_low, b = &_mid, c = &_high)
					{
						int** series = stackalloc[] { a, b, c };
						int* valueToModify = series[offset / 27];
						if (value)
						{
							*valueToModify |= 1 << offset % 27;
						}
						else
						{
							*valueToModify &= ~(1 << offset % 27);
						}
					}
				}
#endif
			}
		}


		/// <summary>
		/// Deconstruct the instance.
		/// </summary>
		/// <param name="high">(out parameter) Higher 32 bits.</param>
		/// <param name="mid">(out parameter) Middle 32 bits.</param>
		/// <param name="low">(out parameter) Lower 32 bits.</param>
		[OnDeconstruction]
		public readonly void Deconstruct(out int high, out int mid, out int low) =>
			(high, mid, low) = (_high, _mid, _low);

		/// <inheritdoc/>
		public override readonly bool Equals(object? obj) =>
			obj is GridMap comparer && Equals(comparer);

		/// <summary>
		/// Indicates whether the current object has the same value with the other one.
		/// </summary>
		/// <param name="other">The other value to compare.</param>
		/// <returns>
		/// The result of this comparsion. <c>true</c> if two instances hold a same
		/// value; otherwise, <c>false</c>.
		/// </returns>
		public readonly bool Equals(GridMap other) =>
			_high == other._high && _mid == other._mid && _low == other._low;

		/// <inheritdoc/>
		public readonly IEnumerator<bool> GetEnumerator()
		{
			int h = _high, m = _mid, l = _low;
			while (l != 0)
			{
				yield return (l & 1) != 0;
				l >>= 1;
			}

			while (m != 0)
			{
				yield return (m & 1) != 0;
				m >>= 1;
			}

			while (h != 0)
			{
				yield return (h & 1) != 0;
				h >>= 1;
			}
		}

		/// <inheritdoc/>
		public override readonly int GetHashCode() =>
			GetType().GetHashCode() ^ _low ^ _mid ^ _high;

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		public override readonly string ToString()
		{
			static void OnOutputing(StringBuilder sb, int value)
			{
				for (int i = 0; i < 27; i++, value >>= 1)
				{
					sb.Append(value & 1);
				}
			}

			var sb = new StringBuilder();
			OnOutputing(sb, _low);
			sb.Append(" ");
			OnOutputing(sb, _mid);
			sb.Append(" ");
			OnOutputing(sb, _high);
			return sb.Reverse().ToString();
		}

		/// <inheritdoc/>
		readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		/// <summary>
		/// Set the specified cell as <c>true</c> value.
		/// </summary>
		/// <param name="offset">The cell offset.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(int offset) => this[offset] = true;

		/// <summary>
		/// Set the specified cell as <c>false</c> value.
		/// </summary>
		/// <param name="offset">The cell offset.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Remove(int offset) => this[offset] = false;
		
		/// <summary>
		/// Set all peers as <c>true</c> value.
		/// </summary>
		/// <param name="offset">The cell offset.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetPeersTrue(int offset) => UnionWith(new GridMap(offset));

		/// <summary>
		/// Set all peers as <c>false</c> value.
		/// </summary>
		/// <param name="offset">The cell offset.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetPeersFalse(int offset) => IntersectWith(new GridMap(offset));

		/// <summary>
		/// Negate all peers' value.
		/// </summary>
		/// <param name="offset">The cell offset.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void NegatePeers(int offset) => SymmetricalExceptWith(new GridMap(offset));

		/// <summary>
		/// Intersect with the other instance.
		/// </summary>
		/// <param name="other">The other instance.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void IntersectWith(GridMap other) =>
			(_low, _mid, _high) = (_low & other._low, _mid & other._mid, _high & other._high);

		/// <summary>
		/// Union with the other instance.
		/// </summary>
		/// <param name="other">The other instance.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void UnionWith(GridMap other) =>
			(_low, _mid, _high) = (_low | other._low, _mid | other._mid, _high | other._high);

		/// <summary>
		/// Symmetrical except with the other instance.
		/// </summary>
		/// <param name="other">The other instance.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SymmetricalExceptWith(GridMap other) =>
			(_low, _mid, _high) = (_low ^ other._low, _mid ^ other._mid, _high ^ other._high);

		/// <summary>
		/// Negate all values.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Negate() => (_low, _mid, _high) = (~_low, ~_mid, ~_high);


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
		/// Negate all values.
		/// </summary>
		/// <param name="gridMap">The instance to negate.</param>
		/// <returns>The negative result.</returns>
		public static GridMap operator ~(GridMap gridMap) =>
			new GridMap(~gridMap._high, ~gridMap._mid, ~gridMap._low);

		/// <summary>
		/// Intersect among two values.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>The intersection result.</returns>
		public static GridMap operator &(GridMap left, GridMap right) =>
			new GridMap(left._high & right._high, left._mid & right._mid, left._low & right._low);

		/// <summary>
		/// Union among two values.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>The union result.</returns>
		public static GridMap operator |(GridMap left, GridMap right) =>
			new GridMap(left._high | right._high, left._mid | right._mid, left._low | right._low);

		/// <summary>
		/// Symmetrical except among two values.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>The result.</returns>
		public static GridMap operator ^(GridMap left, GridMap right) =>
			new GridMap(left._high ^ right._high, left._mid ^ right._mid, left._low ^ right._low);
	}
}
