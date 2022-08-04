namespace Sudoku.Solving.Patterns;

/// <summary>
/// Defines a data structure that describes an AHS.
/// </summary>
/// <remarks>
/// An <b>Almost Hidden Set</b> is a sudoku concept, which describes a case that
/// only <c>n</c> digits can be filled into <c>n + 1</c> cells in a house.
/// </remarks>
public sealed class AlmostHiddenSet :
	IEquatable<AlmostHiddenSet>,
	IEqualityOperators<AlmostHiddenSet, AlmostHiddenSet>,
	ITechniquePattern<AlmostHiddenSet>,
	ITechniquePatternGatherable<AlmostHiddenSet>
{
	/// <summary>
	/// Initializes an <see cref="AlmostHiddenSet"/> instance
	/// via the specified digit mask and the map of cells.
	/// </summary>
	/// <param name="digitMask">The digit mask.</param>
	/// <param name="allDigitsMask">All digits appearing in the AHS structure..</param>
	/// <param name="map">The map.</param>
	/// <param name="digitsMap">The digits' distribution maps.</param>
	internal AlmostHiddenSet(short digitMask, short allDigitsMask, scoped in Cells map, Cells?[] digitsMap)
	{
		(DigitsMask, Map, AllDigitsMask) = (digitMask, map, allDigitsMask);

		var tempDic = new Dictionary<int, Cells>(9);
		for (int i = 0; i < digitsMap.Length; i++)
		{
			var currentDigitMap = digitsMap[i];
			if (currentDigitMap is { } digitMapNotNull)
			{
				tempDic.Add(i, digitMapNotNull);
			}
		}

		DigitsMap = tempDic;
	}


	/// <summary>
	/// Indicates the house used.
	/// </summary>
	public int House
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			_ = Map.AllSetsAreInOneHouse(out int houseIndex);
			return houseIndex;
		}
	}

	/// <summary>
	/// Indicates the mask of all digits appeared in the AHS structure.
	/// </summary>
	public short AllDigitsMask { get; }

	/// <summary>
	/// Indicates the mask of digits used.
	/// </summary>
	public short DigitsMask { get; }

	/// <inheritdoc/>
	public Cells Map { get; }

	/// <summary>
	/// Indicates all weak links in this AHS. The return value is described as an array of quadruples,
	/// indicating two digits, and cells used in the weak link.
	/// </summary>
	public (int Digit1, Cells Cells1, int Digit2, Cells Cells2)[] WeakLinks
	{
		get
		{
			var result = new List<(int, Cells, int, Cells)>();
			short unusedDigitsMask = (short)(AllDigitsMask & ~DigitsMask);
			foreach (int[] digitPair in unusedDigitsMask.GetAllSets().GetSubsets(2))
			{
				int digit1 = digitPair[0];
				int digit2 = digitPair[1];
				var cells1 = DigitsMap[digit1];
				var cells2 = DigitsMap[digit2];
				foreach (var cells1Case in cells1 | cells1.Count)
				{
					foreach (var cells2Case in cells2 | cells2.Count)
					{
						if ((cells1Case, cells2Case) is ([var cells1Cell], [var cells2Cell])
							&& cells1Cell == cells2Cell)
						{
							// This case has already been handled by sole cell weak inferences.
							continue;
						}

						result.Add((digit1, cells1Case, digit2, cells2Case));
					}
				}
			}

			return result.ToArray();
		}
	}

	/// <summary>
	/// Indicates a dictionary of elements, indicating the cells of digits' own distribution.
	/// </summary>
	public IReadOnlyDictionary<int, Cells> DigitsMap { get; }


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals(object? obj) => Equals(obj as AlmostHiddenSet);

	/// <inheritdoc/>
	public bool Equals([NotNullWhen(true)] AlmostHiddenSet? other)
		=> other is not null && DigitsMask == other.DigitsMask && Map == other.Map;

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		short mask = 0;
		int i = 0;
		foreach (int cell in HouseCells[House])
		{
			if (Map.Contains(cell))
			{
				mask |= (short)(1 << i);
			}

			i++;
		}

		return House << 18 | mask << 9 | (int)DigitsMask;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
	{
		string digitsStr = new DigitCollection(DigitsMask).ToSimpleString();
		string houseStr = new HouseCollection(House).ToString();
		return $"{digitsStr}/{Map} {R["KeywordIn"]} {houseStr}";
	}


	/// <inheritdoc/>
	public static AlmostHiddenSet[] Gather(scoped in Grid grid)
	{
		_ = grid is { EmptyCells: var emptyMap, CandidatesMap: var candidatesMap };

		var result = new List<AlmostHiddenSet>();

		for (int house = 0; house < 27; house++)
		{
			if ((HouseMaps[house] & emptyMap) is not { Count: >= 3 } tempMap)
			{
				continue;
			}

			short digitsMask = grid.GetDigitsUnion(tempMap);
			for (int size = 2; size < tempMap.Count - 1; size++)
			{
				foreach (int[] digitCombination in digitsMask.GetAllSets().GetSubsets(size))
				{
					var cells = Cells.Empty;
					foreach (int digit in digitCombination)
					{
						cells |= candidatesMap[digit] & HouseMaps[house];
					}
					if (cells.Count - 1 != size)
					{
						continue;
					}

					short finalDigitsMask = 0;
					foreach (int digit in digitCombination)
					{
						finalDigitsMask |= (short)(1 << digit);
					}

					short allDigitsMask = grid.GetDigitsUnion(cells);
					var finalMaps = new Cells?[9];
					for (int digit = 0; digit < 9; digit++)
					{
						if ((finalDigitsMask >> digit & 1) != 0 || (allDigitsMask >> digit & 1) != 0)
						{
							finalMaps[digit] = candidatesMap[digit] & cells;
						}
					}

					result.Add(new(finalDigitsMask, allDigitsMask, cells, finalMaps));
				}
			}
		}

		return result.ToArray();
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(AlmostHiddenSet left, AlmostHiddenSet right) => left.Equals(right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(AlmostHiddenSet left, AlmostHiddenSet right) => !(left == right);
}
