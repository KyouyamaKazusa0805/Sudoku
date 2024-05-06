namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the guardian in <see cref="GuardianStep"/>.
/// </summary>
/// <seealso cref="GuardianStep"/>
public sealed partial class GuardianFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(ICellListTrait.CellSize), nameof(IGuardianTrait.GuardianCellsCount)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(GuardianStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula
		=> static args => OeisSequences.A004526((int)args![0]! + OeisSequences.A004526((int)args![1]!));
}
