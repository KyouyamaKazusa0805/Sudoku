namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a constraint that determines whether bottleneck step represents the specified technique.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class BottleneckTechniqueConstraint : Constraint
{
	/// <inheritdoc/>
	public override bool AllowDuplicate => false;

	/// <summary>
	/// Indicates the techniques selected.
	/// </summary>
	[HashCodeMember]
	public TechniqueSet Techniques { get; set; } = [];

	[StringMember]
	private string TechniquesString => Techniques.ToString();


	/// <inheritdoc/>
	public override bool Check(ConstraintCheckingContext context)
	{
		var maxStep = context.AnalyzerResult.InterimSteps!.MaxBy(static step => step.Difficulty);
		return maxStep is not null && Techniques.Contains(maxStep.Code);
	}

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is BottleneckTechniqueConstraint comparer && Techniques == comparer.Techniques;

	/// <inheritdoc/>
	public override string ToString(CultureInfo? culture = null)
		=> string.Format(ResourceDictionary.Get("BottleneckTechniqueConstraint", culture), Techniques.ToString(culture));
}
