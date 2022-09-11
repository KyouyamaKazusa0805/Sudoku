namespace Sudoku.Techniques.Patterns;

/// <summary>
/// Defines a data structure that describes an AHS.
/// </summary>
/// <remarks>
/// An <b>Almost Hidden Set</b> is a sudoku concept, which describes a case that
/// only <c>n</c> digits can be filled into <c>n + 1</c> cells in a house.
/// </remarks>
public sealed class AlmostHiddenSet :
	IEquatable<AlmostHiddenSet>,
	IEqualityOperators<AlmostHiddenSet, AlmostHiddenSet, bool>,
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
	internal AlmostHiddenSet(short digitMask, short allDigitsMask, scoped in CellMap map, CellMap?[] digitsMap)
	{
		(DigitsMask, Map, AllDigitsMask) = (digitMask, map, allDigitsMask);

		var tempDic = new Dictionary<int, CellMap>(9);
		for (var i = 0; i < digitsMap.Length; i++)
		{
			if (digitsMap[i] is { } digitMap)
			{
				tempDic.Add(i, digitMap);
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
			_ = Map.AllSetsAreInOneHouse(out var houseIndex);
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
	public CellMap Map { get; }

	/// <summary>
	/// Indicates all weak links in this AHS. The return value is described as an array of quadruples,
	/// indicating two digits, and cells used in the weak link.
	/// </summary>
	public (int Digit1, CellMap Cells1, int Digit2, CellMap Cells2)[] WeakLinks
	{
		get
		{
			var result = new List<(int, CellMap, int, CellMap)>();
			var unusedDigitsMask = (short)(AllDigitsMask & ~DigitsMask);
			foreach (var digitPair in unusedDigitsMask.GetAllSets().GetSubsets(2))
			{
				var digit1 = digitPair[0];
				var digit2 = digitPair[1];
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
	public IReadOnlyDictionary<int, CellMap> DigitsMap { get; }


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals(object? obj) => Equals(obj as AlmostHiddenSet);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals([NotNullWhen(true)] AlmostHiddenSet? other)
		=> other is not null && DigitsMask == other.DigitsMask && Map == other.Map;

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		short mask = 0;
		var i = 0;
		foreach (var cell in HouseCells[House])
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
		var digitsStr = DigitMaskFormatter.Format(DigitsMask);
		var houseStr = HouseFormatter.Format(1 << House);
		return $"{digitsStr}/{Map} {R["KeywordIn"]} {houseStr}";
	}


	/// <inheritdoc/>
	public static AlmostHiddenSet[] Gather(scoped in Grid grid)
	{
		_ = grid is { EmptyCells: var emptyMap, CandidatesMap: var candidatesMap };

		var result = new List<AlmostHiddenSet>();

		for (var house = 0; house < 27; house++)
		{
			if ((HousesMap[house] & emptyMap) is not { Count: >= 3 } tempMap)
			{
				continue;
			}

			var digitsMask = grid.GetDigitsUnion(tempMap);
			for (var size = 2; size < tempMap.Count - 1; size++)
			{
				foreach (var digitCombination in digitsMask.GetAllSets().GetSubsets(size))
				{
					var cells = CellMap.Empty;
					foreach (var digit in digitCombination)
					{
						cells |= candidatesMap[digit] & HousesMap[house];
					}
					if (cells.Count - 1 != size)
					{
						continue;
					}

					short finalDigitsMask = 0;
					foreach (var digit in digitCombination)
					{
						finalDigitsMask |= (short)(1 << digit);
					}

					var allDigitsMask = grid.GetDigitsUnion(cells);
					var finalMaps = new CellMap?[9];
					for (var digit = 0; digit < 9; digit++)
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
