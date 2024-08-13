namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the number of conjugate pairs used in a <see cref="UniqueLoopConjugatePairsTypeStep"/>.
/// </summary>
/// <seealso cref="UniqueLoopConjugatePairsTypeStep"/>
public sealed class UniqueLoopConjugatePairsCountFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(UniqueLoopConjugatePairsTypeStep.ConjugatePairsCount)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniqueLoopConjugatePairsTypeStep);

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<object?>, int> Formula => static args => (int)args![0]!;
}
