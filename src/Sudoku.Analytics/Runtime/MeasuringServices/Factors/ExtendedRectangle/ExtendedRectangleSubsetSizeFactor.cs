namespace Sudoku.Runtime.MeasuringServices.Factors;

/// <summary>
/// Represents a factor that describes the size of the subset appeared in <see cref="ExtendedRectangleType3Step"/>.
/// </summary>
/// <seealso cref="ExtendedRectangleType3Step"/>
public sealed class ExtendedRectangleSubsetSizeFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(IPatternType3StepTrait<ExtendedRectangleType3Step>.SubsetSize)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(ExtendedRectangleType3Step);

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<object?>, int> Formula => static args => (int)args![0]!;
}
