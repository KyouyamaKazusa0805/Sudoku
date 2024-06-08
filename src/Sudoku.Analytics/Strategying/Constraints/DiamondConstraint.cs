namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a constraint that checks for diamond property.
/// </summary>
public sealed class DiamondConstraint() : PearlOrDiamondConstraint(false)
{
	/// <inheritdoc/>
	public override DiamondConstraint Clone() => new() { IsNegated = IsNegated, ShouldBePearlOrDiamond = ShouldBePearlOrDiamond };
}
