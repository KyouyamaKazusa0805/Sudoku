namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the factor for <see cref="ComplexSingleStep"/>.
/// </summary>
/// <seealso cref="ComplexSingleStep"/>
public sealed class ComplexSingleFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(ComplexSingleStep.IndirectTechniques)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(ComplexSingleStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula
		=> static args =>
		{
			var (result, max) = (0, 0);
			foreach (var technique in from techniqueGroup in (Technique[][])args![0]! from technique in techniqueGroup select technique)
			{
				technique.GetDefaultRating(out var directRatingValue);
				result += directRatingValue;
				if (directRatingValue >= max)
				{
					max = directRatingValue;
				}
			}
			return (max >> 1) + ChainingLength.GetLengthDifficulty(result);
		};
}
