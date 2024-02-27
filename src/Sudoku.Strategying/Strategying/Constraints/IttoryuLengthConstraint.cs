namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents ittoryu length constraint.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class IttoryuLengthConstraint : Constraint, IComparisonOperatorConstraint
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
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is IttoryuLengthConstraint comparer && (Length, Operator) == (comparer.Length, comparer.Operator);

	/// <inheritdoc/>
	public override ConflictionResult VerifyConfliction(Constraint other)
	{
		if (other is not IttoryuConstraint casted)
		{
			return ConflictionResult.Successful;
		}

		switch (casted)
		{
			case { Operator: ComparisonOperator.GreaterThan, Rounds: >= 1 }:
			case { Operator: ComparisonOperator.GreaterThanOrEqual, Rounds: > 1 }:
			case { Operator: ComparisonOperator.Equality, Rounds: not 1 }:
			case { Operator: ComparisonOperator.Inequality, Rounds: 1 }:
			{
				if (this is { Operator: ComparisonOperator.Equality, Length: not 0 })
				{
					return ConflictionResult.Failed(other, ConflictionType.ValueDiffers, Severity.Error);
				}

				break;
			}
		}

		return ConflictionResult.Successful;
	}

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
