namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a type that supports for comparison operator.
/// </summary>
public interface IComparisonOperatorConstraint
{
	/// <summary>
	/// Indicates the comparison operator used.
	/// </summary>
	public abstract ComparisonOperator Operator { get; set; }
}
