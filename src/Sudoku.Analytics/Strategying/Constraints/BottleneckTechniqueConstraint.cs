namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a constraint that determines whether bottleneck step represents the specified technique.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class BottleneckTechniqueConstraint : Constraint
{
	/// <summary>
	/// Indicates the techniques selected.
	/// </summary>
	[HashCodeMember]
	public TechniqueSet Techniques { get; set; } = [];

	[StringMember]
	private string TechniquesString => Techniques.ToString();


	/// <inheritdoc/>
	public override bool Check(ConstraintCheckingContext context)
		=> !(Techniques & [.. from step in context.AnalyzerResult.BottleneckSteps select step.Code]) is var result
		&& (IsNegated ? !result : result);

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is BottleneckTechniqueConstraint comparer && Techniques == comparer.Techniques;

	/// <inheritdoc/>
	public override string ToString(CultureInfo? culture = null)
		=> string.Format(ResourceDictionary.Get("BottleneckTechniqueConstraint", culture), Techniques.ToString(culture));
}
