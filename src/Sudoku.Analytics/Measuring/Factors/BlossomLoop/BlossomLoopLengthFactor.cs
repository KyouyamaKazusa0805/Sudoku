namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the length property of <see cref="BlossomLoopStep"/>.
/// </summary>
/// <seealso cref="BlossomLoopStep"/>
public sealed class BlossomLoopLengthFactor : LengthFactor
{
	/// <inheritdoc/>
	public override string FormulaString => "LengthDifficulty({0}.Potentials.Sum(AncestorsCount))";

	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(BlossomLoopStep.Chains)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(BlossomLoopStep);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			BlossomLoopStep { Chains.Potentials: var potentials }
				=> GetLengthDifficulty(potentials.Sum(ChainingStep.AncestorsCountOf)),
			_ => null
		};
}
