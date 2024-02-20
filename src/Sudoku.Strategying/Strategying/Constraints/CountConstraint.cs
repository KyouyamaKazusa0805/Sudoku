namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a rule that checks for the number of cells appeared in cell, using the specified cell state.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class CountConstraint : Constraint
{
	/// <summary>
	/// Indicates the limit number of cells to be checked.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public required int LimitCount { get; set; }

	/// <summary>
	/// Indicates the cell state to be checked.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public required CellState CellState { get; set; }

	/// <summary>
	/// Indicates the comparison operator.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public required ComparisonOperator Operator { get; set; }

	/// <inheritdoc/>
	public override ConstraintCheckingProperty ConstraintCheckingProperties => ConstraintCheckingProperty.Grid;

	/// <inheritdoc/>
	protected internal override ValidationResult ValidationResult
		=> (CellState, Operator, LimitCount) switch
		{
			(_, not (>= ComparisonOperator.Equality and <= ComparisonOperator.LessThanOrEqual), _)
				=> new FailedValidationResult(nameof(Operator), ValidationFailedReason.EnumerationFieldNotDefined, ValidationFailedSeverity.Error),
			(CellState.Undefined, _, _)
				=> new FailedValidationResult(nameof(CellState), ValidationFailedReason.OutOfRange, ValidationFailedSeverity.Error),
			(not (CellState.Given or CellState.Modifiable or CellState.Empty), _, _)
				=> new FailedValidationResult(nameof(CellState), ValidationFailedReason.EnumerationFieldNotDefined, ValidationFailedSeverity.Error),
			(not CellState.Given, _, _)
				=> new SuccessValidationResult(),
			(_, ComparisonOperator.Equality or ComparisonOperator.Inequality, < 17 or > 81) or
			(_, ComparisonOperator.GreaterThan, >= 81) or
			(_, ComparisonOperator.GreaterThanOrEqual, > 81) or
			(_, ComparisonOperator.LessThan, <= 17) or
			(_, ComparisonOperator.LessThanOrEqual, < 17)
				=> new FailedValidationResult(nameof(LimitCount), ValidationFailedReason.AlwaysFalse, ValidationFailedSeverity.Error),
			(_, ComparisonOperator.Equality, >= 17 and <= 22) or
			(_, ComparisonOperator.LessThan, > 17 and <= 23) or
			(_, ComparisonOperator.LessThanOrEqual, >= 17 and <= 22)
				=> new FailedValidationResult(nameof(LimitCount), ValidationFailedReason.TooStrict, ValidationFailedSeverity.Warning),
			(_, ComparisonOperator.GreaterThan, < 17) or
			(_, ComparisonOperator.GreaterThanOrEqual, <= 17) or
			(_, ComparisonOperator.LessThan, > 81) or
			(_, ComparisonOperator.LessThanOrEqual, >= 81)
				=> new FailedValidationResult(nameof(LimitCount), ValidationFailedReason.AlwaysTrue, ValidationFailedSeverity.Warning)
		};


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is CountConstraint comparer
		&& (CellState, LimitCount, Operator) == (comparer.CellState, comparer.LimitCount, comparer.Operator);

	/// <inheritdoc/>
	protected internal override bool CheckCore(scoped ConstraintCheckingContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		var @operator = Operator.GetOperator<int>();
		var factCount = CellState switch
		{
			CellState.Empty => grid.EmptiesCount,
			CellState.Modifiable => grid.ModifiablesCount,
			_ => grid.GivensCount
		};
		return @operator(factCount, LimitCount);
	}
}
