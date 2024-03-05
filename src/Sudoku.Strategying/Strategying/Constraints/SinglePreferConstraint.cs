namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a single prefer constraint.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class SinglePreferConstraint : Constraint
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
	/// Indicates which way a user likes to finish a grid.
	/// </summary>
	[HashCodeMember]
	public SingleTechniquePrefer SinglePrefer { get; set; }

	[StringMember(nameof(SinglePrefer))]
	private string SinglePreferString => SinglePrefer.ToTechniqueString();


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is SinglePreferConstraint comparer && SinglePrefer == comparer.SinglePrefer;

	/// <inheritdoc/>
	public override bool Check(ConstraintCheckingContext context)
	{
		scoped var feature = new GridFeature(context.Grid);
		return SinglePrefer == SingleTechniquePrefer.HiddenSingle
			? feature.CanOnlyUseHiddenSingle(AllowsHiddenSingleInRowsOrColumns)
			: feature.CanOnlyUseNakedSingle();
	}

	/// <inheritdoc/>
	public override string ToString(CultureInfo? culture = null)
		=> string.Format(
			ResourceDictionary.Get("SinglePreferConstraint", culture),
			SinglePrefer.ToTechniqueString(culture),
			AllowsHiddenSingleInRowsOrColumns ? string.Empty : ResourceDictionary.Get("NoString", culture)
		);
}
