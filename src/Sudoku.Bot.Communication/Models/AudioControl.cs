namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the instance that describes an audio instance.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/audio/model.html#audiocontrol">this link</see>.
/// </remarks>
public sealed class AudioControl
{
	/// <summary>
	/// Inidcates the URL corresponds to the audio itself.
	/// The audio data will be transferred when the property <see cref="Status"/> is 0.
	/// </summary>
	/// <seealso cref="Status"/>
	[JsonPropertyName("audio_url")]
	public string? AudioUrl { get; set; }

	/// <summary>
	/// Indicates the text that introduces the audio instance.
	/// The audio data will be transferred when the property <see cref="Status"/> is 0.
	/// </summary>
	/// <seealso cref="Status"/>
	[JsonPropertyName("text")]
	public string? Text { get; set; }

	/// <summary>
	/// Indicates the playing status for an audio instance.
	/// </summary>
	[JsonPropertyName("status")]
	public AudioPlayingStatus Status { get; set; }
}
