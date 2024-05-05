namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the shape of <see cref="ComplexFishStep"/>.
/// </summary>
/// <seealso cref="ComplexFishStep"/>
public sealed class ComplexFishShapeFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(ComplexFishStep.IsFranken), nameof(ComplexFishStep.Size)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(ComplexFishStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula
		=> static args => (bool)args![0]!
			? (int)args[1]! switch { 2 => 0, 3 or 4 => 11, 5 or 6 or 7 => 12, _ => 13 }
			: (int)args[1]! switch { 2 => 0, 3 or 4 => 14, 5 or 6 => 16, 7 => 17, _ => 20 };
}
