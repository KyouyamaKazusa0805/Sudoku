namespace Sudoku.Measuring.Factors;

/// <inheritdoc/>
public sealed partial class BasicDeathBlossomPetalsCountFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(DeathBlossomStep.Branches)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(DeathBlossomStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => A002024(((NormalBlossomBranchCollection)args![0]!).Count);
}
