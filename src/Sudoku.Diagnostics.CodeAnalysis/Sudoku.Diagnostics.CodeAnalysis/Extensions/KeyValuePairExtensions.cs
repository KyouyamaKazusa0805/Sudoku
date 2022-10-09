namespace System.Collections.Generic;

/// <summary>
/// Provides with extension methods on <see cref="KeyValuePair{TKey, TValue}"/>.
/// </summary>
/// <seealso cref="KeyValuePair{TKey, TValue}"/>
internal static class KeyValuePairExtensions
{
	/// <include
	///     file="../../../global-doc-comments.xml"
	///     path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> @this, out TKey key, out TValue value)
		=> (key, value) = (@this.Key, @this.Value);
}
