namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a constraint that checks for pearl property.
/// </summary>
public sealed class PearlConstraint() : PearlOrDiamondConstraint(true)
{
	/// <inheritdoc/>
	public override PearlConstraint Clone() => new() { IsNegated = IsNegated, ShouldBePearlOrDiamond = ShouldBePearlOrDiamond };
}
