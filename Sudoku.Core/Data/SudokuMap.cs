using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Sudoku.Data.Collections;
using Sudoku.Extensions;
using static Sudoku.Constants.Processings;

namespace Sudoku.Data
{
	/// <summary>
	/// Encapsulates a data structure of a sudoku map that contains 729 bits, which is similar with
	/// <see cref="GridMap"/>.
	/// </summary>
	public sealed class SudokuMap : ICloneable<SudokuMap>, IEnumerable<int>, IEquatable<SudokuMap?>
	{
		/// <summary>
		/// <para>
		/// Indicates an empty instance (all bits are 0).
		/// </para>
		/// </summary>
		/// <seealso cref="SudokuMap()"/>
		public static readonly SudokuMap Empty = new SudokuMap();


		/// <summary>
		/// Indicates the inner array.
		/// </summary>
		private readonly BitArray _innerArray;


		/// <include file='../GlobalDocComments.xml' path='comments/defaultConstructor'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SudokuMap() => _innerArray = new BitArray(729);

		/// <summary>
		/// Initializes an instance with another map.
		/// </summary>
		/// <param name="another">The another map.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SudokuMap(SudokuMap another)
		{
			_innerArray = (BitArray)another._innerArray.Clone();
			Count = another.Count;
		}

		/// <summary>
		/// Initializes an instance with the specified candidate and its peers.
		/// </summary>
		/// <param name="candidate">The candidate.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SudokuMap(int candidate) : this(candidate, true)
		{
		}

		/// <summary>
		/// Inidicates an instance with the peer candidates of the specified candidate and a <see cref="bool"/>
		/// value indicating whether the map will process itself with <see langword="true"/> value.
		/// </summary>
		/// <param name="candidate">The candidate.</param>
		/// <param name="setItself">
		/// Indicates whether the map will process itself with <see langword="true"/> value.
		/// </param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SudokuMap(int candidate, bool setItself)
		{
			_innerArray = AssignBitArray(candidate, setItself);
			Count = setItself ? 29 : 28;
		}

		/// <summary>
		/// Initializes an instance with the specified candidates.
		/// </summary>
		/// <param name="candidates">The candidates.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SudokuMap(int[] candidates) : this((IEnumerable<int>)candidates)
		{
		}

		/// <summary>
		/// Initializes an instance with the specified candidates.
		/// </summary>
		/// <param name="candidates">The candidates.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SudokuMap(ReadOnlySpan<int> candidates) : this() => AddRange(candidates);

		/// <summary>
		/// Initializes an instance with the specified candidates.
		/// </summary>
		/// <param name="candidates">The candidates.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SudokuMap(IEnumerable<int> candidates) : this() => AddRange(candidates);

		/// <summary>
		/// Initializes an instance with the specified <see cref="BitArray"/> instance.
		/// </summary>
		/// <param name="innerArray">The <see cref="BitArray"/> instance.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private SudokuMap(BitArray innerArray) => Count = (_innerArray = innerArray).GetCardinality();


		/// <summary>
		/// Indicates whether the map has no set bits.
		/// This property is equivalent to code '<c>!this.IsNotEmpty</c>'.
		/// </summary>
		/// <seealso cref="IsNotEmpty"/>
		public bool IsEmpty => _innerArray.Length == 0;

		/// <summary>
		/// Indicates whether the map has at least one set bit.
		/// This property is equivalent to code '<c>!this.IsEmpty</c>'.
		/// </summary>
		/// <seealso cref="IsEmpty"/>
		public bool IsNotEmpty => _innerArray.Count != 0;

		/// <summary>
		/// Indicates the total number of all set bits.
		/// </summary>
		public int Count { get; private set; }

		/// <summary>
		/// Indicates the all bits.
		/// </summary>
		public bool[] Bits
		{
			get
			{
				bool[] result = new bool[729];
				_innerArray.CopyTo(result, 0);
				return result;
			}
		}

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
		/// var map = SudokuMap.CreateInstance(testMap);
		/// </code>
		/// </example>
		public SudokuMap PeerIntersection => CreateInstance(Offsets);

		/// <summary>
		/// Indicates all indices of set bits.
		/// </summary>
		private IEnumerable<int> Offsets
		{
			get
			{
				if (_innerArray.Count == 0)
				{
					yield break;
				}

				for (int i = 0; i < 729; i++)
				{
					if (this[i])
					{
						yield return i;
					}
				}
			}
		}


		/// <summary>
		/// Gets or sets the result set case of the specified index.
		/// </summary>
		/// <param name="candidate">The candidate offset (index).</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		/// <value>The <see cref="bool"/> value to set.</value>
		public bool this[int candidate] { get => _innerArray[candidate]; set => _innerArray[candidate] = value; }


