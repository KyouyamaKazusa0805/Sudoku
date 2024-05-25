namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a constraint that contains between rule property.
/// </summary>
public interface IBetweenRuleConstraint
{
	/// <summary>
	/// Indicates the between rule.
	/// </summary>
	public abstract BetweenRule BetweenRule { get; set; }
}
