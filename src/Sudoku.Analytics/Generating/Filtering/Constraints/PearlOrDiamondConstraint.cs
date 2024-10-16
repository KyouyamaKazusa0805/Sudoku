namespace Sudoku.Generating.Filtering.Constraints;

/// <summary>
/// Represents a constraint that checks for pearl or diamond property.
/// </summary>
/// <param name="checkPearl">Indicates whether the constraint checks for pearl.</param>
[TypeImpl(TypeImplFlags.Object_GetHashCode | TypeImplFlags.Object_ToString)]
public abstract partial class PearlOrDiamondConstraint([Property, HashCodeMember, StringMember] bool checkPearl) : Constraint
{
	/// <summary>
	/// Indicates whether the puzzle should be pearl or diamond.
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public bool ShouldBePearlOrDiamond { get; set; }


	/// <inheritdocs/>
	public sealed override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is PearlOrDiamondConstraint comparer
		&& (CheckPearl, ShouldBePearlOrDiamond) == (comparer.CheckPearl, comparer.ShouldBePearlOrDiamond);

	/// <inheritdoc/>
	public sealed override string ToString(IFormatProvider? formatProvider)
	{
		var culture = formatProvider as CultureInfo;
		return string.Format(
			SR.Get("PearlOrDiamondConstraint", culture),
			[
				SR.Get(CheckPearl ? "PearlString" : "DiamondString", culture).ToUpper(),
				ShouldBePearlOrDiamond ? string.Empty : SR.Get("NoString", culture),
				SR.Get(CheckPearl ? "PearlString" : "DiamondString", culture)
			]
		);
	}

	/// <inheritdoc/>
	protected override bool CheckCore(ConstraintCheckingContext context)
	{
		var isPearl = context.AnalyzerResult.IsPearl;
		var isDiamond = context.AnalyzerResult.IsDiamond;
		return !(ShouldBePearlOrDiamond ^ ((CheckPearl ? isPearl : isDiamond) ?? false));
	}
}
