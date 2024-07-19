namespace Sudoku.Snyder;

using static IGridConstants<PencilmarkGrid>;

/// <summary>
/// Represents a sudoku grid that can be only used by users to append pencilmarks.
/// </summary>
[CollectionBuilder(typeof(PencilmarkGrid), nameof(Create))]
[DebuggerDisplay($$"""{{{nameof(ToString)}}("#")}""")]
[DebuggerStepThrough]
[InlineArray(CellsCount)]
[JsonConverter(typeof(Converter))]
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.AllOperators, IsLargeStructure = true)]
public partial struct PencilmarkGrid : IGrid<PencilmarkGrid>
{
	/// <inheritdoc cref="IGridConstants{TSelf}.Undefined"/>
	public static readonly PencilmarkGrid Empty = default;


	/// <inheritdoc cref="IGrid{TSelf}.FirstMaskRef"/>
	private Mask _values;


	/// <inheritdoc/>
	public readonly bool IsEmpty => this == Empty;

	/// <inheritdoc/>
	public readonly bool IsMissingCandidates
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			var thisCandidates = Candidates.AsCandidateMap();
			var convertedCandidates = ((Grid)this).Candidates.AsCandidateMap();
			return thisCandidates == convertedCandidates;
		}
	}

	/// <inheritdoc/>
	public readonly bool IsSolved => ((Grid)this).IsSolved;

	/// <inheritdoc/>
	public readonly SymmetricType Symmetry => ((Grid)this).Symmetry;

	/// <inheritdoc/>
	public readonly Cell GivensCount => ((Grid)this).GivensCount;

	/// <inheritdoc/>
	public readonly Cell ModifiablesCount => ((Grid)this).ModifiablesCount;

	/// <inheritdoc/>
	public readonly Cell EmptiesCount => ((Grid)this).EmptiesCount;

	/// <inheritdoc/>
	public readonly Candidate CandidatesCount
	{
		get
		{
			var count = 0;
			for (var cell = 0; cell < 81; cell++)
			{
				if (GetState(cell) == CellState.Empty)
				{
					count += PopCount((uint)GetCandidates(cell));
				}
			}
			return count;
		}
	}

	/// <inheritdoc/>
	public readonly HouseMask EmptyHouses
	{
		get
		{
			var result = 0;
			for (var (house, valueCells) = (0, ~EmptyCells); house < 27; house++)
			{
				if (valueCells / house == 0)
				{
					result |= 1 << house;
				}
			}
			return result;
		}
	}

	/// <inheritdoc/>
	public readonly HouseMask CompletedHouses
	{
		get
		{
			var emptyCells = EmptyCells;
			var result = 0;
			for (var house = 0; house < 27; house++)
			{
				if (!(HousesMap[house] & emptyCells))
				{
					result |= 1 << house;
				}
			}
			return result;
		}
	}

	/// <inheritdoc/>
	public readonly unsafe CellMap GivenCells => IGridProperties<PencilmarkGrid>.GetMap(in this, &GridPredicates.GivenCells);

	/// <inheritdoc/>
	public readonly unsafe CellMap ModifiableCells => IGridProperties<PencilmarkGrid>.GetMap(in this, &GridPredicates.ModifiableCells);

	/// <inheritdoc/>
	public readonly unsafe CellMap EmptyCells => IGridProperties<PencilmarkGrid>.GetMap(in this, &GridPredicates.EmptyCells);

	/// <inheritdoc/>
	public readonly unsafe CellMap BivalueCells => IGridProperties<PencilmarkGrid>.GetMap(in this, &GridPredicates.BivalueCells);

	/// <inheritdoc/>
	public readonly unsafe ReadOnlySpan<CellMap> CandidatesMap => IGridProperties<PencilmarkGrid>.GetMaps(in this, &GridPredicates.CandidatesMap);

	/// <inheritdoc/>
	public readonly unsafe ReadOnlySpan<CellMap> DigitsMap => IGridProperties<PencilmarkGrid>.GetMaps(in this, &GridPredicates.DigitsMap);

	/// <inheritdoc/>
	public readonly unsafe ReadOnlySpan<CellMap> ValuesMap => IGridProperties<PencilmarkGrid>.GetMaps(in this, &GridPredicates.ValuesMap);

	/// <inheritdoc/>
	public readonly ReadOnlySpan<Candidate> Candidates
	{
		get
		{
			var candidates = new Candidate[CandidatesCount];
			for (var (cell, i) = (0, 0); cell < CellsCount; cell++)
			{
				if (GetState(cell) == CellState.Empty)
				{
					foreach (var digit in GetCandidates(cell))
					{
						candidates[i++] = cell * CellCandidatesCount + digit;
					}
				}
			}
			return candidates;
		}
	}

	/// <inheritdoc/>
	public readonly ReadOnlySpan<Conjugate> ConjugatePairs
	{
		get
		{
			var conjugatePairs = new List<Conjugate>();
			var candidatesMap = CandidatesMap;
			for (var digit = 0; digit < CellCandidatesCount; digit++)
			{
				ref readonly var cellsMap = ref candidatesMap[digit];
				foreach (var houseMap in HousesMap)
				{
					if ((houseMap & cellsMap) is { Count: 2 } temp)
					{
						conjugatePairs.Add(new(in temp, digit));
					}
				}
			}
			return conjugatePairs.AsReadOnlySpan();
		}
	}

	/// <inheritdoc/>
	public readonly PencilmarkGrid ResetGrid => Preserve(GivenCells);

	/// <summary>
	/// Gets the grid where all empty cells are filled with all possible candidates.
	/// </summary>
	public readonly PencilmarkGrid ResetCandidatesGrid
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			var result = this;
			result.ResetCandidates();
			return result;
		}
	}

	/// <inheritdoc/>
	public readonly PencilmarkGrid UnfixedGrid
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			var result = this;
			result.Unfix();
			return result;
		}
	}

	/// <inheritdoc/>
	public readonly PencilmarkGrid FixedGrid
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			var result = this;
			result.Fix();
			return result;
		}
	}


	/// <inheritdoc/>
	readonly bool IGridConstants<PencilmarkGrid>.IsUndefined => IsEmpty;

	/// <inheritdoc/>
	[UnscopedRef]
	readonly ref readonly Mask IGrid<PencilmarkGrid>.FirstMaskRef => ref this[0];


	/// <inheritdoc/>
	static string IGridConstants<PencilmarkGrid>.EmptyString => Grid.EmptyString;

	/// <inheritdoc/>
	static Mask IGridConstants<PencilmarkGrid>.EmptyMask => Grid.EmptyMask;

	/// <inheritdoc/>
	static Mask IGridConstants<PencilmarkGrid>.ModifiableMask => Grid.ModifiableMask;

	/// <inheritdoc/>
	static Mask IGridConstants<PencilmarkGrid>.GivenMask => Grid.GivenMask;

	/// <inheritdoc/>
	static Mask IGridConstants<PencilmarkGrid>.MaxCandidatesMask => Grid.MaxCandidatesMask;

	/// <inheritdoc/>
	static ref readonly PencilmarkGrid IGridConstants<PencilmarkGrid>.Empty => ref Empty;

	/// <inheritdoc/>
	static ref readonly PencilmarkGrid IGridConstants<PencilmarkGrid>.Undefined => ref Empty;


	/// <inheritdoc/>
	public readonly Mask this[ref readonly CellMap cells] => ((Grid)this)[in cells];

	/// <inheritdoc/>
	public readonly Mask this[ref readonly CellMap cells, bool withValueCells, [ConstantExpected] char mergingMethod = '|']
		=> ((Grid)this)[in cells, withValueCells, mergingMethod];

	/// <inheritdoc/>
	[UnscopedRef]
	ref Mask IGridOperations<PencilmarkGrid>.this[Cell cell] => ref this[cell];


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool GetExistence(Cell cell, Digit digit) => (this[cell] >> digit & 1) != 0;

	/// <inheritdoc/>
	public readonly bool ConflictWith(Cell cell, Digit digit)
	{
		foreach (var tempCell in PeersMap[cell])
		{
			if (GetDigit(tempCell) == digit)
			{
				return true;
			}
		}
		return false;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(ref readonly PencilmarkGrid other) => this[..].SequenceEqual(other[..]);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
	{
		var targetString = ToString(format.IsEmpty ? null : format.ToString(), provider);
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
	public readonly bool? Exists(Candidate candidate)
		=> Exists(candidate / CellCandidatesCount, candidate % CellCandidatesCount);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool? Exists(Cell cell, Digit digit) => GetState(cell) == CellState.Empty ? GetExistence(cell, digit) : null;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly int CompareTo(ref readonly PencilmarkGrid other) => ToString("#").CompareTo(other.ToString("#"));

	/// <inheritdoc/>
	public override readonly int GetHashCode() => IsEmpty ? 1 : ToString("#").GetHashCode();

	/// <inheritdoc cref="object.ToString"/>
	public override readonly string ToString() => ToString(default(string));

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(string? format) => ToString(format, null);

	/// <inheritdoc/>
	public readonly string ToString(IFormatProvider? formatProvider)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc/>
	public readonly string ToString(string? format, IFormatProvider? formatProvider)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly CellState GetState(Cell cell) => MaskOperations.MaskToCellState(this[cell]);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Mask GetCandidates(Cell cell) => (Mask)(this[cell] & Grid.MaxCandidatesMask);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Digit GetDigit(Cell cell)
		=> GetState(cell) == CellState.Empty ? -1 : TrailingZeroCount(GetCandidates(this[cell]));

	/// <inheritdoc/>
	public void Apply(Conclusion conclusion) => throw new NotImplementedException();

	/// <inheritdoc/>
	public void Fix() => throw new NotImplementedException();

	/// <inheritdoc/>
	public void Reset() => throw new NotImplementedException();

	/// <inheritdoc/>
	public void SetDigit(Cell cell, Digit digit) => throw new NotImplementedException();

	/// <inheritdoc/>
	public void SetExistence(Cell cell, Digit digit, bool isOn) => throw new NotImplementedException();

	/// <inheritdoc/>
	public void SetMask(Cell cell, Mask mask) => throw new NotImplementedException();

	/// <inheritdoc/>
	public void SetState(Cell cell, CellState state) => throw new NotImplementedException();

	/// <inheritdoc/>
	public void Unfix() => throw new NotImplementedException();

	/// <summary>
	/// Reset the sudoku grid, but only making candidates to be reset to the initial state related to the current grid
	/// from given and modifiable values.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ResetCandidates()
	{
		if (ToString("#") is var p && p.IndexOf(':') is var colonTokenPos and not -1)
		{
			this = Parse(p[..colonTokenPos]);
		}
	}

	/// <inheritdoc/>
	public readonly Digit[] ToArray()
	{
		var result = new Digit[CellsCount];
		for (var i = 0; i < CellsCount; i++)
		{
			// -1..8 -> 0..9
			result[i] = GetDigit(i) + 1;
		}
		return result;
	}

	/// <inheritdoc/>
	public readonly Mask[] ToCandidateMaskArray()
	{
		var result = new Mask[CellsCount];
		for (var cell = 0; cell < CellsCount; cell++)
		{
			result[cell] = (Mask)(this[cell] & Grid.MaxCandidatesMask);
		}
		return result;
	}

	/// <inheritdoc/>
	readonly IEnumerator<Digit> IEnumerable<int>.GetEnumerator()
	{
		for (var cell = 0; cell < 81; cell++)
		{
			yield return GetDigit(cell);
		}
	}

	/// <summary>
	/// Gets a sudoku grid, removing all value digits not appearing in the specified <paramref name="pattern"/>.
	/// </summary>
	/// <param name="pattern">The pattern.</param>
	/// <returns>The result grid.</returns>
	private readonly PencilmarkGrid Preserve(ref readonly CellMap pattern)
	{
		var result = this;
		foreach (var cell in ~pattern)
		{
			result.SetDigit(cell, -1);
		}
		return result;
	}


	/// <inheritdoc cref="Grid.Create(ReadOnlySpan{Mask})"/>
	private static PencilmarkGrid Create(ReadOnlySpan<Mask> values)
	{
		switch (values.Length)
		{
			case 0:
			{
				return Empty;
			}
			case 1:
			{
				var result = Empty;
				var uniformValue = values[0];
				for (var cell = 0; cell < CellsCount; cell++)
				{
					result[cell] = uniformValue;
				}
				return result;
			}
			case CellsCount:
			{
				var result = Empty;
				for (var cell = 0; cell < CellsCount; cell++)
				{
					result[cell] = values[cell];
				}
				return result;
			}
			default:
			{
				throw new InvalidOperationException($"The argument '{nameof(values)}' must contain {CellsCount} elements.");
			}
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParse([NotNullWhen(true)] string? s, [MaybeNullWhen(false)] out PencilmarkGrid result)
		=> TryParse(s, null, out result);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParse(ReadOnlySpan<char> s, [MaybeNullWhen(false)] out PencilmarkGrid result)
		=> TryParse(s, null, out result);

	/// <inheritdoc/>
	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out PencilmarkGrid result)
	{
		try
		{
			if (s is null)
			{
				throw new FormatException();
			}

			result = Parse(s, provider);
			return true;
		}
		catch (FormatException)
		{
			result = Empty;
			return false;
		}
	}

	/// <inheritdoc/>
	public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out PencilmarkGrid result)
	{
		try
		{
			result = Parse(s, provider);
			return true;
		}
		catch (FormatException)
		{
			result = Empty;
			return false;
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PencilmarkGrid Parse(string? s) => s is null ? throw new FormatException() : Parse(s, null);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PencilmarkGrid Parse(ReadOnlySpan<char> s) => Parse(s, null);

	/// <inheritdoc/>
	public static PencilmarkGrid Parse(string s, IFormatProvider? provider)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc/>
	public static PencilmarkGrid Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc/>
	static void IGridProperties<PencilmarkGrid>.OnValueChanged(ref PencilmarkGrid @this, Cell cell, Mask oldMask, Mask newMask, Digit setValue)
		=> OnValueChanged(ref @this, cell, oldMask, newMask, setValue);

	/// <inheritdoc/>
	static void IGridProperties<PencilmarkGrid>.OnRefreshingCandidates(ref PencilmarkGrid @this) => OnRefreshingCandidates(ref @this);

	/// <inheritdoc/>
	static PencilmarkGrid IGrid<PencilmarkGrid>.Create(ReadOnlySpan<Mask> values) => Create(values);

	/// <inheritdoc cref="Grid.OnValueChanged(ref Grid, Cell, Mask, Mask, Digit)"/>
	private static void OnValueChanged(ref PencilmarkGrid @this, Cell cell, Mask oldMask, Mask newMask, Digit setValue)
	{
		if (setValue == -1)
		{
			// This method will do nothing if 'setValue' is -1.
			return;
		}

		foreach (var peerCell in PeersMap[cell])
		{
			if (@this.GetState(peerCell) == CellState.Empty)
			{
				@this[peerCell] &= (Mask)~(1 << setValue);
			}
		}
	}

	/// <inheritdoc cref="Grid.OnRefreshingCandidates(ref Grid)"/>
	private static void OnRefreshingCandidates(ref PencilmarkGrid @this)
	{
		for (var cell = 0; cell < CellsCount; cell++)
		{
			if (@this.GetState(cell) == CellState.Empty)
			{
				// Remove all appeared digits.
				var mask = Grid.MaxCandidatesMask;
				foreach (var currentCell in PeersMap[cell])
				{
					if (@this.GetDigit(currentCell) is var digit and not -1)
					{
						mask &= (Mask)~(1 << digit);
					}
				}
				@this[cell] = (Mask)(Grid.EmptyMask | mask);
			}
		}
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator PencilmarkGrid(Mask[] maskArray)
	{
		var result = Empty;
		Unsafe.CopyBlock(
			ref @ref.ByteRef(ref result[0]),
			in @ref.ReadOnlyByteRef(in maskArray[0]),
			(uint)(sizeof(Mask) * maskArray.Length)
		);
		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator checked PencilmarkGrid(Mask[] maskArray)
	{
		static bool maskMatcher(Mask element) => element >> CellCandidatesCount is 0 or 1 or 2 or 4;
		ArgumentOutOfRangeException.ThrowIfNotEqual(maskArray.Length, CellsCount);
		ArgumentOutOfRangeException.ThrowIfNotEqual(Array.TrueForAll(maskArray, maskMatcher), true);

		var result = Empty;
		Unsafe.CopyBlock(ref @ref.ByteRef(ref result[0]), in @ref.ReadOnlyByteRef(in maskArray[0]), sizeof(Mask) * CellsCount);
		return result;
	}

	/// <summary>
	/// Converts the current instance into a <see cref="Grid"/>.
	/// </summary>
	/// <param name="this">A <see cref="Grid"/> instance converted.</param>
	public static explicit operator Grid(in PencilmarkGrid @this)
	{
		var result = Grid.Empty;
		for (var cell = 0; cell < 81; cell++)
		{
			switch (@this.GetState(cell))
			{
				case CellState.Modifiable:
				{
					result.SetDigit(cell, @this.GetDigit(cell));
					result.SetState(cell, CellState.Modifiable);
					break;
				}
				case CellState.Given:
				{
					result.SetDigit(cell, @this.GetDigit(cell));
					result.SetState(cell, CellState.Given);
					break;
				}
			}
		}
		return result;
	}
}

/// <summary>
/// Indicates the JSON converter of the current type.
/// </summary>
file sealed class Converter : JsonConverter<PencilmarkGrid>
{
	/// <inheritdoc/>
	public override bool HandleNull => true;


	/// <inheritdoc/>
	public override PencilmarkGrid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> reader.GetString() is { } s ? PencilmarkGrid.Parse(s) : PencilmarkGrid.Empty;

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, PencilmarkGrid value, JsonSerializerOptions options)
		=> writer.WriteStringValue(value.ToString("#"));
}
