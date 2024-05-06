namespace Sudoku.Measuring.Factors;

/// <inheritdoc/>
public sealed partial class HouseDeathBlossomPetalsCountFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(IBranchTrait.BranchesCount)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(HouseDeathBlossomStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => A002024((int)args![0]!);
}
