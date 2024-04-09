namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes cannibalism rule of <see cref="UniqueRectangleWithSueDeCoqStep"/>.
/// </summary>
/// <param name="options"><inheritdoc/></param>
/// <seealso cref="UniqueRectangleWithSueDeCoqStep"/>
public sealed class RectangleSueDeCoqCannibalismFactor(StepSearcherOptions options) : Factor(options)
{
	/// <inheritdoc/>
	public override string FormulaString => "{0} ? 1 : 0";

	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(UniqueRectangleWithSueDeCoqStep.IsCannibalistic)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniqueRectangleWithSueDeCoqStep);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			UniqueRectangleWithSueDeCoqStep { IsCannibalistic: var isCannibalism } => isCannibalism ? 1 : 0,
			_ => null
		};
}
