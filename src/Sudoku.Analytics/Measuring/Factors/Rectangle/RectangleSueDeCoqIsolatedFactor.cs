namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the isolated rule of <see cref="UniqueRectangleWithSueDeCoqStep"/>.
/// </summary>
/// <seealso cref="UniqueRectangleWithSueDeCoqStep"/>
public sealed class RectangleSueDeCoqIsolatedFactor : Factor
{
	/// <inheritdoc/>
	public override string FormulaString => "!{0} && {1} != 0 ? 1 : 0";

	/// <inheritdoc/>
	public override string[] ParameterNames
		=> [nameof(UniqueRectangleWithSueDeCoqStep.IsCannibalistic), nameof(UniqueRectangleWithSueDeCoqStep.IsolatedDigitsMask)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniqueRectangleWithSueDeCoqStep);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			UniqueRectangleWithSueDeCoqStep { IsCannibalistic: var isCannibalism, IsolatedDigitsMask: var mask }
				=> !isCannibalism && mask != 0 ? 1 : 0,
			_ => null
		};
}
