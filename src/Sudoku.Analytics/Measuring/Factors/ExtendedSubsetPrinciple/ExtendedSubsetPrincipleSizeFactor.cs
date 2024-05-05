namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that measures the size of the pattern appeared in a <see cref="ExtendedSubsetPrincipleStep"/>.
/// </summary>
/// <seealso cref="ExtendedSubsetPrincipleStep"/>
public sealed partial class ExtendedSubsetPrincipleSizeFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(ExtendedSubsetPrincipleStep.Cells)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(ExtendedSubsetPrincipleStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula
		=> static args => ((CellMap)args![0]!).Count switch { 3 or 4 => 0, 5 or 6 or 7 => 2, 8 or 9 => 4 };
}
