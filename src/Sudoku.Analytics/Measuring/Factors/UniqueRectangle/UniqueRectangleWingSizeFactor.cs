namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of the wing appeared in <see cref="UniqueRectangleWithWingStep"/>.
/// </summary>
/// <seealso cref="UniqueRectangleWithWingStep"/>
public sealed partial class UniqueRectangleWingSizeFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(UniqueRectangleWithWingStep.Code)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniqueRectangleWithWingStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula
		=> static args => (Technique)args![0]! switch
		{
			Technique.UniqueRectangleXyWing or Technique.AvoidableRectangleXyWing => 2,
			Technique.UniqueRectangleXyzWing or Technique.AvoidableRectangleXyzWing => 3,
			Technique.UniqueRectangleWxyzWing or Technique.AvoidableRectangleWxyzWing => 5
		};
}
