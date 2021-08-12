namespace Sudoku.Solving.Manual.Alses;

/// <summary>
/// Encapsulates a normal ALS.
/// </summary>
[AutoDeconstruct(nameof(Region), nameof(DigitsMask), nameof(Map))]
[AutoDeconstruct(nameof(IsBivalueCell), nameof(Region), nameof(DigitsMask), nameof(Map), nameof(PossibleEliminationSet), nameof(StrongLinksMask))]
[AutoEquality(nameof(DigitsMask), nameof(Map))]
public readonly partial struct Als : IValueEquatable<Als>
{
	/// <summary>
	/// Indicates an array of the total number of the strong relations in an ALS of the different size.
	/// The field is only unsed in the property <see cref="StrongLinksMask"/>.
	/// </summary>
	/// <seealso cref="StrongLinksMask"/>
	private static readonly int[] StrongRelationsCount = new[] { 0, 1, 3, 6, 10, 15, 21, 34, 45 };


	/// <summary>
	/// Initializes an instance with the specified digit mask and the map of cells.
	/// </summary>
	/// <param name="digitMask">The digit mask.</param>
	/// <param name="map">The map.</param>
	public Als(short digitMask, in Cells map) : this(digitMask, map, Cells.Empty)
	{
	}

	/// <summary>
	/// Initializes an instance with the specified digit mask and the map of cells.
	/// </summary>
	/// <param name="digitMask">The digit mask.</param>
	/// <param name="map">The map.</param>
	/// <param name="possibleEliminationSet">
	/// The possible elimination set.
	/// </param>
	public Als(short digitMask, in Cells map, in Cells possibleEliminationSet)
	{
		DigitsMask = digitMask;
		Map = map;
		IsBivalueCell = map.Count == 1;
		PossibleEliminationSet = possibleEliminationSet;
		Map.AllSetsAreInOneRegion(out int region);
		Region = region;
	}


	/// <summary>
	/// Indicates whether this instance is a bi-value-cell ALS.
	/// </summary>
	public bool IsBivalueCell { get; }

	/// <summary>
	/// Indicates the region that the instance lies in.
	/// </summary>
	public int Region { get; }

	/// <summary>
	/// Indicates the mask of each digit.
	/// </summary>
	public short DigitsMask { get; }

	/// <summary>
	/// Indicates the map that ALS lying on.
	/// </summary>
	public Cells Map { get; }

	/// <summary>
	/// Indicates the possible elimination set.
	/// </summary>
	public Cells PossibleEliminationSet { get; }

	/// <summary>
	/// Indicates all strong links in this ALS. The result will be represented
	/// as a <see cref="short"/> mask of 9 bits indicating which bits used.
	/// </summary>
	public unsafe short[] StrongLinksMask
	{
		get
		{
			var digits = DigitsMask.GetAllSets();
			short[] result = new short[StrongRelationsCount[digits.Length - 1]];
			fixed (int* pDigits = digits)
			{
				for (int i = 0, x = 0, length = digits.Length, iterationLength = length - 1; i < iterationLength; i++)
				{
					for (int j = i + 1; j < length; j++)
					{
						result[x++] = (short)(1 << pDigits[i] | 1 << pDigits[j]);
					}
				}
			}

			return result;
		}
	}


	/// <summary>
	/// Indicates whether the specified grid contains the digit.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="digit">The digit.</param>
	/// <param name="result">The result.</param>
	/// <returns>A <see cref="bool"/> value.</returns>
	public bool ContainsDigit(in SudokuGrid grid, int digit, out Cells result)
	{
		result = Cells.Empty;
		foreach (int cell in Map)
		{
			if ((grid.GetCandidates(cell) >> digit & 1) != 0)
			{
				result.AddAnyway(cell);
			}
		}

		return !result.IsEmpty;
	}

	/// <inheritdoc cref="object.GetHashCode"/>
	/// <remarks>
	/// If you want to determine the equality of two instance, I recommend you
	/// <b>should</b> use method <see cref="Equals(in Als)"/> instead of this method.
	/// </remarks>
	public override int GetHashCode()
	{
		short mask = 0;
		int i = 0;
		foreach (int cell in RegionCells[Region])
		{
			if (Map.Contains(cell))
			{
				mask |= (short)(1 << i);
			}

			i++;
		}

		return Region << 18 | mask << 9 | (int)DigitsMask;
	}

	/// <inheritdoc/>
	public override string ToString()
	{
		var sb = new ValueStringBuilder(stackalloc char[50]);

		if (IsBivalueCell)
		{
			sb.Append(new DigitCollection(DigitsMask).ToString(null));
			sb.Append('/');
			sb.Append(Map.ToString());
		}
		else
		{
			sb.Append(new DigitCollection(DigitsMask).ToString(null));
			sb.Append('/');
			sb.Append(Map.ToString());
			sb.Append(' ');
			sb.Append("in");
			sb.Append(' ');
			sb.Append(new RegionCollection(Region).ToString());
		}

		return sb.ToString();
	}


	/// <summary>
	/// To search for all ALSes in the specified grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>All ALSes searched.</returns>
	public static Als[] GetAllAlses(in SudokuGrid grid)
	{
		// Get all bi-value-cell ALSes.
		var result = new List<Als>();
		foreach (int cell in BivalueMap)
		{
			result.Add(new(grid.GetCandidates(cell), new() { cell }, PeerMaps[cell] & EmptyMap));
		}

		// Get all non-bi-value-cell ALSes.
		var list = new List<int>();
		for (int region = 0; region < 27; region++)
		{
			if ((RegionMaps[region] & EmptyMap) is not { Count: >= 3 } tempMap)
			{
				continue;
			}

			int[] emptyCells = tempMap.ToArray();
			list.Clear();
			list.AddRange(emptyCells);
			for (int size = 2; size <= emptyCells.Length - 1; size++)
			{
				foreach (Cells map in list.GetSubsets(size))
				{
					short blockMask = map.BlockMask;
					if (blockMask != 0 && (blockMask & blockMask - 1) == 0 && region >= 9)
					{
						// All ALS cells lying on a box-row or a box-column
						// will be processed as a block ALS.
						continue;
					}

					// Get all candidates in these cells.
					short digitsMask = 0;
					foreach (int cell in map)
					{
						digitsMask |= grid.GetCandidates(cell);
					}
					if (PopCount((uint)digitsMask) - 1 != size)
					{
						continue;
					}

					int coveredLine = map.CoveredLine;
					result.Add(
						new(
							digitsMask,
							map,
							region < 9 && coveredLine is >= 9 and not InvalidFirstSet
							? ((RegionMaps[region] | RegionMaps[coveredLine]) & EmptyMap) - map
							: tempMap - map
						)
					);
				}
			}
		}

		return result.ToArray();
	}
}
