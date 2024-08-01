namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of <see cref="ExtendedRectangleStep"/>.
/// </summary>
/// <seealso cref="ExtendedRectangleStep"/>
public sealed class ExtendedRectangleSizeFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(ICellListTrait.CellSize)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(ExtendedRectangleStep);

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<object?>, int> Formula => static args => OeisSequences.A004526((int)args![0]!) - 2;
}
