namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a constraint that checks for pearl or diamond property.
/// </summary>
/// <param name="checkPearl">Indicates whether the constraint checks for pearl.</param>
[ToString]
[TypeImpl(TypeImplFlag.Object_GetHashCode | TypeImplFlag.Object_ToString)]
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
	public sealed override string ToString(IFormatProvider? formatProvider)
	{
		var culture = formatProvider as CultureInfo;
		return string.Format(
			ResourceDictionary.Get("PearlOrDiamondConstraint", culture),
#if NET9_0_OR_GREATER
			[
#endif
			ResourceDictionary.Get(CheckPearl ? "PearlString" : "DiamondString", culture).ToUpper(),
			ShouldBePearlOrDiamond ? string.Empty : ResourceDictionary.Get("NoString", culture),
			ResourceDictionary.Get(CheckPearl ? "PearlString" : "DiamondString", culture)
#if NET9_0_OR_GREATER
			]
#endif
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
