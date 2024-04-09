namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of the wing appeared in <see cref="UniqueRectangleWithWingStep"/>.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="UniqueRectangleWithWingStep"/>
public sealed class UniqueRectangleWingSizeFactor(StepSearcherOptions options) : Factor(options)
{
	/// <inheritdoc/>
	public override string FormulaString
		=> """
		{0} switch
		{{
			Technique.UniqueRectangleXyWing or Technique.AvoidableRectangleXyWing => 2,
			Technique.UniqueRectangleXyzWing or Technique.AvoidableRectangleXyzWing => 3,
			Technique.UniqueRectangleWxyzWing or Technique.AvoidableRectangleWxyzWing => 5
		}}
		""";

	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(UniqueRectangleWithWingStep.Code)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniqueRectangleWithWingStep);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			UniqueRectangleWithWingStep { Code: var code } => code switch
			{
				Technique.UniqueRectangleXyWing or Technique.AvoidableRectangleXyWing => 2,
				Technique.UniqueRectangleXyzWing or Technique.AvoidableRectangleXyzWing => 3,
				Technique.UniqueRectangleWxyzWing or Technique.AvoidableRectangleWxyzWing => 5
			},
			_ => null
		};
}
