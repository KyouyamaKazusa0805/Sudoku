namespace Sudoku.Concepts;

using GridBase = IGrid<MarkerGrid>;

/// <summary>
/// Represents a sudoku grid that will be used for candidate marking.
/// Different with <see cref="Grid"/>, this type will aim to adding candidates, rather than removing candidates
/// at initial state with full candidates.
/// </summary>
/// <remarks>
/// The backing implementation is nearly same as <see cref="Grid"/>, but the only difference is that <see cref="Grid"/> instances
/// will contain all candidates at the initial state. Candidates can be removed by using indexers to update candidates,
/// or <see cref="Grid.SetCandidates(Cell, Mask)"/> to replace candidate masks with new values;
/// however, <see cref="MarkerGrid"/> instances won't contain any candidates at initial state.
/// All candicates should be manually appended into grid.
/// </remarks>
/// <seealso cref="Grid"/>
/// <seealso cref="Grid.SetCandidates(Cell, Mask)"/>
[CollectionBuilder(typeof(Grid), nameof(Create))]
[DebuggerStepThrough]
[InlineArray(81)]
[TypeImpl(TypeImplFlags.Object_Equals | TypeImplFlags.Equatable | TypeImplFlags.EqualityOperators, IsLargeStructure = true)]
public partial struct MarkerGrid : GridBase
{
	/// <summary>
	/// Represents not supported information.
	/// </summary>
	private const string ErrorInfo_NotSupported = "This member is not supported.";


	/// <summary>
	/// <inheritdoc cref="GridBase.Undefined" path="/summary"/>
	/// </summary>
	/// <remarks>
	/// For <see cref="MarkerGrid"/>, <see cref="Undefined"/> is equivalent to <see cref="Empty"/>
	/// because initial states of such two fields are same.
	/// </remarks>
	public static readonly MarkerGrid Undefined = default;

	/// <summary>
	/// <inheritdoc cref="GridBase.Empty" path="/summary"/>
	/// </summary>
	/// <remarks>
	/// For <see cref="MarkerGrid"/>, <see cref="Undefined"/> is equivalent to <see cref="Empty"/>
	/// because initial states of such two fields are same.
	/// </remarks>
	public static readonly MarkerGrid Empty = default;


	/// <inheritdoc cref="GridBase.FirstMaskRef"/>
	private Mask _values;


	/// <summary>
	/// Creates a <see cref="MarkerGrid"/> instance via the pointer of the first element of the cell digit, and the creating option.
	/// </summary>
	/// <param name="firstElement">The reference of the first element.</param>
	/// <param name="creatingOption">The creating option.</param>
	/// <exception cref="ArgumentNullException">
	/// Throws when the argument <paramref name="firstElement"/> is <see langword="null"/> reference.
	/// </exception>
	private MarkerGrid(ref readonly Digit firstElement, GridCreatingOption creatingOption = GridCreatingOption.None)
	{
		// Firstly we should initialize the inner values.
		this = Empty;

		// Then traverse the array (span, pointer or etc.), to get refresh the values.
		var minusOneEnabled = creatingOption == GridCreatingOption.MinusOne;
		for (var i = 0; i < 81; i++)
		{
			var value = Unsafe.Add(ref Unsafe.AsRef(in firstElement), i);
			if ((minusOneEnabled ? value - 1 : value) is var realValue and not -1)
			{
				// Calls the indexer to trigger the event (Clear the candidates in peer cells).
				SetDigit(i, realValue);

				// Set the state to 'CellState.Given'.
				SetState(i, CellState.Given);
			}
		}
	}


	/// <inheritdoc/>
	public readonly Candidate CandidatesCount
	{
		get
		{
			var result = 0;
			for (var cell = 0; cell < 81; cell++)
			{
				result += Mask.PopCount(GetCandidates(cell));
			}
			return result;
		}
	}

	/// <inheritdoc/>
	public readonly ReadOnlySpan<Candidate> Candidates
	{
		get
		{
			var candidates = new Candidate[CandidatesCount];
			for (var (cell, i) = (0, 0); cell < 81; cell++)
			{
				if (GetState(cell) == CellState.Empty)
				{
					foreach (var digit in GetCandidates(cell))
					{
						candidates[i++] = cell * 9 + digit;
					}
				}
			}
			return candidates;
		}
	}

	/// <inheritdoc/>
	public readonly MarkerGrid ResetGrid => Preserve(GivenCells);

