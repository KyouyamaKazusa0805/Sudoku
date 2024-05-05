namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the length of <see cref="UniqueLoopStep"/>.
/// </summary>
/// <seealso cref="UniqueLoopStep"/>
public sealed class UniqueLoopLengthFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(UniqueLoopStep.Loop)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniqueLoopStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => A004526(((CellMap)args![0]!).Count) - 3;
}
