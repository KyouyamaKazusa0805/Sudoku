namespace System;

/// <summary>
/// Provides with extension methods on <see cref="string"/>.
/// </summary>
/// <seealso cref="string"/>
public static class StringExtensions
{
	/// <summary>
	/// Try to convert the specified identifier into pascal casing.
	/// </summary>
	public static string ToPascalCasing(this string @this) => $"{char.ToUpper(@this[0])}{@this[1..]}";

	/// <summary>
	/// Try to convert the specified identifier into camel casing.
	/// </summary>
	public static string ToCamelCasing(this string @this) => $"{char.ToLower(@this[0])}{@this[1..]}";

	/// <summary>
	/// Try to convert the specified identifier into camel casing, with a leading underscore character.
	/// </summary>
	public static string ToUnderscoreCamelCasing(this string @this) => $"_{@this.ToCamelCasing()}";
}