		/// <summary>
		/// Set the specified candidates as <see langword="true"/> value.
		/// </summary>
		/// <param name="candidates">The candidate offsets.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddRange(int[] candidates) => AddRange((IEnumerable<int>)candidates);

		/// <summary>
		/// Set the specified candidate as <see langword="true"/> value.
		/// </summary>
		/// <param name="candidate">The candidate offset.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(int candidate)
		{
			if (this[candidate])
			{
				return;
			}

			this[candidate] = true;
			Count++;
		}

		/// <summary>
		/// Set the specified candidates as <see langword="true"/> value.
		/// </summary>
		/// <param name="candidates">The candidate offsets.</param>
		public void AddRange(ReadOnlySpan<int> candidates)
		{
			foreach (int candidate in candidates)
			{
				Add(candidate);
			}
		}

		/// <summary>
		/// Set the specified candidates as <see langword="true"/> value.
		/// </summary>
		/// <param name="candidates">The candidate offsets.</param>
		public void AddRange(IEnumerable<int> candidates)
		{
			foreach (int candidate in candidates)
			{
				Add(candidate);
			}
		}

		/// <summary>
		/// Set the specified candidate as <see langword="false"/> value.
		/// </summary>
		/// <param name="candidate">The cell offset.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Remove(int candidate)
		{
			if (!this[candidate])
			{
				return;
			}

			this[candidate] = false;
			Count--;
		}

		/// <summary>
		/// Set the specified candidates as <see langword="false"/> value.
		/// </summary>
		/// <param name="candidates">The candidate offsets.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveRange(int[] candidates) => RemoveRange((IEnumerable<int>)candidates);

		/// <summary>
		/// Set the specified candidates as <see langword="false"/> value.
		/// </summary>
		/// <param name="candidates">The candidate offsets.</param>
		public void RemoveRange(ReadOnlySpan<int> candidates)
		{
			foreach (int candidate in candidates)
			{
				Remove(candidate);
			}
		}

		/// <summary>
		/// Set the specified candidates as <see langword="false"/> value.
		/// </summary>
		/// <param name="candidates">The candidate offsets.</param>
		public void RemoveRange(IEnumerable<int> candidates)
		{
			foreach (int candidate in candidates)
			{
				Remove(candidate);
			}
		}

		/// <summary>
		/// Clear all bits.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			_innerArray.SetAll(false);
			Count = 0;
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object? obj) => Equals(obj as SudokuMap);

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(SudokuMap? other) => Equals(this, other);

		/// <summary>
		/// Indicates whether this map overlaps another one.
		/// </summary>
		/// <param name="other">The other map.</param>
		/// <returns>The <see cref="bool"/> value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Overlaps(SudokuMap other) => (this & other).IsNotEmpty;

		/// <summary>
		/// Get a n-th index of the <see langword="true"/> bit in this instance.
		/// </summary>
		/// <param name="index">The true bit index order.</param>
		/// <returns>The real index.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int SetAt(int index) => Offsets.ElementAt(index);

		/// <summary>
		/// Get a n-th index of the <see langword="true"/> bit in this instance.
		/// </summary>
		/// <param name="index">The true bit index order.</param>
		/// <returns>The real index.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Throws when the map cannot find the index.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int SetAt(Index index) => Offsets.ElementAt(index.GetOffset(Count));

		/// <summary>
		/// Get all cell offsets whose bits are set <see langword="true"/>.
		/// </summary>
		/// <returns>An array of cell offsets.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int[] ToArray() => Offsets.ToArray();

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			var result = (BigInteger)0;
			foreach (int candidate in Offsets)
			{
				result += BigInteger.Pow(2, candidate);
			}

			return result.GetHashCode();
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString() => new CandidateCollection(Offsets).ToString();

