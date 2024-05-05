namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of <see cref="ComplexFishStep"/>.
/// </summary>
/// <seealso cref="ComplexFishStep"/>
public sealed partial class ComplexFishSizeFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(ComplexFishStep.Size)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(ComplexFishStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula
		=> static args => (int)args![0]! switch { 2 => 0, 3 => 6, 4 => 20, 5 => 33, 6 => 45, 7 => 56, _ => 66 };
}
