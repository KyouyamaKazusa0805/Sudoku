namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a constraint that checks for pearl or diamond property.
/// </summary>
/// <param name="checkPearl">Indicates whether the constraint checks for pearl.</param>
[GetHashCode]
[ToString]
public abstract partial class PearlOrDiamondConstraint([PrimaryConstructorParameter, HashCodeMember, StringMember] bool checkPearl) :
	Constraint
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
	public sealed override bool Check(ConstraintCheckingContext context)
	{
		var isPearl = context.AnalyzerResult.IsPearl;
		var isDiamond = context.AnalyzerResult.IsDiamond;
		var result = !(ShouldBePearlOrDiamond ^ ((CheckPearl ? isPearl : isDiamond) ?? false));
		return IsNegated ? !result : result;
	}

	/// <inheritdoc/>
	public sealed override string ToString(CultureInfo? culture = null)
		=> string.Format(
			ResourceDictionary.Get("PearlOrDiamondConstraint", culture),
			ResourceDictionary.Get(CheckPearl ? "PearlString" : "DiamondString", culture).ToUpper(),
			ShouldBePearlOrDiamond ? string.Empty : ResourceDictionary.Get("NoString", culture),
			ResourceDictionary.Get(CheckPearl ? "PearlString" : "DiamondString", culture)
		);
}
