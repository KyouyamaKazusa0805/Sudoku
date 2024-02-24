namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents symmetry constraint. This constraint won't be used because <see cref="HodokuPuzzleGenerator"/> can controls this.
/// </summary>
/// <seealso cref="HodokuPuzzleGenerator"/>
[GetHashCode]
[ToString]
public sealed partial class SymmetryConstraint : Constraint
{
	/// <summary>
	/// Indicates the supported symmetry types to be used.
	/// </summary>
	[HashCodeMember]
	public required SymmetricType SymmetricTypes { get; set; }

	[StringMember]
	private string SymmetricTypesString => SymmetricTypes.ToString();


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is SymmetryConstraint comparer && SymmetricTypes == comparer.SymmetricTypes;

	/// <inheritdoc/>
	protected internal override bool CheckCore(scoped ConstraintCheckingContext context)
		=> ((int)SymmetricTypes >> (int)context.Grid.Symmetry & 1) != 0;

	/// <inheritdoc/>
	protected internal override ValidationResult ValidateCore()
	{
		foreach (var flag in (int)SymmetricTypes)
		{
			if (!Enum.IsDefined((SymmetricType)(1 << flag)))
			{
				return ValidationResult.Failed(
					nameof(SymmetricTypes),
					ValidationReason.EnumerationFieldNotDefined,
					Severity.Error
				);
			}
		}

		return ValidationResult.Successful;
	}
}
