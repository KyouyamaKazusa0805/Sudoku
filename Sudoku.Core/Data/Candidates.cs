using System;
using System.Collections;
using System.Collections.Generic;
using System.Extensions;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Sudoku.DocComments;
using static Sudoku.Constants.Processings;

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
			AssignFixedArray(candidate, setItself);
		}

		/// <summary>
		/// Initializes an instance with the specified candidates.
		/// </summary>
		/// <param name="candidates">The candidates.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Candidates(int[] candidates) : this(candidates.AsEnumerable())
		{
		}

		/// <summary>
		/// Initializes an instance with the binary array.
		/// </summary>
		/// <param name="binary">The array.</param>
		/// <exception cref="ArgumentException">Throws when the length is invalid.</exception>
		public Candidates(long[] binary)
		{
			_ = binary.Length != BufferLength ? throw new ArgumentException("The specified argument is invalid due to its length.", nameof(binary)) : 0;

			int count = 0;
			fixed (long* pThis = _innerBinary)
			{
				long* p = pThis;
				for (int i = 0; i < BufferLength; i++)
				{
					long v = binary[i];
					*p++ = v;
					count += v.PopCount();
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
			_ = length != BufferLength ? throw new ArgumentException("The specified argument is invalid due to its length.", nameof(length)) : 0;

			int count = 0;
			fixed (long* pThis = _innerBinary)
			{
				long* p = pThis;
				for (int i = 0; i < BufferLength; i++, *p++ = *binary++, count += binary->PopCount()) ;
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
				this[cell * 9 + digit] = true;
				Count++;
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
		/// This property is equivalent to code '<c>!this.IsNotEmpty</c>'.
		/// </summary>
		/// <seealso cref="IsNotEmpty"/>
		public readonly bool IsEmpty => Count == 0;

		/// <summary>
		/// Indicates whether the map has at least one set bit.
		/// This property is equivalent to code '<c>!this.IsEmpty</c>'.
		/// </summary>
		/// <seealso cref="IsEmpty"/>
		public readonly bool IsNotEmpty => Count != 0;

		/// <summary>
		/// Indicates how many bits are set <see langword="true"/>.
		/// </summary>
		public int Count { readonly get; private set; }

		/// <summary>
		/// Gets the first set bit position. If the current map is empty,
		/// the return value will be <c>-1</c>.
		/// </summary>
		/// <remarks>
		/// The property will use the same process with <see cref="Offsets"/>,
		/// but the <see langword="yield"/> clause will be replaced with normal <see langword="return"/>s.
		/// </remarks>
		/// <seealso cref="Offsets"/>
		public readonly int First
		{
			get
			{
				if (IsEmpty)
				{
					return -1;
				}

				fixed (long* pArray = _innerBinary)
				{
					int blockPos = 0;
					for (long* p = pArray; blockPos < BufferLength; blockPos++, p++)
					{
						if (*p == 0)
						{
							continue;
						}

						int i = 0;
						for (long value = *p; i < Shifting; i++, value >>= 1)
						{
							if ((value & 1) != 0)
							{
								return blockPos * Shifting + i;
							}
						}
					}
				}

				return default; // Here is only used for a placeholder.
			}
		}

		/// <summary>
		/// Indicates the map of cells, which is the peer intersections.
		/// </summary>
		public readonly Candidates PeerIntersection => CreateInstance(Offsets);

		/// <summary>
		/// Indicates all indices of set bits.
		/// </summary>
		private readonly IEnumerable<int> Offsets
		{
			get
			{
				if (Count == 0)
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
		[IndexerName("Candidate")]
		public bool this[int candidate]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get => ((_innerBinary[candidate / Shifting] >> candidate % Shifting) & 1) != 0;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				fixed (long* pArray = _innerBinary)
				{
					long* block = &pArray[candidate / Shifting];
					bool older = this[candidate];
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
		/// Indicates whether this map overlaps another one.
		/// </summary>
		/// <param name="other">(<see langword="in"/> parameter) The other map.</param>
		/// <returns>The <see cref="bool"/> value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Overlaps(in Candidates other) => (this & other).IsNotEmpty;

		/// <summary>
		/// Get a n-th index of the <see langword="true"/> bit in this instance.
		/// </summary>
		/// <param name="index">The true bit index order.</param>
		/// <returns>The real index.</returns>
		/// <remarks>
		/// If you want to select the first set bit, please use <see cref="First"/> instead.
		/// </remarks>
		/// <seealso cref="First"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int SetAt(int index) => index == 0 ? First : Offsets.ElementAt(index);

		/// <summary>
		/// Get a n-th index of the <see langword="true"/> bit in this instance.
		/// </summary>
		/// <param name="index">(<see langword="in"/> parameter) The true bit index order.</param>
		/// <returns>The real index.</returns>
		/// <remarks>
		/// If you want to select the first set bit, please use <see cref="First"/> instead.
		/// </remarks>
		/// <seealso cref="First"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int SetAt(in Index index) => Offsets.ElementAt(index.GetOffset(Count));

		/// <inheritdoc cref="object.GetHashCode"/>
		public override readonly int GetHashCode()
		{
			long @base = 0xDECADE;
			for (int i = 0; i < BufferLength; i++)
			{
				@base ^= _innerBinary[i];
			}

			return (int)(@base & 0xABCDEF);
		}

		/// <summary>
		/// Get all cell offsets whose bits are set <see langword="true"/>.
		/// </summary>
		/// <returns>An array of cell offsets.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int[] ToArray()
		{
			// To avoid the implicitly copy, we use the pointer to point to the same memory.
			fixed (Candidates* @this = &this)
			{
				return @this->Offsets.ToArray();
			}
		}

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
				if (this[RegionCells[region][i] * 9 + digit])
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
					int candidate = First, cell = candidate / 9, digit = candidate % 9;
					return $"r{cell / 9 + 1}c{cell % 9 + 1}({digit + 1})";
				}
				default:
				{
					const string separator = ", ";
					var sb = new StringBuilder();

					int[] candidates = ToArray();
					foreach (var digitGroup in from candidate in candidates group candidate by candidate % 9)
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
				if (this[cell * 9 + digit])
				{
					result.AddAnyway(cell);
				}
			}

			return result;
		}

		/// <inheritdoc/>
		public readonly IEnumerator<int> GetEnumerator() => Offsets.GetEnumerator();

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
		/// <remarks>
		/// <para>
		/// For example, if the offset is -2 (~1), the [1] will be assigned <see langword="false"/>:
		/// <code>
		/// var map = new GridMap(xxx) { ~1 };
		/// </code>
		/// which is equivalent to:
		/// <code>
		/// var map = new GridMap(xxx);
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
			switch (offset)
			{
				case >= 0 when !this[offset]:
				{
					this[offset] = true;
					break;
				}
				case < 0 when this[~offset]:
				{
					this[~offset] = false;
					break;
				}
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
		public void AddAnyway(int offset) => this[offset] = true;

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
		public void Remove(int offset) => this[offset] = false;

		/// <summary>
		/// Set the specified candidates as <see langword="true"/> value.
		/// </summary>
		/// <param name="candidates">(<see langword="in"/> parameter) The candidate offsets.</param>
		public void AddRange(in ReadOnlySpan<int> candidates)
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
		/// Calls the method while initializing.
		/// </summary>
		/// <param name="candidate">The candidate.</param>
		/// <param name="setItself">Indicates whether the map should set itself.</param>
		private void AssignFixedArray(int candidate, bool setItself)
		{
			int cell = candidate / 9, digit = candidate % 9;
			foreach (int c in PeerMaps[cell])
			{
				this[c * 9 + digit] = true;
			}
			for (int d = 0; d < 9; d++)
			{
				if (d != digit || d == digit && setItself)
				{
					this[cell * 9 + d] = true;
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

			return new(result);
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
		[SkipLocalsInit]
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
		[SkipLocalsInit]
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
		[SkipLocalsInit]
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
		[SkipLocalsInit]
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
		[SkipLocalsInit]
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
