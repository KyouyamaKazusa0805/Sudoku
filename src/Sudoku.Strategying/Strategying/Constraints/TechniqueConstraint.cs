namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a rule that checks whether the specified analyzer result after analyzed by a grid
/// contains the specified techniques.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class TechniqueConstraint : Constraint, IComparisonOperatorConstraint
{
	/// <summary>
	/// Indicates the appearing times.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public required int LimitCount { get; set; }

	/// <inheritdoc/>
	[HashCodeMember]
	[StringMember]
	public required ComparisonOperator Operator { get; set; }

	/// <summary>
	/// Indicates the technique used.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public required Technique Technique { get; set; }


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is TechniqueConstraint comparer
		&& (LimitCount, Operator, Technique) == (comparer.LimitCount, comparer.Operator, comparer.Technique);

	/// <inheritdoc/>
	protected internal override bool CheckCore(scoped ConstraintCheckingContext context)
	{
		if (!context.RequiresAnalyzer)
		{
			return false;
		}

		var times = 0;
		foreach (var step in context.AnalyzerResult)
		{
			if (Technique == step.Code)
			{
				times++;
			}
		}

		return Operator.GetOperator<int>()(times, LimitCount);
	}

	/// <inheritdoc/>
	protected internal override ValidationResult ValidateCore() => ValidationResult.Successful;
}
