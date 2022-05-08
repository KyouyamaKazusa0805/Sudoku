namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the privacy type that corresponds to the cases
/// that a user is allowed doing in a channel.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/channel_permissions/model.html#permissions">this link</see>.
/// </remarks>
[Flags]
public enum PrivacyType
{
	/// <summary>
	/// Indicates the user has no permissions to visit the channel.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates the user can visit the channel. In other words, the channel is visible for the user.
	/// </summary>
	Visible = 1 << 0,

	/// <summary>
	/// Indicates the user can manage the channel.
	/// </summary>
	Managable = 1 << 1,

	/// <summary>
	/// Indicates the user can talk in the channel.
	/// </summary>
	Speakable = 1 << 2
}
