namespace System.Text.RegularExpressions;

/// <summary>
/// Provides extension methods on <see cref="Regex"/>.
/// </summary>
/// <seealso cref="Regex"/>
internal static class RegexExtensions
{
	/// <summary>
	/// Try to match the specified pattern using the specified input value, and gets the result.
	/// If failed to match or even any exceptions thrown, the method will return <see langword="false"/>,
	/// and the argument <paramref name="result"/> will keep the <see langword="null"/> value.
	/// </summary>
	/// <param name="input">The input string.</param>
	/// <param name="pattern">The pattern to match.</param>
	/// <param name="result">The result <see cref="Match"/> instance.</param>
	/// <returns>The <see cref="bool"/> value indicating whether the operation is successful.</returns>
	public static bool TryMatch(string input, string pattern, out Match? result)
	{
		try
		{
			result = Regex.Match(input, pattern);
			return true;
		}
		catch (Exception ex) when (ex is RegexMatchTimeoutException or ArgumentException)
		{
			result = null;
			return false;
		}
	}
}
