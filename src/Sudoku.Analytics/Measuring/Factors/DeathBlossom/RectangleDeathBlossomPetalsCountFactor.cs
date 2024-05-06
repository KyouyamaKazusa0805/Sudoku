namespace Sudoku.Measuring.Factors;

/// <inheritdoc/>
public sealed partial class RectangleDeathBlossomPetalsCountFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(IBranchTrait.BranchesCount)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(RectangleDeathBlossomStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => OeisSequences.A002024((int)args![0]!);
}
