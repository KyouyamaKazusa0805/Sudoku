namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents minimal constraint.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class MinimalConstraint : Constraint
{
	/// <summary>
	/// Indicates whether the puzzle shsould be minimal.
	/// </summary>
	[HashCodeMember]
	public bool ShouldBeMinimal { get; set; }

	[StringMember]
	private string ShouldBeMinimalString => ShouldBeMinimal.ToString();


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is MinimalConstraint comparer && ShouldBeMinimal == comparer.ShouldBeMinimal;

	/// <inheritdoc/>
	protected internal override bool CheckCore(scoped ConstraintCheckingContext context)
		=> context.Grid.IsMinimal == ShouldBeMinimal;

	/// <inheritdoc/>
	protected internal override ValidationResult ValidateCore() => ValidationResult.Successful;
}
