namespace System.Collections.Generic;

/// <summary>
/// Provides with the extension methods on type <see cref="KeyValuePair{TKey, TValue}"/>.
/// </summary>
public static class KeyValuePairExtensions
{
	/// <summary>
	/// Deconstruct the instance of type <see cref="KeyValuePair{TKey, TValue}"/> into two values:
	/// <list type="table">
	/// <item>
	/// <term><see cref="KeyValuePair{TKey, TValue}.Key"/></term>
	/// <description>The key.</description>
	/// </item>
	/// <item>
	/// <term><see cref="KeyValuePair{TKey, TValue}.Value"/></term>
	/// <description>The value.</description>
	/// </item>
	/// </list>
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	/// <param name="this">The instance to be deconstructed.</param>
	/// <param name="key">The key.</param>
	/// <param name="value">The value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Deconstruct<TKey, TValue>(
		this KeyValuePair<TKey, TValue> @this, out TKey key, out TValue value)
	{
		key = @this.Key;
		value = @this.Value;
	}
}
