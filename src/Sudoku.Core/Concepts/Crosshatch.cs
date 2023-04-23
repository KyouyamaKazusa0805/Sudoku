namespace Sudoku.Concepts;

/// <summary>
/// Represents a crosshatch factor. The type can describe an elimination that starts with a certain cell
/// (or some cells), and eliminates candidates in some cells. The concept is commonly used
/// by logical technique "Hidden Single".
/// </summary>
/// <param name="mask">
/// <para>Indicates the inner mask.</para>
/// <para>
/// The mask uses 46 bits to describe two intersections and a digit information.
/// The higher 4 bits is the digit, and last 42 bits are two intersections representing the start
/// and end intersection. For example, <c>r5c5(3)</c> can eliminate <c>r789c5(3)</c> in block 8,
/// then we can construct two intersections to describe <c>r5c5</c> and <c>r789c5</c>,
/// putting them into the last 42 bits. A single candidate will use 7 bits to describe
/// (for cell indices from 0 to 80, i.e. <c>r1c1</c> to <c>r9c9</c>).
/// </para>
/// </param>
public readonly partial struct Crosshatch([PrimaryConstructorParameter(MemberKinds.Field)] ulong mask) :
	IEquatable<Crosshatch>,
	IEqualityOperators<Crosshatch, Crosshatch, bool>
{
	/// <summary>
	/// Indicates the mask that is used for getting and checking the cells in the mask <see cref="_mask"/>.
	/// </summary>
	/// <seealso cref="_mask"/>
	private const ulong CellsGroupMask = (1UL << 21) - 1;

	/// <summary>
	/// Indicates the mask that is used for separating multiple cells in a group.
	/// </summary>
	private const ulong PerCellMaxMask = (1UL << 7) - 1;


	/// <summary>
	/// Indicates the houses list that represents the possible houses that a crosshatch will be used.
	/// </summary>
	private static readonly House[][] CrosshatchingHousesList =
	{
		new[] { 1, 2, 3, 6 }, new[] { 0, 2, 4, 7 }, new[] { 1, 2, 5, 8 },
		new[] { 0, 4, 5, 6 }, new[] { 1, 3, 5, 7 }, new[] { 2, 3, 4, 8 },
		new[] { 0, 3, 7, 8 }, new[] { 1, 4, 6, 8 }, new[] { 2, 5, 6, 7 }
	};


	/// <inheritdoc cref="Create(Digit, in CellMap, in CellMap)"/>
	[DebuggerHidden]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[JsonConstructor]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Obsolete(RequiresJsonSerializerDynamicInvocationMessage.DynamicInvocationByJsonSerializerOnly, true)]
	[RequiresUnreferencedCode(RequiresJsonSerializerDynamicInvocationMessage.DynamicInvocationByJsonSerializerOnly)]
	public Crosshatch(Digit digit, CellMap from, CellMap to) : this(CreateMask(digit, from, to))
	{
	}


	/// <summary>
	/// Indicates the digit used.
	/// </summary>
	[JsonInclude]
	public Digit Digit
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Digit)(_mask >> 42);
	}

	/// <summary>
	/// Indicates what cells the current crosshatch start.
	/// </summary>
	[JsonInclude]
	public CellMap From
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			var mask = _mask & CellsGroupMask;
			var cell1 = mask & PerCellMaxMask;
			var cell2 = mask >> 7 & PerCellMaxMask;
			var cell3 = mask >> 14 & PerCellMaxMask;

			var result = CellMap.Empty;
			if (cell1 != PerCellMaxMask)
			{
				result.Add((Cell)cell1);
			}
			if (cell2 != PerCellMaxMask)
			{
				result.Add((Cell)cell2);
			}
			if (cell3 != PerCellMaxMask)
			{
				result.Add((Cell)cell3);
			}

			return result;
		}
	}

	/// <summary>
	/// Indicates what cells the current crosshatch end.
	/// </summary>
	[JsonInclude]
	public CellMap To
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			var mask = _mask >> 21 & CellsGroupMask;
			var cell1 = mask & PerCellMaxMask;
			var cell2 = mask >> 7 & PerCellMaxMask;
			var cell3 = mask >> 14 & PerCellMaxMask;

			var result = CellMap.Empty;
			if (cell1 != PerCellMaxMask)
			{
				result.Add((Cell)cell1);
			}
			if (cell2 != PerCellMaxMask)
			{
				result.Add((Cell)cell2);
			}
			if (cell3 != PerCellMaxMask)
			{
				result.Add((Cell)cell3);
			}

			return result;
		}
	}


	[GeneratedOverridingMember(GeneratedEqualsBehavior.TypeCheckingAndCallingOverloading)]
	public override partial bool Equals(object? obj);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Crosshatch other) => _mask == other._mask;

	[GeneratedOverridingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(Digit), nameof(From), nameof(To))]
	public override partial int GetHashCode();

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
	{
		var digitStr = (Digit + 1).ToString();
		return $"{From}({digitStr}) -> {To}({digitStr})";
	}


	/// <summary>
	/// Initializes a <see cref="Crosshatch"/> instance via the specified digit, start and end cell.
	/// </summary>
	/// <param name="digit">The digit.</param>
	/// <param name="from">The start cell.</param>
	/// <param name="to">The end cell.</param>
	/// <returns>A <see cref="Crosshatch"/> result instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Crosshatch Create(Digit digit, Cell from, Cell to) => Create(digit, CellsMap[from], CellsMap[to]);

	/// <summary>
	/// Initializes a <see cref="Crosshatch"/> instance via the specified digit, start cell and end cells.
	/// </summary>
	/// <param name="digit">The digit.</param>
	/// <param name="from">The start cell.</param>
	/// <param name="to">The end cells.</param>
	/// <returns>A <see cref="Crosshatch"/> result instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Crosshatch Create(Digit digit, Cell from, scoped in CellMap to) => Create(digit, CellsMap[from], to);

	/// <summary>
	/// Initializes a <see cref="Crosshatch"/> instance via the specified digit, start and end cells.
	/// </summary>
	/// <param name="digit">The digit.</param>
	/// <param name="from">The start cells.</param>
	/// <param name="to">The end cells.</param>
	/// <returns>A <see cref="Crosshatch"/> result instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Crosshatch Create(Digit digit, scoped in CellMap from, scoped in CellMap to) => new(CreateMask(digit, from, to));

	/// <summary>
	/// Try to get all crosshatches for a single candidate.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="digit">The digit.</param>
	/// <param name="house">The house that the candidate as the hidden single formed in.</param>
	/// <param name="targetCell">The target cell.</param>
	/// <returns>All found crosshatches.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="house"/> is out of range (lower than 0 or greater than 27).
	/// </exception>
	/// <exception cref="ArgumentException">
	/// Throws when the grid <paramref name="grid"/> is invalid, causing no valid crosshatching operation
	/// can be found in the specified house <paramref name="house"/> of the specified digit <paramref name="digit"/>.
	/// </exception>
	/// <exception cref="InvalidOperationException">
	/// Throws when the target house <paramref name="house"/> does not contain any possible hidden single
	/// of the specified digit <paramref name="digit"/>.
	/// </exception>
	public static Crosshatch[] GetCrosshatches(scoped in Grid grid, Digit digit, House house, out Cell targetCell)
	{
		const string error_TargetHouseNotContainHiddenSingle = "The target house does not contain a valid hidden single of the current digit.";
		const string error_ArgHouseIsInvalid = $"The argument '{nameof(house)}' is invalid.";
		const string error_ArgGridIsInvalid = "The grid is invalid: The crosshatching operation is failed to be checked.";

		var houseCells = HousesMap[house];
		var valueCellsOfDigit = (house, grid) switch
		{
			( >= 0 and < 27, { CandidatesMap: var cMap, ValuesMap: var vMap }) => (cMap[digit] & houseCells) switch
			{
				[var c] when (targetCell = c) is var _ => house switch
				{
					>= 0 and < 9 => getByBlock(vMap, digit, house),
					_ => getByLine(vMap, targetCell, digit)
				},
				_ => throw new InvalidOperationException(error_TargetHouseNotContainHiddenSingle)
			},
			_ => throw new ArgumentOutOfRangeException(error_ArgHouseIsInvalid, nameof(house))
		};

		var cellsShouldBeEliminated = houseCells - targetCell;
		var tempTargetCombination = default(CellMap?);
		foreach (var combination in valueCellsOfDigit | valueCellsOfDigit.Count)
		{
			if ((combination.ExpandedPeers & houseCells) == cellsShouldBeEliminated)
			{
				tempTargetCombination = combination;
				break;
			}
		}
		if (tempTargetCombination is not { } targetCombination)
		{
			throw new ArgumentException(error_ArgGridIsInvalid, nameof(grid));
		}

		using scoped var list = new ValueList<Crosshatch>(6);
		foreach (var cell in targetCombination)
		{
			list.Add(Create(digit, cell, PeersMap[cell] & grid.EmptyCells & houseCells));
		}

		return list.ToArray();


		static CellMap getByBlock(CellMap[] valuesMap, Digit digit, House house)
		{
			var result = CellMap.Empty;
			var crosshatchingHouses = CrosshatchingHousesList[house];
			foreach (var crosshatchingHouse in crosshatchingHouses)
			{
				if ((HousesMap[crosshatchingHouse] & valuesMap[digit]) is [var cell])
				{
					result.Add(cell);
				}
			}

			return result;
		}

		static CellMap getByLine(CellMap[] valuesMap, Cell targetCell, Digit digit)
		{
			var result = CellMap.Empty;
			var block = targetCell.ToHouseIndex(HouseType.Block);
			for (var i = 0; i < 9; i++)
			{
				if (block == i)
				{
					continue;
				}

				if ((HousesMap[i] & valuesMap[digit]) is [var cell])
				{
					result.Add(cell);
				}
			}

			return result;
		}
	}

	/// <summary>
	/// Initializes a <see cref="Crosshatch"/> instance via the specified digit, start and end cells.
	/// </summary>
	/// <param name="digit"><inheritdoc cref="Create(Digit, in CellMap, in CellMap)" path="/param[@name='digit']"/></param>
	/// <param name="from"><inheritdoc cref="Create(Digit, in CellMap, in CellMap)" path="/param[@name='from']"/></param>
	/// <param name="to"><inheritdoc cref="Create(Digit, in CellMap, in CellMap)" path="/param[@name='to']"/></param>
	/// <returns>A <see cref="ulong"/> result indicating the target mask.</returns>
	/// <exception cref="ArgumentException">
	/// Throws when the argument <paramref name="from"/> or <paramref name="to"/> does not contain at most 3 cells,
	/// or it is empty, or it is not an intersection, or the specified digit value <paramref name="digit"/>
	/// is less than 0 or greater than 9.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong CreateMask(Digit digit, scoped in CellMap from, scoped in CellMap to)
	{
		Argument.ThrowIfFalse(from is { Count: 1 } or { Count: 2 or 3, IsInIntersection: true });
		Argument.ThrowIfFalse(to is { Count: 1 } or { Count: 2 or 3, IsInIntersection: true });
		Argument.ThrowIfFalse(digit is >= 0 and < 9);

		return f2(from[0]) | f1(from[1], 7) | f1(from[2], 14) | f1(to[0], 21) | f1(to[1], 28) | f1(to[2], 35) | (ulong)digit << 42;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static ulong f1(Cell cell, int shifting) => (cell != -1 ? (ulong)cell : PerCellMaxMask) << shifting;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static ulong f2(Cell cell) => cell != -1 ? (ulong)cell : PerCellMaxMask;
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Crosshatch left, Crosshatch right) => left.Equals(right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Crosshatch left, Crosshatch right) => !left.Equals(right);
}
