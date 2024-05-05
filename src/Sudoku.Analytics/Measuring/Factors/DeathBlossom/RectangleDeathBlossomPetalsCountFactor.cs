namespace Sudoku.Measuring.Factors;

/// <inheritdoc/>
public sealed class RectangleDeathBlossomPetalsCountFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(RectangleDeathBlossomStep.Branches)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(RectangleDeathBlossomStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => A002024(((RectangleBlossomBranchCollection)args![0]!).Count);
}
