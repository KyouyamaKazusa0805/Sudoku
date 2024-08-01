namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes cannibalism rule of <see cref="SueDeCoqStep"/>.
/// </summary>
/// <seealso cref="SueDeCoqStep"/>
public sealed class SueDeCoqCannibalismFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(SueDeCoqStep.IsCannibalistic)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(SueDeCoqStep);

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<object?>, int> Formula => static args => (bool)args![0]! ? 1 : 0;
}
