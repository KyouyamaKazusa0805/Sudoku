namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the type that controls the user talking in a channel.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/channel/model.html#speakpermission">this link</see>.
/// </remarks>
public enum ChannelTalkingPermission
{
	/// <summary>
	/// Indicates the permission is invalid.
	/// </summary>
	Null,

	/// <summary>
	/// Indicates the permission level is everyone can talk in this channel.
	/// </summary>
	Everyone,

	/// <summary>
	/// Indicates the permission level is that only channel owner, administrators and some specified members
	/// can talk in this channel.
	/// </summary>
	SpecialIdentitiesWithSpecifiedMembers
}
