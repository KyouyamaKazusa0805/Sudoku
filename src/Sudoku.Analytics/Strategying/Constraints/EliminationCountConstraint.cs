namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents an elimination count constraint.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class EliminationCountConstraint : Constraint, IComparisonOperatorConstraint, ILimitCountConstraint<int>
{
	/// <inheritdoc/>
	[HashCodeMember]
	[StringMember]
	public int LimitCount { get; set; }

	/// <summary>
	/// Indicates the technique used.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public Technique Technique { get; set; }

	/// <inheritdoc/>
	[HashCodeMember]
	[StringMember]
	public ComparisonOperator Operator { get; set; }

	/// <inheritdoc/>
	static int ILimitCountConstraint<int>.Maximum => 30;

	/// <inheritdoc/>
	static int ILimitCountConstraint<int>.Minimum => 0;


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is EliminationCountConstraint comparer && (LimitCount, Operator) == (comparer.LimitCount, comparer.Operator);

	/// <inheritdoc/>
	public override string ToString(CultureInfo? culture = null)
		=> string.Format(
			ResourceDictionary.Get("EliminationCountConstraint", culture),
			Operator.GetOperatorString(),
			LimitCount,
			LimitCount != 1 ? string.Empty : ResourceDictionary.Get("NounPluralSuffix", culture),
			Technique.GetName(culture)
		);

	/// <inheritdoc/>
	protected override bool CheckCore(ConstraintCheckingContext context)
	{
		var @operator = Operator.GetOperator<int>();
		foreach (var step in context.AnalyzerResult)
		{
			if (step.Code == Technique && @operator(LimitCount, step.Conclusions.Length))
			{
				return true;
			}
		}

		return false;
	}
}
