namespace Sudoku.Analytics.Traits;

/// <summary>
/// Represents a step of type <typeparamref name="TSelf"/>, presenting the third subtype.
/// </summary>
/// <typeparam name="TSelf"><include file="../../global-doc-comments.xml" path="/g/self-type-constraint"/></typeparam>
public interface IPatternType3StepTrait<TSelf> : ITrait where TSelf : Step, IPatternType3StepTrait<TSelf>
{
	/// <summary>
	/// Indicates whether the subset is hidden.
	/// </summary>
	public abstract bool IsHidden { get; }

	/// <summary>
	/// Indicates the size of the subset. The value may not be equal to the length of <see cref="SubsetCells"/>.
	/// </summary>
	public abstract int SubsetSize { get; }

	/// <summary>
	/// Indicates the digits mask including all digits appeared in the subset.
	/// </summary>
	public abstract Mask SubsetDigitsMask { get; }

	/// <summary>
	/// Indicates the subset cells.
	/// </summary>
	public abstract CellMap SubsetCells { get; }
}
