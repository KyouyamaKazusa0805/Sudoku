namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents ittoryu length constraint.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class IttoryuLengthConstraint : Constraint, IComparisonOperatorConstraint, ILimitCountConstraint<int>
{
	/// <summary>
	/// Indicates the disordered ittoryu finder.
	/// </summary>
	private static readonly DisorderedIttoryuFinder Finder = new();


	/// <inheritdoc/>
	public override bool AllowDuplicate => false;

	/// <inheritdoc/>
	public int Minimum => 0;

	/// <inheritdoc/>
	public int Maximum => 9;

	/// <summary>
	/// Indicates the length.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public int Length { get; set; }

	/// <inheritdoc/>
	[HashCodeMember]
	[StringMember]
	public ComparisonOperator Operator { get; set; }

	/// <inheritdoc/>
	int ILimitCountConstraint<int>.LimitCount { get => Length; set => Length = value; }


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is IttoryuLengthConstraint comparer && (Length, Operator) == (comparer.Length, comparer.Operator);

	/// <inheritdoc/>
	public override bool Check(scoped ConstraintCheckingContext context)
	{
		var factLength = Finder.FindPath(in context.Grid).Digits.Length;
		return Operator.GetOperator<int>()(factLength, Length);
	}

	/// <inheritdoc/>
	public override string ToString(CultureInfo? culture = null)
		=> string.Format(ResourceDictionary.Get("IttoryuLengthConstraint", culture), Operator.GetOperatorString(), Length);
}