		/// <summary>
		/// Get the final <see cref="GridMap"/> to get all cells that the corresponding indices
		/// are set <see langword="true"/>.
		/// </summary>
		/// <param name="digit">The digit.</param>
		/// <returns>The map of all cells chosen.</returns>
		public GridMap Reduce(int digit)
		{
			var result = GridMap.Empty;
			for (int cell = 0; cell < 81; cell++)
			{
				if (this[cell * 9 + digit])
				{
					result.Add(cell);
				}
			}

			return result;
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SudokuMap Clone() => new SudokuMap((BitArray)_innerArray.Clone());

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IEnumerator<int> GetEnumerator() => Offsets.GetEnumerator();

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


		/// <summary>
		/// Get the map of candidates, which is the peer intersections from the specified candidates.
		/// </summary>
		/// <param name="candidates">All candidates.</param>
		/// <returns>The result map.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SudokuMap CreateInstance(int[] candidates) => CreateInstance((IEnumerable<int>)candidates);

		/// <summary>
		/// Get the map of candidates, which is the peer intersections from the specified candidates.
		/// </summary>
		/// <param name="candidates">All candidates.</param>
		/// <returns>The result map.</returns>
		public static SudokuMap CreateInstance(ReadOnlySpan<int> candidates)
		{
			var result = new BitArray(729);
			foreach (int candidate in candidates)
			{
				result.And(AssignBitArray(candidate, false));
			}

			return new SudokuMap(result);
		}

		/// <summary>
		/// Get the map of candidates, which is the peer intersections from the specified candidates.
		/// </summary>
		/// <param name="candidates">All candidates.</param>
		/// <returns>The result map.</returns>
		public static SudokuMap CreateInstance(IEnumerable<int> candidates)
		{
			var result = new BitArray(729, true);
			foreach (int candidate in candidates)
			{
				result.And(AssignBitArray(candidate, false));
			}

			return new SudokuMap(result);
		}

		/// <summary>
		/// Check whether two instances are equal.
		/// </summary>
		/// <param name="left">The left map.</param>
		/// <param name="right">The right map.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool Equals(SudokuMap? left, SudokuMap? right) =>
			(left is null, right is null) switch
			{
				(true, true) => true,
				(false, false) => Enumerable.Range(0, 729).All(i => left![i] == right![i]),
				_ => false
			};

		/// <summary>
		/// To assign the <see cref="BitArray"/> which is used for <see cref="SudokuMap(int, bool)"/>.
		/// </summary>
		/// <param name="candidate">The candidate.</param>
		/// <param name="setItself">The <see cref="bool"/> value.</param>
		/// <returns>The result array.</returns>
		/// <seealso cref="SudokuMap(int, bool)"/>
		private static BitArray AssignBitArray(int candidate, bool setItself)
		{
			var result = new BitArray(729);
			int cell = candidate / 9, digit = candidate % 9;
			foreach (int c in PeerMaps[cell])
			{
				result[c * 9 + digit] = true;
			}
			for (int d = 0; d < 9; d++)
			{
				if (d != digit || d == digit && setItself)
				{
					result[cell * 9 + d] = true;
				}
			}

			return result;
		}


		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(SudokuMap? left, SudokuMap? right) => Equals(left, right);

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(SudokuMap? left, SudokuMap? right) => !(left == right);

		/// <summary>
		/// Get a <see cref="SudokuMap"/> that contains all <paramref name="left"/> candidates
		/// but not in <paramref name="right"/> candidates.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>The result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SudokuMap operator -(SudokuMap left, SudokuMap right) =>
			new SudokuMap(((BitArray)left._innerArray.Clone()).And(right._innerArray).Not());

		/// <summary>
		/// Get all candidates that two <see cref="SudokuMap"/>s both contain.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>The intersection result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SudokuMap operator &(SudokuMap left, SudokuMap right) =>
			new SudokuMap(((BitArray)left._innerArray.Clone()).And(right._innerArray));

		/// <summary>
		/// Get all candidates from two <see cref="SudokuMap"/>s.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>The union result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SudokuMap operator |(SudokuMap left, SudokuMap right) =>
			new SudokuMap(((BitArray)left._innerArray.Clone()).Or(right._innerArray));

		/// <summary>
		/// Get all candidates that only appears once in two <see cref="SudokuMap"/>s.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>The symmetrical difference result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SudokuMap operator ^(SudokuMap left, SudokuMap right) =>
			new SudokuMap(((BitArray)left._innerArray.Clone()).Xor(right._innerArray));

		/// <summary>
		/// Reverse status for all candidates, which means all <see langword="true"/> bits
		/// will be set <see langword="false"/>, and all <see langword="false"/> bits
		/// will be set <see langword="true"/>.
		/// </summary>
		/// <param name="map">The instance to negate.</param>
		/// <returns>The negative result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SudokuMap operator ~(SudokuMap map) =>
			new SudokuMap(((BitArray)map._innerArray.Clone()).Not());


		/// <summary>
		/// Implicite cast from <see cref="int"/>[] to <see cref="SudokuMap"/>.
		/// </summary>
		/// <param name="cells">The cells.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator SudokuMap(int[] cells) => new SudokuMap(cells);

		/// <summary>
		/// Implicit cast from <see cref="ReadOnlySpan{T}"/> to <see cref="SudokuMap"/>.
		/// </summary>
		/// <param name="cells">The cells.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator SudokuMap(ReadOnlySpan<int> cells) => new SudokuMap(cells);

		/// <summary>
		/// Explicit cast from <see cref="SudokuMap"/> to <see cref="bool"/>[].
		/// </summary>
		/// <param name="map">The map.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator bool[](SudokuMap map) => map.Bits;
	}
}
