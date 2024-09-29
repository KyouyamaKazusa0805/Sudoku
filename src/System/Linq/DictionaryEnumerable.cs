namespace System.Linq;

/// <summary>
/// Represents LINQ methods used by <see cref="Dictionary{TKey, TValue}"/> instances.
/// </summary>
/// <seealso cref="Dictionary{TKey, TValue}"/>
public static class DictionaryEnumerable
{
	/// <summary>
	/// Get the maximal value in all <see cref="Dictionary{TKey, TValue}.Values"/>.
	/// </summary>
	/// <typeparam name="TKey">The type of each key.</typeparam>
	/// <typeparam name="TValue">The type of each value.</typeparam>
	/// <param name="this">The dictionary instance.</param>
	/// <returns>An instance of type <typeparamref name="TValue"/> as maximal-valued one.</returns>
	/// <seealso cref="Dictionary{TKey, TValue}.Values"/>
	public static TValue MaxByValue<TKey, TValue>(this Dictionary<TKey, TValue> @this)
		where TKey : notnull
		where TValue : IComparable<TValue>, IComparisonOperators<TValue, TValue, bool>, IMinMaxValue<TValue>
	{
		var result = TValue.MinValue;
		foreach (var value in @this.Values)
		{
			if (value >= result)
			{
				result = value;
			}
		}
		return result;
	}

	/// <summary>
	/// Get the minimal value in all <see cref="Dictionary{TKey, TValue}.Values"/>.
	/// </summary>
	/// <typeparam name="TKey">The type of each key.</typeparam>
	/// <typeparam name="TValue">The type of each value.</typeparam>
	/// <param name="this">The dictionary instance.</param>
	/// <returns>An instance of type <typeparamref name="TValue"/> as minimal-valued one.</returns>
	/// <seealso cref="Dictionary{TKey, TValue}.Values"/>
	public static TValue MinByValue<TKey, TValue>(this Dictionary<TKey, TValue> @this)
		where TKey : notnull
		where TValue : IComparable<TValue>, IComparisonOperators<TValue, TValue, bool>, IMinMaxValue<TValue>
	{
		var result = TValue.MaxValue;
		foreach (var value in @this.Values)
		{
			if (value <= result)
			{
				result = value;
			}
		}
		return result;
	}

	/// <summary>
	/// Determine whether all keys in the dictionary satisfy the specified condition.
	/// </summary>
	/// <typeparam name="TKey">The type of each key.</typeparam>
	/// <typeparam name="TValue">The type of each value.</typeparam>
	/// <param name="this">The dictionary to be checked.</param>
	/// <param name="predicate">The condition to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public static bool AllKey<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> @this, Func<TKey, bool> predicate)
		where TKey : notnull
	{
		foreach (var key in @this.Keys)
		{
			if (!predicate(key))
			{
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// Determine whether all values in the dictionary satisfy the specified condition.
	/// </summary>
	/// <typeparam name="TKey">The type of each key.</typeparam>
	/// <typeparam name="TValue">The type of each value.</typeparam>
	/// <param name="this">The dictionary to be checked.</param>
	/// <param name="predicate">The condition to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public static bool AllValue<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> @this, Func<TValue, bool> predicate)
		where TKey : notnull
	{
		foreach (var value in @this.Values)
		{
			if (!predicate(value))
			{
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// Determine whether at least one key in the dictionary satisfies the specified condition.
	/// </summary>
	/// <typeparam name="TKey">The type of each key.</typeparam>
	/// <typeparam name="TValue">The type of each value.</typeparam>
	/// <param name="this">The dictionary to be checked.</param>
	/// <param name="predicate">The condition to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public static bool AnyKey<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> @this, Func<TKey, bool> predicate)
		where TKey : notnull
	{
		foreach (var key in @this.Keys)
		{
			if (predicate(key))
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Determine whether at least one value in the dictionary satisfies the specified condition.
	/// </summary>
	/// <typeparam name="TKey">The type of each key.</typeparam>
	/// <typeparam name="TValue">The type of each value.</typeparam>
	/// <param name="this">The dictionary to be checked.</param>
	/// <param name="predicate">The condition to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public static bool AnyValue<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> @this, Func<TValue, bool> predicate)
		where TKey : notnull
	{
		foreach (var value in @this.Values)
		{
			if (predicate(value))
			{
				return true;
			}
		}
		return false;
	}
}
