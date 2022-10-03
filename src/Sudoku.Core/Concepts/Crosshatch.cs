namespace Sudoku.Concepts;

/// <summary>
/// Represents a crosshatch factor. The type can describe an elimination that starts with a certain cell
/// (or some cells), and eliminates candidates in some cells. The concept is commonly used
/// by logical technique "Hidden Single".
/// </summary>
public readonly struct Crosshatch : IEquatable<Crosshatch>, IEqualityOperators<Crosshatch, Crosshatch, bool>
{
	/// <summary>
	/// Indicates the mask that is used for getting and checking the cells in the mask <see cref="_value"/>.
	/// </summary>
	/// <seealso cref="_value"/>
	private const ulong CellsGroupMask = (1UL << 30) - 1;

	/// <summary>
	/// Indicates the mask that is used for separating multiple cells in a group.
	/// </summary>
	private const ulong PerCellMaxMask = (1UL << 10) - 1;


	/// <summary>
	/// Indicates the houses list that represents the possible houses that a crosshatch will be used.
	/// </summary>
	private static readonly int[][] CrosshatchingHousesList =
	{
		new[] { 1, 2, 3, 6 }, new[] { 0, 2, 4, 7 }, new[] { 1, 2, 5, 8 },
		new[] { 0, 4, 5, 6 }, new[] { 1, 3, 5, 7 }, new[] { 2, 3, 4, 8 },
		new[] { 0, 3, 7, 8 }, new[] { 1, 4, 6, 8 }, new[] { 2, 5, 6, 7 }
	};


	/// <summary>
	/// <para>
	/// Indicates the inner mask.
	/// </para>
	/// <para>
	/// The mask uses 46 bits to describe two intersections and a digit information.
	/// The higher 4 bits is the digit, and last 42 bits are two intersections representing the start
	/// and end intersection. For example, <c>r5c5(3)</c> can eliminate <c>r789c5(3)</c> in block 8,
	/// then we can construct two intersections to describe <c>r5c5</c> and <c>r789c5</c>,
	/// putting them into the last 42 bits. A single candidate will use 7 bits to describe
	/// (for cell indices from 0 to 80, i.e. <c>r1c1</c> to <c>r9c9</c>).
	/// </para>
	/// </summary>
	private readonly ulong _value;


	/// <summary>
	/// Initializes a <see cref="Crosshatch"/> instance via the specified digit, start and end cell.
	/// </summary>
	/// <param name="digit">The digit.</param>
	/// <param name="start">The start cell.</param>
	/// <param name="end">The end cell.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Crosshatch(int digit, int start, int end) : this(digit, CellsMap[start], CellsMap[end])
	{
	}

	/// <summary>
	/// Initializes a <see cref="Crosshatch"/> instance via the specified digit, start cell and end cells.
	/// </summary>
	/// <param name="digit">The digit.</param>
	/// <param name="start">The start cell.</param>
	/// <param name="end">The end cells.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Crosshatch(int digit, int start, scoped in CellMap end) : this(digit, CellsMap[start], end)
	{
	}

	/// <summary>
	/// Initializes a <see cref="Crosshatch"/> instance via the specified digit, start and end cells.
	/// </summary>
	/// <param name="digit">The digit.</param>
	/// <param name="start">The start cells.</param>
	/// <param name="end">The end cells.</param>
	/// <exception cref="ArgumentException">
	/// Throws when the argument <paramref name="start"/> or <paramref name="end"/> does not contain at most 3 cells,
	/// or it is empty, or it is not an intersection, or the specified digit value <paramref name="digit"/>
	/// is less than 0 or greater than 9.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Crosshatch(int digit, scoped in CellMap start, scoped in CellMap end)
	{
		Argument.ThrowIfFalse(start is { Count: 1 } or { Count: 2 or 3, IsInIntersection: true });
		Argument.ThrowIfFalse(end is { Count: 1 } or { Count: 2 or 3, IsInIntersection: true });
		Argument.ThrowIfFalse(digit is >= 0 and < 9);

		_value = f1(start[0], 21) | f1(start[1], 28) | f1(start[2], 35) | f2(end[0]) | f1(end[1], 7) | f1(end[2], 14)
			| (ulong)digit << 42;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static ulong f1(int cell, int shiftiing) => (cell != -1 ? (ulong)cell : PerCellMaxMask) << shiftiing;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static ulong f2(int cell) => (cell != -1 ? (ulong)cell : PerCellMaxMask);
	}


	/// <summary>
	/// Indicates the digit used.
	/// </summary>
	public int Digit
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (int)(_value >> 42);
	}

	/// <summary>
	/// Indicates what cells the current crosshatch start.
	/// </summary>
	public CellMap Start
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			var mask = _value >> 21 & CellsGroupMask;
			var cell1 = (uint)(mask >> 14 & PerCellMaxMask);
			var cell2 = (uint)(mask >> 7 & PerCellMaxMask);
			var cell3 = (uint)(mask & PerCellMaxMask);

			var result = CellMap.Empty;
			if (cell1 != uint.MaxValue) // Same as 'unchecked((uint)-1)'
			{
				result.Add((int)cell1);
			}
			if (cell2 != uint.MaxValue)
			{
				result.Add((int)cell2);
			}
			if (cell3 != uint.MaxValue)
			{
				result.Add((int)cell3);
			}

			return result;
		}
	}

	/// <summary>
	/// Indicates what cells the current crosshatch end.
	/// </summary>
	public CellMap End
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			var mask = _value & CellsGroupMask;
			var cell1 = (uint)(mask >> 14 & PerCellMaxMask);
			var cell2 = (uint)(mask >> 7 & PerCellMaxMask);
			var cell3 = (uint)(mask & PerCellMaxMask);
			var result = CellMap.Empty;
			if (cell1 != uint.MaxValue) // Same as 'unchecked((uint)-1)'
			{
				result.Add((int)cell1);
			}
			if (cell2 != uint.MaxValue)
			{
				result.Add((int)cell2);
			}
			if (cell3 != uint.MaxValue)
			{
				result.Add((int)cell3);
			}

			return result;
		}
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] object? obj) => obj is Crosshatch comparer && Equals(comparer);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Crosshatch other) => _value == other._value;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => HashCode.Combine(Digit, Start, End);

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
	{
		var digitStr = (Digit + 1).ToString();
		return $"{Start}({digitStr}) -> {End}({digitStr})";
	}


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
	public static Crosshatch[] GetCrosshatches(scoped in Grid grid, int digit, int house, out int targetCell)
	{
		var houseCells = HousesMap[house];
		var methodGetByBlock = getByBlock;
		var methodGetByLine = getByLine;
		return house switch
		{
			>= 0 and < 27 => (grid.CandidatesMap[digit] & houseCells) switch
			{
				[var c] => (house is >= 0 and < 9 ? methodGetByBlock : methodGetByLine)(grid, targetCell = c),
				_ => throw new InvalidOperationException("The target house does not contain a valid hidden single of the current digit.")
			},
			_ => throw new ArgumentOutOfRangeException($"The argument '{nameof(house)}' is invalid.", nameof(house))
		};


		Crosshatch[] getByBlock(scoped in Grid grid, int targetCell)
		{
			var cellsShouldBeEliminated = houseCells - targetCell;
			var valueCellsOfDigit = CellMap.Empty;
			var crosshatchingHouses = CrosshatchingHousesList[house];
			foreach (var crosshatchingHouse in crosshatchingHouses)
			{
				if ((HousesMap[crosshatchingHouse] & grid.ValuesMap[digit]) is [var cell])
				{
					valueCellsOfDigit.Add(cell);
				}
			}

			var tempTargetCombination = (CellMap?)null;
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
				throw new ArgumentException(
					"The grid is invalid: The crosshatching operation is failed to be checked.",
					nameof(grid)
				);
			}

			using scoped var list = new ValueList<Crosshatch>(4);
			foreach (var cell in targetCombination)
			{
				list.Add(new(digit, cell, PeersMap[cell] & grid.EmptyCells & houseCells));
			}

			return list.ToArray();
		}

		Crosshatch[] getByLine(scoped in Grid grid, int targetCell)
			=> throw new NotImplementedException("The method will be implemented later.");
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Crosshatch left, Crosshatch right) => left.Equals(right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Crosshatch left, Crosshatch right) => !(left == right);
}
