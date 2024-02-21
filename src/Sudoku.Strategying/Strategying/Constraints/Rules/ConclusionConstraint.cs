namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a conclusion constraint.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class ConclusionConstraint : Constraint
{
	/// <summary>
	/// Indicates the universal quantifier.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public required UniversalQuantifier UniversalQuantifier { get; set; }

	/// <inheritdoc/>
	public override ConstraintCheckingProperty CheckingProperties => ConstraintCheckingProperty.AnalyzerResult;

	/// <summary>
	/// Indicates the conclusions to be checked.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public required Conclusion[] Conclusions { get; set; }

	/// <inheritdoc/>
	protected internal override ValidationResult ValidationResult
		=> UniversalQuantifier is UniversalQuantifier.Any or UniversalQuantifier.All
			? ValidationResult.Successful
			: ValidationResult.Failed(
				nameof(UniversalQuantifier),
				ValidationReason.EnumerationFieldNotDefined,
				ValidationSeverity.Error
			);


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is ConclusionConstraint comparer
		&& (UniversalQuantifier, Conclusions.AsConclusionSet()) == (comparer.UniversalQuantifier, comparer.Conclusions.AsConclusionSet());

	/// <inheritdoc/>
	protected internal override bool CheckCore(scoped ConstraintCheckingContext context)
	{
		if (!context.RequiresAnalyzer)
		{
			return false;
		}

		var thisSet = Conclusions.AsConclusionSet();
		foreach (var step in context.AnalyzerResult)
		{
			if (UniversalQuantifier == UniversalQuantifier.Any)
			{
				if ((step.Conclusions.AsConclusionSet() & thisSet) == thisSet)
				{
					return true;
				}
			}
			else if ((step.Conclusions.AsConclusionSet() & thisSet) != thisSet)
			{
				return false;
			}
		}

		return UniversalQuantifier == UniversalQuantifier.All;
	}
}
