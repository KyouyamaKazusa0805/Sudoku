namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a lasting constraint.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class LastingConstraint : Constraint, ILimitCountConstraint<int>, IComparisonOperatorConstraint
{
	/// <inheritdoc/>
	[HashCodeMember]
	[StringMember]
	public int LimitCount { get; set; }

	/// <inheritdoc/>
	[HashCodeMember]
	[StringMember]
	public ComparisonOperator Operator { get; set; }

	/// <inheritdoc/>
	static int ILimitCountConstraint<int>.Minimum => 1;

	/// <inheritdoc/>
	static int ILimitCountConstraint<int>.Maximum => 8;


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is LastingConstraint comparer && (LimitCount, Operator) == (comparer.LimitCount, comparer.Operator);

	/// <inheritdoc/>
	public override string ToString(CultureInfo? culture = null)
		=> string.Format(
			ResourceDictionary.Get("LastingConstraint", culture),
			Technique.NakedSingle.GetName(culture),
			Operator.GetOperatorString(),
			LimitCount.ToString()
		);

	/// <inheritdoc/>
	public override LastingConstraint Clone() => new() { LimitCount = LimitCount, Operator = Operator, IsNegated = IsNegated };

	/// <inheritdoc/>
	protected override bool CheckCore(ConstraintCheckingContext context)
		=> context.AnalyzerResult.Any(s => s is NakedSingleStep { Lasting: var l } && Operator.GetOperator<int>()(LimitCount, l));
}
