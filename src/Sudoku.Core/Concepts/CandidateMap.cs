using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.SourceGeneration;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Sudoku.Concepts.Converters;
using Sudoku.Concepts.Parsers;
using Sudoku.Concepts.Primitive;
using Sudoku.Linq;
using static System.Numerics.BitOperations;
using static Sudoku.SolutionWideReadOnlyFields;

namespace Sudoku.Concepts;

/// <summary>
/// Encapsulates a binary series of candidate state table.
/// The internal buffer size 12 is equivalent to expression <c><![CDATA[floor(729 / sizeof(long) << 6)]]></c>.
/// </summary>
/// <remarks>
/// <para>
/// This type holds a <see langword="static readonly"/> field called <see cref="Empty"/>,
/// it is the only field provided to be used as the entry to create or update collection.
/// If you want to add elements into it, you can use <see cref="Add(Candidate)"/>, <see cref="AddRange(IEnumerable{Candidate})"/>
/// or just <see cref="op_Addition(in CandidateMap, Candidate)"/> or <see cref="op_Addition(in CandidateMap, IEnumerable{Candidate})"/>:
/// <code><![CDATA[
/// var map = CandidateMap.Empty;
/// map += 0; // Adds 'r1c1(1)' into the collection.
/// map.Add(1); // Adds 'r1c1(2)' into the collection.
/// map.AddRange([2, 3, 4]); // Adds 'r1c1(345)' into the collection.
/// map |= anotherMap; // Adds a list of another instance of type 'CandidateMap' into the current collection.
/// ]]></code>
/// </para>
/// <para>
/// <include file="../../global-doc-comments.xml" path="/g/large-structure"/>
/// </para>
/// </remarks>
[JsonConverter(typeof(Converter))]
[StructLayout(LayoutKind.Auto)]
[CollectionBuilder(typeof(CandidateMap), nameof(Create))]
[InlineArrayField<long>("_bits", 12)]
[LargeStructure]
[Equals]
[EqualityOperators]
public partial struct CandidateMap :
	IAdditionOperators<CandidateMap, Candidate, CandidateMap>,
	IAdditionOperators<CandidateMap, IEnumerable<Candidate>, CandidateMap>,
	ICoordinateObject<CandidateMap>,
	IDivisionOperators<CandidateMap, Digit, CellMap>,
	ISubtractionOperators<CandidateMap, Candidate, CandidateMap>,
	ISubtractionOperators<CandidateMap, IEnumerable<Candidate>, CandidateMap>,
	IBitStatusMap<CandidateMap, Candidate>
{
	/// <inheritdoc cref="IBitStatusMap{T, TElement}.Empty"/>
	public static readonly CandidateMap Empty;

	/// <inheritdoc cref="IMinMaxValue{TSelf}.MaxValue"/>
	public static readonly CandidateMap MaxValue = ~default(CandidateMap);

	/// <inheritdoc cref="IMinMaxValue{TSelf}.MinValue"/>
	/// <remarks>
	/// This value is equivalent to <see cref="Empty"/>.
	/// </remarks>
	public static readonly CandidateMap MinValue;


	/// <summary>
	/// Initializes a <see cref="CandidateMap"/> instance via a list of candidate offsets represented as a RxCy notation.
	/// </summary>
	/// <param name="segments">The candidate offsets, represented as a RxCy notation.</param>
	[JsonConstructor]
	public CandidateMap(string[] segments)
	{
		this = Empty;
		foreach (var segment in segments)
		{
			this |= ParseExact(segment, new RxCyParser());
		}
	}

	/// <summary>
	/// Indicates a <see cref="CandidateMap"/> instance with the peer candidates of the specified candidate and a <see cref="bool"/>
	/// value indicating whether the map will process itself with <see langword="true"/> value.
	/// </summary>
	/// <param name="candidate">The candidate.</param>
	/// <param name="withItself">Indicates whether the map will process itself with <see langword="true"/> value.</param>
	private CandidateMap(Candidate candidate, bool withItself)
	{
		(this, var cell, var digit) = (Empty, candidate / 9, candidate % 9);
		foreach (var c in PeersMap[cell])
		{
			Add(c * 9 + digit);
		}
		for (var d = 0; d < 9; d++)
		{
			if (d != digit || d == digit && withItself)
			{
				Add(cell * 9 + d);
			}
		}
	}


	/// <inheritdoc/>
	[ImplicitField(RequiredReadOnlyModifier = false)]
	public readonly int Count
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _count;
	}

	/// <inheritdoc/>
	[JsonInclude]
	public readonly string[] StringChunks
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this switch { { _count: 0 } => [], [var a] => [new RxCyConverter().CandidateConverter([a])], _ => f(Offsets) };


			static string[] f(Candidate[] offsets)
			{
				var list = new List<string>();
				foreach (var digitGroup in
					from candidate in offsets
					group candidate by candidate % 9 into digitGroups
					orderby digitGroups.Key
					select digitGroups)
				{
					scoped var sb = new StringHandler(50);
					var cells = CellMap.Empty;
					foreach (var candidate in digitGroup)
					{
						cells.Add(candidate / 9);
					}

					sb.Append(new RxCyConverter().CellConverter(in cells));
					sb.Append('(');
					sb.Append(digitGroup.Key + 1);
					sb.Append(')');

					list.Add(sb.ToStringAndClear());
				}

				return [.. list];
			}
		}
	}

	/// <inheritdoc/>
	public readonly CandidateMap PeerIntersection
	{
		get
		{
			if (_count == 0)
			{
				// Empty list can't contain any peer intersections.
				return Empty;
			}

			var result = ~Empty;
			foreach (var candidate in Offsets)
			{
				result &= new CandidateMap(candidate, false);
			}

			return result;
		}
	}

	/// <inheritdoc/>
	readonly int IBitStatusMap<CandidateMap, Candidate>.Shifting => sizeof(long) << 3;

	/// <inheritdoc/>
	readonly Candidate[] IBitStatusMap<CandidateMap, Candidate>.Offsets => Offsets;

	/// <summary>
	/// Indicates the cell offsets in this collection.
	/// </summary>
	private readonly Candidate[] Offsets
	{
		get
		{
			if (!this)
			{
				return [];
			}

			var arr = new Candidate[_count];
			var pos = 0;
			for (var i = 0; i < 729; i++)
			{
				if (Contains(i))
				{
					arr[pos++] = i;
				}
			}

			return arr;
		}
	}

	/// <inheritdoc/>
	static Candidate IBitStatusMap<CandidateMap, Candidate>.MaxCount => 9 * 9 * 9;

	/// <inheritdoc/>
	static CandidateMap IBitStatusMap<CandidateMap, Candidate>.Empty => Empty;

	/// <inheritdoc/>
	static CandidateMap IMinMaxValue<CandidateMap>.MaxValue => MaxValue;

	/// <inheritdoc/>
	static CandidateMap IMinMaxValue<CandidateMap>.MinValue => MinValue;


	/// <inheritdoc/>
	[IndexerName("CandidateIndex")]
	public readonly Candidate this[int index]
	{
		get
		{
			if (!this)
			{
				return -1;
			}

			var pos = 0;
			for (var i = 0; i < 729; i++)
			{
				if (Contains(i) && pos++ == index)
				{
					return i;
				}
			}

			return -1;
		}
	}


	/// <inheritdoc/>
	public readonly unsafe void CopyTo(Candidate* arr, int length)
	{
		if (length < 729)
		{
			return;
		}

		Unsafe.CopyBlock(
			ref Unsafe.As<Candidate, byte>(ref arr[0]),
			in Unsafe.As<Candidate, byte>(ref Offsets[0]),
			(uint)(sizeof(Candidate) * length)
		);
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Contains(Candidate offset) => (_bits[offset >> 6] >> (offset & 63) & 1) != 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[ExplicitInterfaceImpl(typeof(IEquatable<>))]
	public readonly bool Equals(scoped ref readonly CandidateMap other) => _bits == other._bits;

	/// <inheritdoc/>
	public readonly void ForEach(Action<Candidate> action)
	{
		foreach (var element in this)
		{
			action(element);
		}
	}

	/// <inheritdoc cref="object.GetHashCode"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly int GetHashCode() => _bits.GetHashCode();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Candidate[] ToArray() => Offsets;

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly string ToString() => ToString(new RxCyConverter());

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(CoordinateConverter converter) => converter.CandidateConverter(in this);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly OneDimensionalArrayEnumerator<Candidate> GetEnumerator() => Offsets.Enumerate();

	/// <inheritdoc/>
	public readonly CandidateMap Slice(int start, int count)
	{
		var result = Empty;
		var offsets = Offsets;
		for (int i = start, end = start + count; i < end; i++)
		{
			result.Add(offsets[i]);
		}

		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly unsafe CandidateMap[] GetSubsets(int subsetSize)
	{
		if (subsetSize == 0 || subsetSize > _count)
		{
			return [];
		}

		if (subsetSize == _count)
		{
			return [this];
		}

		var n = _count;
		var buffer = stackalloc int[subsetSize];
		if (n <= 30 && subsetSize <= 30)
		{
			// Optimization: Use table to get the total number of result elements.
			var totalIndex = 0;
			var result = new CandidateMap[PascalTriangle[n - 1][subsetSize - 1]];
			enumerateWithLimit(subsetSize, n, subsetSize, Offsets);
			return result;


			void enumerateWithLimit(int size, int last, int index, Candidate[] offsets)
			{
				for (var i = last; i >= index; i--)
				{
					buffer[index - 1] = i - 1;
					if (index > 1)
					{
						enumerateWithLimit(size, i - 1, index - 1, offsets);
					}
					else
					{
						var temp = new Candidate[size];
						for (var j = 0; j < size; j++)
						{
							temp[j] = offsets[buffer[j]];
						}

						result[totalIndex++] = [.. temp];
					}
				}
			}
		}
		else
		{
			if (n > 30 && subsetSize > 30)
			{
				throw new NotSupportedException(IBitStatusMap<CandidateMap, Candidate>.ErrorInfo_SubsetsExceeded);
			}
			var result = new List<CandidateMap>();
			enumerateWithoutLimit(subsetSize, n, subsetSize, Offsets);
			return [.. result];


			void enumerateWithoutLimit(int size, int last, int index, Candidate[] offsets)
			{
				for (var i = last; i >= index; i--)
				{
					buffer[index - 1] = i - 1;
					if (index > 1)
					{
						enumerateWithoutLimit(size, i - 1, index - 1, offsets);
					}
					else
					{
						var temp = new Candidate[size];
						for (var j = 0; j < size; j++)
						{
							temp[j] = offsets[buffer[j]];
						}

						result.Add([.. temp]);
					}
				}
			}
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly CandidateMap[] GetAllSubsets() => GetAllSubsets(_count);

	/// <inheritdoc/>
	public readonly CandidateMap[] GetAllSubsets(int limitSubsetSize)
	{
		if (limitSubsetSize == 0 || !this)
		{
			return [];
		}

		var (n, desiredSize) = (_count, 0);
		var length = Math.Min(n, limitSubsetSize);
		for (var i = 1; i <= length; i++)
		{
			desiredSize += PascalTriangle[n - 1][i - 1];
		}

		var result = new List<CandidateMap>(desiredSize);
		for (var i = 1; i <= length; i++)
		{
			result.AddRange(GetSubsets(i));
		}

		return [.. result];
	}

	/// <inheritdoc/>
	public readonly ReadOnlySpan<TResult> Select<TResult>(Func<Candidate, TResult> selector)
	{
		var offsets = Offsets;
		var result = new TResult[offsets.Length];
		for (var i = 0; i < offsets.Length; i++)
		{
			result[i] = selector(offsets[i]);
		}

		return result;
	}

	/// <inheritdoc/>
	public readonly CandidateMap Where(Func<Candidate, bool> predicate)
	{
		var result = this;
		foreach (var cell in Offsets)
		{
			if (!predicate(cell))
			{
				result.Remove(cell);
			}
		}

		return result;
	}

	/// <inheritdoc/>
	public readonly ReadOnlySpan<BitStatusMapGroup<CandidateMap, Candidate, TKey>> GroupBy<TKey>(Func<Candidate, TKey> keySelector)
		where TKey : notnull
	{
		var dictionary = new Dictionary<TKey, CandidateMap>();
		foreach (var candidate in this)
		{
			var key = keySelector(candidate);
			if (!dictionary.TryAdd(key, [candidate]))
			{
				var originalElement = dictionary[key];
				originalElement.Add(candidate);
				dictionary[key] = originalElement;
			}
		}

		var result = new BitStatusMapGroup<CandidateMap, Candidate, TKey>[dictionary.Count];
		var i = 0;
		foreach (var (key, value) in dictionary)
		{
			result[i++] = new(key, in value);
		}

		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Add(Candidate offset)
	{
		scoped ref var v = ref _bits[offset >> 6];
		var older = Contains(offset);
		v |= 1L << (offset & 63);
		if (!older)
		{
			_count++;
			return true;
		}

		return false;
	}

	/// <inheritdoc/>
	public void AddRange(IEnumerable<Candidate> offsets)
	{
		foreach (var element in offsets)
		{
			Add(element);
		}
	}

	/// <inheritdoc/>
	public void RemoveRange(IEnumerable<Candidate> offsets)
	{
		foreach (var element in offsets)
		{
			Remove(element);
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Clear() => this = default;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Remove(Candidate offset)
	{
		scoped ref var v = ref _bits[offset >> 6];
		var older = Contains(offset);
		v &= ~(1L << (offset & 63));
		if (older)
		{
			_count--;
			return true;
		}

		return false;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void IBitStatusMap<CandidateMap, Candidate>.ExceptWith(IEnumerable<Candidate> other) => this -= other;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void IBitStatusMap<CandidateMap, Candidate>.IntersectWith(IEnumerable<Candidate> other) => this &= Empty + other;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void IBitStatusMap<CandidateMap, Candidate>.SymmetricExceptWith(IEnumerable<Candidate> other)
		=> this = (this - other) | (Empty + other - this);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void IBitStatusMap<CandidateMap, Candidate>.UnionWith(IEnumerable<Candidate> other) => this += other;


	/// <inheritdoc/>
	public static bool TryParse(string str, out CandidateMap result)
	{
		try
		{
			result = Parse(str);
			return true;
		}
		catch (FormatException)
		{
			Unsafe.SkipInit(out result);
			return false;
		}
	}

	/// <summary>
	/// Creates a <see cref="CandidateMap"/> instance via the specified candidates.
	/// </summary>
	/// <param name="candidates">The candidates.</param>
	/// <returns>A <see cref="CandidateMap"/> instance.</returns>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static CandidateMap Create(scoped ReadOnlySpan<Candidate> candidates)
	{
		var result = Empty;
		foreach (var candidate in candidates)
		{
			result.Add(candidate);
		}
		return result;
	}

	/// <inheritdoc/>
	public static CandidateMap Parse(string str)
	{
		foreach (var parser in IBitStatusMap<CandidateMap, Candidate>.Parsers)
		{
			if (parser.CandidateParser(str) is { Count: not 0 } result)
			{
				return result;
			}
		}

		throw new FormatException("The string is invalid to be parsed.");
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CandidateMap ParseExact(string str, CoordinateParser parser) => parser.CandidateParser(str);

	/// <inheritdoc/>
	static bool IParsable<CandidateMap>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out CandidateMap result)
	{
		try
		{
			if (s is null)
			{
				goto ReturnFalse;
			}

			return TryParse(s, out result);
		}
		catch
		{
		}

	ReturnFalse:
		Unsafe.SkipInit(out result);
		return false;
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !(scoped in CandidateMap offsets) => offsets._count == 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator true(scoped in CandidateMap value) => value._count != 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator false(scoped in CandidateMap value) => value._count == 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CandidateMap operator ~(scoped in CandidateMap offsets)
	{
		var result = offsets;
		result._bits[0] = ~result._bits[0];
		result._bits[1] = ~result._bits[1];
		result._bits[2] = ~result._bits[2];
		result._bits[3] = ~result._bits[3];
		result._bits[4] = ~result._bits[4];
		result._bits[5] = ~result._bits[5];
		result._bits[6] = ~result._bits[6];
		result._bits[7] = ~result._bits[7];
		result._bits[8] = ~result._bits[8];
		result._bits[9] = ~result._bits[9];
		result._bits[10] = ~result._bits[10];
		result._bits[11] = ~result._bits[11] & 0x1FFFFFF;

		result._count = 729 - offsets._count;
		return result;
	}

	/// <inheritdoc cref="IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)"/>
	[ExplicitInterfaceImpl(typeof(IDivisionOperators<,,>))]
	public static CellMap operator /(scoped in CandidateMap offsets, Digit digit)
	{
		var result = CellMap.Empty;
		foreach (var element in offsets)
		{
			if (element % 9 == digit)
			{
				result.Add(element / 9);
			}
		}

		return result;
	}

	/// <inheritdoc/>
	public static CandidateMap operator +(scoped in CandidateMap collection, Candidate offset)
	{
		var copied = collection;
		copied.Add(offset);

		return copied;
	}

	/// <inheritdoc cref="op_Addition(in CandidateMap, IEnumerable{Candidate})"/>
	public static CandidateMap operator +(scoped in CandidateMap collection, scoped ValueList<Candidate> offsets)
	{
		var copied = collection;
		foreach (var element in offsets)
		{
			copied.Add(element);
		}

		return copied;
	}

	/// <inheritdoc/>
	public static CandidateMap operator +(scoped in CandidateMap collection, IEnumerable<Candidate> offsets)
	{
		var copied = collection;
		copied.AddRange(offsets);

		return copied;
	}

	/// <inheritdoc/>
	public static CandidateMap operator -(scoped in CandidateMap collection, Candidate offset)
	{
		var copied = collection;
		copied.Remove(offset);

		return copied;
	}

	/// <inheritdoc/>
	public static CandidateMap operator -(scoped in CandidateMap collection, IEnumerable<Candidate> offsets)
	{
		var copied = collection;
		foreach (var element in offsets)
		{
			copied.Remove(element);
		}

		return copied;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CandidateMap operator &(scoped in CandidateMap left, scoped in CandidateMap right)
	{
		var finalCount = 0;
		var copied = left;
		finalCount += PopCount((ulong)(copied._bits[0] &= right._bits[0]));
		finalCount += PopCount((ulong)(copied._bits[1] &= right._bits[1]));
		finalCount += PopCount((ulong)(copied._bits[2] &= right._bits[2]));
		finalCount += PopCount((ulong)(copied._bits[3] &= right._bits[3]));
		finalCount += PopCount((ulong)(copied._bits[4] &= right._bits[4]));
		finalCount += PopCount((ulong)(copied._bits[5] &= right._bits[5]));
		finalCount += PopCount((ulong)(copied._bits[6] &= right._bits[6]));
		finalCount += PopCount((ulong)(copied._bits[7] &= right._bits[7]));
		finalCount += PopCount((ulong)(copied._bits[8] &= right._bits[8]));
		finalCount += PopCount((ulong)(copied._bits[9] &= right._bits[9]));
		finalCount += PopCount((ulong)(copied._bits[10] &= right._bits[10]));
		finalCount += PopCount((ulong)(copied._bits[11] &= right._bits[11]));

		copied._count = finalCount;
		return copied;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CandidateMap operator |(scoped in CandidateMap left, scoped in CandidateMap right)
	{
		var finalCount = 0;
		var copied = left;
		finalCount += PopCount((ulong)(copied._bits[0] |= right._bits[0]));
		finalCount += PopCount((ulong)(copied._bits[1] |= right._bits[1]));
		finalCount += PopCount((ulong)(copied._bits[2] |= right._bits[2]));
		finalCount += PopCount((ulong)(copied._bits[3] |= right._bits[3]));
		finalCount += PopCount((ulong)(copied._bits[4] |= right._bits[4]));
		finalCount += PopCount((ulong)(copied._bits[5] |= right._bits[5]));
		finalCount += PopCount((ulong)(copied._bits[6] |= right._bits[6]));
		finalCount += PopCount((ulong)(copied._bits[7] |= right._bits[7]));
		finalCount += PopCount((ulong)(copied._bits[8] |= right._bits[8]));
		finalCount += PopCount((ulong)(copied._bits[9] |= right._bits[9]));
		finalCount += PopCount((ulong)(copied._bits[10] |= right._bits[10]));
		finalCount += PopCount((ulong)(copied._bits[11] |= right._bits[11]));

		copied._count = finalCount;
		return copied;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CandidateMap operator ^(scoped in CandidateMap left, scoped in CandidateMap right)
	{
		var finalCount = 0;
		var copied = left;
		finalCount += PopCount((ulong)(copied._bits[0] ^= right._bits[0]));
		finalCount += PopCount((ulong)(copied._bits[1] ^= right._bits[1]));
		finalCount += PopCount((ulong)(copied._bits[2] ^= right._bits[2]));
		finalCount += PopCount((ulong)(copied._bits[3] ^= right._bits[3]));
		finalCount += PopCount((ulong)(copied._bits[4] ^= right._bits[4]));
		finalCount += PopCount((ulong)(copied._bits[5] ^= right._bits[5]));
		finalCount += PopCount((ulong)(copied._bits[6] ^= right._bits[6]));
		finalCount += PopCount((ulong)(copied._bits[7] ^= right._bits[7]));
		finalCount += PopCount((ulong)(copied._bits[8] ^= right._bits[8]));
		finalCount += PopCount((ulong)(copied._bits[9] ^= right._bits[9]));
		finalCount += PopCount((ulong)(copied._bits[10] ^= right._bits[10]));
		finalCount += PopCount((ulong)(copied._bits[11] ^= right._bits[11]));

		copied._count = finalCount;
		return copied;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CandidateMap operator -(scoped in CandidateMap left, scoped in CandidateMap right)
	{
		var finalCount = 0;
		var copied = left;
		finalCount += PopCount((ulong)(copied._bits[0] &= ~right._bits[0]));
		finalCount += PopCount((ulong)(copied._bits[1] &= ~right._bits[1]));
		finalCount += PopCount((ulong)(copied._bits[2] &= ~right._bits[2]));
		finalCount += PopCount((ulong)(copied._bits[3] &= ~right._bits[3]));
		finalCount += PopCount((ulong)(copied._bits[4] &= ~right._bits[4]));
		finalCount += PopCount((ulong)(copied._bits[5] &= ~right._bits[5]));
		finalCount += PopCount((ulong)(copied._bits[6] &= ~right._bits[6]));
		finalCount += PopCount((ulong)(copied._bits[7] &= ~right._bits[7]));
		finalCount += PopCount((ulong)(copied._bits[8] &= ~right._bits[8]));
		finalCount += PopCount((ulong)(copied._bits[9] &= ~right._bits[9]));
		finalCount += PopCount((ulong)(copied._bits[10] &= ~right._bits[10]));
		finalCount += PopCount((ulong)(copied._bits[11] &= ~right._bits[11]));

		copied._count = finalCount;
		return copied;
	}

	/// <summary>
	/// Expands the operator to <c><![CDATA[(a & b).PeerIntersection & b]]></c>.
	/// </summary>
	/// <param name="base">The base map.</param>
	/// <param name="template">The template map that the base map to check and cover.</param>
	/// <returns>The result map.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CandidateMap operator %(scoped in CandidateMap @base, scoped in CandidateMap template)
		=> (@base & template).PeerIntersection & template;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CandidateMap IAdditionOperators<CandidateMap, Candidate, CandidateMap>.operator +(CandidateMap left, Candidate right) => left + right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CandidateMap IAdditionOperators<CandidateMap, IEnumerable<Candidate>, CandidateMap>.operator +(CandidateMap left, IEnumerable<Candidate> right)
		=> left + right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CandidateMap ISubtractionOperators<CandidateMap, Candidate, CandidateMap>.operator -(CandidateMap left, Candidate right) => left - right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CandidateMap ISubtractionOperators<CandidateMap, IEnumerable<Candidate>, CandidateMap>.operator -(CandidateMap left, IEnumerable<Candidate> right)
		=> left - right;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator CandidateMap(Candidate[] array) => [.. array];

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator CandidateMap(scoped ReadOnlySpan<Candidate> values) => [.. values];
}

/// <summary>
/// Indicates the JSON converter of the current type.
/// </summary>
file sealed class Converter : JsonConverter<CandidateMap>
{
	/// <inheritdoc/>
	public override bool HandleNull => false;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override CandidateMap Read(scoped ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> new(JsonSerializer.Deserialize<string[]>(ref reader, options)!);

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, CandidateMap value, JsonSerializerOptions options)
	{
		writer.WriteStartArray();
		foreach (var element in value.StringChunks)
		{
			writer.WriteStringValue(element);
		}
		writer.WriteEndArray();
	}
}
