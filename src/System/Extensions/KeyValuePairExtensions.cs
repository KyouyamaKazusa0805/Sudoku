namespace System.Collections.Generic;

/// <summary>
/// Provides with extension methods on <see cref="KeyValuePair{TKey, TValue}"/>.
/// </summary>
/// <seealso cref="KeyValuePair{TKey, TValue}"/>
public static class KeyValuePairExtensions
{
	/// <summary>
	/// Converts the current <see cref="KeyValuePair{TKey, TValue}"/> instance into a pair of values.
	/// </summary>
	/// <typeparam name="T1">The type of the key.</typeparam>
	/// <typeparam name="T2">The type of the value.</typeparam>
	/// <param name="this">A <see cref="KeyValuePair{TKey, TValue}"/> instance to be converted from.</param>
	/// <returns>The final pair of values converted.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (T1, T2) ToTuple<T1, T2>(this KeyValuePair<T1, T2> @this) => (@this.Key, @this.Value);
}
