namespace Sudoku.Analytics.InternalHelpers;

/// <summary>
/// Used by <see cref="NormalFishStepSearcher"/> and <see cref="ComplexFishStepSearcher"/>.
/// </summary>
/// <seealso cref="NormalFishStepSearcher"/>
/// <seealso cref="ComplexFishStepSearcher"/>
internal static class FishStepSearcherHelper
{
	/// <summary>
	/// Check whether the fish is sashimi.
	/// </summary>
	/// <param name="baseSets">The base sets.</param>
	/// <param name="fins">All fins.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>
	/// <para>A <see cref="bool"/> value indicating that.</para>
	/// <para>
	/// <inheritdoc cref="ComplexFishStep.IsSashimi" path="/remarks"/>
	/// </para>
	/// </returns>
	public static bool? IsSashimi(int[] baseSets, scoped in CellMap fins, int digit)
	{
		if (!fins)
		{
			return null;
		}

		var isSashimi = false;
		foreach (var baseSet in baseSets)
		{
			if ((HousesMap[baseSet] - fins & CandidatesMap[digit]).Count == 1)
			{
				isSashimi = true;
				break;
			}
		}

		return isSashimi;
	}

	/// <summary>
	/// Try to get the <see cref="Technique"/> code instance from the specified name, where the name belongs
	/// to a complex fish name, such as "Finned Franken Swordfish".
	/// </summary>
	/// <param name="name">The name.</param>
	/// <returns>The <see cref="Technique"/> code instance.</returns>
	/// <seealso cref="Technique"/>
	public static unsafe Technique GetComplexFishTechniqueCodeFromName(string name)
	{
		// Creates a buffer to store the characters that isn't a space or a bar.
		var buffer = stackalloc char[name.Length];
		var bufferLength = 0;
		fixed (char* p = name)
		{
			for (var ptr = p; *ptr != '\0'; ptr++)
			{
				if (*ptr is not ('-' or ' '))
				{
					buffer[bufferLength++] = *ptr;
				}
			}
		}

		// Parses via the buffer, and returns the result.
		return Enum.Parse<Technique>(new string(PointerOperations.GetArrayFromStart(buffer, bufferLength, 0)));
	}
}