	/// <inheritdoc/>
	public readonly MarkerGrid UnfixedGrid
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
	public readonly MarkerGrid FixedGrid
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			var result = this;
			result.Fix();
			return result;
		}
	}

	/// <inheritdoc cref="GridBase.GivenCells"/>
	public readonly unsafe CellMap GivenCells => GridBase.GetMap(this, &GridPredicates.GivenCells);

	/// <inheritdoc cref="GridBase.ModifiableCells"/>
	public readonly unsafe CellMap ModifiableCells => GridBase.GetMap(this, &GridPredicates.ModifiableCells);

	/// <inheritdoc cref="GridBase.EmptyCells"/>
	public readonly unsafe CellMap EmptyCells => GridBase.GetMap(this, &GridPredicates.EmptyCells);

	/// <inheritdoc cref="GridBase.BivalueCells"/>
	public readonly unsafe CellMap BivalueCells => GridBase.GetMap(this, &GridPredicates.BivalueCells);

	/// <inheritdoc/>
	public readonly unsafe ReadOnlySpan<CellMap> CandidatesMap => GridBase.GetMaps(this, &GridPredicates.CandidatesMap);

	/// <inheritdoc/>
	public readonly unsafe ReadOnlySpan<CellMap> DigitsMap => GridBase.GetMaps(this, &GridPredicates.DigitsMap);

	/// <inheritdoc/>
	public readonly unsafe ReadOnlySpan<CellMap> ValuesMap => GridBase.GetMaps(this, &GridPredicates.ValuesMap);

	/// <inheritdoc/>
	/// <remarks>
	/// For <see cref="MarkerGrid"/> instances, this property always returns <see langword="true"/>.
	/// </remarks>
	readonly bool GridBase.IsMissingCandidates => true;

	/// <inheritdoc/>
	/// <remarks>
	/// For <see cref="MarkerGrid"/> instances, this property always returns <see langword="false"/>.
	/// </remarks>
	readonly bool GridBase.IsSolved => false;

	/// <inheritdoc/>
	readonly CellMap GridBase.GivenCells => ((Grid)this).GivenCells;

	/// <inheritdoc/>
	readonly CellMap GridBase.ModifiableCells => ((Grid)this).ModifiableCells;

	/// <inheritdoc/>
	readonly CellMap GridBase.EmptyCells => ((Grid)this).EmptyCells;

	/// <inheritdoc/>
	readonly CellMap GridBase.BivalueCells => ((Grid)this).BivalueCells;

	/// <inheritdoc/>
	[UnscopedRef]
	readonly ReadOnlySpan<Mask> IInlineArray<MarkerGrid, Mask>.Elements => this[..];

	/// <inheritdoc/>
	readonly ReadOnlySpan<Conjugate> GridBase.ConjugatePairs => ((Grid)this).ConjugatePairs;

	/// <inheritdoc/>
	readonly HouseMask GridBase.CompletedHouses => ((Grid)this).CompletedHouses;

	/// <inheritdoc/>
	readonly HouseMask GridBase.EmptyHouses => ((Grid)this).EmptyHouses;

	/// <inheritdoc/>
	[UnscopedRef]
	readonly ref readonly Mask GridBase.FirstMaskRef => ref this[0];


	/// <inheritdoc/>
	static int IInlineArray<MarkerGrid, Mask>.InlineArrayLength => 81;

	/// <inheritdoc/>
	static string GridBase.EmptyString => Grid.EmptyString;

	/// <inheritdoc/>
	static Mask GridBase.DefaultMask => Grid.DefaultMask;

	/// <inheritdoc/>
	static Mask GridBase.MaxCandidatesMask => Grid.MaxCandidatesMask;

	/// <inheritdoc/>
	static Mask GridBase.EmptyMask => Grid.EmptyMask;

	/// <inheritdoc/>
	static Mask GridBase.ModifiableMask => Grid.ModifiableMask;

	/// <inheritdoc/>
	static Mask GridBase.GivenMask => Grid.GivenMask;

	/// <inheritdoc/>
	static ref readonly MarkerGrid GridBase.Undefined => ref Undefined;

	/// <inheritdoc/>
	static ref readonly MarkerGrid GridBase.Empty => ref Empty;


	/// <inheritdoc/>
	public readonly Mask this[in CellMap cells]
	{
		get
		{
			var result = (Mask)0;
			foreach (var cell in cells)
			{
				result |= this[cell];
			}
			return (Mask)(result & Grid.MaxCandidatesMask);
		}
	}

	/// <inheritdoc/>
	public readonly unsafe Mask this[in CellMap cells, bool withValueCells, MaskAggregator aggregator = MaskAggregator.Or]
	{
		get
		{
			var result = aggregator switch
			{
				MaskAggregator.AndNot or MaskAggregator.And => Grid.MaxCandidatesMask,
				MaskAggregator.Or => (Mask)0,
				_ => throw new ArgumentOutOfRangeException(nameof(aggregator))
			};
			delegate*<ref Mask, in MarkerGrid, Cell, void> mergingFunctionPtr = aggregator switch
			{
				MaskAggregator.AndNot => &andNot,
				MaskAggregator.And => &and,
				MaskAggregator.Or => &or
			};
			foreach (var cell in cells)
			{
				if (withValueCells || GetState(cell) == CellState.Empty)
				{
					mergingFunctionPtr(ref result, this, cell);
				}
			}
			return (Mask)(result & Grid.MaxCandidatesMask);


			static void andNot(ref Mask result, in MarkerGrid grid, Cell cell) => result &= (Mask)~grid[cell];

			static void and(ref Mask result, in MarkerGrid grid, Cell cell) => result &= grid[cell];

			static void or(ref Mask result, in MarkerGrid grid, Cell cell) => result |= grid[cell];
		}
	}

	/// <inheritdoc/>
	[UnscopedRef]
	ref Mask IInlineArray<MarkerGrid, Mask>.this[Cell cell] => ref this[cell];


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
	public readonly bool GetExistence(Cell cell, Digit digit) => (this[cell] >> digit & 1) != 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool? Exists(Cell cell, Digit digit) => GetState(cell) == CellState.Empty ? GetExistence(cell, digit) : null;

	/// <inheritdoc/>
	public override readonly int GetHashCode() => ToString("#").GetHashCode();

	/// <inheritdoc cref="object.ToString"/>
	public override readonly string ToString() => ToString(null, null);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(string? format) => ToString(format, null);

	/// <inheritdoc/>
	public readonly string ToString(string? format, IFormatProvider? formatProvider)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(IFormatProvider? formatProvider) => ToString(null, formatProvider);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly CellState GetState(Cell cell) => MaskOperations.MaskToCellState(this[cell]);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Mask GetCandidates(Cell cell) => (Mask)(this[cell] & Grid.MaxCandidatesMask);

	/// <inheritdoc/>
	public readonly Mask[] ToCandidateMaskArray()
	{
		var result = new Mask[81];
		for (var cell = 0; cell < 81; cell++)
		{
			result[cell] = (Mask)(this[cell] & Grid.MaxCandidatesMask);
		}
		return result;
	}

	/// <summary>
	/// Creates an array of <see cref="Mask"/> values that is a copy for the current inline array data structure.
	/// </summary>
	/// <returns>An array of <see cref="Mask"/> values.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Mask[] ToMaskArray() => this[..].ToArray();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Digit GetDigit(Cell cell)
		=> GetState(cell) switch
		{
			CellState.Empty => -1,
			CellState.Modifiable or CellState.Given => Mask.Log2(this[cell]),
			_ => throw new InvalidOperationException(SR.ExceptionMessage("GridInvalidCellState"))
		};

	/// <inheritdoc/>
	public readonly Digit[] ToDigitsArray()
	{
		var result = new Digit[81];
		for (var cell = 0; cell < 81; cell++)
		{
			if (GetState(cell) != CellState.Empty)
			{
				result[cell] = Mask.Log2(GetCandidates(cell));
			}
		}
		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Reset() => this = Preserve(GivenCells);

	/// <inheritdoc/>
	public void Fix()
	{
		for (var i = 0; i < 81; i++)
		{
			if (GetState(i) == CellState.Modifiable)
			{
				SetState(i, CellState.Given);
			}
		}
	}

	/// <inheritdoc/>
	public void Unfix()
	{
		for (var i = 0; i < 81; i++)
		{
			if (GetState(i) == CellState.Given)
			{
				SetState(i, CellState.Modifiable);
			}
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetState(Cell cell, CellState state)
	{
		ref var mask = ref this[cell];
		mask = (Mask)((Mask)((int)state << 9) | (Mask)(mask & Grid.MaxCandidatesMask));
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetDigit(Cell cell, Digit digit)
	{
		if (digit switch
		{
			-1 when GetState(cell) == CellState.Modifiable => Grid.DefaultMask,
			>= 0 and < 9 => (Mask)(Grid.ModifiableMask | 1 << digit),
			_ => Mask.MinValue
		} is var validMask and not Mask.MinValue)
		{
			this[cell] = validMask;
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetCandidates(Cell cell, Mask mask)
		=> this[cell] = (Mask)((Mask)((int)GetState(cell) << 9) | (Mask)(mask & Grid.MaxCandidatesMask));

	/// <inheritdoc/>
	public void SetExistence(Cell cell, Digit digit, bool isOn)
	{
		if ((cell, digit) is ( >= 0 and < 81, >= 0 and < 9))
		{
			if (isOn)
			{
				AddCandidate(cell, digit);
			}
			else
			{
				RemoveCandidate(cell, digit);
			}
		}
	}

	/// <summary>
	/// Adds a new candidate into the grid.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	public void AddCandidate(Cell cell, Digit digit)
	{
		if (GetState(cell) == CellState.Empty)
		{
			this[cell] |= (Mask)(1 << digit);
		}
	}

	/// <summary>
	/// Removes a candidate from the grid.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	public void RemoveCandidate(Cell cell, Digit digit)
	{
		if (GetState(cell) == CellState.Empty)
		{
			this[cell] &= (Mask)~(1 << digit);
		}
	}

	/// <inheritdoc/>
	[UnscopedRef]
	readonly ReadOnlySpan<Mask> IInlineArray<MarkerGrid, Mask>.AsReadOnlySpan() => this;

	/// <inheritdoc/>
	readonly MarkerGrid IElementSwappingTransformable<MarkerGrid, Digit>.Shuffle()
	{
		var rng = Random.Shared;
		var current = this;
		for (var d1 = 0; d1 < 9; d1++)
		{
			Digit d2;
			do
			{
				d2 = rng.NextDigit();
			} while (d1 == d2);
			current = current.SwapDigit(d1, d2);
		}
		return current;
	}

	/// <inheritdoc/>
	[DoesNotReturn]
	readonly int GridBase.CompareTo(in MarkerGrid other) => throw new NotSupportedException(ErrorInfo_NotSupported);

	/// <inheritdoc/>
	readonly IEnumerator<Digit> IEnumerable<Digit>.GetEnumerator() => ToDigitsArray().AsEnumerable().GetEnumerator();

	/// <inheritdoc/>
	readonly IEnumerable<Candidate> IWhereMethod<MarkerGrid, Candidate>.Where(Func<Candidate, bool> predicate)
		=> this.Where(predicate).ToArray();

	/// <inheritdoc/>
	readonly IEnumerable<TResult> ISelectMethod<MarkerGrid, Candidate>.Select<TResult>(Func<Candidate, TResult> selector)
		=> this.Select(selector).ToArray();

	/// <inheritdoc/>
	void GridBase.SetMask(Cell cell, Mask mask) => this[cell] = mask;

	/// <inheritdoc/>
	[DoesNotReturn]
	void GridBase.Apply(Conclusion conclusion) => throw new NotSupportedException(ErrorInfo_NotSupported);

	/// <inheritdoc/>
	MarkerGrid IBoardTransformable<MarkerGrid>.RotateClockwise() => this.RotateClockwise();

	/// <inheritdoc/>
	MarkerGrid IBoardTransformable<MarkerGrid>.MirrorLeftRight() => this.MirrorLeftRight();

	/// <inheritdoc/>
	MarkerGrid IBoardTransformable<MarkerGrid>.MirrorTopBottom() => this.MirrorTopBottom();

	/// <inheritdoc/>
	MarkerGrid IBoardTransformable<MarkerGrid>.MirrorDiagonal() => this.MirrorDiagonal();

	/// <inheritdoc/>
	MarkerGrid IBoardTransformable<MarkerGrid>.MirrorAntidiagonal() => this.MirrorAntidiagonal();

	/// <inheritdoc/>
	MarkerGrid IElementSwappingTransformable<MarkerGrid, Digit>.SwapElement(Digit element1, Digit element2)
		=> this.SwapDigit(element1, element2);

	/// <inheritdoc/>
	[UnscopedRef]
	Span<Mask> IInlineArray<MarkerGrid, Mask>.AsSpan() => this;

	/// <inheritdoc/>
	[UnscopedRef]
	ref Mask IInlineArray<MarkerGrid, Mask>.GetPinnableReference() => ref this[0];

	/// <summary>
	/// Gets a sudoku grid, removing all value digits not appearing in the specified <paramref name="pattern"/>.
	/// </summary>
	/// <param name="pattern">The pattern.</param>
	/// <returns>The result grid.</returns>
	private readonly MarkerGrid Preserve(in CellMap pattern)
	{
		var result = this;
		foreach (var cell in ~pattern)
		{
			result.SetDigit(cell, -1);
		}
		return result;
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParse(ReadOnlySpan<char> s, out MarkerGrid result) => TryParse(s.ToString(), null, out result);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out MarkerGrid result)
		=> TryParse(s.ToString(), provider, out result);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParse(string? s, out MarkerGrid result) => TryParse(s, null, out result);

	/// <inheritdoc/>
	public static bool TryParse(string? s, IFormatProvider? provider, out MarkerGrid result)
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
			result = default;
			return false;
		}
	}

	/// <inheritdoc/>
	public static MarkerGrid Create(params ReadOnlySpan<Mask> values)
	{
		switch (values)
		{
			case []:
			{
				return Undefined;
			}
			case [var uniformValue]:
			{
				var result = Undefined;
				if (uniformValue == 0) { result[..].Clear(); } else { result[..].Fill(uniformValue); }
				return result;
			}
			case { Length: 81 }:
			{
				var result = Undefined;
				values[..].CopyTo(result[..]);
				return result;
			}
			default:
			{
				throw new InvalidOperationException($"The argument '{nameof(values)}' must contain {81} elements.");
			}
		}
	}

	/// <summary>
	/// Creates a <see cref="MarkerGrid"/> instance using grid values.
	/// </summary>
	/// <param name="gridValues">The array of grid values.</param>
	/// <param name="creatingOption">The grid creating option.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static MarkerGrid Create(Digit[] gridValues, GridCreatingOption creatingOption = 0)
		=> new(in gridValues[0], creatingOption);

	/// <summary>
	/// Creates a <see cref="MarkerGrid"/> instance via the array of cell digits
	/// of type <see cref="ReadOnlySpan{T}"/> of <see cref="Digit"/>.
	/// </summary>
	/// <param name="gridValues">The list of cell digits.</param>
	/// <param name="creatingOption">The grid creating option.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static MarkerGrid Create(ReadOnlySpan<Digit> gridValues, GridCreatingOption creatingOption = 0)
		=> new(in gridValues[0], creatingOption);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static MarkerGrid Parse(ReadOnlySpan<char> s) => Parse(s, null);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static MarkerGrid Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s.ToString(), provider);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static MarkerGrid Parse(string? s) => Parse(s ?? string.Empty, null);

	/// <inheritdoc/>
	public static MarkerGrid Parse(string s, IFormatProvider? provider) => (MarkerGrid)Grid.Parse(s, provider);


	/// <summary>
	/// Casts the current instance into a <see cref="Grid"/>, only with given and modifiable cells reserved.
	/// </summary>
	/// <param name="grid">The grid to be cast from.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator Grid(in MarkerGrid grid) => Grid.Create(grid.ToDigitsArray());

	/// <summary>
	/// Casts the current instance into a <see cref="MarkerGrid"/>, only with given and modifiable cells reserved.
	/// </summary>
	/// <param name="grid">The grid to be cast from.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator MarkerGrid(in Grid grid) => Create(grid.ToDigitsArray());


	/// <inheritdoc/>
	[DoesNotReturn]
	static bool IComparisonOperators<MarkerGrid, MarkerGrid, bool>.operator >(MarkerGrid left, MarkerGrid right)
		=> throw new NotSupportedException(ErrorInfo_NotSupported);

	/// <inheritdoc/>
	[DoesNotReturn]
	static bool IComparisonOperators<MarkerGrid, MarkerGrid, bool>.operator >=(MarkerGrid left, MarkerGrid right)
		=> throw new NotSupportedException(ErrorInfo_NotSupported);

	/// <inheritdoc/>
	[DoesNotReturn]
	static bool IComparisonOperators<MarkerGrid, MarkerGrid, bool>.operator <(MarkerGrid left, MarkerGrid right)
		=> throw new NotSupportedException(ErrorInfo_NotSupported);

	/// <inheritdoc/>
	[DoesNotReturn]
	static bool IComparisonOperators<MarkerGrid, MarkerGrid, bool>.operator <=(MarkerGrid left, MarkerGrid right)
		=> throw new NotSupportedException(ErrorInfo_NotSupported);

	/// <inheritdoc/>
	[DoesNotReturn]
	static bool GridBase.operator >(in MarkerGrid left, in MarkerGrid right)
		=> throw new NotSupportedException(ErrorInfo_NotSupported);

	/// <inheritdoc/>
	[DoesNotReturn]
	static bool GridBase.operator >=(in MarkerGrid left, in MarkerGrid right)
		=> throw new NotSupportedException(ErrorInfo_NotSupported);

	/// <inheritdoc/>
	[DoesNotReturn]
	static bool GridBase.operator <(in MarkerGrid left, in MarkerGrid right)
		=> throw new NotSupportedException(ErrorInfo_NotSupported);

	/// <inheritdoc/>
	[DoesNotReturn]
	static bool GridBase.operator <=(in MarkerGrid left, in MarkerGrid right)
		=> throw new NotSupportedException(ErrorInfo_NotSupported);
}
