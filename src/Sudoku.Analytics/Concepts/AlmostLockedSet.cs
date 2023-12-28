namespace Sudoku.Concepts;

/// <summary>
/// Defines a data pattern that describes an ALS.
/// </summary>
/// <param name="digitsMask">Indicates the mask of digits used.</param>
/// <param name="cells">Indicates the cells used.</param>
/// <param name="possibleEliminationMap">Gets the possible cells that can store eliminations for the ALS.</param>
/// <param name="eliminationMap">
/// The cells that can be eliminated, grouped by digit. The former 9 elements of the array is the cells
/// that can be eliminated for the corresponding digit, and the last element is the merged cells.
/// </param>
/// <remarks>
/// An <b>Almost Locked Set</b> is a sudoku concept, which describes a case that
/// <c>n</c> cells contains <c>(n + 1)</c> kinds of different digits.
/// The special case is a bi-value cell.
/// </remarks>
[Equals]
[GetHashCode]
[ComparisonOperators]
[EqualityOperators]
public sealed partial class AlmostLockedSet(
	[Data, HashCodeMember] Mask digitsMask,
	[Data, HashCodeMember] scoped ref readonly CellMap cells,
	[Data] scoped ref readonly CellMap possibleEliminationMap,
	[Data] CellMap[] eliminationMap
) :
	IComparable<AlmostLockedSet>,
	IComparisonOperators<AlmostLockedSet, AlmostLockedSet, bool>,
	ICoordinateObject<AlmostLockedSet>,
	IEquatable<AlmostLockedSet>,
	IEqualityOperators<AlmostLockedSet, AlmostLockedSet, bool>
{
	/// <summary>
	/// Indicates an array of the total number of the strong relations in an ALS of the different size.
	/// The field is only unused in the property <see cref="StrongLinks"/>.
	/// </summary>
	/// <seealso cref="StrongLinks"/>
	private static readonly int[] StrongRelationsCount = [0, 1, 3, 6, 10, 15, 21, 28, 36, 45];


	/// <summary>
	/// Indicates the house used.
	/// </summary>
	public House House
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			Cells.InOneHouse(out var houseIndex);
			return houseIndex;
		}
	}

	/// <summary>
	/// Indicates whether the ALS only uses a bi-value cell.
	/// </summary>
	public bool IsBivalueCell
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Cells.Count == 1;
	}

	/// <summary>
	/// Indicates all strong links in this ALS.
	/// The result will be represented as a <see cref="Mask"/> mask of 9 bits indicating which bits used.
	/// </summary>
	public ReadOnlySpan<Mask> StrongLinks
	{
		get
		{
			scoped var digits = DigitsMask.GetAllSets();
			var result = new Mask[StrongRelationsCount[digits.Length - 1]];
			for (var (i, x, l) = (0, 0, digits.Length); i < l - 1; i++)
			{
				for (var j = i + 1; j < l; j++)
				{
					result[x++] = (Mask)(1 << digits[i] | 1 << digits[j]);
				}
			}

			return result;
		}
	}


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out Mask digitsMask, out CellMap cells) => (digitsMask, cells) = (DigitsMask, Cells);

	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out Mask digitsMask, out CellMap cells, out CellMap possibleEliminationMap)
		=> ((digitsMask, cells), possibleEliminationMap) = (this, PossibleEliminationMap);

	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out Mask digitsMask, out CellMap cells, out CellMap possibleEliminationMap, out CellMap[] eliminationMap)
		=> ((digitsMask, cells, possibleEliminationMap), eliminationMap) = (this, EliminationMap);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals([NotNullWhen(true)] AlmostLockedSet? other)
		=> other is not null && DigitsMask == other.DigitsMask && Cells == other.Cells;

	/// <inheritdoc/>
	/// <exception cref="ArgumentNullException">Throws when the argument <paramref name="other"/> is <see langword="null"/>.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(AlmostLockedSet? other)
		=> other is null
			? throw new ArgumentNullException(nameof(other))
			: Cells.Count.CompareTo(other.Cells.Count) is var p and not 0 ? p : Cells.CompareTo(other.Cells);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => ToString(GlobalizedConverter.InvariantCultureConverter);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(CultureInfo? culture = null) => ToString(GlobalizedConverter.GetConverter(culture ?? CultureInfo.CurrentUICulture));

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(CoordinateConverter converter)
	{
		var digitsStr = converter.DigitConverter(DigitsMask);
		var houseStr = converter.HouseConverter(1 << House);
		var cellsStr = converter.CellConverter(Cells);
		return IsBivalueCell
			? $"{digitsStr}/{cellsStr}"
			: $"{digitsStr}/{cellsStr} {GetString("KeywordIn", converter.CurrentCulture ?? CultureInfo.CurrentUICulture)} {houseStr}";
	}

	/// <inheritdoc/>
	public static AlmostLockedSet ParseExact(string str, CoordinateParser parser)
		=> str.SplitBy(['/']) is [var digitsStr, var cellsStrAndHouseStr]
			? cellsStrAndHouseStr.SplitBy([' ']) is [var cellsStr, _, _]
				? new(parser.DigitParser(digitsStr), parser.CellParser(cellsStr), [], [])
				: throw new FormatException("Missing cells or target house.")
			: throw new FormatException("The ALS notation must contain only 1 slash character.");

	/// <summary>
	/// Collects all possible <see cref="AlmostLockedSet"/>s in the specified grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>All possible found <see cref="AlmostLockedSet"/> instances.</returns>
	public static AlmostLockedSet[] Collect(scoped ref readonly Grid grid)
	{
		_ = grid is { EmptyCells: var emptyMap, BivalueCells: var bivalueMap, CandidatesMap: var candidatesMap };

		// Get all bi-value-cell ALSes.
		var result = new List<AlmostLockedSet>();
		foreach (var cell in bivalueMap)
		{
			var eliminationMap = new CellMap[9];
			foreach (var digit in grid.GetCandidates(cell))
			{
				eliminationMap[digit] = PeersMap[cell] & candidatesMap[digit];
			}

			result.Add(new(grid.GetCandidates(cell), in CellsMap[cell], PeersMap[cell] & emptyMap, eliminationMap));
		}

		// Get all non-bi-value-cell ALSes.
		for (var house = 0; house < 27; house++)
		{
			if ((HousesMap[house] & emptyMap) is not { Count: >= 3 } tempMap)
			{
				continue;
			}

			for (var size = 2; size <= tempMap.Count - 1; size++)
			{
				foreach (ref readonly var map in tempMap.GetSubsets(size))
				{
					var blockMask = map.BlockMask;
					if (IsPow2(blockMask) && house >= 9)
					{
						// All ALS cells lying on a box-row or a box-column
						// will be processed as a block ALS.
						continue;
					}

					// Get all candidates in these cells.
					var digitsMask = grid[in map];
					if (PopCount((uint)digitsMask) - 1 != size)
					{
						continue;
					}

					var eliminationMap = new CellMap[9];
					foreach (var digit in digitsMask)
					{
						eliminationMap[digit] = map % candidatesMap[digit];
					}

					var coveredLine = map.CoveredLine;
					result.Add(
						new(
							digitsMask,
							in map,
							house < 9 && coveredLine is >= 9 and not TrailingZeroCountFallback
								? ((HousesMap[house] | HousesMap[coveredLine]) & emptyMap) - map
								: tempMap - map,
							eliminationMap
						)
					);
				}
			}
		}

		return [.. result];
	}
}
