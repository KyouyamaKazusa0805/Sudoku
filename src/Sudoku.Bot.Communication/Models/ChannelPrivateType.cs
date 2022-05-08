namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the private type that describes a channel.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/channel/model.html#privatetype">this link</see>.
/// </remarks>
public enum ChannelPrivateType
{
	/// <summary>
	/// Indicates the channel is exposed to everyone.
	/// </summary>
	Everyone,

	/// <summary>
	/// Indicates the channel is only exposed to the owner and administrators.
	/// </summary>
	SpecialIdentities,

	/// <summary>
	/// Indicates the channel is only exposed to the owner, administrators and some specified members.
	/// </summary>
	SpecialIdentitiesAndSpecifiedMembers
}
