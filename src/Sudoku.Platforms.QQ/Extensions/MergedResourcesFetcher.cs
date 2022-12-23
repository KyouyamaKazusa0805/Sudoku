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
	public static string? Command(this MergedResources @this, string command) => @this[$"_{nameof(Command)}_{command}"];

	/// <summary>
	/// Try to fetch the command segment.
	/// </summary>
	/// <param name="this">The resource fetcher.</param>
	/// <param name="commandSegment">
	/// The internal name of the command segment as he key, with the prefix "<c>_CommandSegment_</c>" removed.
	/// </param>
	/// <returns>The target command segment in the resource dictionary.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string? CommandSegment(this MergedResources @this, string commandSegment) => @this[$"_{nameof(CommandSegment)}_{commandSegment}"];

	/// <summary>
	/// Try to fetch the token.
	/// </summary>
	/// <param name="this">The resource fetcher.</param>
	/// <param name="token">The internal name of the token, with the prefix "<c>_Token_</c>" removed.</param>
	/// <returns>The target token in the resource dictionary.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string? Token(this MergedResources @this, string token) => @this[$"_{nameof(Token)}_{token}"];

	/// <summary>
	/// Try to fetch the message format.
	/// </summary>
	/// <param name="this">The resource fetcher.</param>
	/// <param name="messageFormat">The message format, with the prefix "<c>_MessageFormat_</c>" removed.</param>
	/// <returns>The target message format in the resource dictionary.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string? MessageFormat(this MergedResources @this, string messageFormat) => @this[$"_{nameof(MessageFormat)}_{messageFormat}"];
}
