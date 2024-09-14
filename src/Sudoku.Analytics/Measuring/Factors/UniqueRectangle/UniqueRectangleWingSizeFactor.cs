namespace Sudoku.Measuring.Factors;

/// <summary>
/// Represents a factor that describes the size of the wing appeared in <see cref="UniqueRectangleRegularWingStep"/>.
/// </summary>
/// <seealso cref="UniqueRectangleRegularWingStep"/>
public sealed class UniqueRectangleWingSizeFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(UniqueRectangleRegularWingStep.Code)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(UniqueRectangleRegularWingStep);

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<object?>, int> Formula
		=> static args => (Technique)args![0]! switch
		{
			Technique.UniqueRectangleXyWing or Technique.AvoidableRectangleXyWing => 2,
			Technique.UniqueRectangleXyzWing or Technique.AvoidableRectangleXyzWing => 3,
			Technique.UniqueRectangleWxyzWing or Technique.AvoidableRectangleWxyzWing => 5
		};
}
