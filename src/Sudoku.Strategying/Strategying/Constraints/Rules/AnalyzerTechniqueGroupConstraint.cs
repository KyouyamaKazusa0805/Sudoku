namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a constraint that checks for a technique group.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class AnalyzerTechniqueGroupConstraint : Constraint
{
	/// <summary>
	/// Indicates the desired technique group.
	/// </summary>
	[HashCodeMember]
	public required TechniqueGroup TechniqueGroup { get; set; }

	/// <inheritdoc/>
	public override ConstraintCheckingProperty CheckingProperties => ConstraintCheckingProperty.CollectorResult;

	[StringMember]
	private string TechniqueGroupString => TechniqueGroup.GetName();


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is AnalyzerTechniqueGroupConstraint comparer && TechniqueGroup == comparer.TechniqueGroup;

	/// <inheritdoc/>
	protected internal override bool CheckCore(scoped ConstraintCheckingContext context)
	{
		if (!context.RequiresAnalyzer)
		{
			return false;
		}

		foreach (var step in context.AnalyzerResult)
		{
			if (step.Code.TryGetGroup() is { } group && group == TechniqueGroup)
			{
				return true;
			}
		}

		return false;
	}

	/// <inheritdoc/>
	protected internal override ValidationResult ValidateCore()
		=> Enum.IsDefined(TechniqueGroup)
			? ValidationResult.Successful
			: ValidationResult.Failed(
				nameof(TechniqueGroup),
				ValidationReason.EnumerationFieldNotDefined,
				Severity.Error
			);
}
