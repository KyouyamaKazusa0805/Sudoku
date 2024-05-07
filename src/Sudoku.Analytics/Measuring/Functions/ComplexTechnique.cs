namespace Sudoku.Measuring.Functions;

/// <summary>
/// Represents a function that calculates for complex technique rules.
/// </summary>
public sealed partial class ComplexTechniqueUsages : IFunctionProvider
{
	/// <summary>
	/// Gets the complexity difficulty for complex single techniques usages.
	/// </summary>
	/// <param name="techniques">The techniques used.</param>
	/// <returns>The result.</returns>
	[ExportFunction("ComplexityDifficulty")]
	public static int GetComplexityDifficulty(Technique[][] techniques)
	{
		var result = 0;
		foreach (var technique in from techniqueGroup in techniques from technique in techniqueGroup select technique)
		{
			technique.GetDefaultRating(out var directRatingValue);
			result += directRatingValue;
		}
		return result;
	}
}
