namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the private message source instance.
/// </summary>
/// <param name="GuildId">Indicates the GUILD ID value.</param>
/// <param name="ChannelId">Indicates the channel ID.</param>
/// <param name="Time">Indicates the time that the message created.</param>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/dms/model.html">this link</see>.
/// </remarks>
public sealed record class DirectMessageSource(
	[property: JsonPropertyName("guild_id")] string GuildId,
	[property: JsonPropertyName("channel_id")] string ChannelId,
	[property: JsonPropertyName("create_time"), JsonConverter(typeof(DateTimeTimestampConverter))] DateTime Time);
