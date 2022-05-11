namespace System.Collections.Generic;

/// <summary>
/// Provides with extension methods on <see cref="Dictionary{TKey, TValue}"/> of <see cref="string"/>
/// and <see cref="string"/>.
/// </summary>
/// <seealso cref="Dictionary{TKey, TValue}"/>
internal static class DictionaryOfStringStringExtensions
{
	public static Dictionary<TKey, TValue> AppendOrCover<TKey, TValue>(
		this Dictionary<TKey, TValue> @this, TKey key, TValue value)
		where TKey : notnull
	{
		if (@this.ContainsKey(key))
		{
			@this[key] = value;
		}
		else
		{
			@this.Add(key, value);
		}

		return @this;
	}
}
