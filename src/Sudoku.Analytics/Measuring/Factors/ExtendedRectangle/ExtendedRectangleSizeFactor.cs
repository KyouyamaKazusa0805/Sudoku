namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of <see cref="ExtendedRectangleStep"/>.
/// </summary>
/// <seealso cref="ExtendedRectangleStep"/>
public sealed class ExtendedRectangleSizeFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(ExtendedRectangleStep.Cells)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(ExtendedRectangleStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => A004526(((CellMap)args![0]!).Count) - 2;
}
