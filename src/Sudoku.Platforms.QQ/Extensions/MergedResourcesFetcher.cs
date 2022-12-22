namespace Sudoku.Resources;

/// <summary>
/// Provides with extension methods on <see cref="MergedResources"/>.
/// </summary>
/// <seealso cref="MergedResources"/>
internal static class MergedResourcesFetcher
{
	/// <summary>
	/// Try to fetch the command name.
	/// </summary>
	/// <param name="this">The resource fetcher.</param>
	/// <param name="command">The internal name of the command as the key, with the prefix "<c>_Command_</c>" removed.</param>
	/// <returns>The target command in the resource dictionary.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string? CommandName(this MergedResources @this, string command) => @this[$"_Command_{command}"];

	/// <summary>
	/// Try to fetch the token.
	/// </summary>
	/// <param name="this">The resource fetcher.</param>
	/// <param name="token">The internal name o the token, with the prefix "<c>_Token_</c>" removed.</param>
	/// <returns>The target token in the resource dictionary.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string? Token(this MergedResources @this, string token) => @this[$"_Token_{token}"];
}
