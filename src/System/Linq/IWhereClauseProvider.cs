namespace System.Linq;

/// <summary>
/// Defines a type that supports <see langword="where"/> clause.
/// </summary>
/// <typeparam name="T">The type of the element.</typeparam>
public interface IWhereClauseProvider<T> : ILinqProvider<T>
{
	/// <summary>
	/// Filters the specified collection, removing elements not satisfying the specified condition.
	/// </summary>
	/// <param name="condition">The condition to filter elements.</param>
	/// <returns>The filtered collection of element type <typeparamref name="T"/>.</returns>
	public abstract IEnumerable<T> Where(Func<T, bool> condition);

	/// <inheritdoc cref="Where(Func{T, bool})"/>
	public sealed unsafe IEnumerable<T> WhereUnsafe(delegate*<T, bool> condition) => Where(e => condition(e));
}
