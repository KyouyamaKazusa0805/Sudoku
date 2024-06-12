namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>SelectMany</c>.
/// </summary>
/// <inheritdoc/>
public interface ISelectManyMethod<TSelf, TSource> : IQueryExpressionMethod<TSelf, TSource>
	where TSelf :
		ISelectManyMethod<TSelf, TSource>
#if NET9_0_OR_GREATER
		,
		allows ref struct
#endif
{
	/// <inheritdoc cref="Enumerable.SelectMany{TSource, TResult}(IEnumerable{TSource}, Func{TSource, IEnumerable{TResult}})"/>
	public virtual IEnumerable<TResult> SelectMany<TResult>(Func<TSource, IEnumerable<TResult>> selector)
	{
		var result = new List<TResult>();
		foreach (var element in this)
		{
			foreach (var nestedElement in selector(element))
			{
				result.Add(nestedElement);
			}
		}
		return result;
	}

	/// <inheritdoc cref="Enumerable.SelectMany{TSource, TResult}(IEnumerable{TSource}, Func{TSource, int, IEnumerable{TResult}})"/>
	public virtual IEnumerable<TResult> SelectMany<TResult>(Func<TSource, int, IEnumerable<TResult>> selector)
	{
		var result = new List<TResult>();
		var i = 0;
		foreach (var element in this)
		{
			foreach (var nestedElement in selector(element, i++))
			{
				result.Add(nestedElement);
			}
		}
		return result;
	}

	/// <inheritdoc cref="Enumerable.SelectMany{TSource, TCollection, TResult}(IEnumerable{TSource}, Func{TSource, IEnumerable{TCollection}}, Func{TSource, TCollection, TResult})"/>
	public virtual IEnumerable<TResult> SelectMany<TCollection, TResult>(Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
	{
		var result = new List<TResult>();
		foreach (var element in this)
		{
			foreach (var nestedElement in collectionSelector(element))
			{
				result.Add(resultSelector(element, nestedElement));
			}
		}
		return result;
	}

	/// <inheritdoc cref="Enumerable.SelectMany{TSource, TCollection, TResult}(IEnumerable{TSource}, Func{TSource, int, IEnumerable{TCollection}}, Func{TSource, TCollection, TResult})"/>
	public virtual IEnumerable<TResult> SelectMany<TCollection, TResult>(Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
	{
		var result = new List<TResult>();
		var i = 0;
		foreach (var element in this)
		{
			foreach (var nestedElement in collectionSelector(element, i++))
			{
				result.Add(resultSelector(element, nestedElement));
			}
		}
		return result;
	}
}
