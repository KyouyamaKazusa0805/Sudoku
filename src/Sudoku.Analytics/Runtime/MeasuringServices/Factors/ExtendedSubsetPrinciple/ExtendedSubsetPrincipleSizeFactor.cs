namespace Sudoku.Runtime.MeasuringServices.Factors;

/// <summary>
/// Represents a factor that measures the size of the pattern appeared in a <see cref="ExtendedSubsetPrincipleStep"/>.
/// </summary>
/// <seealso cref="ExtendedSubsetPrincipleStep"/>
public sealed class ExtendedSubsetPrincipleSizeFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(ICellListTrait.CellSize)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(ExtendedSubsetPrincipleStep);

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<object?>, int> Formula
		=> static args => (int)args![0]! switch { 3 or 4 => 0, 5 or 6 or 7 => 2, 8 or 9 => 4 };
}