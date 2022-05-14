namespace Sudoku.Bot.Communication;

/// <summary>
/// Provides with extension methods on <see cref="string"/>, represents the message content.
/// </summary>
internal static class MessageContent
{
	/// <summary>
	/// Removes the tag in the message content.
	/// </summary>
	/// <param name="messageContent">The message content.</param>
	/// <param name="user">The user information to get the tag.</param>
	/// <returns>The removed value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string RemoveTag(string messageContent, User user)
		=> messageContent.Trim().ReplaceStart(user.Tag).TrimStart();
}
