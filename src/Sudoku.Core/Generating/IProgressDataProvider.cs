namespace Sudoku.Generating;

/// <summary>
/// Indicates the data provider type,
/// </summary>
/// <typeparam name="TSelf"><include file="../../global-doc-comments.xml" path="/g/self-type-constraint"/></typeparam>
public interface IProgressDataProvider<TSelf>
	where TSelf : struct, IEquatable<TSelf>, IProgressDataProvider<TSelf>, allows ref struct
{
	/// <summary>
	/// Indicates the number of puzzles having been generated.
	/// </summary>
	public abstract int Count { get; init; }


	/// <summary>
	/// Try to fetch display string for the current instance.
	/// </summary>
	/// <returns>The display string.</returns>
	public abstract string ToDisplayString();


	/// <summary>
	/// Try to create a <typeparamref name="TSelf"/> instance.
	/// </summary>
	/// <param name="count">The number of puzzles generated.</param>
	/// <param name="succeeded">The number of puzzles has passed the checking.</param>
	/// <returns>A <typeparamref name="TSelf"/> instance.</returns>
	public static abstract TSelf Create(int count, int succeeded);
}
