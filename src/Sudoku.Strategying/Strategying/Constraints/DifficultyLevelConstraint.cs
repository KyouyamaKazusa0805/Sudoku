namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents difficulty level constraint.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class DifficultyLevelConstraint : Constraint
{
	/// <summary>
	/// Indicates the difficulty level.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public required DifficultyLevel DifficultyLevel { get; set; }

	/// <summary>
	/// Indicates the operator.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public required ComparisonOperator Operator { get; set; }


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is DifficultyLevelConstraint comparer && (DifficultyLevel, Operator) == (comparer.DifficultyLevel, comparer.Operator);

	/// <inheritdoc/>
	protected internal override bool CheckCore(scoped ConstraintCheckingContext context)
	{
		if (!context.RequiresAnalyzer)
		{
			return false;
		}

		var factDifficultyLevel = context.AnalyzerResult.DifficultyLevel;
		return Operator.GetOperator<int>()((int)factDifficultyLevel, (int)DifficultyLevel);
	}

	/// <inheritdoc/>
	protected internal override ValidationResult ValidateCore()
	{
		if (!Enum.IsDefined(DifficultyLevel))
		{
			return ValidationResult.Failed(
				nameof(DifficultyLevel),
				ValidationReason.EnumerationFieldNotDefined,
				Severity.Error
			);
		}

		if (DifficultyLevel == DifficultyLevel.Unknown)
		{
			return ValidationResult.Failed(
				nameof(DifficultyLevel),
				ValidationReason.OutOfRange,
				Severity.Error
			);
		}

		if (!Enum.IsDefined(Operator))
		{
			return ValidationResult.Failed(
				nameof(Operator),
				ValidationReason.EnumerationFieldNotDefined,
				Severity.Error
			);
		}

		return ValidationResult.Successful;
	}
}
