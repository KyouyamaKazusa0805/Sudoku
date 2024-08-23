namespace Sudoku.Runtime.MeasuringServices.Factors;

/// <summary>
/// Represents a factor that describes the size of the subset appeared in <see cref="BivalueUniversalGraveType3Step"/>.
/// </summary>
/// <seealso cref="BivalueUniversalGraveType3Step"/>
public sealed class BivalueUniversalGraveSubsetSizeFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(IPatternType3StepTrait<BivalueUniversalGraveType3Step>.SubsetSize)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(BivalueUniversalGraveType3Step);

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<object?>, int> Formula => static args => (int)args![0]!;
}
