using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Extensions;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Sudoku.CodeGen;
using static System.Numerics.BitOperations;
using static Sudoku.Constants;
using static Sudoku.Constants.Tables;
using ParsingOptions = Sudoku.Data.CandidatesParsingOptions;

namespace Sudoku.Data
{
	/// <summary>
	/// Encapsulates a map that contains 729 positions to represent a candidate.
	/// </summary>
	[AutoHashCode(nameof(_1), nameof(_2), nameof(_3), nameof(_4), nameof(_5), nameof(_6), nameof(_7), nameof(_8), nameof(_9), nameof(_10), nameof(_11))]
	[AutoEquality(nameof(_1), nameof(_2), nameof(_3), nameof(_4), nameof(_5), nameof(_6), nameof(_7), nameof(_8), nameof(_9), nameof(_10), nameof(_11))]
	[AutoGetEnumerator(nameof(Offsets), MemberConversion = "((IEnumerable<int>)@).*")]
	public unsafe partial struct Candidates : IEnumerable<int>, IValueEquatable<Candidates>
	{
		/// <summary>
		/// Indicates the size of each unit.
		/// </summary>
		private const int Shifting = sizeof(long) * 8;

		/// <summary>
		/// Indicates the number of all segments.
		/// </summary>
		private const int Len = 12;

		/// <summary>
		/// Indicates the length of the collection that all bits are set <see langword="true"/>.
		/// </summary>
		private const int FullCount = 729;


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
		private long _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11;


		/// <summary>
		///  Initializes an instance with another one.
		/// </summary>
		/// <param name="another">The another instance.</param>
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
		public Candidates(int* candidates, int length) : this()
		{
			for (int i = 0; i < length; i++)
			{
				InternalAdd(candidates[i], true);
			}
		}

