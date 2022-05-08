namespace System;

/// <summary>
/// Provides with extension methods on <see cref="string"/>.
/// </summary>
/// <seealso cref="string"/>
public static class StringExtensions
{
	/// <summary>
	/// Replace several characters beginning with the <see cref="string"/> instance
	/// with the newer <see cref="string"/> value, via the specified <see cref="bool"/> value
	/// indicating whether the comparison ignores the casing.
	/// </summary>
	/// <param name="this">The source string instance.</param>
	/// <param name="oldValue">The value you want to search for.</param>
	/// <param name="newValue">The new value you want to replace with.</param>
	/// <param name="ignoreCase">Indicates whether the comparison ignores the casing.</param>
	/// <returns>The string value after replaced.</returns>
	public static string ReplaceStart(this string @this, string oldValue, string newValue = "", bool ignoreCase = false)
		=> string.IsNullOrEmpty(@this)
			? @this
			: !@this.StartsWith(oldValue, ignoreCase, null)
				? @this
				: string.Concat(newValue, @this.AsSpan(oldValue.Length));

	/// <summary>
	/// Replace several characters ending with the <see cref="string"/> instance
	/// with the newer <see cref="string"/> value, via the specified <see cref="bool"/> value
	/// indicating whether the comparison ignores the casing.
	/// </summary>
	/// <param name="this">The source string instance.</param>
	/// <param name="oldValue">The value you want to search for.</param>
	/// <param name="newValue">The new value you want to replace with.</param>
	/// <param name="ignoreCase">Indicates whether the comparison ignores the casing.</param>
	/// <returns>The string value after replaced.</returns>
	public static string ReplaceEnd(this string @this, string oldValue, string newValue = "", bool ignoreCase = false)
		=> string.IsNullOrEmpty(@this)
			? @this
			: !@this.EndsWith(oldValue, ignoreCase, null)
				? @this
				: string.Concat(@this.Remove(@this.Length - oldValue.Length), newValue);

	/// <summary>
	/// Replace several characters both beginning and ending with the <see cref="string"/> instance
	/// with the newer <see cref="string"/> value, via the specified <see cref="bool"/> value
	/// indicating whether the comparison ignores the casing.
	/// </summary>
	/// <param name="this">The source string instance.</param>
	/// <param name="oldValue">The value you want to search for.</param>
	/// <param name="newValue">The new value you want to replace with.</param>
	/// <param name="ignoreCase">Indicates whether the comparison ignores the casing.</param>
	/// <returns>The string value after replaced.</returns>
	public static string Replace(this string @this, string oldValue, string newValue = "", bool ignoreCase = false)
		=> @this.ReplaceStart(oldValue, newValue, ignoreCase).ReplaceEnd(oldValue, newValue, ignoreCase);
}
