namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the number of branches appeared in a <see cref="MultiBranchWWingStep"/>.
/// </summary>
/// <seealso cref="MultiBranchWWingStep"/>
public sealed class MultiBranchWWingBranchesCountFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(MultiBranchWWingStep.Size)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(MultiBranchWWingStep);

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<object?>, int> Formula => static args => (int)args![0]! == 3 ? 3 : 0;
}
