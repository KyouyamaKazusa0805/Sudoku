namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents minimal constraint.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class MinimalConstraint : Constraint
{
	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other) => other is MinimalConstraint;

	/// <inheritdoc/>
	protected internal override bool CheckCore(scoped ConstraintCheckingContext context) => context.Grid.IsMinimal;

	/// <inheritdoc/>
	protected internal override ValidationResult ValidateCore() => ValidationResult.Successful;
}
