namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the length of <see cref="UniqueLoopStep"/>.
/// </summary>
/// <seealso cref="UniqueLoopStep"/>
public sealed class UniqueLoopLengthFactor : Factor
{
	/// <inheritdoc/>
	public override string FormulaString => "A004526({0}.Count) - 3";

	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(UniqueLoopStep.Loop)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniqueLoopStep);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			UniqueLoopStep { Loop.Count: var count } => A004526(count) - 3,
			_ => null
		};
}
