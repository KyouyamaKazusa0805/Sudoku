namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of the subset appeared in <see cref="DirectSubsetStep"/>.
/// </summary>
/// <seealso cref="DirectSubsetStepSearcher"/>
public sealed class DirectSubsetSizeFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(ICellListTrait.CellSize)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(DirectSubsetStep);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => (int)args![0]! switch { 2 => 0, 3 => 6, 4 => 20 };
}
