namespace Sudoku.Solving.Collections;

/// <summary>
/// Defines a normal pattern.
/// </summary>
/// <typeparam name="TSelf">The type of the pattern itself.</typeparam>
/// <typeparam name="TMask">The type of the mask.</typeparam>
public interface IPattern<TSelf, TMask>
where TSelf : struct, IEquatable<TSelf>, IPattern<TSelf, TMask>
where TMask : struct
{
	/// <summary>
	/// Indicates the mask value.
	/// </summary>
	TMask Mask { get; init; }

	/// <summary>
	/// Indicates the summary map that holds all cells of this pattern.
	/// </summary>
	Cells Map { get; }


	/// <summary>
	/// Implicit cast from <typeparamref name="TMask"/> to <typeparamref name="TSelf"/>.
	/// </summary>
	/// <param name="mask">The mask.</param>
	static abstract implicit operator TSelf(TMask mask);
}
