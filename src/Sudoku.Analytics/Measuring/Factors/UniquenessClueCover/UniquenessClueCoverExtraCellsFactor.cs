namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the extra cells used in <see cref="UniquenessClueCoverStep"/>.
/// </summary>
/// <seealso cref="UniquenessClueCoverStep"/>
public sealed class UniquenessClueCoverExtraCellsFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(IExtraCellListTrait.ExtraCellSize)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniquenessClueCoverStep);

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<object?>, int> Formula => static args => OeisSequences.A004526((int)args![0]!);
}
