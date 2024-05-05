namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of regular wing comes from <see cref="RegularWingStep"/>.
/// </summary>
/// <seealso cref="RegularWingStep"/>
public sealed partial class RegularWingSizeFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(RegularWingStep.Size)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(RegularWingStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula
		=> static args => (int)args![0]! switch { 3 => 0, 4 => 2, 5 => 4, 6 => 7, 7 => 10, 8 => 13, 9 => 16, _ => 20 };
}
