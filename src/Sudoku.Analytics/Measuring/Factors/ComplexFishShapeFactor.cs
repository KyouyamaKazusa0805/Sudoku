namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the shape of <see cref="ComplexFishStep"/>.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="ComplexFishStep"/>
public sealed class ComplexFishShapeFactor(StepSearcherOptions options) : Factor(options)
{
	/// <inheritdoc/>
	public override string FormulaString
		=> """
		{0}
			? {1} switch {{ 2 => 0, 3 or 4 => 11, 5 or 6 or 7 => 12, _ => 13 }}
			: {1} switch {{ 2 => 0, 3 or 4 => 14, 5 or 6 => 16, 7 => 17, _ => 20 }}
		""";

	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(ComplexFishStep.IsFranken), nameof(ComplexFishStep.Size)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(ComplexFishStep);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			ComplexFishStep { IsFranken: var isFranken, Size: var size } => isFranken switch
			{
				true => size switch { 2 => 0, 3 or 4 => 11, 5 or 6 or 7 => 12, _ => 13 },
				_ => size switch { 2 => 0, 3 or 4 => 14, 5 or 6 => 16, 7 => 17, _ => 20 }
			},
			_ => null
		};
}
