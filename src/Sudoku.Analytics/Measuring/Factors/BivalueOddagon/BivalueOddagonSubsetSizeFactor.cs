namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that measures subset size of <see cref="BivalueOddagonStep"/>.
/// </summary>
/// <seealso cref="BivalueOddagonStep"/>
public sealed class BivalueOddagonSubsetSizeFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(BivalueOddagonType3Step.ExtraCells)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(BivalueOddagonType3Step);

	/// <inheritdoc/>
	public override ParameterizedFormula Formula => static args => A004526(((CellMap)args![0]!).Count);
}
