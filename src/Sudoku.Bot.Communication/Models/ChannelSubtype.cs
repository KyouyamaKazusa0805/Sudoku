namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the subtype of the channel. The type will only be used if the property <see cref="Channel.Type"/>
/// corresponds to <see cref="ChannelType.Text"/>.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/channel/model.html#channelsubtype">this link</see>.
/// </remarks>
/// <seealso cref="Channel.Type"/>
/// <seealso cref="ChannelType"/>
public enum ChannelSubtype
{
	/// <summary>
	/// Indicates the subtype is gossiping.
	/// </summary>
	Gossiping,

	/// <summary>
	/// Indicates the subtype is an announcement.
	/// </summary>
	Announcement,

	/// <summary>
	/// Indicates the subtype is strategy.
	/// </summary>
	Strategy,

	/// <summary>
	/// Indicates the subtype is gaming (on game called "Honor of Kings").
	/// </summary>
	/// <remarks>
	/// The abbreviation "HoK" in this field is the game name "Honor of Kings".
	/// </remarks>
	GammingOnHok
}
