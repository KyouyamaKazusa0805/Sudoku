namespace Sudoku.Bot.Communication.Resources;

/// <summary>
/// Provides a type that can easily get the string resources.
/// </summary>
internal static class StringResource
{
	/// <summary>
	/// Try to get the value via the specified key.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <returns>The value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string? Get(string key) => ResourceDictionary.ResourceManager.GetString(key);
}
