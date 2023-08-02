namespace Sudoku.Runtime.MaskServices;

/// <summary>
/// Provides with a set of methods that operates with mask defined in basic sudoku concepts, as data structures.
/// </summary>
public static class MaskOperations
{
	/// <summary>
	/// To get the digits that the current mask represents for. The mask must be between 0 and 512, and exclude 512.
	/// </summary>
	/// <param name="digitMask">The digit mask.</param>
	/// <returns>The digits returned.</returns>
	public static ReadOnlySpan<Digit> MaskToDigits(Mask digitMask)
	{
		var result = new Digit[PopCount((uint)digitMask)];
		for (var (i, p) = (0, 0); i < 9; i++)
		{
			if ((digitMask >> i & 1) != 0)
			{
				result[p++] = i;
			}
		}

		return result;
	}

	/// <summary>
	/// To get the cell status for a mask value. The mask is an inner representation to describe a cell's state.
	/// For more information please visit the details of the design for type <see cref="Grid"/>.
	/// </summary>
	/// <param name="mask">The mask.</param>
	/// <returns>The cell status.</returns>
	/// <seealso cref="Grid"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellStatus MaskToStatus(Mask mask) => (CellStatus)(mask >> 9 & 7);

	/// <summary>
	/// Get all mask combinations.
	/// </summary>
	/// <param name="value">The mask.</param>
	/// <returns>The result list.</returns>
	public static Mask[] GetMaskSubsets(Mask value)
	{
		var maskSubsets = new Mask[9][];
		for (var size = 1; size <= 9; size++)
		{
			maskSubsets[size - 1] = GetMaskSubsets(value, size);
		}

		return from maskSubset in maskSubsets from mask in maskSubset select mask;
	}

	/// <summary>
	/// Get all mask combinations.
	/// </summary>
	/// <param name="value">The mask.</param>
	/// <param name="size">The size.</param>
	/// <returns>The result list.</returns>
	public static Mask[] GetMaskSubsets(Mask value, int size)
	{
		var listToIterate = value.GetAllSets().GetSubsets(size);
		var (result, index) = (new Mask[listToIterate.Length], 0);
		foreach (var target in listToIterate)
		{
			var mask = (Mask)0;
			foreach (var targetValue in target)
			{
				mask |= (Mask)(1 << targetValue);
			}
			result[index++] = mask;
		}

		return result;
	}
}
