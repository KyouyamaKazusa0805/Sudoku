namespace Sudoku.Solving.Collections;

/// <summary>
/// Defines a normal pattern.
/// </summary>
/// <typeparam name="TSelf">The type of the pattern itself.</typeparam>
public interface IPattern<TSelf> where TSelf : struct, IEquatable<TSelf>, IPattern<TSelf>
{
	/// <summary>
	/// Indicates the summary map that holds all cells of this pattern.
	/// </summary>
	Cells Map { get; }
}
