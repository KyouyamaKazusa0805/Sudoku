namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the instance that describes an audio action.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/audio/model.html#audioaction">this link</see>.
/// </remarks>
public sealed class AudioAction
{
	/// <summary>
	/// Indicates the GUILD ID.
	/// </summary>
	[JsonPropertyName("guild_id")]
	public string? GuildId { get; set; }

	/// <summary>
	/// Indicates the channel ID.
	/// </summary>
	[JsonPropertyName("channel_id")]
	public string? ChannelId { get; set; }

	/// <summary>
	/// Indicates the URL corresponds to the audio itself.
	/// The audio data will be transferred when the property <see cref="AudioControl.Status"/> is 0.
	/// </summary>
	/// <seealso cref="AudioControl.Status"/>
	[JsonPropertyName("audio_url")]
	public string? AudioUrl { get; set; }

	/// <summary>
	/// Indicates the text that introduces the audio.
	/// The audio data will be transferred when the property <see cref="AudioControl.Status"/> is 0.
	/// </summary>
	/// <seealso cref="AudioControl.Status"/>
	[JsonPropertyName("text")]
	public string? Text { set; get; }
}
