using Sudoku.Collections;
using static System.Numerics.BitOperations;
using static Sudoku.Constants.Tables;
using static Sudoku.Solving.Manual.Buffer.FastProperties;
using static Sudoku.Solving.Manual.Constants;

namespace Sudoku.Solving.Collections;

/// <summary>
/// Encapsulates a normal almost locked set (ALS as its abberivation).
/// </summary>
/// <param name="DigitsMask">Indicates the mask of each digit.</param>
/// <param name="Map">Indicates the map that ALS lying on.</param>
/// <param name="PossibleEliminationSet">Indicates the possible elimination set.</param>
/// <param name="IsBivalueCell">Indicates whether this instance is a bi-value-cell ALS.</param>
/// <param name="Region">Indicates the region that the instance lies in.</param>
public readonly record struct AlmostLockedSet(
	short DigitsMask,
	in Cells Map,
	in Cells PossibleEliminationSet,
	bool IsBivalueCell,
	int Region = -1
)
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
	/// <param name="possibleEliminationSet">The possible elimination set.</param>
	public AlmostLockedSet(short digitMask, in Cells map, in Cells possibleEliminationSet) :
		this(digitMask, map, possibleEliminationSet, map.Count == 1, InitRegionProperty(map))
	{
	}


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
	/// Determine whether the specified <see cref="AlmostLockedSet"/> instance holds the same
	/// <see cref="DigitsMask"/> and <see cref="Map"/> property values as the current instance.
	/// </summary>
	/// <param name="other">The instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(in AlmostLockedSet other) => DigitsMask == other.DigitsMask && Map == other.Map;

	/// <summary>
	/// Indicates whether the specified grid contains the digit.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="digit">The digit.</param>
	/// <param name="result">The result.</param>
	/// <returns>A <see cref="bool"/> value.</returns>
	public bool ContainsDigit(in Grid grid, int digit, out Cells result)
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
	/// <b>should</b> use method <see cref="Equals(in AlmostLockedSet)"/> instead of this method.
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
	public override string ToString() =>
		IsBivalueCell
			? $"{new DigitCollection(DigitsMask).ToString(null)}/{Map}"
			: $"{new DigitCollection(DigitsMask).ToString(null)}/{Map} in {new RegionCollection(Region).ToString()}";


	/// <summary>
	/// To search for all ALSes from the specified grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <returns>All ALSes searched.</returns>
	/// <remarks>
	/// <see cref="AlmostLockedSet"/> is a large-object type. If you want to iterate them,
	/// you can use the new feature '<see langword="ref"/> and <see langword="ref readonly"/> iteration variable' to do so, just call
	/// the extension method <see cref="ArrayExtensions.EnumerateRef{T}(T[])"/>. Then you can get:
	/// <code><![CDATA[
	/// var collection = GetAllAlses(grid);
	/// foreach (ref readonly var als in collection.EnumerateRef())
	/// {
	///     // ...
	/// }
	/// ]]></code>
	/// </remarks>
	/// <seealso cref="ArrayExtensions.EnumerateRef{T}(T[])"/>
	public static AlmostLockedSet[] Gather(in Grid grid)
	{
		// Get all bi-value-cell ALSes.
		var result = new List<AlmostLockedSet>();
		foreach (int cell in BivalueMap)
		{
			result.Add(new(grid.GetCandidates(cell), new() { cell }, PeerMaps[cell] & EmptyMap));
		}

		// Get all non-bi-value-cell ALSes.
		for (int region = 0; region < 27; region++)
		{
			if ((RegionMaps[region] & EmptyMap) is not { Count: >= 3 } tempMap)
			{
				continue;
			}

			for (int size = 2; size <= tempMap.Count - 1; size++)
			{
				foreach (var map in tempMap & size)
				{
					short blockMask = map.BlockMask;
					if (IsPow2(blockMask) && region >= 9)
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

	/// <summary>
	/// To initialize the property <see cref="Region"/>.
	/// </summary>
	/// <param name="map">The value of property <see cref="Map"/>.</param>
	/// <returns>The region value.</returns>
	/// <seealso cref="Region"/>
	/// <seealso cref="Map"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int InitRegionProperty(in Cells map)
	{
		map.AllSetsAreInOneRegion(out int region);
		return region;
	}


	/// <summary>
	/// Determine whether two <see cref="AlmostLockedSet"/>s hold a same <see cref="DigitsMask"/>
	/// and <see cref="Map"/> property values.
	/// </summary>
	/// <param name="left">The left-side instance to compare.</param>
	/// <param name="right">The right-side instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(in AlmostLockedSet left, in AlmostLockedSet right) => left.Equals(right);

	/// <summary>
	/// Determine whether two <see cref="AlmostLockedSet"/>s don't hold a same <see cref="DigitsMask"/>
	/// and <see cref="Map"/> property values.
	/// </summary>
	/// <param name="left">The left-side instance to compare.</param>
	/// <param name="right">The right-side instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(in AlmostLockedSet left, in AlmostLockedSet right) => !(left == right);
}
