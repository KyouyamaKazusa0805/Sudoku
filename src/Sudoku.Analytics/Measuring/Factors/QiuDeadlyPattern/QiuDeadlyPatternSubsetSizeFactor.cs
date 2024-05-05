namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of the subset appeared in <see cref="QiuDeadlyPatternType3Step"/>.
/// </summary>
/// <seealso cref="QiuDeadlyPatternType3Step"/>
public sealed partial class QiuDeadlyPatternSubsetSizeFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(IPatternType3StepTrait<QiuDeadlyPatternType3Step>.SubsetSize)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(QiuDeadlyPatternType3Step);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => (int)args![0]!;
}
