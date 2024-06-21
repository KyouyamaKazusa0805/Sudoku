namespace Sudoku.Concepts;

using static IBitStatusMap<CandidateMap, Candidate, CandidateMap.Enumerator>;

/// <summary>
/// Encapsulates a binary series of candidate state table.
/// The internal buffer size 12 is equivalent to expression <c><![CDATA[floor(729 / sizeof(long) << 6)]]></c>.
/// </summary>
/// <remarks>
/// <include file="../../global-doc-comments.xml" path="/g/large-structure"/>
/// </remarks>
[JsonConverter(typeof(Converter))]
[StructLayout(LayoutKind.Auto)]
[CollectionBuilder(typeof(CandidateMap), nameof(Create))]
[DebuggerStepThrough]
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.Object_ToString | TypeImplFlag.AllOperators, IsLargeStructure = true)]
public partial struct CandidateMap : IBitStatusMap<CandidateMap, Candidate, CandidateMap.Enumerator>
{
	/// <inheritdoc cref="IBitStatusMap{TSelf, TElement, TEnumerator}.Empty"/>
	public static readonly CandidateMap Empty = [];

	/// <inheritdoc cref="IBitStatusMap{TSelf, TElement, TEnumerator}.Full"/>
	public static readonly CandidateMap Full = ~default(CandidateMap);


	/// <summary>
	/// Indicates the internal field that provides the visit entry for fixed-sized buffer type <see cref="BackingBuffer"/>.
	/// </summary>
	/// <seealso cref="BackingBuffer"/>
	private BackingBuffer _bits;


	/// <summary>
	/// Initializes a <see cref="CandidateMap"/> instance via a list of candidate offsets represented as a RxCy notation.
	/// </summary>
	/// <param name="segments">The candidate offsets, represented as a RxCy notation.</param>
	[JsonConstructor]
	public CandidateMap(string[] segments)
	{
		this = [];
		foreach (var segment in segments)
		{
			this |= Parse(segment, new RxCyParser());
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
		(this, var cell, var digit) = ([], candidate / 9, candidate % 9);
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
	public int Count { get; private set; }

	/// <inheritdoc/>
	[JsonInclude]
	public readonly string[] StringChunks
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this switch
			{
				{ Count: 0 } => [],
				[var a] => [CoordinateConverter.InvariantCultureConverter.CandidateConverter(a)],
				_ => f(Offsets)
			};


			static string[] f(Candidate[] offsets)
			{
				var list = new List<string>();
				foreach (var digitGroup in
					from candidate in offsets
					group candidate by candidate % 9 into digitGroups
					orderby digitGroups.Key
					select digitGroups)
				{
					var sb = new StringBuilder(50);
					var cells = CellMap.Empty;
					foreach (var candidate in digitGroup)
					{
						cells.Add(candidate / 9);
					}

					list.Add(
						sb
							.Append(CoordinateConverter.InvariantCultureConverter.CellConverter(cells))
							.Append($"({digitGroup.Key + 1})")
							.ToString()
					);
				}
				return [.. list];
			}
		}
	}

	/// <summary>
	/// Indicates the digits used in this pattern.
	/// </summary>
	public readonly Mask Digits
	{
		get
		{
			var result = (Mask)0;
			for (var digit = 0; digit < 9; digit++)
			{
				if (this / digit)
				{
					result |= (Mask)(1 << digit);
				}
			}
			return result;
		}
	}

	/// <summary>
	/// Indicates the cells used in this pattern.
	/// </summary>
	public readonly CellMap Cells => [.. CellDistribution.Keys];

	/// <inheritdoc/>
	public readonly CandidateMap PeerIntersection
	{
		get
		{
			if (Count == 0)
			{
				// Empty list can't contain any peer intersections.
				return [];
			}

			var result = Full;
			foreach (var candidate in Offsets)
			{
				result &= new CandidateMap(candidate, false);
			}
			return result;
		}
	}

