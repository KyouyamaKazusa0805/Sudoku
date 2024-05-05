namespace Sudoku.Measuring.Factors;

/// <inheritdoc/>
public sealed class NTimesAlmostLockedSetsDeathBlossomPetalsCountFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(NTimesAlmostLockedSetDeathBlossomStep.Branches)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(NTimesAlmostLockedSetDeathBlossomStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => A002024(((NTimesAlmostLockedSetsBlossomBranchCollection)args![0]!).Count);
}
