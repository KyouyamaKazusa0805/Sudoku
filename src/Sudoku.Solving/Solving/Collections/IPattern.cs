namespace Sudoku.Solving.Collections;

/// <summary>
/// Defines a normal pattern.
/// </summary>
/// <typeparam name="T">The type of the pattern itself.</typeparam>
public interface IPattern<[Self] T> where T : struct, IEquatable<T>, IPattern<T>
{
	/// <summary>
	/// Indicates the summary map that holds all cells of this pattern.
	/// </summary>
	Cells Map { get; }
}
