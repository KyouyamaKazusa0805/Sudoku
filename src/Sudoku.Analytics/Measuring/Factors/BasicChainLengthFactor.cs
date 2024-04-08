namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the length of a chain.
/// </summary>
/// <param name="options"><inheritdoc/></param>
public sealed class BasicChainLengthFactor(StepSearcherOptions options) : LengthFactor(options)
{
	/// <inheritdoc/>
	public override string FormulaString => "LengthDifficulty({0} - 2)";

	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(ChainingStep.Complexity)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(ChainingStep);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			ChainingStep { Complexity: var complexity } => GetLengthDifficulty(complexity - 2),
			_ => null
		};
}