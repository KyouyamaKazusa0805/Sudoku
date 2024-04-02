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
	public static int Minimum => 0;

	/// <inheritdoc/>
	public static int Maximum => 9;


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is IttoryuLengthConstraint comparer && (Length, Operator) == (comparer.Length, comparer.Operator);

	/// <inheritdoc/>
	public override string ToString(CultureInfo? culture = null)
		=> string.Format(ResourceDictionary.Get("IttoryuLengthConstraint", culture), Operator.GetOperatorString(), Length);

	/// <inheritdoc/>
	public override IttoryuLengthConstraint Clone() => new() { IsNegated = IsNegated, Length = Length, Operator = Operator };

	/// <inheritdoc/>
	protected override bool CheckCore(ConstraintCheckingContext context)
	{
		if (context.AnalyzerResult is not { IsSolved: true, DifficultyLevel: DifficultyLevel.Easy })
		{
			return false;
		}

		var factLength = Finder.FindPath(context.Grid).Digits.Length;
		return Operator.GetOperator<int>()(factLength, Length);
	}
}
