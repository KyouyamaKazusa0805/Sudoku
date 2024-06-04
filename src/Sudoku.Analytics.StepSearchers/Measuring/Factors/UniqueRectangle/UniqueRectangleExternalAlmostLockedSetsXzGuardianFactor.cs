namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of guardians appeared in <see cref="UniqueRectangleExternalAlmostLockedSetsXzStep"/>.
/// </summary>
/// <seealso cref="UniqueRectangleExternalAlmostLockedSetsXzStep"/>
public sealed class UniqueRectangleExternalAlmostLockedSetsXzGuardianFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(IGuardianTrait.GuardianCellsCount)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniqueRectangleExternalAlmostLockedSetsXzStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => OeisSequences.A004526((int)args![0]!);
}
