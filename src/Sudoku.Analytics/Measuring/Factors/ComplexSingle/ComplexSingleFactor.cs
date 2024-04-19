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
				ComplexSingleStep { IndirectTechniques: var techniques } => d(from technique in techniques select technique[0]),
				_ => null
			};


			static int d(Technique[] techniques)
			{
				var (result, ratingScale) = (0D, 1D);
				foreach (var technique in techniques)
				{
					technique.GetDefaultRating(out var directRatingValue);
					result += directRatingValue / ratingScale;
					ratingScale *= 3;
				}
				return (int)result;
			}
		}
	}
}
