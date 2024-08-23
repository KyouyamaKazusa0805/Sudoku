namespace Sudoku.Runtime.MeasuringServices.Factors;

/// <summary>
/// Represents a factor that describes the factor for <see cref="ComplexSingleStep"/>.
/// </summary>
/// <seealso cref="ComplexSingleStep"/>
public sealed class ComplexSingleFactor : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => [nameof(ComplexSingleStep.IndirectTechniques)];

	/// <inheritdoc/>
	public override Type ReflectedStepType => typeof(ComplexSingleStep);

	/// <inheritdoc/>
	public override Func<ReadOnlySpan<object?>, int> Formula => static args => ComplexTechniqueUsages.GetComplexityDifficulty((Technique[][])args![0]!);
}
