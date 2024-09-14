namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of a <see cref="NormalFishStep"/>.
/// </summary>
/// <seealso cref="NormalFishStep"/>
public sealed class NormalFishSizeFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(NormalFishStep.Size)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(NormalFishStep);

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<object?>, int> Formula => static args => (int)args![0]! switch { 2 => 0, 3 => 6, 4 => 20 };
}
