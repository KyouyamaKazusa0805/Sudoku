namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes whether the subset appeare in <see cref="BivalueUniversalGraveType3Step"/> is hidden.
/// </summary>
/// <seealso cref="BivalueUniversalGraveType3Step"/>
public sealed partial class BivalueUniversalGraveSubsetIsHiddenFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(IPatternType3StepTrait<BivalueUniversalGraveType3Step>.IsHidden)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(BivalueUniversalGraveType3Step);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => (bool)args![0]! ? 1 : 0;
}
