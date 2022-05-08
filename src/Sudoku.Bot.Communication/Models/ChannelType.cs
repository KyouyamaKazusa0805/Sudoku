namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the type of the channel.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/channel/model.html#channeltype">this link</see>.
/// </remarks>
public enum ChannelType
{
	/// <summary>
	/// Indicates the text channel.
	/// </summary>
	Text = 0,

	/// <summary>
	/// The value doesn't correspond to any meaningful channel types. The field is only used for being reserved.
	/// </summary>
	ReservedField_1 = 1,

	/// <summary>
	/// Indicates the audio channel.
	/// </summary>
	Audio = 2,

	/// <summary>
	/// The value doesn't correspond to any meaningful channel types. The field is only used for being reserved.
	/// </summary>
	ReservedField_2 = 3,

	/// <summary>
	/// Indicates the grouping channel.
	/// </summary>
	Grouping = 4,

	/// <summary>
	/// Indicates the live channel.
	/// </summary>
	Live = 10005,

	/// <summary>
	/// Indicates the application channel.
	/// </summary>
	Application = 10006,

	/// <summary>
	/// Indicates the BBS channel.
	/// </summary>
	Bbs = 10007
}
