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
	public required bool ShouldBePearlOrDiamond { get; set; }


	/// <inheritdocs/>
	public sealed override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is PearlOrDiamondConstraint comparer
		&& (CheckPearl, ShouldBePearlOrDiamond) == (comparer.CheckPearl, comparer.ShouldBePearlOrDiamond);

	/// <inheritdoc/>
	protected internal sealed override bool CheckCore(ConstraintCheckingContext context)
	{
		if (!context.RequiresAnalyzer)
		{
			return false;
		}

		var isPearl = context.AnalyzerResult.IsPearl;
		var isDiamond = context.AnalyzerResult.IsDiamond;
		return !(ShouldBePearlOrDiamond ^ ((CheckPearl ? isPearl : isDiamond) ?? false));
	}

	/// <inheritdoc/>
	protected internal sealed override ValidationResult ValidateCore() => ValidationResult.Successful;
}
