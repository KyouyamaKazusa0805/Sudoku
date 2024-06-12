namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>Select</c>.
/// </summary>
/// <inheritdoc/>
public interface ISelectMethod<TSelf, TSource> : IQueryExpressionMethod<TSelf, TSource>
	where TSelf :
		ISelectMethod<TSelf, TSource>
#if NET9_0_OR_GREATER
		,
		allows ref struct
#endif
{
	/// <inheritdoc cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, TResult})"/>
	public virtual IEnumerable<TResult> Select<TResult>(Func<TSource, TResult> selector)
	{
		var result = new List<TResult>();
		foreach (var element in this)
		{
			result.Add(selector(element));
		}
		return result;
	}

	/// <inheritdoc cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, int, TResult})"/>
	public virtual IEnumerable<TResult> Select<TResult>(Func<TSource, int, TResult> selector)
	{
		var result = new List<TResult>();
		var i = 0;
		foreach (var element in this)
		{
			result.Add(selector(element, i++));
		}
		return result;
	}
}
