using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
	/// Encapsulats a map that contains 729 positions to represent a candidate.
	/// </summary>
	public unsafe struct Candidates : IEnumerable<int>, IValueEquatable<Candidates>
	{
		/// <summary>
		/// The length of the buffer.
		/// </summary>
		/// <remarks>
		/// Why 12? Because 12 is equals to <c>Ceiling(729 / 64)</c>.
		/// </remarks>
		private const int BufferLength = 12;

		/// <summary>
		/// Indicates the size of each unit.
		/// </summary>
		private const int Shifting = sizeof(long) * 8;


		/// <summary>
		/// <para>Indicates an empty instance (all bits are 0).</para>
		/// <para>
		/// I strongly recommend you <b>should</b> use this instance instead of default constructor
		/// <see cref="Candidates()"/>.
		/// </para>
		/// </summary>
		/// <seealso cref="Candidates()"/>
		public static readonly Candidates Empty;


		/// <summary>
		/// The inner binary values.
		/// </summary>
		private fixed long _innerBinary[BufferLength];


		/// <summary>
		/// (Copy constructor) Initializes an instance with another one.
		/// </summary>
		/// <param name="another">(<see langword="in"/> parameter) The another instance.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Candidates(in Candidates another) => this = another;

		/// <summary>
		/// Initializes an instance with the specified candidate and its peers.
		/// </summary>
		/// <param name="candidate">The candidate.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Candidates(int candidate) : this(candidate, true)
		{
		}

		/// <summary>
		/// Initializes an instance with the candidate list specified as a pointer.
		/// </summary>
		/// <param name="candidates">The pointer points to an array of elements.</param>
		/// <param name="length">The length of the array.</param>
		[CLSCompliant(false)]
		public Candidates(int* candidates, int length) : this()
		{
			for (int i = 0; i < length; i++)
			{
				InternalAdd(candidates[i], true);
			}
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
		public Candidates(int candidate, bool setItself)
		{
			Count = setItself ? 29 : 28;
			assign(ref this, candidate, setItself);

			static void assign(ref Candidates @this, int candidate, bool setItself)
			{
				int cell = candidate / 9, digit = candidate % 9;
				foreach (int c in PeerMaps[cell])
				{
					@this.InternalAdd(c * 9 + digit, true);
				}
				for (int d = 0; d < 9; d++)
				{
					if (d != digit || d == digit && setItself)
					{
						@this.InternalAdd(cell * 9 + d, true);
					}
				}
			}
		}

		/// <summary>
		/// Initializes an instance with the specified candidates.
		/// </summary>
		/// <param name="candidates">The candidates.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Candidates(int[] candidates) : this((IEnumerable<int>)candidates)
		{
		}

		/// <summary>
		/// Initializes an instance with the binary array.
		/// </summary>
		/// <param name="binary">The array.</param>
		/// <exception cref="ArgumentException">Throws when the length is invalid.</exception>
		public Candidates(long[] binary)
		{
			if (binary.Length != BufferLength)
			{
				throw new ArgumentException($"The length of the array should be {BufferLength.ToString()}.", nameof(binary));
			}

			int count = 0;
			fixed (long* pThis = _innerBinary)
			{
				long* p = pThis;
				for (int i = 0; i < BufferLength; i++)
				{
					long v = binary[i];
					*p++ = v;
					count += PopCount((ulong)v);
				}
			}

			Count = count;
		}

		/// <summary>
		/// Initializes an instance with the pointer to the binary array and the length.
		/// </summary>
		/// <param name="binary">The pointer to the binary array.</param>
		/// <param name="length">The length.</param>
		/// <exception cref="ArgumentException">Throws when the length is invalid.</exception>
		[CLSCompliant(false)]
		public Candidates(long* binary, int length)
		{
			if (length != BufferLength)
			{
				throw new ArgumentException($"Argument 'length' should be {BufferLength.ToString()}.", nameof(length));
			}

			int count = 0;
			fixed (long* pThis = _innerBinary)
			{
				long* p = pThis;
				for (int i = 0; i < BufferLength; i++, *p++ = *binary++)
				{
					count += PopCount((ulong)*binary);
				}
			}

			Count = count;
		}

		/// <summary>
		/// Initializes an instance with the specified <see cref="Cells"/> and the number
		/// representing.
		/// </summary>
		/// <param name="map">(<see langword="in"/> parameter) The map.</param>
		/// <param name="digit">The digit.</param>
		public Candidates(in Cells map, int digit) : this()
		{
			foreach (int cell in map)
			{
				InternalAdd(cell * 9 + digit, true);
			}
		}

		/// <summary>
		/// Initializes an instance with the specified candidates.
		/// </summary>
		/// <param name="candidates">(<see langword="in"/> parameter) The candidates.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Candidates(in ReadOnlySpan<int> candidates) : this() => AddRange(candidates);

		/// <summary>
		/// Initializes an instance with the specified candidates.
		/// </summary>
		/// <param name="candidates">The candidates.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Candidates(IEnumerable<int> candidates) : this() => AddRange(candidates);


		/// <summary>
		/// Indicates whether the map has no set bits.
		/// </summary>
		public readonly bool IsEmpty => Count == 0;

		/// <summary>
		/// Indicates how many bits are set <see langword="true"/>.
		/// </summary>
		public int Count { readonly get; private set; }

		/// <summary>
		/// Indicates the map of cells, which is the peer intersections.
		/// </summary>
		public readonly Candidates PeerIntersection => CreateInstance(Offsets);

		/// <summary>
		/// Indicates all indices of set bits.
		/// </summary>
		private readonly int[] Offsets
		{
			get
			{
				if (Count == 0)
				{
					return Array.Empty<int>();
				}

				int[] result = new int[Count];
				int count = 0;
				for (int i = 0; i < 729; i++)
				{
					if (Contains(i))
					{
						result[count++] = i;
					}
				}

				return result;
			}
		}


		/// <summary>
		/// Gets the result set candidate at the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>
		/// The candidate at that index. If the index is invalid, the return value will be -1.
		/// </returns>
		[IndexerName("Candidate")]
		public readonly int this[int index]
		{
			get
			{
				for (int i = 0, count = -1; i < 729; i++)
				{
					if (Contains(i) && ++count == index)
					{
						return i;
					}
				}

				return -1;
			}
		}


		/// <inheritdoc cref="object.Equals(object?)"/>
		public override readonly bool Equals(object? obj) => obj is Candidates comparer && Equals(comparer);

		/// <inheritdoc/>
		[CLSCompliant(false)]
		public readonly bool Equals(in Candidates other)
		{
			for (int i = 0; i < BufferLength; i++)
			{
				if (_innerBinary[i] != other._innerBinary[i])
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Check whether the specified candidate is in the current list.
		/// </summary>
		/// <param name="candidate">The candidate to check.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Contains(int candidate) =>
			(_innerBinary[candidate / Shifting] >> candidate % Shifting & 1) != 0;

		/// <inheritdoc cref="object.GetHashCode"/>
		public override readonly int GetHashCode()
		{
			long @base = 0xDECADE;
			for (int i = 0; i < BufferLength; i++)
			{
				@base ^= _innerBinary[i];
			}

			return (int)(@base & int.MaxValue);
		}

		/// <summary>
		/// Get all cell offsets whose bits are set <see langword="true"/>.
		/// </summary>
		/// <returns>An array of cell offsets.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int[] ToArray() => Offsets;

		/// <summary>
		/// Get the first position of the inner binary array.
		/// </summary>
		/// <returns>The reference of the first position.</returns>
		public readonly ref readonly long GetPinnableReference() => ref _innerBinary[0];

		/// <summary>
		/// Get the subview mask of this map.
		/// </summary>
		/// <param name="region">The region.</param>
		/// <param name="digit">The digit.</param>
		/// <returns>The mask.</returns>
		public readonly short GetSubviewMask(int region, int digit)
		{
			short p = 0;
			for (int i = 0; i < RegionCells[region].Length; i++)
			{
				if (Contains(RegionCells[region][i] * 9 + digit))
				{
					p |= (short)(1 << i);
				}
			}

			return p;
		}

		/// <inheritdoc/>
		public override readonly string ToString()
		{
			switch (Count)
			{
				case 0:
				{
					return "{ }";
				}
				case 1:
				{
					int candidate = this[0], cell = candidate / 9, digit = candidate % 9;
					return $"r{(cell / 9 + 1).ToString()}c{(cell % 9 + 1).ToString()}({(digit + 1).ToString()})";
				}
				default:
				{
					const string separator = ", ";
					var sb = new StringBuilder();

					foreach (var digitGroup in
						from candidate in Offsets
						group candidate by candidate % 9 into digitGroups
						orderby digitGroups.Key
						select digitGroups)
					{
						sb
							.Append(new Cells(from candidate in digitGroup select candidate / 9).ToString())
							.Append('(')
							.Append(digitGroup.Key + 1)
							.Append(')')
							.Append(separator);
					}

					return sb.RemoveFromEnd(separator.Length).ToString();
				}
			}
		}

		/// <summary>
		/// Get the final <see cref="Cells"/> to get all cells that the corresponding indices
		/// are set <see langword="true"/>.
		/// </summary>
		/// <param name="digit">The digit.</param>
		/// <returns>The map of all cells chosen.</returns>
		public readonly Cells Reduce(int digit)
		{
			var result = Cells.Empty;
			for (int cell = 0; cell < 81; cell++)
			{
				if (Contains(cell * 9 + digit))
				{
					result.AddAnyway(cell);
				}
			}

			return result;
		}

		/// <inheritdoc/>
		public readonly IEnumerator<int> GetEnumerator() => ((IEnumerable<int>)Offsets).GetEnumerator();

		/// <inheritdoc/>
		readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		/// <summary>
		/// Set the specified cell as <see langword="true"/> or <see langword="false"/> value.
		/// </summary>
		/// <param name="offset">
		/// The cell offset. This value can be positive and negative. If 
		/// negative, the offset will be assigned <see langword="false"/>
		/// into the corresponding bit position of its absolute value.
		/// </param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Add(int offset)
		{
			switch (offset)
			{
				case >= 0 when !Contains(offset):
				{
					InternalAdd(offset, true);
					break;
				}
				case < 0 when Contains(~offset):
				{
					InternalAdd(~offset, false);
					break;
				}
			}
		}

		/// <summary>
		/// Set the specified candidate as <see langword="true"/> value.
		/// </summary>
		/// <param name="candidate">The candidate offset.</param>
		/// <remarks>
		/// Different with <see cref="Add(int)"/>, the method will process negative values,
		/// but this won't.
		/// </remarks>
		/// <seealso cref="Add(int)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddAnyway(int candidate) => InternalAdd(candidate, true);

		/// <summary>
		/// Set the specified candidate as <see langword="false"/> value.
		/// </summary>
		/// <param name="candidate">The cell offset.</param>
		/// <remarks>
		/// Different with <see cref="Add(int)"/>, this method <b>can't</b> receive
		/// the negative value as the parameter.
		/// </remarks>
		/// <seealso cref="Add(int)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Remove(int candidate) => InternalAdd(candidate, false);

		/// <summary>
		/// Set the specified candidates as <see langword="true"/> value.
		/// </summary>
		/// <param name="candidates">(<see langword="in"/> parameter) The candidate offsets.</param>
		public void AddRange(in ReadOnlySpan<int> candidates)
		{
			foreach (int candidate in candidates)
			{
				AddAnyway(candidate);
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
				AddAnyway(candidate);
			}
		}

		/// <summary>
		/// Clear all bits.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			fixed (long* pArray = _innerBinary)
			{
				long* p = pArray;
				for (int i = 0; i < BufferLength; i++, p++)
				{
					// Clear the memory.
					*p = 0;
				}
			}

			Count = 0;
		}

		/// <summary>
		/// The add method.
		/// </summary>
		/// <param name="candidate">The candidate.</param>
		/// <param name="value">The value.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void InternalAdd(int candidate, bool value)
		{
			fixed (long* pArray = _innerBinary)
			{
				long* block = pArray + candidate / Shifting;
				bool older = Contains(candidate);
				if (value)
				{
					*block |= 1L << candidate % Shifting;
					if (!older)
					{
						Count++;
					}
				}
				else
				{
					*block &= ~(1L << candidate % Shifting);
					if (older)
					{
						Count--;
					}
				}
			}
		}


		/// <summary>
		/// Get the map of candidates, which is the peer intersections from the specified candidates.
		/// </summary>
		/// <param name="candidates">All candidates.</param>
		/// <returns>The result map.</returns>
		public static Candidates CreateInstance(IEnumerable<int> candidates)
		{
			var result = ~Empty;
			foreach (int candidate in candidates)
			{
				result &= new Candidates(candidate, false);
			}

			return result;
		}


		/// <inheritdoc cref="Operators.operator =="/>
		public static bool operator ==(in Candidates left, in Candidates right) => left.Equals(right);

		/// <inheritdoc cref="Operators.operator !="/>
		public static bool operator !=(in Candidates left, in Candidates right) => !(left == right);

		/// <summary>
		/// Reverse status for all candidates, which means all <see langword="true"/> bits
		/// will be set <see langword="false"/>, and all <see langword="false"/> bits
		/// will be set <see langword="true"/>.
		/// </summary>
		/// <param name="map">(<see langword="in"/> parameter) The instance to negate.</param>
		/// <returns>The negative result.</returns>
		public static Candidates operator ~(in Candidates map)
		{
			long* result = stackalloc long[BufferLength];
			for (int i = 0; i < BufferLength; i++)
			{
				result[i] = ~map._innerBinary[i];
			}

			return new(result, BufferLength);
		}

		/// <summary>
		/// Get all candidates that two <see cref="Candidates"/>s both contain.
		/// </summary>
		/// <param name="left">(<see langword="in"/> parameter) The left instance.</param>
		/// <param name="right">(<see langword="in"/> parameter) The right instance.</param>
		/// <returns>The intersection result.</returns>
		public static Candidates operator &(in Candidates left, in Candidates right)
		{
			long* result = stackalloc long[BufferLength];
			for (int i = 0; i < BufferLength; i++)
			{
				result[i] = left._innerBinary[i] & right._innerBinary[i];
			}

			return new(result, BufferLength);
		}

		/// <summary>
		/// Get all candidates from two <see cref="Candidates"/>s.
		/// </summary>
		/// <param name="left">(<see langword="in"/> parameter) The left instance.</param>
		/// <param name="right">(<see langword="in"/> parameter) The right instance.</param>
		/// <returns>The union result.</returns>
		public static Candidates operator |(in Candidates left, in Candidates right)
		{
			long* result = stackalloc long[BufferLength];
			for (int i = 0; i < BufferLength; i++)
			{
				result[i] = left._innerBinary[i] | right._innerBinary[i];
			}

			return new(result, BufferLength);
		}

		/// <summary>
		/// Get all candidates that only appears once in two <see cref="Candidates"/>s.
		/// </summary>
		/// <param name="left">(<see langword="in"/> parameter) The left instance.</param>
		/// <param name="right">(<see langword="in"/> parameter) The right instance.</param>
		/// <returns>The symmetrical difference result.</returns>
		public static Candidates operator ^(in Candidates left, in Candidates right)
		{
			long* result = stackalloc long[BufferLength];
			for (int i = 0; i < BufferLength; i++)
			{
				result[i] = left._innerBinary[i] ^ right._innerBinary[i];
			}

			return new(result, BufferLength);
		}

		/// <summary>
		/// Get a <see cref="Cells"/> that contains all <paramref name="left"/> candidates
		/// but not in <paramref name="right"/> candidates.
		/// </summary>
		/// <param name="left">(<see langword="in"/> parameter) The left instance.</param>
		/// <param name="right">(<see langword="in"/> parameter) The right instance.</param>
		/// <returns>The result.</returns>
		public static Candidates operator -(in Candidates left, in Candidates right)
		{
			long* result = stackalloc long[BufferLength];
			for (int i = 0; i < BufferLength; i++)
			{
				result[i] = left._innerBinary[i] & ~right._innerBinary[i];
			}

			return new(result, BufferLength);
		}


		/// <summary>
		/// Implicit cast from <see cref="Candidates"/> to <see cref="int"/>[].
		/// </summary>
		/// <param name="map">(<see langword="in"/> parameter) The map.</param>
		public static implicit operator int[](in Candidates map) => map.ToArray();

		/// <summary>
		/// Implicit cast from <see cref="int"/>[] to <see cref="Candidates"/>.
		/// </summary>
		/// <param name="array">The array.</param>
		public static implicit operator Candidates(int[] array) => new(array);
	}
}
