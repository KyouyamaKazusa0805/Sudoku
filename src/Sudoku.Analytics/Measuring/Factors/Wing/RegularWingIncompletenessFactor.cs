namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes incompleteness of <see cref="RegularWingStep"/>.
/// </summary>
/// <seealso cref="RegularWingStep"/>
public sealed class RegularWingIncompletenessFactor : Factor
{
	/// <inheritdoc/>
	public override string FormulaString
		=> """
		({0}, {1}) switch
		{{
			(Technique.XyWing, _) => 0,
			(Technique.XyzWing, _) => 2,
			(_, true) => 1,
			_ => 0
		}}
		""";

	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(RegularWingStep.Code), nameof(RegularWingStep.IsIncomplete)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(RegularWingStep);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			RegularWingStep { Code: var code, IsIncomplete: var incompleteness } => (code, incompleteness) switch
			{
				(Technique.XyWing, _) => 0,
				(Technique.XyzWing, _) => 2,
				(_, true) => 1,
				_ => 0
			},
			_ => null
		};
}
