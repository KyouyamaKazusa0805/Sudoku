namespace Sudoku.Bot.Communication.Models.MessageTemplates;

/// <summary>
/// Provides a creator that can create tags for mentioning.
/// </summary>
public static class TagCreator
{
	/// <summary>
	/// Creates a tag message that mentions the specified user.
	/// </summary>
	/// <param name="user">The user instance.</param>
	/// <returns>The string representation of the tag.</returns>
	public static string Tag(this User user) => $"<@!{user.Id}>";

	/// <summary>
	/// Creates a tag message that mentions the channel.
	/// </summary>
	/// <param name="channel">The channel instance.</param>
	/// <returns>The string representation of the channel.</returns>
	public static string Tag(this Channel channel) => $"<#{channel.Id}>";
}
