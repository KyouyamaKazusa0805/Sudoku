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
	/// <typeparam name="TKey">The type of key.</typeparam>
	/// <typeparam name="TValue">The type of value.</typeparam>
	/// <param name="this">A <see cref="KeyValuePair{TKey, TValue}"/> instance to be converted from.</param>
	/// <returns>The final pair of values converted.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (TKey, TValue) ToTuple<TKey, TValue>(this KeyValuePair<TKey, TValue> @this) => (@this.Key, @this.Value);

	/// <summary>
	/// Get reference to key.
	/// </summary>
	/// <typeparam name="TKey">The type of key.</typeparam>
	/// <typeparam name="TValue">The type of value.</typeparam>
	/// <param name="this">The current <see cref="KeyValuePair{TKey, TValue}"/> instance.</param>
	/// <returns>The reference to key.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref readonly TKey KeyRef<TKey, TValue>(this ref readonly KeyValuePair<TKey, TValue> @this)
		=> ref Entry<TKey, TValue>.GetKey(in @this);

	/// <summary>
	/// Get reference to value.
	/// </summary>
	/// <typeparam name="TKey">The type of key.</typeparam>
	/// <typeparam name="TValue">The type of value.</typeparam>
	/// <param name="this">The current <see cref="KeyValuePair{TKey, TValue}"/> instance.</param>
	/// <returns>The reference to value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref readonly TValue ValueRef<TKey, TValue>(this ref readonly KeyValuePair<TKey, TValue> @this)
		=> ref Entry<TKey, TValue>.GetValue(in @this);
}

/// <summary>
/// Represents an entry to call internal fields on <see cref="KeyValuePair{TKey, TValue}"/>.
/// </summary>
/// <typeparam name="TKey">The key type of each element in <see cref="KeyValuePair{TKey, TValue}"/>.</typeparam>
/// <typeparam name="TValue">The value type of each element in <see cref="KeyValuePair{TKey, TValue}"/>.</typeparam>
/// <seealso cref="KeyValuePair{TKey, TValue}"/>
file sealed class Entry<TKey, TValue>
{
	/// <summary>
	/// Try to fetch the internal field <c>key</c> in type <see cref="KeyValuePair{TKey, TValue}"/>.
	/// </summary>
	/// <param name="this">The key-value pair.</param>
	/// <returns>The reference to the internal field.</returns>
	/// <remarks>
	/// <include
	///     file="../../global-doc-comments.xml"
	///     path="//g/dotnet/version[@value='8']/feature[@name='unsafe-accessor']/target[@name='others']"/>
	/// <include
	///     file="../../global-doc-comments.xml"
	///     path="//g/dotnet/version[@value='8']/feature[@name='unsafe-accessor']/target[@name='field-related-method']"/>
	/// <include
	///     file="../../global-doc-comments.xml"
	///     path="//g/dotnet/version[@value='8']/feature[@name='unsafe-accessor']/target[@type='struct']"/>
	/// </remarks>
	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = LibraryIdentifiers.KeyValuePair_Key)]
	public static extern ref TKey GetKey(ref readonly KeyValuePair<TKey, TValue> @this);

	/// <summary>
	/// Try to fetch the internal field <c>value</c> in type <see cref="KeyValuePair{TKey, TValue}"/>.
	/// </summary>
	/// <param name="this">The key-value pair.</param>
	/// <returns>The reference to the internal field.</returns>
	/// <remarks>
	/// <include
	///     file="../../global-doc-comments.xml"
	///     path="//g/dotnet/version[@value='8']/feature[@name='unsafe-accessor']/target[@name='others']"/>
	/// <include
	///     file="../../global-doc-comments.xml"
	///     path="//g/dotnet/version[@value='8']/feature[@name='unsafe-accessor']/target[@name='field-related-method']"/>
	/// <include
	///     file="../../global-doc-comments.xml"
	///     path="//g/dotnet/version[@value='8']/feature[@name='unsafe-accessor']/target[@type='struct']"/>
	/// </remarks>
	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = LibraryIdentifiers.KeyValuePair_Value)]
	public static extern ref TValue GetValue(ref readonly KeyValuePair<TKey, TValue> @this);
}
