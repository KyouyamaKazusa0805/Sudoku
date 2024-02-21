namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a constraint that checks for pearl or diamond property.
/// </summary>
/// <param name="checkPearl">Indicates whether the constraint checks for pearl.</param>
[GetHashCode]
[ToString]
public abstract partial class PearlOrDiamondConstraint([PrimaryConstructorParameter, HashCodeMember] bool checkPearl) : Constraint
{
	/// <inheritdoc/>
	public sealed override ConstraintCheckingProperty CheckingProperties => ConstraintCheckingProperty.AnalyzerResult;

	[StringMember]
	private string CheckPearlPropertyValue => CheckPearl.ToString();


	/// <inheritdocs/>
	public sealed override bool Equals([NotNullWhen(true)] Constraint? other)
		=> other is PearlOrDiamondConstraint comparer && CheckPearl == comparer.CheckPearl;

	/// <inheritdoc/>
	protected internal sealed override bool CheckCore(ConstraintCheckingContext context)
	{
		if (!context.RequiresAnalyzer)
		{
			return false;
		}

		var isPearl = context.AnalyzerResult.IsPearl;
		var isDiamond = context.AnalyzerResult.IsDiamond;
		return (CheckPearl ? isPearl : isDiamond) ?? false;
	}

	/// <inheritdoc/>
	protected internal sealed override ValidationResult ValidateCore() => ValidationResult.Successful;
}
