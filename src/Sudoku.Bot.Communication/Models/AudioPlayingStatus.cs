namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the playing status for an <see cref="AudioControl"/> instance.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/audio/model.html#status">this link</see>.
/// </remarks>
public enum AudioPlayingStatus
{
	/// <summary>
	/// Indicates the audio is started playing.
	/// </summary>
	Start,

	/// <summary>
	/// Indicates the audio is paused.
	/// </summary>
	Pause,

	/// <summary>
	/// Indicates the audio is resumed.
	/// </summary>
	Resume,

	/// <summary>
	/// Indicates the audio is stopped.
	/// </summary>
	Stop
}
