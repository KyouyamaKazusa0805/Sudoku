namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes cannibalism rule of <see cref="SueDeCoqStep"/>.
/// </summary>
/// <seealso cref="SueDeCoqStep"/>
public sealed class SueDeCoqCannibalismFactor : Factor
{
	/// <inheritdoc/>
	public override string FormulaString => "{0} ? 1 : 0";

	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(SueDeCoqStep.IsCannibalistic)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(SueDeCoqStep);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			SueDeCoqStep { IsCannibalistic: var isCannibalism } => isCannibalism ? 1 : 0,
			_ => null
		};
}
