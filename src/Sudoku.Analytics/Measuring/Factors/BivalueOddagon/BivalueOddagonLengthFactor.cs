namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that measures length of <see cref="BivalueOddagonStep"/>.
/// </summary>
/// <seealso cref="BivalueOddagonStep"/>
public sealed class BivalueOddagonLengthFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(ICellListTrait.CellSize)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(BivalueOddagonStep);

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<object?>, int> Formula => static args => OeisSequences.A004526((int)args![0]!);
}