		/// <summary>
		/// Indicates an instance with the peer candidates of the specified candidate and a <see cref="bool"/>
		/// value indicating whether the map will process itself with <see langword="true"/> value.
		/// </summary>
		/// <param name="candidate">The candidate.</param>
		/// <param name="setItself">
		/// Indicates whether the map will process itself with <see langword="true"/> value.
		/// </param>
		public Candidates(int candidate, bool setItself) : this()
		{
			int cell = candidate / 9, digit = candidate % 9;
			foreach (int c in PeerMaps[cell])
			{
				InternalAdd(c * 9 + digit, true);
			}
			for (int d = 0; d < 9; d++)
			{
				if (d != digit || d == digit && setItself)
				{
					InternalAdd(cell * 9 + d, true);
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Candidates(long[] binary)
		{
			if (binary.Length != Len)
			{
				throw new ArgumentException(
					$"The length of the array should be {Len.ToString()}.", nameof(binary));
			}

			int count = 0;

			_0 = binary[0]; count += PopCount((ulong)binary[0]);
			_1 = binary[1]; count += PopCount((ulong)binary[1]);
			_2 = binary[2]; count += PopCount((ulong)binary[2]);
			_3 = binary[3]; count += PopCount((ulong)binary[3]);
			_4 = binary[4]; count += PopCount((ulong)binary[4]);
			_5 = binary[5]; count += PopCount((ulong)binary[5]);
			_6 = binary[6]; count += PopCount((ulong)binary[6]);
			_7 = binary[7]; count += PopCount((ulong)binary[7]);
			_8 = binary[8]; count += PopCount((ulong)binary[8]);
			_9 = binary[9]; count += PopCount((ulong)binary[9]);
			_10 = binary[10]; count += PopCount((ulong)binary[10]);
			_11 = binary[11]; count += PopCount((ulong)binary[11]);

			Count = count;
		}

		/// <summary>
		/// Initializes an instance with the pointer to the binary array and the length.
		/// </summary>
		/// <param name="binary">The pointer to the binary array.</param>
		/// <param name="length">The length.</param>
		/// <exception cref="ArgumentException">Throws when the length is invalid.</exception>
		public Candidates(long* binary, int length)
		{
			if (length != Len)
			{
				throw new ArgumentException(
					$"Argument '{nameof(length)}' should be {Len.ToString()}.", nameof(length));
			}

			int count = 0;

			_0 = binary[0]; count += PopCount((ulong)binary[0]);
			_1 = binary[1]; count += PopCount((ulong)binary[1]);
			_2 = binary[2]; count += PopCount((ulong)binary[2]);
			_3 = binary[3]; count += PopCount((ulong)binary[3]);
			_4 = binary[4]; count += PopCount((ulong)binary[4]);
			_5 = binary[5]; count += PopCount((ulong)binary[5]);
			_6 = binary[6]; count += PopCount((ulong)binary[6]);
			_7 = binary[7]; count += PopCount((ulong)binary[7]);
			_8 = binary[8]; count += PopCount((ulong)binary[8]);
			_9 = binary[9]; count += PopCount((ulong)binary[9]);
			_10 = binary[10]; count += PopCount((ulong)binary[10]);
			_11 = binary[11]; count += PopCount((ulong)binary[11]);

			Count = count;
		}

		/// <summary>
		/// Initializes an instance with the specified <see cref="Cells"/> and the number
		/// representing.
		/// </summary>
		/// <param name="map">The map.</param>
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
		/// <param name="candidates">The candidates.</param>
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
		public readonly bool IsEmpty
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Count == 0;
		}

		/// <summary>
		/// Indicates how many bits are set <see langword="true"/>.
		/// </summary>
		public int Count { get; private set; }

		/// <summary>
		/// Indicates the map of cells, which is the peer intersections.
		/// </summary>
		public readonly Candidates PeerIntersection
		{
			get
			{
				if (Count == 0)
				{
					// Empty list can't contain any peer intersections.
					return Empty;
				}

				var result = ~Empty;
				foreach (int candidate in Offsets)
				{
					result &= new Candidates(candidate, false);
				}

				return result;
			}
		}

		/// <summary>
		/// Indicates all indices of set bits.
		/// </summary>
		private readonly int[] Offsets
		{
			get
			{
				if (IsEmpty)
				{
					return Array.Empty<int>();
				}

				int[] result = new int[Count];
				int count = 0;
				for (int i = 0; i < FullCount; i++)
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
		public readonly int this[int index]
		{
			get
			{
				for (int i = 0, count = -1; i < FullCount; i++)
				{
					if (Contains(i) && ++count == index)
					{
						return i;
					}
				}

				return -1;
			}
		}


		/// <summary>
		/// Copies the current instance to the target array specified as an <see cref="int"/>*.
		/// </summary>
		/// <param name="arr">The pointer that points to an array of type <see cref="int"/>.</param>
		/// <param name="length">The length of that array.</param>
		public readonly void CopyTo(int* arr, int length)
		{
			if (IsEmpty)
			{
				return;
			}

			if (length < Count)
			{
				throw new ArgumentException("The capacity is not enough.", nameof(arr));
			}

			for (int i = 0, count = 0; i < FullCount; i++)
			{
				if (Contains(i))
				{
					arr[count++] = i;
				}
			}
		}

		/// <summary>
		/// Copies the current instance to the target <see cref="Span{T}"/> instance.
		/// </summary>
		/// <param name="span">
		/// The target <see cref="Span{T}"/> instance.
		/// </param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void CopyTo(ref Span<int> span)
		{
			fixed (int* arr = span)
			{
				CopyTo(arr, span.Length);
			}
		}


		/// <summary>
		/// Check whether the specified candidate is in the current list.
		/// </summary>
		/// <param name="candidate">The candidate to check.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Contains(int candidate) =>
		(
			(candidate / Shifting) switch
			{
				0 => _0,
				1 => _1,
				2 => _2,
				3 => _3,
				4 => _4,
				5 => _5,
				6 => _6,
				7 => _7,
				8 => _8,
				9 => _9,
				10 => _10,
				11 => _11
			} >> candidate % Shifting & 1
		) != 0;

		/// <summary>
		/// Get all cell offsets whose bits are set <see langword="true"/>.
		/// </summary>
		/// <returns>An array of cell offsets.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int[] ToArray() => Offsets;

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
					var sb = new ValueStringBuilder(stackalloc char[50]);

					foreach (var digitGroup in
						from candidate in Offsets
						group candidate by candidate % 9 into digitGroups
						orderby digitGroups.Key
						select digitGroups)
					{
						var cells = Cells.Empty;
						foreach (int candidate in digitGroup)
						{
							cells.AddAnyway(candidate / 9);
						}

						sb.Append(cells.ToString());
						sb.Append('(');
						sb.Append(digitGroup.Key + 1);
						sb.Append(')');
						sb.Append(separator);
					}

					sb.RemoveFromEnd(separator.Length);
					return sb.ToString();
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

		/// <summary>
		/// Converts the current instance to a <see cref="Span{T}"/> of type <see cref="int"/>.
		/// </summary>
		/// <returns>The <see cref="Span{T}"/> of <see cref="int"/> result.</returns>
		public readonly Span<int> ToSpan() => Offsets.AsSpan();

		/// <summary>
		/// Converts the current instance to a <see cref="ReadOnlySpan{T}"/> of type <see cref="int"/>.
		/// </summary>
		/// <returns>The <see cref="ReadOnlySpan{T}"/> of <see cref="int"/> result.</returns>
		public readonly ReadOnlySpan<int> ToReadOnlySpan() => Offsets.AsSpan();

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
		/// <param name="candidates">The candidate offsets.</param>
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
			_0 = _1 = _2 = _3 = _4 = _5 = _6 = _7 = _8 = _9 = _10 = _11 = 0;
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
			fixed (Candidates* pThis = &this)
			{
				bool older = Contains(candidate);
				long* block = (candidate / Shifting) switch
				{
					0 => &pThis->_0,
					1 => &pThis->_1,
					2 => &pThis->_2,
					3 => &pThis->_3,
					4 => &pThis->_4,
					5 => &pThis->_5,
					6 => &pThis->_6,
					7 => &pThis->_7,
					8 => &pThis->_8,
					9 => &pThis->_9,
					10 => &pThis->_10,
					11 => &pThis->_11
				};
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
		/// Parse a <see cref="string"/> and convert to the <see cref="Candidates"/> instance.
		/// </summary>
		/// <param name="str">The string text.</param>
		/// <param name="options">
		/// The options to parse. The default value is <see cref="ParsingOptions.All"/>.
		/// </param>
		/// <returns>The result cell instance.</returns>
		/// <exception cref="ArgumentException">Throws when <paramref name="options"/> is invalid.</exception>
		/// <exception cref="FormatException">Throws when the specified text is invalid to parse.</exception>
		/// <seealso cref="ParsingOptions.All"/>
		public static unsafe Candidates Parse(string str, ParsingOptions options = ParsingOptions.All)
		{
			if (options is ParsingOptions.None or > ParsingOptions.All)
			{
				throw new ArgumentException("The option is invalid.", nameof(options));
			}

			var regex = new Regex(
				RegularExpressions.CandidateOrCandidateList,
				RegexOptions.ExplicitCapture,
				TimeSpan.FromSeconds(5)
			);

			// Check whether the match is successful.
			var matches = regex.Matches(str);
			if (matches.Count == 0)
			{
				throw new FormatException("The specified string can't match any candidate instance.");
			}

			var result = Empty;

			// Iterate on each match item.
			int* bufferDigits = stackalloc int[9];
			foreach (Match match in matches)
			{
				string value = match.Value;
				if (
					options.Flags(ParsingOptions.ShortForm)
					&& value.SatisfyPattern(RegularExpressions.CandidateListShortForm)
				)
				{
					result.AddAnyway((value[1] - '1') * 81 + (value[2] - '1') * 9 + value[0] - '1');
				}
				else if (
					options.Flags(ParsingOptions.BracketForm)
					&& value.SatisfyPattern(RegularExpressions.CandidateListPrepositionalForm)
				)
				{
					var cells = Cells.Parse(value);
					int digitsCount = 0;
					fixed (char* pValue = value)
					{
						for (char* ptr = pValue; *ptr is not ('{' or 'R' or 'r'); ptr++)
						{
							bufferDigits[digitsCount++] = *ptr - '1';
						}
					}

					foreach (int cell in cells)
					{
						for (int i = 0; i < digitsCount; i++)
						{
							result.AddAnyway(cell * 9 + bufferDigits[i]);
						}
					}
				}
				else if (
					options.Flags(ParsingOptions.PrepositionalForm)
					&& value.SatisfyPattern(RegularExpressions.CandidateListPostpositionalForm)
				)
				{
					var cells = Cells.Parse(value);
					int digitsCount = 0;
					for (int i = value.IndexOf('(') + 1, length = value.Length; i < length; i++)
					{
						bufferDigits[digitsCount++] = value[i] - '1';
					}

					foreach (int cell in cells)
					{
						for (int i = 0; i < digitsCount; i++)
						{
							result.AddAnyway(cell * 9 + bufferDigits[i]);
						}
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Try to parse the specified <see cref="string"/>, and convert it to the <see cref="Candidates"/>
		/// instance.
		/// </summary>
		/// <param name="str">The string to parse.</param>
		/// <param name="result">The result that converted.</param>
		/// <returns>
		/// A <see cref="bool"/> result indicating whether the parsing operation
		/// has been successfully executed.
		/// </returns>
		public static bool TryParse(string str, out Candidates result)
		{
			try
			{
				result = Parse(str);
				return true;
			}
			catch (FormatException)
			{
				result = default;
				return false;
			}
		}


		/// <summary>
		/// Reverse status for all candidates, which means all <see langword="true"/> bits
		/// will be set <see langword="false"/>, and all <see langword="false"/> bits
		/// will be set <see langword="true"/>.
		/// </summary>
		/// <param name="map">The instance to negate.</param>
		/// <returns>The negative result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Candidates operator ~(in Candidates map)
		{
			const long s = (1 << FullCount - Shifting * (Len - 1)) - 1;

			long* result = stackalloc long[Len];
			result[0] = ~map._0;
			result[1] = ~map._1;
			result[2] = ~map._2;
			result[3] = ~map._3;
			result[4] = ~map._4;
			result[5] = ~map._5;
			result[6] = ~map._6;
			result[7] = ~map._7;
			result[8] = ~map._8;
			result[9] = ~map._9;
			result[10] = ~map._10;
			result[11] = ~map._11 & s;

			return new(result, Len);
		}

		/// <summary>
		/// Get all candidates that two <see cref="Candidates"/>'s both contain.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>The intersection result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Candidates operator &(in Candidates left, in Candidates right)
		{
			long* result = stackalloc long[Len];
			result[0] = left._0 & right._0;
			result[1] = left._1 & right._1;
			result[2] = left._2 & right._2;
			result[3] = left._3 & right._3;
			result[4] = left._4 & right._4;
			result[5] = left._5 & right._5;
			result[6] = left._6 & right._6;
			result[7] = left._7 & right._7;
			result[8] = left._8 & right._8;
			result[9] = left._9 & right._9;
			result[10] = left._10 & right._10;
			result[11] = left._11 & right._11;

			return new(result, Len);
		}

		/// <summary>
		/// Get all candidates from two <see cref="Candidates"/>s.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>The union result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Candidates operator |(in Candidates left, in Candidates right)
		{
			long* result = stackalloc long[Len];
			result[0] = left._0 | right._0;
			result[1] = left._1 | right._1;
			result[2] = left._2 | right._2;
			result[3] = left._3 | right._3;
			result[4] = left._4 | right._4;
			result[5] = left._5 | right._5;
			result[6] = left._6 | right._6;
			result[7] = left._7 | right._7;
			result[8] = left._8 | right._8;
			result[9] = left._9 | right._9;
			result[10] = left._10 | right._10;
			result[11] = left._11 | right._11;

			return new(result, Len);
		}

		/// <summary>
		/// Get all candidates that only appears once in two <see cref="Candidates"/>s.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>The symmetrical difference result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Candidates operator ^(in Candidates left, in Candidates right)
		{
			long* result = stackalloc long[Len];
			result[0] = left._0 ^ right._0;
			result[1] = left._1 ^ right._1;
			result[2] = left._2 ^ right._2;
			result[3] = left._3 ^ right._3;
			result[4] = left._4 ^ right._4;
			result[5] = left._5 ^ right._5;
			result[6] = left._6 ^ right._6;
			result[7] = left._7 ^ right._7;
			result[8] = left._8 ^ right._8;
			result[9] = left._9 ^ right._9;
			result[10] = left._10 ^ right._10;
			result[11] = left._11 ^ right._11;

			return new(result, Len);
		}

		/// <summary>
		/// Get a <see cref="Cells"/> that contains all <paramref name="left"/> candidates
		/// but not in <paramref name="right"/> candidates.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>The result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Candidates operator -(in Candidates left, in Candidates right)
		{
			long* result = stackalloc long[Len];
			result[0] = left._0 & ~right._0;
			result[1] = left._1 & ~right._1;
			result[2] = left._2 & ~right._2;
			result[3] = left._3 & ~right._3;
			result[4] = left._4 & ~right._4;
			result[5] = left._5 & ~right._5;
			result[6] = left._6 & ~right._6;
			result[7] = left._7 & ~right._7;
			result[8] = left._8 & ~right._8;
			result[9] = left._9 & ~right._9;
			result[10] = left._10 & ~right._10;
			result[11] = left._11 & ~right._11;

			return new(result, Len);
		}

		/// <summary>
		/// Simplified calls <see cref="Reduce(int)"/>.
		/// </summary>
		/// <param name="candidates">The candidates.</param>
		/// <param name="digit">The digit.</param>
		/// <returns>The cells.</returns>
		/// <seealso cref="Reduce(int)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Cells operator /(in Candidates candidates, int digit) => candidates.Reduce(digit);


		/// <summary>
		/// Implicit cast from <see cref="int"/>[] to <see cref="Candidates"/>.
		/// </summary>
		/// <param name="array">The array.</param>
		public static implicit operator Candidates(int[] array) => new(array);

		/// <summary>
		/// Implicit cast from <see cref="Candidates"/> to <see cref="Span{T}"/>.
		/// </summary>
		/// <param name="map">The map.</param>
		public static implicit operator Span<int>(in Candidates map) => map.ToSpan();

		/// <summary>
		/// Implicit cast from <see cref="Candidates"/> to <see cref="ReadOnlySpan{T}"/>.
		/// </summary>
		/// <param name="map">The map.</param>
		public static implicit operator ReadOnlySpan<int>(in Candidates map) => map.ToReadOnlySpan();

		/// <summary>
		/// Explicit cast from <see cref="Candidates"/> to <see cref="int"/>[].
		/// </summary>
		/// <param name="map">The map.</param>
		public static explicit operator int[](in Candidates map) => map.ToArray();
	}
}
