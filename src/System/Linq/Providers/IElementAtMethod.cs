namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>ElementAt</c>.
/// </summary>
/// <inheritdoc/>
public interface IElementAtMethod<TSelf, TSource> : ICountMethod<TSelf, TSource>, ILinqMethod<TSelf, TSource>
	where TSelf :
		IElementAtMethod<TSelf, TSource>
#if NET9_0_OR_GREATER
		,
		allows ref struct
#endif
{
	/// <inheritdoc cref="Enumerable.ElementAt{TSource}(IEnumerable{TSource}, int)"/>
	public virtual TSource ElementAt(int index)
	{
		var i = -1;
		foreach (var element in this)
		{
			if (++i == index)
			{
				return element;
			}
		}
		throw new InvalidOperationException();
	}

	/// <inheritdoc cref="Enumerable.ElementAt{TSource}(IEnumerable{TSource}, Index)"/>
	public virtual TSource ElementAt(Index index)
	{
		var targetIndex = index.GetOffset(Count());

		var i = -1;
		foreach (var element in this)
		{
			if (++i == targetIndex)
			{
				return element;
			}
		}
		throw new InvalidOperationException();
	}

	/// <inheritdoc cref="Enumerable.ElementAtOrDefault{TSource}(IEnumerable{TSource}, int)"/>
	public virtual TSource? ElementAtOrDefault(int index)
	{
		var i = -1;
		foreach (var element in this)
		{
			if (++i == index)
			{
				return element;
			}
		}
		return default;
	}

	/// <inheritdoc cref="Enumerable.ElementAtOrDefault{TSource}(IEnumerable{TSource}, Index)"/>
	public virtual TSource? ElementAtOrDefault(Index index)
	{
		var targetIndex = index.GetOffset(Count());

		var i = -1;
		foreach (var element in this)
		{
			if (++i == targetIndex)
			{
				return element;
			}
		}
		return default;
	}
}
