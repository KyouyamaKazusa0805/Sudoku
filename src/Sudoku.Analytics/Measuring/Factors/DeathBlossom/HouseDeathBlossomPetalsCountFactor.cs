namespace Sudoku.Measuring.Factors;

/// <inheritdoc/>
public sealed class HouseDeathBlossomPetalsCountFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(HouseDeathBlossomStep.Branches)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(HouseDeathBlossomStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => A002024(((HouseBlossomBranchCollection)args![0]!).Count);
}
