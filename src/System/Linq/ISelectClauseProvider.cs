namespace System.Linq;

/// <summary>
/// Defines a type that supports <see langword="select"/> and <see langword="let"/> clauses.
/// </summary>
/// <typeparam name="T">The type of the element.</typeparam>
public interface ISelectClauseProvider<T> : ILinqProvider<T>
{
	/// <summary>
	/// Projects the collection, to an immutable result of target type <typeparamref name="TResult"/>.
	/// </summary>
	/// <typeparam name="TResult">The target type converted.</typeparam>
	/// <param name="selector">
	/// The selector to project the <typeparamref name="T"/> instance
	/// into type <typeparamref name="TResult"/>.
	/// </param>
	/// <returns>The projected collection of element type <typeparamref name="TResult"/>.</returns>
	public abstract IEnumerable<TResult> Select<TResult>(Func<T, TResult> selector);

	/// <inheritdoc cref="Select{TResult}(Func{T, TResult})"/>
	public sealed unsafe IEnumerable<TResult> SelectUnsafe<TResult>(delegate*<T, TResult> selector)
		=> Select(e => selector(e));
}
