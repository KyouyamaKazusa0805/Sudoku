namespace Sudoku.Bot.Oicq.Concepts.Helpers;

/// <summary>
/// Provides extension methods for an <c>*.INI</c> file object.
/// </summary>
public static class IniObjectExtensions
{
	/// <summary>
	/// Converts from an INI object to a <see cref="bool"/> value.
	/// </summary>
	/// <param name="str">The INI object as the string representation.</param>
	/// <returns>A <see cref="bool"/> value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ToBool(this string str) => bool.TryParse(str, out bool parsedResult) && parsedResult;

	/// <summary>
	/// Converts from an INI object to an <see cref="int"/> value.
	/// </summary>
	/// <param name="str">The INI object as the string representation.</param>
	/// <returns>An <see cref="int"/> value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ToInt32(this string str) => string.IsNullOrEmpty(str) ? 0 : int.TryParse(str, out int r) ? r : 0;
}
