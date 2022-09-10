namespace Sudoku.Runtime.MaskServices;

/// <summary>
/// Provides with a set of methods that operates with mask defined in basic sudoku concepts, as data structures.
/// </summary>
public static class MaskOperations
{
	/// <summary>
	/// To get the cell status for a mask value. The mask is an inner representation to describe a cell's state.
	/// For more information please visit the details of the design for type <see cref="Grid"/>.
	/// </summary>
	/// <param name="mask">The mask.</param>
	/// <returns>The cell status.</returns>
	/// <seealso cref="Grid"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellStatus MaskToStatus(short mask) => (CellStatus)(mask >> 9 & 7);

	/// <summary>
	/// Get all mask combinations.
	/// </summary>
	/// <param name="value">The mask.</param>
	/// <returns>The result list.</returns>
	public static short[] GetMaskSubsets(short value)
	{
		var maskSubsets = new short[9][];
		for (var size = 1; size <= 9; size++)
		{
			maskSubsets[size - 1] = GetMaskSubsets(value, size);
		}

		var listOfResults = new List<short>();
		foreach (var maskSubset in maskSubsets)
		{
			listOfResults.AddRange(maskSubset);
		}

		return listOfResults.ToArray();
	}

	/// <summary>
	/// Get all mask combinations.
	/// </summary>
	/// <param name="value">The mask.</param>
	/// <param name="size">The size.</param>
	/// <returns>The result list.</returns>
	public static short[] GetMaskSubsets(short value, int size)
	{
		var listToIterate = value.GetAllSets().GetSubsets(size);
		var result = new short[listToIterate.Count];
		var index = 0;
		foreach (var target in listToIterate)
		{
			short mask = 0;
			foreach (var targetValue in target)
			{
				mask |= (short)(1 << targetValue);
			}
			result[index++] = mask;
		}

		return result;
	}
}
