namespace System.Collections.Generic;

/// <summary>
/// Provides with extension methods on <see cref="Dictionary{TKey, TValue}"/>.
/// </summary>
/// <seealso cref="Dictionary{TKey, TValue}"/>
public static class DictionaryExtensions
{
	/// <summary>
	/// Try to get the reference to the value whose corresponding key is specified one.
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	/// <param name="this">The current dictionary instance.</param>
	/// <param name="key">The key to be checked.</param>
	/// <returns>The reference to the value; or a <see langword="null"/> reference if the key is not found.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref TValue GetValueRef<TKey, TValue>(this Dictionary<TKey, TValue> @this, ref readonly TKey key)
		where TKey : notnull
		=> ref CollectionsMarshal.GetValueRefOrNullRef(@this, key);
}
