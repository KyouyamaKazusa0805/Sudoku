namespace Sudoku.Runtime.MeasuringServices.Factors;

/// <inheritdoc/>
public sealed class HouseDeathBlossomPetalsCountFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(IBranchTrait.BranchesCount)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(HouseDeathBlossomStep);

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<object?>, int> Formula => static args => OeisSequences.A002024((int)args![0]!);
}
