namespace Sudoku.Runtime.MeasuringServices.Factors;

/// <summary>
/// Represents a factor that describes the size of the pattern from <see cref="MultisectorLockedSetsStep"/>.
/// </summary>
/// <seealso cref="MultisectorLockedSetsStep"/>
public sealed class MultisectorLockedSetsSizeFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(ICellListTrait.CellSize)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(MultisectorLockedSetsStep);

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<object?>, int> Formula => static args => OeisSequences.A002024((int)args![0]!);
}
