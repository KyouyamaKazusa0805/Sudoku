#pragma warning disable IDE0032, IDE0064
namespace Sudoku.Concepts;

/// <summary>
/// Encapsulates a binary series of candidate status table.
/// </summary>
/// <remarks>
/// <para>
/// This type holds a <see langword="static readonly"/> field called <see cref="Empty"/>,
/// it is the only field provided to be used as the entry to create or update collection.
/// If you want to add elements into it, you can use <see cref="Add(int)"/>, <see cref="AddRange(IEnumerable{int})"/>
/// or just <see cref="op_Addition(in CandidateMap, int)"/> or <see cref="op_Addition(in CandidateMap, IEnumerable{int})"/>:
/// <code><![CDATA[
/// var candidateMap = CandidateMap.Empty;
/// candidateMap += 0; // Adds 'r1c1(1)' into the collection.
/// candidateMap.Add(1); // Adds 'r1c1(2)' into the collection.
/// candidateMap.AddRange(stackalloc[] { 2, 3, 4 }); // Adds 'r1c1(345)' into the collection.
/// candidateMap |= anotherMap; // Adds a list of another instance of type 'CandidateMap' into the current collection.
/// ]]></code>
/// </para>
/// <para>
/// <include file="../../global-doc-comments.xml" path="/g/large-structure"/>
/// </para>
/// </remarks>
[JsonConverter(typeof(Converter))]
public unsafe partial struct CandidateMap :
	IAdditionOperators<CandidateMap, Candidate, CandidateMap>,
	IAdditionOperators<CandidateMap, IEnumerable<Candidate>, CandidateMap>,
	IDivisionOperators<CandidateMap, Digit, CellMap>,
	ISubtractionOperators<CandidateMap, Candidate, CandidateMap>,
	ISubtractionOperators<CandidateMap, IEnumerable<Candidate>, CandidateMap>,
	IBitStatusMap<CandidateMap>
{
	/// <inheritdoc cref="IBitStatusMap{T}.Empty"/>
	public static readonly CandidateMap Empty;

	/// <inheritdoc cref="IMinMaxValue{TSelf}.MaxValue"/>
	public static readonly CandidateMap MaxValue = ~default(CandidateMap);

	/// <inheritdoc cref="IMinMaxValue{TSelf}.MinValue"/>
	/// <remarks>
	/// This value is equivalent to <see cref="Empty"/>.
	/// </remarks>
	public static readonly CandidateMap MinValue;


	/// <summary>
	/// The background field of the property <see cref="Count"/>.
	/// </summary>
	/// <remarks><b><i>This field is explicitly declared on purpose. Please don't use auto property.</i></b></remarks>
	/// <seealso cref="Count"/>
	private int _count;

	/// <summary>
	/// Indicates the internal bits. 12 is for floor(729 / <see langword="sizeof"/>(<see cref="long"/>) <![CDATA[<<]]> 6).
	/// </summary>
	/// <seealso cref="IBitStatusMap{TSelf}.Shifting"/>
	private fixed long _bits[12];


	/// <summary>
	/// Initializes a <see cref="CandidateMap"/> instance via a list of candidate offsets
	/// represented as a RxCy notation defined by <see cref="RxCyNotation"/>.
	/// </summary>
	/// <param name="segments">The candidate offsets, represented as a RxCy notation.</param>
	/// <seealso cref="RxCyNotation"/>
	[DebuggerHidden]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[JsonConstructor]
	[Obsolete(RequiresJsonSerializerDynamicInvocationMessage.DynamicInvocationByJsonSerializerOnly, true)]
	[RequiresUnreferencedCode(RequiresJsonSerializerDynamicInvocationMessage.DynamicInvocationByJsonSerializerOnly)]
	public CandidateMap(string[] segments)
	{
		this = Empty;
		foreach (var segment in segments)
		{
			this |= RxCyNotation.ParseCandidates(segment);
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
		(this, var cell, var digit) = (default, candidate / 9, candidate % 9);

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
			return this switch
			{
				{ _count: 0 } => Array.Empty<string>(),
				[var a] => new[] { RxCyNotation.ToCandidateString(a) },
				_ => f(Offsets)
			};


			static string[] f(int[] offsets)
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

					sb.Append(RxCyNotation.ToCellsString(cells));
					sb.Append('(');
					sb.Append(digitGroup.Key + 1);
					sb.Append(')');

					list.Add(sb.ToStringAndClear());
				}

				return list.ToArray();
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
	readonly int IBitStatusMap<CandidateMap>.Shifting => sizeof(long) << 3;

	/// <inheritdoc/>
	readonly Candidate[] IBitStatusMap<CandidateMap>.Offsets => Offsets;

	/// <summary>
	/// Indicates the cell offsets in this collection.
	/// </summary>
	private readonly Candidate[] Offsets
	{
		get
		{
			if (!this)
			{
				return Array.Empty<Candidate>();
			}

			var arr = new Candidate[_count];
			var pos = 0;
			for (var i = 0; i < 729; i++)
			{
				if ((_bits[i >> 6] >> (i & 63) & 1) != 0)
				{
					arr[pos++] = i;
				}
			}

			return arr;
		}
	}

	/// <inheritdoc/>
	static CandidateMap IBitStatusMap<CandidateMap>.Empty => Empty;

	/// <inheritdoc/>
	static CandidateMap IMinMaxValue<CandidateMap>.MaxValue => MaxValue;

	/// <inheritdoc/>
	static CandidateMap IMinMaxValue<CandidateMap>.MinValue => MinValue;


	/// <inheritdoc/>
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
				if ((_bits[i >> 6] >> (i & 63) & 1) != 0)
				{
					if (pos++ == index)
					{
						return i;
					}
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

		var target = Offsets;
		fixed (Candidate* pTarget = target)
		{
			CopyBlock(arr, pTarget, (uint)(sizeof(Candidate) * length));
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Contains(Candidate offset) => (_bits[offset >> 6] >> (offset & 63) & 1) != 0;

	[GeneratedOverridingMember(GeneratedEqualsBehavior.TypeCheckingAndCallingOverloading)]
	public override partial bool Equals(object? obj);

	/// <inheritdoc/>
	public readonly bool Equals(scoped in CandidateMap other)
	{
		for (var i = 0; i < 12; i++)
		{
			if (_bits[i] != other._bits[i])
			{
				return false;
			}
		}

		return true;
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
	public override readonly int GetHashCode()
	{
		var hashCode = new HashCode();
		for (var i = 0; i < 12; i++)
		{
			if ((i & 1) != 0)
			{
				hashCode.Add(_bits[i]);
			}
			else
			{
				var bitBlock = _bits[i];
				hashCode.Add(bitBlock >> 32 | bitBlock & int.MaxValue << 32);
			}
		}

		return hashCode.ToHashCode();
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Candidate[] ToArray() => Offsets;

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly string ToString() => RxCyNotation.ToCandidatesString(this);

	/// <summary>
	/// Gets <see cref="string"/> representation of the current <see cref="CandidateMap"/> instance, using pre-defined formatters.
	/// </summary>
	/// <param name="candidateMapFormatter">
	/// The <see cref="CandidateMap"/> formatter instance to format the current instance.
	/// </param>
	/// <returns>The <see cref="string"/> result.</returns>
	public readonly string ToString(ICandidateMapFormatter candidateMapFormatter) => candidateMapFormatter.ToString(this);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(string? format, IFormatProvider? formatProvider)
		=> (format, formatProvider) switch
		{
			(null, null) => ToString(RxCyFormat.Default),
			//(not null, _) => ToString(format),
			(_, ICandidateMapFormatter formatter) => formatter.ToString(this),
			(_, CultureInfo { Name: ['Z' or 'z', 'H' or 'h', ..] }) => ToString(K9Format.Default),
			_ => ToString(RxCyFormat.Default)
		};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly OneDimensionalArrayEnumerator<Candidate> GetEnumerator() => Offsets.EnumerateImmutable();

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
	public void Add(Candidate offset)
	{
		scoped ref var v = ref _bits[offset >> 6];
		var older = Contains(offset);
		v |= 1L << (offset & 63);
		if (!older)
		{
			_count++;
		}
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
	public void Remove(Candidate offset)
	{
		scoped ref var v = ref _bits[offset >> 6];
		var older = Contains(offset);
		v &= ~(1L << (offset & 63));
		if (older)
		{
			_count--;
		}
	}

	/// <inheritdoc/>
	readonly void IBitStatusMap<CandidateMap>.AddRangeChecked(IEnumerable<Candidate> offsets)
	{
		foreach (var cell in offsets)
		{
			((IBitStatusMap<CandidateMap>)this).AddChecked(cell);
		}
	}

	/// <inheritdoc/>
	readonly void IBitStatusMap<CandidateMap>.RemoveRangeChecked(IEnumerable<Candidate> offsets)
	{
		foreach (var cell in offsets)
		{
			((IBitStatusMap<CandidateMap>)this).RemoveChecked(cell);
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly string ISimpleFormattable.ToString(string? format) => ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void IBitStatusMap<CandidateMap>.AddChecked(Candidate offset)
		=> Add(offset is >= 0 and < 729 ? offset : throw new ArgumentOutOfRangeException(nameof(offset), "The candidate offset is invalid."));

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void IBitStatusMap<CandidateMap>.RemoveChecked(Candidate offset)
		=> Remove(offset is >= 0 and < 729 ? offset : throw new ArgumentOutOfRangeException(nameof(offset), "The candidate offset is invalid."));

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void IBitStatusMap<CandidateMap>.ExceptWith(IEnumerable<Candidate> other) => this -= other;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void IBitStatusMap<CandidateMap>.IntersectWith(IEnumerable<Candidate> other) => this &= Empty + other;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void IBitStatusMap<CandidateMap>.SymmetricExceptWith(IEnumerable<Candidate> other) => this = (this - other) | (Empty + other - this);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void IBitStatusMap<CandidateMap>.UnionWith(IEnumerable<Candidate> other) => this += other;


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
			SkipInit(out result);
			return false;
		}
	}

	/// <inheritdoc/>
	public static CandidateMap Parse(string str) => (CandidateMap)RxCyNotation.ParseCandidates(str);

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
		SkipInit(out result);
		return false;
	}


	/// <inheritdoc/>
	public static bool operator !(scoped in CandidateMap offsets) => offsets ? false : true;

	/// <inheritdoc/>
	public static bool operator true(scoped in CandidateMap value) => value._count != 0;

	/// <inheritdoc/>
	public static bool operator false(scoped in CandidateMap value) => value._count == 0;

	/// <inheritdoc/>
	public static CandidateMap operator ~(scoped in CandidateMap offsets)
	{
		var result = offsets;
		result._bits[11] = ~result._bits[11] & 0x1FFFFFF;
		for (var i = 0; i < 11; i++)
		{
			result._bits[i] = ~result._bits[i];
		}

		return result;
	}

	/// <inheritdoc cref="IDivisionOperators{TSelf, TOther, TResult}.op_Division(TSelf, TOther)"/>
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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(scoped in CandidateMap left, scoped in CandidateMap right) => left.Equals(right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(scoped in CandidateMap left, scoped in CandidateMap right) => !left.Equals(right);

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
	public static CandidateMap operator &(scoped in CandidateMap left, scoped in CandidateMap right)
	{
		var copied = left;
		foreach (var pair in new RefEnumerator(ref copied._bits[0], right._bits[0]))
		{
			pair.First &= pair.Second;
		}

		return copied;
	}

	/// <inheritdoc/>
	public static CandidateMap operator |(scoped in CandidateMap left, scoped in CandidateMap right)
	{
		var copied = left;
		foreach (var pair in new RefEnumerator(ref copied._bits[0], right._bits[0]))
		{
			pair.First |= pair.Second;
		}

		return copied;
	}

	/// <inheritdoc/>
	public static CandidateMap operator ^(scoped in CandidateMap left, scoped in CandidateMap right)
	{
		var copied = left;
		foreach (var pair in new RefEnumerator(ref copied._bits[0], right._bits[0]))
		{
			pair.First ^= pair.Second;
		}

		return copied;
	}

	/// <inheritdoc/>
	public static CandidateMap operator -(scoped in CandidateMap left, scoped in CandidateMap right)
	{
		var copied = left;
		foreach (var pair in new RefEnumerator(ref copied._bits[0], right._bits[0]))
		{
			pair.First &= ~pair.Second;
		}

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
	public static CandidateMap[] operator &(scoped in CandidateMap offsets, int subsetSize)
	{
		if (subsetSize == 0 || subsetSize > offsets._count)
		{
			return Array.Empty<CandidateMap>();
		}

		if (subsetSize == offsets._count)
		{
			return new[] { offsets };
		}

		var n = offsets._count;
		var buffer = stackalloc int[subsetSize];
		if (n <= 30 && subsetSize <= 30)
		{
			// Optimization: Use table to get the total number of result elements.
			var totalIndex = 0;
			var result = new CandidateMap[Combinatorial[n - 1, subsetSize - 1]];
			f(subsetSize, n, subsetSize, offsets.Offsets);
			return result;


			void f(int size, int last, int index, Candidate[] offsets)
			{
				for (var i = last; i >= index; i--)
				{
					buffer[index - 1] = i - 1;
					if (index > 1)
					{
						f(size, i - 1, index - 1, offsets);
					}
					else
					{
						var temp = new Candidate[size];
						for (var j = 0; j < size; j++)
						{
							temp[j] = offsets[buffer[j]];
						}

						result[totalIndex++] = (CandidateMap)temp;
					}
				}
			}
		}
		else if (n > 30 && subsetSize > 30)
		{
			throw new NotSupportedException(
				"""
				Both cells count and subset size is too large, which may cause potential out of memory exception. 
				This operator will throw this exception to calculate the result, in order to prevent any possible exceptions thrown.
				""".RemoveLineEndings()
			);
		}
		else
		{
			var result = new List<CandidateMap>();
			f(subsetSize, n, subsetSize, offsets.Offsets);
			return result.ToArray();


			void f(int size, int last, int index, Candidate[] offsets)
			{
				for (var i = last; i >= index; i--)
				{
					buffer[index - 1] = i - 1;
					if (index > 1)
					{
						f(size, i - 1, index - 1, offsets);
					}
					else
					{
						var temp = new Candidate[size];
						for (var j = 0; j < size; j++)
						{
							temp[j] = offsets[buffer[j]];
						}

						result.Add((CandidateMap)temp);
					}
				}
			}
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CellMap IDivisionOperators<CandidateMap, Digit, CellMap>.operator /(CandidateMap left, Digit right) => left / right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CandidateMap IAdditionOperators<CandidateMap, Candidate, CandidateMap>.operator +(CandidateMap left, Candidate right) => left + right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CandidateMap IAdditionOperators<CandidateMap, Candidate, CandidateMap>.operator checked +(CandidateMap left, Candidate right)
	{
		var copied = left;
		((IBitStatusMap<CandidateMap>)copied).AddChecked(right);

		return copied;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CandidateMap IAdditionOperators<CandidateMap, IEnumerable<Candidate>, CandidateMap>.operator +(CandidateMap left, IEnumerable<Candidate> right)
		=> left + right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CandidateMap IAdditionOperators<CandidateMap, IEnumerable<Candidate>, CandidateMap>.operator checked +(CandidateMap left, IEnumerable<Candidate> right)
	{
		var copied = left;
		((IBitStatusMap<CandidateMap>)copied).AddRangeChecked(right);

		return copied;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CandidateMap ISubtractionOperators<CandidateMap, Candidate, CandidateMap>.operator -(CandidateMap left, Candidate right) => left - right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CandidateMap ISubtractionOperators<CandidateMap, Candidate, CandidateMap>.operator checked -(CandidateMap left, Candidate right)
	{
		var copied = left;
		((IBitStatusMap<CandidateMap>)copied).RemoveChecked(right);

		return copied;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CandidateMap ISubtractionOperators<CandidateMap, IEnumerable<Candidate>, CandidateMap>.operator -(CandidateMap left, IEnumerable<Candidate> right)
		=> left - right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CandidateMap ISubtractionOperators<CandidateMap, IEnumerable<Candidate>, CandidateMap>.operator checked -(CandidateMap left, IEnumerable<Candidate> right)
	{
		var copied = left;
		((IBitStatusMap<CandidateMap>)copied).RemoveRangeChecked(right);

		return copied;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CandidateMap IBitStatusMap<CandidateMap>.operator checked +(scoped in CandidateMap collection, Candidate offset)
	{
		var copied = collection;
		((IBitStatusMap<CandidateMap>)copied).AddChecked(offset);

		return copied;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CandidateMap IBitStatusMap<CandidateMap>.operator checked +(scoped in CandidateMap collection, IEnumerable<Candidate> offsets)
	{
		var copied = collection;
		((IBitStatusMap<CandidateMap>)copied).AddRangeChecked(offsets);

		return copied;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CandidateMap IBitStatusMap<CandidateMap>.operator checked -(scoped in CandidateMap collection, Candidate offset)
	{
		var copied = collection;
		((IBitStatusMap<CandidateMap>)copied).RemoveChecked(offset);

		return copied;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CandidateMap IBitStatusMap<CandidateMap>.operator checked -(scoped in CandidateMap collection, IEnumerable<Candidate> offsets)
	{
		var copied = collection;
		((IBitStatusMap<CandidateMap>)copied).RemoveRangeChecked(offsets);

		return copied;
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Candidate[](scoped in CandidateMap offsets) => offsets.Offsets;

	/// <inheritdoc/>
	public static implicit operator CandidateMap(scoped Span<Candidate> offsets)
	{
		var result = Empty;
		foreach (var offset in offsets)
		{
			result.Add(offset);
		}

		return result;
	}

	/// <inheritdoc/>
	public static implicit operator CandidateMap(scoped ReadOnlySpan<Candidate> offsets)
	{
		var result = Empty;
		foreach (var offset in offsets)
		{
			result.Add(offset);
		}

		return result;
	}

	/// <inheritdoc/>
	public static explicit operator CandidateMap(Candidate[] offsets)
	{
		var result = Empty;
		foreach (var offset in offsets)
		{
			result.Add(offset);
		}

		return result;
	}

	/// <inheritdoc/>
	public static explicit operator checked CandidateMap(Candidate[] offsets)
	{
		var result = Empty;
		foreach (var offset in offsets)
		{
			((IBitStatusMap<CandidateMap>)result).AddChecked(offset);
		}

		return result;
	}

	/// <inheritdoc/>
	static implicit IBitStatusMap<CandidateMap>.operator CandidateMap(scoped Span<Candidate> offsets)
	{
		var result = Empty;
		foreach (var offset in offsets)
		{
			result.Add(offset);
		}

		return result;
	}

	/// <inheritdoc/>
	static implicit IBitStatusMap<CandidateMap>.operator CandidateMap(scoped ReadOnlySpan<Candidate> offsets)
	{
		var result = Empty;
		foreach (var offset in offsets)
		{
			result.Add(offset);
		}

		return result;
	}

	/// <inheritdoc/>
	static explicit IBitStatusMap<CandidateMap>.operator CandidateMap(Candidate[] offsets)
	{
		var result = Empty;
		foreach (var offset in offsets)
		{
			result.Add(offset);
		}

		return result;
	}

	/// <inheritdoc/>
	static explicit IBitStatusMap<CandidateMap>.operator checked CandidateMap(Candidate[] offsets)
	{
		var result = Empty;
		foreach (var offset in offsets)
		{
			((IBitStatusMap<CandidateMap>)result).AddChecked(offset);
		}

		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static explicit IBitStatusMap<CandidateMap>.operator Span<Candidate>(scoped in CandidateMap offsets) => offsets.Offsets;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static explicit IBitStatusMap<CandidateMap>.operator ReadOnlySpan<Candidate>(scoped in CandidateMap offsets) => offsets.Offsets;
}

/// <summary>
/// A reference pair.
/// </summary>
file readonly ref struct RefPair
{
	/// <summary>
	/// The first reference.
	/// </summary>
	public readonly ref long First;

	/// <summary>
	/// The second reference.
	/// </summary>
	public readonly ref readonly long Second;


	/// <summary>
	/// Initializes a <see cref="RefPair"/> instance.
	/// </summary>
	public RefPair(ref long first, in long second)
	{
		First = ref first;
		Second = ref second;
	}
}

/// <summary>
/// Defines an enumerator that iterates the one-dimensional array.
/// </summary>
/// <param name="first">The first reference.</param>
/// <param name="second">The second reference.</param>
file ref struct RefEnumerator([UnscopedRef] ref long first, [UnscopedRef] in long second)
{
	/// <summary>
	/// Indicates the first reference.
	/// </summary>
	private readonly ref long _refFirst = ref first;

	/// <summary>
	/// Indicates the second reference.
	/// </summary>
	private readonly ref readonly long _refSecond = ref second;

	/// <summary>
	/// Indicates the current index being iterated.
	/// </summary>
	private int _index = -1;


	/// <summary>
	/// Indicates the current instance being iterated. Please note that the value is returned by reference.
	/// </summary>
	public readonly RefPair Current
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new(ref AddByteOffset(ref _refFirst, _index), AddByteOffset(ref AsRef(_refSecond), _index));
	}


	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	public readonly RefEnumerator GetEnumerator() => this;

	/// <summary>
	/// Retrieve the iterator to make it points to the next element.
	/// </summary>
	/// <returns>
	/// A <see cref="bool"/> value indicating whether the moving operation is successful.
	/// Returns <see langword="false"/> when the last iteration is for the last element,
	/// and now there's no elements to be iterated.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool MoveNext() => ++_index < 12;
}

/// <summary>
/// Indicates the JSON converter of the current type.
/// </summary>
file sealed class Converter : JsonConverter<CandidateMap>
{
	/// <inheritdoc/>
	public override bool HandleNull => false;


	/// <inheritdoc/>
	public override CandidateMap Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var result = CandidateMap.Empty;
		var parts = Deserialize<string[]>(ref reader, options) ?? throw new JsonException("Unexpected token type.");
		foreach (var part in parts)
		{
			result |= RxCyNotation.ParseCandidates(part);
		}

		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override void Write(Utf8JsonWriter writer, CandidateMap value, JsonSerializerOptions options)
		=> writer.WriteArray(value.StringChunks, options);
}
