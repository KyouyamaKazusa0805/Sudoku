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


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is DifficultyLevelConstraint comparer && (DifficultyLevel, Operator) == (comparer.DifficultyLevel, comparer.Operator);

	/// <inheritdoc/>
	public override bool Check(scoped ConstraintCheckingContext context)
	{
		if (!context.RequiresAnalyzer)
		{
			return false;
		}

		var factDifficultyLevel = context.AnalyzerResult.DifficultyLevel;
		return Operator.GetOperator<int>()((int)factDifficultyLevel, (int)DifficultyLevel);
	}
}
