namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the extra cells used in <see cref="UniquenessClueCoverStep"/>.
/// </summary>
/// <seealso cref="UniquenessClueCoverStep"/>
public sealed partial class UniquenessClueCoverExtraCellsFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(UniquenessClueCoverStep.ExtraCells)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniquenessClueCoverStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => A004526(((CellMap)args![0]!).Count);
}
