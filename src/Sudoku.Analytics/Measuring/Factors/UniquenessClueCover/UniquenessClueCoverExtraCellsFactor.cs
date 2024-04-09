namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the extra cells used in <see cref="UniquenessClueCoverStep"/>.
/// </summary>
/// <seealso cref="UniquenessClueCoverStep"/>
public sealed class UniquenessClueCoverExtraCellsFactor : Factor
{
	/// <inheritdoc/>
	public override string FormulaString => "A004526({0}.Count)";

	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(UniquenessClueCoverStep.ExtraCells)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniquenessClueCoverStep);

	/// <inheritdoc/>
	public override Func<Step, int?> Formula
		=> static step => step switch
		{
			UniquenessClueCoverStep { ExtraCells.Count: var count } => A004526(count),
			_ => null
		};
}
