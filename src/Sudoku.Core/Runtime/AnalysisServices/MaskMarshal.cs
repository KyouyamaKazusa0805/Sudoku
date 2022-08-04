namespace Sudoku.Runtime.AnalysisServices;

/// <summary>
/// Provides with a serial of methods that handles with masks.
/// </summary>
public static class MaskMarshal
{
	/// <summary>
	/// Get all mask combinations.
	/// </summary>
	/// <param name="value">The mask.</param>
	/// <returns>The result list.</returns>
	public static short[] GetMaskSubsets(short value)
	{
		short[][] maskSubsets = new short[9][];
		for (int size = 1; size <= 9; size++)
		{
			maskSubsets[size - 1] = GetMaskSubsets(value, size);
		}

		var listOfResults = new List<short>();
		foreach (short[] maskSubset in maskSubsets)
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
		short[] result = new short[listToIterate.Count];
		int index = 0;
		foreach (int[] target in listToIterate)
		{
			short mask = 0;
			foreach (int targetValue in target)
			{
				mask |= (short)(1 << targetValue);
			}
			result[index++] = mask;
		}

		return result;
	}
}