	/// <summary>
	/// Returns a <see cref="FrozenDictionary{TKey, TValue}"/> that describes the distribution of digits appeared in cells, grouped by digit.
	/// </summary>
	public readonly FrozenDictionary<Digit, CellMap> DigitDistribution
	{
		get
		{
			var dictionary = new Dictionary<Digit, CellMap>(9);
			for (var digit = 0; digit < 9; digit++)
			{
				var map = this / digit;
				if (map)
				{
					dictionary.Add(digit, map);
				}
			}
			return dictionary.ToFrozenDictionary();
		}
	}

	/// <summary>
	/// Returns a <see cref="FrozenDictionary{TKey, TValue}"/> that describes the distribution of digits appeared in cells, grouped by cell.
	/// </summary>
	public readonly FrozenDictionary<Cell, Mask> CellDistribution
	{
		get
		{
			var dictionary = new Dictionary<Cell, Mask>(81);
			foreach (var element in Offsets)
			{
				if (!dictionary.TryAdd(element / 9, (Mask)(1 << element % 9)))
				{
					dictionary[element / 9] |= (Mask)(1 << element % 9);
				}
			}
			return dictionary.ToFrozenDictionary();
		}
	}

	/// <summary>
	/// Indicates the cell offsets in this collection.
	/// </summary>
	internal readonly Candidate[] Offsets
	{
		get
		{
			if (!this)
			{
				return [];
			}

			var arr = new Candidate[Count];
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
	readonly int IBitStatusMap<CandidateMap, Candidate, Enumerator>.Shifting => sizeof(long) << 3;

	/// <inheritdoc/>
	readonly Candidate[] IBitStatusMap<CandidateMap, Candidate, Enumerator>.Offsets => Offsets;


	/// <inheritdoc/>
	static Candidate IBitStatusMap<CandidateMap, Candidate, Enumerator>.MaxCount => 9 * 9 * 9;

	/// <inheritdoc/>
	static CandidateMap IBitStatusMap<CandidateMap, Candidate, Enumerator>.Empty => Empty;

	/// <inheritdoc/>
	static CandidateMap IBitStatusMap<CandidateMap, Candidate, Enumerator>.Full => Full;


	/// <summary>
	/// Get the offset at the specified position index.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <returns>
	/// The offset at the specified position index. If the value is invalid, the return value will be <c>-1</c>.
	/// </returns>
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
	public readonly void CopyTo(ref Candidate sequence, int length)
	{
		@ref.ThrowIfNullRef(in sequence);

		if (length >= 729)
		{
			Unsafe.CopyBlock(
				ref @ref.ByteRef(ref sequence),
				in @ref.ReadOnlyByteRef(in Offsets[0]),
				(uint)(sizeof(Candidate) * length)
			);
		}
	}

	/// <summary>
	/// Determine whether the map contains the specified offset.
	/// </summary>
	/// <param name="item">The offset.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Contains(Candidate item) => (_bits[item >> 6] >> (item & 63) & 1) != 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(ref readonly CandidateMap other) => _bits == other._bits;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
	{
		var targetString = ToString(provider);
		if (destination.Length < targetString.Length)
		{
			goto ReturnFalse;
		}

		if (targetString.TryCopyTo(destination))
		{
			charsWritten = targetString.Length;
			return true;
		}

	ReturnFalse:
		charsWritten = 0;
		return false;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly int CompareTo(ref readonly CandidateMap other)
	{
		return Count > other.Count ? 1 : Count < other.Count ? -1 : -Math.Sign($"{b(in this)}".CompareTo($"{b(in other)}"));


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static string b(ref readonly CandidateMap f) => f.ToString(new BitmapCandidateMapFormatInfo());
	}

	/// <inheritdoc/>
	public readonly int IndexOf(Candidate offset)
	{
		for (var index = 0; index < Count; index++)
		{
			if (this[index] == offset)
			{
				return index;
			}
		}
		return -1;
	}

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

	/// <summary>
	/// Try to get digits that is in the current collection.
	/// </summary>
	/// <param name="cell">The desired cell.</param>
	/// <returns>The digits.</returns>
	public readonly Mask GetDigitsFor(Cell cell)
	{
		var result = (Mask)0;
		for (var (candidate, digit) = (cell * 9, 0); digit < 9; candidate++, digit++)
		{
			if (Contains(candidate))
			{
				result |= (Mask)(1 << digit);
			}
		}
		return result;
	}

	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(IFormatProvider? formatProvider)
		=> formatProvider switch
		{
			CandidateMapFormatInfo i => i.FormatMap(in this),
			_ => CoordinateConverter.GetConverter(formatProvider).CandidateConverter(this)
		};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Enumerator GetEnumerator() => new(Offsets);

	/// <summary>
	/// Try to enumerate cells on each candidates.
	/// </summary>
	/// <returns>The enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly CellEnumerator EnumerateCells() => new(Offsets);

	/// <summary>
	/// Try to enumerate cell and digit value on each candidates.
	/// </summary>
	/// <returns>The enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly CellDigitEnumerator EnumerateCellDigit() => new(Offsets);

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

	/// <summary>
	/// Add a new <see cref="Candidate"/> into the collection.
	/// </summary>
	/// <param name="item">The offset to be added.</param>
	/// <returns>A <see cref="bool"/> value indicating whether the collection has already contained the offset.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Add(Candidate item)
	{
		ref var v = ref _bits[item >> 6];
		var older = Contains(item);
		v |= 1L << (item & 63);
		if (!older)
		{
			Count++;
			return true;
		}
		return false;
	}

	/// <inheritdoc/>
	public int AddRange(scoped ReadOnlySpan<Candidate> offsets)
	{
		var result = 0;
		foreach (var offset in offsets)
		{
			if (Add(offset))
			{
				result++;
			}
		}
		return result;
	}

	/// <summary>
	/// Removes the specified offset from the current collection.
	/// </summary>
	/// <param name="item">An offset to be removed.</param>
	/// <returns>A <see cref="bool"/> value indicating whether the collection has already contained the specified offset.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Remove(Candidate item)
	{
		ref var v = ref _bits[item >> 6];
		var older = Contains(item);
		v &= ~(1L << (item & 63));
		if (older)
		{
			Count--;
			return true;
		}
		return false;
	}

	/// <summary>
	/// Remove all <see cref="Candidate"/> instances that is equal to the argument <paramref name="cell"/>.
	/// </summary>
	/// <param name="cell">The cell to be removed.</param>
	public int RemoveCell(Cell cell)
	{
		var result = 0;
		for (var digit = 0; digit < 9; digit++)
		{
			result += Remove(cell * 9 + digit) ? 1 : 0;
		}
		return result;
	}

	/// <inheritdoc/>
	public int RemoveRange(scoped ReadOnlySpan<Candidate> offsets)
	{
		var result = 0;
		foreach (var offset in offsets)
		{
			if (Remove(offset))
			{
				result++;
			}
		}
		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Toggle(Candidate offset) => _ = Contains(offset) ? Remove(offset) : Add(offset);

	/// <summary>
	/// Remove all elements stored in the current collection, and set the property <see cref="Count"/> to zero.
	/// </summary>
	/// <seealso cref="Count"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Clear() => this = Empty;

	/// <inheritdoc/>
	readonly bool IEquatable<CandidateMap>.Equals(CandidateMap other) => Equals(in other);

	/// <inheritdoc/>
	readonly bool IAnyAllMethod<CandidateMap, Candidate>.Any() => Count != 0;

	/// <inheritdoc/>
	readonly bool IAnyAllMethod<CandidateMap, Candidate>.Any(Func<Candidate, bool> predicate) => this.Any(predicate);

	/// <inheritdoc/>
	readonly bool IAnyAllMethod<CandidateMap, Candidate>.All(Func<Candidate, bool> predicate) => this.All(predicate);

	/// <inheritdoc/>
	readonly string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString(formatProvider as CultureInfo);

	/// <inheritdoc/>
	readonly Candidate IFirstLastMethod<CandidateMap, Candidate>.First() => this[0];

	/// <inheritdoc/>
	readonly Candidate IFirstLastMethod<CandidateMap, Candidate>.First(Func<Candidate, bool> predicate) => this.First(predicate);

	/// <inheritdoc/>
	readonly IEnumerable<Candidate> IWhereMethod<CandidateMap, Candidate>.Where(Func<Candidate, bool> predicate)
		=> this.Where(predicate);

	/// <inheritdoc/>
	readonly IEnumerable<IGrouping<TKey, Candidate>> IGroupByMethod<CandidateMap, Candidate>.GroupBy<TKey>(Func<Candidate, TKey> keySelector)
		=> this.GroupBy(keySelector).ToArray().Select(static element => (IGrouping<TKey, Candidate>)element);

	/// <inheritdoc/>
	readonly IEnumerable<TResult> ISelectMethod<CandidateMap, Candidate>.Select<TResult>(Func<Candidate, TResult> selector)
		=> this.Select(selector).ToArray();


	/// <inheritdoc cref="IParsable{TSelf}.TryParse(string?, IFormatProvider?, out TSelf)"/>
	public static bool TryParse(string str, out CandidateMap result)
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

	/// <inheritdoc/>
	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out CandidateMap result)
	{
		try
		{
			if (s is null)
			{
				result = [];
				return false;
			}

			result = Parse(s, provider);
			return true;
		}
		catch (FormatException)
		{
			result = [];
			return false;
		}
	}

	/// <inheritdoc cref="TryParse(ReadOnlySpan{char}, IFormatProvider?, out CandidateMap)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParse(ReadOnlySpan<char> s, out CandidateMap result) => TryParse(s, null, out result);

	/// <inheritdoc/>
	public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out CandidateMap result)
	{
		try
		{
			result = Parse(s, provider);
			return true;
		}
		catch (FormatException)
		{
			result = [];
			return false;
		}
	}

	/// <summary>
	/// Creates a <see cref="CandidateMap"/> instance via the specified candidates.
	/// </summary>
	/// <param name="candidates">The candidates.</param>
	/// <returns>A <see cref="CandidateMap"/> instance.</returns>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static CandidateMap Create(ReadOnlySpan<Candidate> candidates)
	{
		if (candidates.IsEmpty)
		{
			return default;
		}

		var result = default(CandidateMap);
		foreach (var candidate in candidates)
		{
			result.Add(candidate);
		}
		return result;
	}

	/// <inheritdoc/>
	public static CandidateMap Parse(string str)
	{
		foreach (var parser in
			from element in Enum.GetValues<CoordinateType>()
			let parser = element.GetParser()
			where parser is not null
			select parser)
		{
			if (parser.CandidateParser(str) is { Count: not 0 } result)
			{
				return result;
			}
		}

		throw new FormatException(SR.ExceptionMessage("StringValueInvalidToBeParsed"));
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CandidateMap Parse(string s, IFormatProvider? provider) => CoordinateParser.GetParser(provider).CandidateParser(s);

	/// <inheritdoc cref="Parse(ReadOnlySpan{char}, IFormatProvider?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CandidateMap Parse(ReadOnlySpan<char> s) => Parse(s, null);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CandidateMap Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s.ToString(), provider);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !(in CandidateMap offsets) => offsets.Count == 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator true(in CandidateMap value) => value.Count != 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator false(in CandidateMap value) => value.Count == 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CandidateMap operator ~(in CandidateMap offsets)
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
		result.Count = 729 - offsets.Count;
		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CandidateMap operator +(in CandidateMap collection, Candidate offset)
	{
		var result = collection;
		result.Add(offset);

		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CandidateMap operator -(in CandidateMap collection, Candidate offset)
	{
		var result = collection;
		result.Remove(offset);
		return result;
	}

	/// <summary>
	/// Expands the operator to <c><![CDATA[(a & b).PeerIntersection & b]]></c>.
	/// </summary>
	/// <param name="base">The base map.</param>
	/// <param name="template">The template map that the base map to check and cover.</param>
	/// <returns>The result map.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CandidateMap operator %(in CandidateMap @base, in CandidateMap template) => (@base & template).PeerIntersection & template;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CandidateMap operator &(in CandidateMap left, in CandidateMap right)
	{
		var finalCount = 0;
		var result = left;
		finalCount += PopCount((ulong)(result._bits[0] &= right._bits[0]));
		finalCount += PopCount((ulong)(result._bits[1] &= right._bits[1]));
		finalCount += PopCount((ulong)(result._bits[2] &= right._bits[2]));
		finalCount += PopCount((ulong)(result._bits[3] &= right._bits[3]));
		finalCount += PopCount((ulong)(result._bits[4] &= right._bits[4]));
		finalCount += PopCount((ulong)(result._bits[5] &= right._bits[5]));
		finalCount += PopCount((ulong)(result._bits[6] &= right._bits[6]));
		finalCount += PopCount((ulong)(result._bits[7] &= right._bits[7]));
		finalCount += PopCount((ulong)(result._bits[8] &= right._bits[8]));
		finalCount += PopCount((ulong)(result._bits[9] &= right._bits[9]));
		finalCount += PopCount((ulong)(result._bits[10] &= right._bits[10]));
		finalCount += PopCount((ulong)(result._bits[11] &= right._bits[11]));
		result.Count = finalCount;
		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CandidateMap operator |(in CandidateMap left, in CandidateMap right)
	{
		var finalCount = 0;
		var result = left;
		finalCount += PopCount((ulong)(result._bits[0] |= right._bits[0]));
		finalCount += PopCount((ulong)(result._bits[1] |= right._bits[1]));
		finalCount += PopCount((ulong)(result._bits[2] |= right._bits[2]));
		finalCount += PopCount((ulong)(result._bits[3] |= right._bits[3]));
		finalCount += PopCount((ulong)(result._bits[4] |= right._bits[4]));
		finalCount += PopCount((ulong)(result._bits[5] |= right._bits[5]));
		finalCount += PopCount((ulong)(result._bits[6] |= right._bits[6]));
		finalCount += PopCount((ulong)(result._bits[7] |= right._bits[7]));
		finalCount += PopCount((ulong)(result._bits[8] |= right._bits[8]));
		finalCount += PopCount((ulong)(result._bits[9] |= right._bits[9]));
		finalCount += PopCount((ulong)(result._bits[10] |= right._bits[10]));
		finalCount += PopCount((ulong)(result._bits[11] |= right._bits[11]));
		result.Count = finalCount;
		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CandidateMap operator ^(in CandidateMap left, in CandidateMap right)
	{
		var finalCount = 0;
		var result = left;
		finalCount += PopCount((ulong)(result._bits[0] ^= right._bits[0]));
		finalCount += PopCount((ulong)(result._bits[1] ^= right._bits[1]));
		finalCount += PopCount((ulong)(result._bits[2] ^= right._bits[2]));
		finalCount += PopCount((ulong)(result._bits[3] ^= right._bits[3]));
		finalCount += PopCount((ulong)(result._bits[4] ^= right._bits[4]));
		finalCount += PopCount((ulong)(result._bits[5] ^= right._bits[5]));
		finalCount += PopCount((ulong)(result._bits[6] ^= right._bits[6]));
		finalCount += PopCount((ulong)(result._bits[7] ^= right._bits[7]));
		finalCount += PopCount((ulong)(result._bits[8] ^= right._bits[8]));
		finalCount += PopCount((ulong)(result._bits[9] ^= right._bits[9]));
		finalCount += PopCount((ulong)(result._bits[10] ^= right._bits[10]));
		finalCount += PopCount((ulong)(result._bits[11] ^= right._bits[11]));
		result.Count = finalCount;
		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static unsafe ReadOnlySpan<CandidateMap> operator &(in CandidateMap map, int subsetSize)
	{
		if (subsetSize == 0 || subsetSize > map.Count)
		{
			return [];
		}

		if (subsetSize == map.Count)
		{
			return (CandidateMap[])[map];
		}

		var n = map.Count;
		var buffer = stackalloc int[subsetSize];
		if (n <= MaxLimit && subsetSize <= MaxLimit)
		{
			// Optimization: Use table to get the total number of result elements.
			var totalIndex = 0;
			var result = new CandidateMap[Combinatorial.PascalTriangle[n - 1][subsetSize - 1]];
			enumerate(result, subsetSize, n, subsetSize, map.Offsets, (r, c) => result[totalIndex++] = c.AsCandidateMap());
			return result;
		}
		else
		{
			if (n > MaxLimit && subsetSize > MaxLimit)
			{
				throw new NotSupportedException(SR.ExceptionMessage("SubsetsExceeded"));
			}
			var result = new List<CandidateMap>();
			enumerate(result, subsetSize, n, subsetSize, map.Offsets, (r, c) => result.AddRef(c.AsCandidateMap()));
			return result.AsReadOnlySpan();
		}


		void enumerate<T>(T result, int size, int last, int index, Candidate[] offsets, CollectionAddingHandler<T> addingAction)
		{
			for (var i = last; i >= index; i--)
			{
				buffer[index - 1] = i - 1;
				if (index > 1)
				{
					enumerate(result, size, i - 1, index - 1, offsets, addingAction);
				}
				else
				{
					var temp = new Candidate[size];
					for (var j = 0; j < size; j++)
					{
						temp[j] = offsets[buffer[j]];
					}
					addingAction(result, temp);
				}
			}
		}
	}

	/// <inheritdoc/>
	public static ReadOnlySpan<CandidateMap> operator |(in CandidateMap map, int subsetSize)
	{
		if (subsetSize == 0 || !map)
		{
			return [];
		}

		var (n, desiredSize) = (map.Count, 0);
		var length = Math.Min(n, subsetSize);
		for (var i = 1; i <= length; i++)
		{
			desiredSize += Combinatorial.PascalTriangle[n - 1][i - 1];
		}

		var result = new List<CandidateMap>(desiredSize);
		for (var i = 1; i <= length; i++)
		{
			result.AddRangeRef(map & i);
		}
		return result.AsReadOnlySpan();
	}

	/// <summary>
	/// Reduces the <see cref="CandidateMap"/> instance, only checks for candidates
	/// whose digit is equal to argument <paramref name="digit"/>,
	/// and merge into a <see cref="CellMap"/> value.
	/// </summary>
	/// <param name="candidates">The candidates to be checked.</param>
	/// <param name="digit">The digit to be checked.</param>
	/// <returns>A <see cref="CellMap"/> instance.</returns>
	public static CellMap operator /(in CandidateMap candidates, Digit digit)
	{
		var result = CellMap.Empty;
		foreach (var element in candidates)
		{
			if (element % 9 == digit)
			{
				result.Add(element / 9);
			}
		}
		return result;
	}
}

/// <summary>
/// Indicates the JSON converter of <see cref="CandidateMap"/> instance.
/// </summary>
/// <seealso cref="CandidateMap"/>
file sealed class Converter : JsonConverter<CandidateMap>
{
	/// <inheritdoc/>
	public override bool HandleNull => false;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override CandidateMap Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
