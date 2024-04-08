namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the length of <see cref="BowmanBingoStep"/>.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="BowmanBingoStep"/>
public sealed class BowmanBingoLengthFactor(StepSearcherOptions options) : LengthFactor(options)
{
	/// <inheritdoc/>
	public override string FormulaString => "LengthDifficulty({0}.Length)";

	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(BowmanBingoStep.ContradictionLinks)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(BowmanBingoStep);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			BowmanBingoStep { ContradictionLinks.Length: var length } => GetLengthDifficulty(length),
			_ => null
		};
}
