namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes incompleteness of <see cref="RegularWingStep"/>.
/// </summary>
/// <seealso cref="RegularWingStep"/>
public sealed partial class RegularWingIncompletenessFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(RegularWingStep.Code), nameof(RegularWingStep.IsIncomplete)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(RegularWingStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula
		=> static args => ((Technique)args![0]!, (bool)args![1]!) switch
		{
			(Technique.XyWing, _) => 0,
			(Technique.XyzWing, _) => 2,
			(_, true) => 1,
			_ => 0
		};
}
