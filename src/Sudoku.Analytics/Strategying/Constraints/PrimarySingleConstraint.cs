namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a primary single constraint.
/// </summary>
[GetHashCode]
[ToString]
public sealed partial class PrimarySingleConstraint : Constraint
{
	/// <summary>
	/// Indicates whether the constraint allows hidden singles on rows or columns.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public bool AllowsHiddenSingleInLines { get; set; }

	/// <summary>
	/// Indicates which technique a user likes to finish a grid.
	/// </summary>
	[HashCodeMember]
	public SingleTechnique Primary { get; set; }

	[StringMember(nameof(Primary))]
	private string SinglePreferString => Primary.GetName();


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is PrimarySingleConstraint comparer && Primary == comparer.Primary;

	/// <inheritdoc/>
	public override string ToString(CultureInfo? culture = null)
		=> string.Format(
			ResourceDictionary.Get("PrimarySingleConstraint", culture),
			Primary.GetName(culture),
			AllowsHiddenSingleInLines ? string.Empty : ResourceDictionary.Get("NoString", culture)
		);

	/// <inheritdoc/>
	public override PrimarySingleConstraint Clone()
		=> new() { IsNegated = IsNegated, AllowsHiddenSingleInLines = AllowsHiddenSingleInLines, Primary = Primary };

	/// <inheritdoc/>
	protected override bool CheckCore(ConstraintCheckingContext context)
		=> Primary switch
		{
			SingleTechnique.FullHouse => context.Grid.CanPrimaryFullHouse(),
			SingleTechnique.HiddenSingle => context.Grid.CanPrimaryHiddenSingle(AllowsHiddenSingleInLines),
			SingleTechnique.NakedSingle => context.Grid.CanPrimaryNakedSingle()
		};
}
