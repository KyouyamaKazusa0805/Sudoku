namespace System;

/// <summary>
/// Provides with extension methods on <see cref="string"/>.
/// </summary>
/// <seealso cref="string"/>
public static class StringExtensions
{
	/// <summary>
	/// 替换字符串开始位置的字符串
	/// </summary>
	/// <param name="input">源字符串</param>
	/// <param name="oldValue">查找的串</param>
	/// <param name="newValue">替换的串</param>
	/// <param name="ignoreCase">查找字符串时忽略大小写</param>
	/// <returns></returns>
	public static string TrimStart(this string input, string oldValue, string newValue = "", bool ignoreCase = false)
		=> string.IsNullOrEmpty(input)
			? input
			: !input.StartsWith(oldValue, ignoreCase, null)
				? input
				: string.Concat(newValue, input.AsSpan(oldValue.Length));

	/// <summary>
	/// 替换字符串末尾位置的字符串
	/// </summary>
	/// <param name="input">源字符串</param>
	/// <param name="oldValue">查找的串</param>
	/// <param name="newValue">替换的串</param>
	/// <param name="ignoreCase">查找字符串时忽略大小写</param>
	/// <returns></returns>
	public static string TrimEnd(this string input, string oldValue, string newValue = "", bool ignoreCase = false)
		=> string.IsNullOrEmpty(input)
			? input
			: !input.EndsWith(oldValue, ignoreCase, null)
				? input
				: string.Concat(input.Remove(input.Length - oldValue.Length), newValue);

	/// <summary>
	/// 替换字符串开始和末尾位置的字符串
	/// </summary>
	/// <param name="input">源字符串</param>
	/// <param name="oldValue">查找的串</param>
	/// <param name="newValue">替换的串</param>
	/// <param name="ignoreCase">查找字符串时忽略大小写</param>
	/// <returns></returns>
	public static string Trim(this string input, string oldValue, string newValue = "", bool ignoreCase = false)
		=> input.TrimStart(oldValue, newValue, ignoreCase).TrimEnd(oldValue, newValue, ignoreCase);

	/// <summary>
	/// 判断字符串是否为空白
	/// <para>效果等同于string.IsNullOrWhiteSpace()</para>
	/// </summary>
	/// <param name="input"></param>
	/// <returns></returns>
	public static bool IsBlank(this string? input) => string.IsNullOrWhiteSpace(input);
}
