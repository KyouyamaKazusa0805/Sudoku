namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes whether the <see cref="ComplexFishStep"/> is cannibalism.
/// </summary>
/// <seealso cref="ComplexFishStep"/>
public sealed class ComplexFishCannibalismFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(ComplexFishStep.IsCannibalism)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(ComplexFishStep);

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<object?>, int> Formula => static args => (bool)args![0]! ? 3 : 0;
}
