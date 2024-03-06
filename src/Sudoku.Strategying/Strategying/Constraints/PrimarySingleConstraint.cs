namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a primary single constraint.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class PrimarySingleConstraint : Constraint
{
	/// <inheritdoc/>
	public override bool AllowDuplicate => false;

	/// <summary>
	/// Indicates whether the constraint allows hidden singles on rows or columns.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public bool AllowsHiddenSingleInRowsOrColumns { get; set; }

	/// <summary>
	/// Indicates which technique a user likes to finish a grid.
	/// </summary>
	[HashCodeMember]
	public SingleTechnique Primary { get; set; }

	[StringMember(nameof(Primary))]
	private string SinglePreferString => Primary.ToTechniqueString();


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is PrimarySingleConstraint comparer && Primary == comparer.Primary;

	/// <inheritdoc/>
	public override bool Check(ConstraintCheckingContext context)
	{
		scoped var feature = new GridFeature(context.Grid);
		return Primary == SingleTechnique.HiddenSingle
			? feature.CanOnlyUseHiddenSingle(AllowsHiddenSingleInRowsOrColumns)
			: feature.CanOnlyUseNakedSingle();
	}

	/// <inheritdoc/>
	public override string ToString(CultureInfo? culture = null)
		=> string.Format(
			ResourceDictionary.Get("PrimarySingleConstraint", culture),
			Primary.ToTechniqueString(culture),
			AllowsHiddenSingleInRowsOrColumns ? string.Empty : ResourceDictionary.Get("NoString", culture)
		);
}
