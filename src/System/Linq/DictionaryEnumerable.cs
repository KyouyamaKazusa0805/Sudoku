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
}
