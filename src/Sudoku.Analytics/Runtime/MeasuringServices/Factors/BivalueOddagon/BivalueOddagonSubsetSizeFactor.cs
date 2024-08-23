namespace Sudoku.Runtime.MeasuringServices.Factors;

/// <summary>
/// Represents a factor that measures subset size of <see cref="BivalueOddagonStep"/>.
/// </summary>
/// <seealso cref="BivalueOddagonStep"/>
public sealed class BivalueOddagonSubsetSizeFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(IExtraCellListTrait.ExtraCellSize)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(BivalueOddagonType3Step);

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<object?>, int> Formula => static args => OeisSequences.A004526((int)args![0]!);
}
