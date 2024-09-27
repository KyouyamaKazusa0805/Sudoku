namespace Sudoku.Measuring.Functions;

/// <summary>
/// Represents a function that calculates for complex technique rules.
/// </summary>
public sealed class ComplexTechniqueUsages : IFunctionProvider
{
	/// <summary>
	/// Gets the complexity difficulty for complex single techniques usages.
	/// </summary>
	/// <param name="techniques">The techniques used.</param>
	/// <returns>The result.</returns>
	public static int GetComplexityDifficulty(Technique[][] techniques)
	{
		var result = 0;
		var p = 1;
		foreach (var technique in
			from techniqueGroup in techniques
			from technique in techniqueGroup.AsReadOnlySpan()
			select technique)
		{
			technique.GetDefaultRating(out var directRatingValue);
			result += (int)(directRatingValue / (double)p);
			p *= 3;
		}
		return result;
	}
}
