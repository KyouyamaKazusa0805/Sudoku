namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of the subset appeared in <see cref="BorescoperDeadlyPatternType3Step"/>.
/// </summary>
/// <seealso cref="BorescoperDeadlyPatternType3Step"/>
public sealed class BorescoperDeadlyPatternSubsetSizeFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(IPatternType3StepTrait<BorescoperDeadlyPatternType3Step>.SubsetSize)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(BorescoperDeadlyPatternType3Step);

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<object?>, int> Formula => static args => (int)args![0]!;
}
