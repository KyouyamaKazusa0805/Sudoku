namespace Sudoku.Platforms.QQ;

/// <summary>
/// Extracts the prefix array provided, used by commands.
/// </summary>
internal static class CommonCommandPrefixes
{
	/// <summary>
	/// The hashtag symbol "<c>#</c>".
	/// </summary>
	public static readonly string[] HashTag = { "#", "\uff03" };

	/// <summary>
	/// The bang symbol "<c>!</c>".
	/// </summary>
	public static readonly string[] Bang = { "!", "\uff01" };
}
