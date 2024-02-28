namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents difficulty level constraint.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class DifficultyLevelConstraint : Constraint, IComparisonOperatorConstraint
{
	/// <inheritdoc/>
	public override bool AllowDuplicate => false;

	/// <summary>
	/// Indicates the difficulty level.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public DifficultyLevel DifficultyLevel { get; set; }

	/// <inheritdoc/>
	[HashCodeMember]
	[StringMember]
	public ComparisonOperator Operator { get; set; }

	/// <summary>
	/// Indicates all possible <see cref="Analytics.Categorization.DifficultyLevel"/> values. 
	/// </summary>
	public DifficultyLevel ValidDifficultyLevels
	{
		get
		{
			var allValues = DifficultyLevel.Easy | DifficultyLevel.Moderate | DifficultyLevel.Hard | DifficultyLevel.Fiendish | DifficultyLevel.Nightmare;
			var allValuesLower = DifficultyLevel switch
			{
				DifficultyLevel.Easy => DifficultyLevel.Unknown,
				DifficultyLevel.Moderate => DifficultyLevel.Easy,
				DifficultyLevel.Hard => DifficultyLevel.Easy | DifficultyLevel.Moderate,
				DifficultyLevel.Fiendish => DifficultyLevel.Easy | DifficultyLevel.Moderate | DifficultyLevel.Hard,
				DifficultyLevel.Nightmare => allValues & ~DifficultyLevel.Nightmare
			};
			var allValuesUpper = DifficultyLevel switch
			{
				DifficultyLevel.Easy => allValues & ~DifficultyLevel.Easy,
				DifficultyLevel.Moderate => DifficultyLevel.Hard | DifficultyLevel.Fiendish | DifficultyLevel.Nightmare,
				DifficultyLevel.Hard => DifficultyLevel.Fiendish | DifficultyLevel.Nightmare,
				DifficultyLevel.Fiendish => DifficultyLevel.Nightmare,
				DifficultyLevel.Nightmare => DifficultyLevel.Unknown
			};
			return Operator switch
			{
				ComparisonOperator.Equality => DifficultyLevel,
				ComparisonOperator.Inequality => allValues & ~DifficultyLevel,
				ComparisonOperator.GreaterThan => allValuesUpper,
				ComparisonOperator.GreaterThanOrEqual => allValuesUpper | DifficultyLevel,
				ComparisonOperator.LessThan => allValuesLower,
				ComparisonOperator.LessThanOrEqual => allValuesLower | DifficultyLevel
			};
		}
	}

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is DifficultyLevelConstraint comparer && (DifficultyLevel, Operator) == (comparer.DifficultyLevel, comparer.Operator);

	/// <inheritdoc/>
	public override bool Check(scoped ConstraintCheckingContext context)
		=> context.RequiresAnalyzer && (ValidDifficultyLevels & context.AnalyzerResult.DifficultyLevel) != 0;
}
