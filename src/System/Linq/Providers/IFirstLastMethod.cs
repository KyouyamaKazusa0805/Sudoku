namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>First</c>, <c>FirstOrDefault</c>, <c>Last</c> and <c>LastOrDefault</c>.
/// </summary>
/// <inheritdoc/>
public interface IFirstLastMethod<TSelf, TSource> : IAnyAllMethod<TSelf, TSource>, ILinqMethod<TSelf, TSource>
	where TSelf : IFirstLastMethod<TSelf, TSource>
{
	/// <inheritdoc cref="Enumerable.First{TSource}(IEnumerable{TSource})"/>
	public virtual TSource First() => TryGetFirst(out var result) ? result : throw new InvalidOperationException();

	/// <inheritdoc cref="Enumerable.First{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
	public virtual TSource First(Func<TSource, bool> predicate)
		=> TryGetFirst(predicate, out var result) ? result : throw new InvalidOperationException();

	/// <inheritdoc cref="Enumerable.FirstOrDefault{TSource}(IEnumerable{TSource})"/>
	public virtual TSource? FirstOrDefault() => TryGetFirst(out var result) ? result : default;

	/// <inheritdoc cref="Enumerable.FirstOrDefault{TSource}(IEnumerable{TSource}, TSource)"/>
	public virtual TSource FirstOrDefault(TSource defaultValue) => TryGetFirst(out var result) ? result : defaultValue;

	/// <inheritdoc cref="Enumerable.FirstOrDefault{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
	public virtual TSource? FirstOrDefault(Func<TSource, bool> predicate)
		=> TryGetFirst(predicate, out var result) ? result : default;

	/// <inheritdoc cref="Enumerable.FirstOrDefault{TSource}(IEnumerable{TSource}, Func{TSource, bool}, TSource)"/>
	public virtual TSource FirstOrDefault(Func<TSource, bool> predicate, TSource defaultValue)
		=> TryGetFirst(predicate, out var result) ? result : defaultValue;

	/// <inheritdoc cref="Enumerable.Last{TSource}(IEnumerable{TSource})"/>
	public virtual TSource Last() => TryGetLast(out var result) ? result : throw new InvalidOperationException();

	/// <inheritdoc cref="Enumerable.Last{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
	public virtual TSource Last(Func<TSource, bool> predicate)
		=> TryGetLast(predicate, out var result) ? result : throw new InvalidOperationException();

	/// <inheritdoc cref="Enumerable.LastOrDefault{TSource}(IEnumerable{TSource})"/>
	public virtual TSource? LastOrDefault() => TryGetLast(out var result) ? result : default;

	/// <inheritdoc cref="Enumerable.LastOrDefault{TSource}(IEnumerable{TSource}, TSource)"/>
	public virtual TSource LastOrDefault(TSource defaultValue) => TryGetLast(out var result) ? result : defaultValue;

	/// <inheritdoc cref="Enumerable.LastOrDefault{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
	public virtual TSource? LastOrDefault(Func<TSource, bool> predicate) => TryGetLast(predicate, out var result) ? result : default;

	/// <inheritdoc cref="Enumerable.LastOrDefault{TSource}(IEnumerable{TSource}, Func{TSource, bool}, TSource)"/>
	public virtual TSource LastOrDefault(Func<TSource, bool> predicate, TSource defaultValue)
		=> TryGetLast(predicate, out var result) ? result : defaultValue;

	/// <summary>
	/// Try to get the first element in the sequence.
	/// </summary>
	/// <param name="result">The result value.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	private bool TryGetFirst([NotNullWhen(true)] out TSource? result)
	{
		using var iterator = GetEnumerator();
		if (!iterator.MoveNext())
		{
			result = default;
			return false;
		}

		result = iterator.Current!;
		return true;
	}

	/// <summary>
	/// Try to get the first element in the sequence satisfying the specified condition.
	/// </summary>
	/// <param name="predicate">The condition to be checked.</param>
	/// <param name="result">The result value.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	private bool TryGetFirst(Func<TSource, bool> predicate, [NotNullWhen(true)] out TSource? result)
	{
		using var iterator = GetEnumerator();
		if (!iterator.MoveNext())
		{
			result = default;
			return false;
		}

		do
		{
			var element = iterator.Current!;
			if (predicate(element))
			{
				result = element;
				return true;
			}
		} while (iterator.MoveNext());

		result = default;
		return false;
	}

	/// <summary>
	/// Try to get the last element in the sequence.
	/// </summary>
	/// <param name="result">The result value.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	private bool TryGetLast([NotNullWhen(true)] out TSource? result)
	{
		if (!Any())
		{
			result = default;
			return false;
		}

		var tempResult = default(TSource);
		foreach (var element in this)
		{
			tempResult = element;
		}

		result = tempResult!;
		return true;
	}

	/// <summary>
	/// Try to get the last element in the sequence satisfying the specified condition.
	/// </summary>
	/// <param name="predicate">The condition to be checked.</param>
	/// <param name="result">The result value.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	private bool TryGetLast(Func<TSource, bool> predicate, [NotNullWhen(true)] out TSource? result)
	{
		if (!Any())
		{
			result = default;
			return false;
		}

		var (hasAtLeastOneElement, tempResult) = (false, default(TSource));
		foreach (var element in this)
		{
			if (predicate(element))
			{
				tempResult = element;
				hasAtLeastOneElement = true;
			}
		}
		if (!hasAtLeastOneElement)
		{
			result = default;
			return false;
		}

		result = tempResult!;
		return true;
	}
}
