namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents ittoryu length constraint.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class IttoryuLengthConstraint : Constraint
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
	public required int Length { get; set; }

	/// <summary>
	/// Indicates the comparison operator.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public required ComparisonOperator Operator { get; set; }


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is IttoryuLengthConstraint comparer && (Length, Operator) == (comparer.Length, comparer.Operator);

	/// <inheritdoc/>
	protected internal override bool CheckCore(scoped ConstraintCheckingContext context)
	{
		var factLength = Finder.FindPath(in context.Grid).Digits.Length;
		return Operator.GetOperator<int>()(factLength, Length);
	}

	/// <inheritdoc/>
	protected internal override ValidationResult ValidateCore()
		=> (Operator, Length) switch
		{
			(ComparisonOperator.GreaterThanOrEqual, 0)
				=> ValidationResult.Failed(nameof(Length), ValidationReason.AlwaysTrue, Severity.Warning),
			(ComparisonOperator.GreaterThan, 9)
				=> ValidationResult.Failed(nameof(Length), ValidationReason.AlwaysFalse, Severity.Error),
			(ComparisonOperator.LessThanOrEqual, 9)
				=> ValidationResult.Failed(nameof(Length), ValidationReason.AlwaysTrue, Severity.Warning),
			(ComparisonOperator.LessThan, 0)
				=> ValidationResult.Failed(nameof(Length), ValidationReason.AlwaysFalse, Severity.Error),
			(not (>= ComparisonOperator.Equality and <= ComparisonOperator.LessThanOrEqual), _)
				=> ValidationResult.Failed(nameof(Operator), ValidationReason.EnumerationFieldNotDefined, Severity.Error),
			_
				=> ValidationResult.Successful
		};
}
