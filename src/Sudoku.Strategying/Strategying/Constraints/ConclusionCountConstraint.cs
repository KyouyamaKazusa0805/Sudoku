namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a conclusion count constraint.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class ConclusionCountConstraint : Constraint, IComparisonOperatorConstraint
{
	/// <inheritdoc/>
	public override bool AllowDuplicate => false;

	/// <summary>
	/// Indicates the limit count.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public int LimitCount { get; set; }

	/// <inheritdoc/>
	[HashCodeMember]
	[StringMember]
	public ComparisonOperator Operator { get; set; }

	/// <summary>
	/// Indicates the conclusion.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public Conclusion Conclusion { get; set; }


	/// <inheritdoc/>
	public override bool Check(scoped ConstraintCheckingContext context)
	{
		var count = 0;
		foreach (var step in context.AnalyzerResult)
		{
			foreach (var conclusion in step.Conclusions)
			{
				if (Conclusion == conclusion)
				{
					count++;
					break;
				}
			}
		}

		return Operator.GetOperator<int>()(count, LimitCount);
	}

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is ConclusionCountConstraint comparer
		&& (Conclusion, LimitCount, Operator) == (comparer.Conclusion, comparer.LimitCount, comparer.Operator);

	/// <inheritdoc/>
	public override string ToString(CultureInfo? culture = null)
		=> string.Format(
			ResourceDictionary.Get("ConclusionCountConstraint", culture),
			Conclusion.ToString(culture),
			$"{Operator.GetOperatorString()} {LimitCount}",
			LimitCount == 1 ? string.Empty : ResourceDictionary.Get("NounPluralSuffix", culture)
		);
}
