using Sudoku.Collections;
using Sudoku.Data;

namespace Sudoku.Solving.Manual;

/// <summary>
/// Provides the extension methods that used and called while gathering the conclusions.
/// </summary>
public static class ConclusionGathering
{
	/// <summary>
	/// Creates an array of <see cref="Conclusion"/>s
	/// that uses the specified conclusion type and the digit used.
	/// </summary>
	/// <param name="this">The current instance.</param>
	/// <param name="digit">The digit.</param>
	/// <param name="conclusionType">
	/// The conclusion type. The default value is <see cref="ConclusionType.Elimination"/>.
	/// </param>
	/// <returns>The array of <see cref="Conclusion"/>s.</returns>
	public static Conclusion[] ToConclusions(
		this in Cells @this,
		int digit,
		ConclusionType conclusionType = ConclusionType.Elimination
	)
	{
		var result = new Conclusion[@this.Count];
		int[] offsets = @this.ToArray();
		for (int i = 0; i < @this.Count; i++)
		{
			result[i] = new(conclusionType, offsets[i], digit);
		}

		return result;
	}

	/// <summary>
	/// Creates an immutable array of <see cref="Conclusion"/>s
	/// that uses the specified conclusion type and the digit used.
	/// </summary>
	/// <param name="this">The current instance.</param>
	/// <param name="digit">The digit.</param>
	/// <param name="conclusionType">
	/// The conclusion type. The default value is <see cref="ConclusionType.Elimination"/>.
	/// </param>
	/// <returns>The immutable array of <see cref="Conclusion"/>s.</returns>
	public static ImmutableArray<Conclusion> ToImmutableConclusions(
		this in Cells @this,
		int digit,
		ConclusionType conclusionType = ConclusionType.Elimination
	)
	{
		var result = new Conclusion[@this.Count];
		int[] offsets = @this.ToArray();
		for (int i = 0; i < @this.Count; i++)
		{
			result[i] = new(conclusionType, offsets[i], digit);
		}

		return ImmutableArray.Create(result);
	}
}
