namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the factor for <see cref="ComplexSingleStep"/>.
/// </summary>
/// <seealso cref="ComplexSingleStep"/>
public sealed class ComplexSingleFactor : Factor
{
	/// <inheritdoc/>
	public override string FormulaString => "GetComplexSingleRating({0})";

	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(ComplexSingleStep.IndirectTechniques)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(ComplexSingleStep);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
	{
		get
		{
			return static step => step switch
			{
				ComplexSingleStep { IndirectTechniques: var techniques }
					=> d(from techniqueGroup in techniques from technique in techniqueGroup select technique),
				_ => null
			};


			static int d(Technique[] techniques)
			{
				var (result, max) = (0, 0);
				foreach (var technique in techniques)
				{
					technique.GetDefaultRating(out var directRatingValue);
					result += directRatingValue;
					if (directRatingValue >= max)
					{
						max = directRatingValue;
					}
				}
				return (max >> 1) + LengthFactor.GetLengthDifficulty(result);
			}
		}
	}
}
