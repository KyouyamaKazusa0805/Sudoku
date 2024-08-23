namespace Sudoku.Runtime.MeasuringServices.Factors;

/// <summary>
/// Represents a factor that describes cannibalism rule of <see cref="ExtendedRectangleType3Step"/>.
/// </summary>
/// <seealso cref="ExtendedRectangleType3Step"/>
public sealed class ExtendedRectangleCannibalismFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(ExtendedRectangleType3Step.IsCannibalism)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(ExtendedRectangleType3Step);

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<object?>, int> Formula => static args => (bool)args![0]! ? 2 : 0;
}
