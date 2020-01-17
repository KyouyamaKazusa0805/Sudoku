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
	public partial struct GridMap : IEnumerable<bool>, IEquatable<GridMap>
	{
		private int _low, _mid, _high;


		public GridMap(int offset) : this(PeerTable[offset])
		{
		}

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

		private GridMap(int high, int mid, int low) =>
			(_high, _mid, _low) = (high, mid, low);


		public readonly int Count => _low.CountSet() + _mid.CountSet() + _high.CountSet();

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


		public bool this[int offset]
		{
			readonly get => (((stackalloc[] { _low, _mid, _high })[offset / 27] >> offset % 27) & 1) != 0;
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


		[OnDeconstruction]
		public readonly void Deconstruct(out int high, out int mid, out int low) =>
			(high, mid, low) = (_high, _mid, _low);

		public override readonly bool Equals(object? obj) =>
			obj is GridMap comparer && Equals(comparer);

		public readonly bool Equals(GridMap other) =>
			_high == other._high && _mid == other._mid && _low == other._low;

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

		public override readonly int GetHashCode() =>
			GetType().GetHashCode() ^ _low ^ _mid ^ _high;

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

		readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(int offset) => this[offset] = true;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Remove(int offset) => this[offset] = false;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetPeersTrue(int offset) => UnionWith(new GridMap(offset));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetPeersFalse(int offset) => IntersectWith(new GridMap(offset));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void NegatePeers(int offset) => SymmetricalExceptWith(new GridMap(offset));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void IntersectWith(GridMap other) =>
			(_low, _mid, _high) = (_low & other._low, _mid & other._mid, _high & other._high);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void UnionWith(GridMap other) =>
			(_low, _mid, _high) = (_low | other._low, _mid | other._mid, _high | other._high);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SymmetricalExceptWith(GridMap other) =>
			(_low, _mid, _high) = (_low ^ other._low, _mid ^ other._mid, _high ^ other._high);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Negate() => (_low, _mid, _high) = (~_low, ~_mid, ~_high);


		public static bool operator ==(GridMap left, GridMap right) => left.Equals(right);

		public static bool operator !=(GridMap left, GridMap right) => !(left == right);

		public static GridMap operator ~(GridMap gridMap) =>
			new GridMap(~gridMap._high, ~gridMap._mid, ~gridMap._low);

		public static GridMap operator &(GridMap left, GridMap right) =>
			new GridMap(left._high & right._high, left._mid & right._mid, left._low & right._low);

		public static GridMap operator |(GridMap left, GridMap right) =>
			new GridMap(left._high | right._high, left._mid | right._mid, left._low | right._low);

		public static GridMap operator ^(GridMap left, GridMap right) =>
			new GridMap(left._high ^ right._high, left._mid ^ right._mid, left._low ^ right._low);
	}
}
