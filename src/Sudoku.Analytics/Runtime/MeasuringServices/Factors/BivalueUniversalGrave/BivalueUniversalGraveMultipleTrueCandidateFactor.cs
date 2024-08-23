namespace Sudoku.Runtime.MeasuringServices.Factors;

/// <summary>
/// Represents a factor that describes the true candidates appeared in <see cref="BivalueUniversalGraveMultipleStep"/>.
/// </summary>
/// <seealso cref="BivalueUniversalGraveMultipleStep"/>
public sealed class BivalueUniversalGraveMultipleTrueCandidateFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(ICandidateListTrait.CandidateSize)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(BivalueUniversalGraveMultipleStep);

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<object?>, int> Formula => static args => OeisSequences.A002024((int)args![0]!);
}
