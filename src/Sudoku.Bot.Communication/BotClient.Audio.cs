namespace Sudoku.Bot.Communication;

partial class BotClient
{
	/// <summary>
	/// To control an audio.
	/// </summary>
	/// <param name="channel_id">The channel ID. <b>The argument cannot be renamed.</b></param>
	/// <param name="audioControl">The audio to be controlled.</param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>
	/// A task that encapsulates a <see cref="Message"/> instance as the result value
	/// after being <see langword="await"/>ed.
	/// </returns>
	public async Task<Message?> AudioControlAsync(string channel_id, AudioControl audioControl, Sender? sender)
		=> BotApis.ControlAudioInChannel is { Path: var path, Method: var method }
		&& path.ReplaceArgument(channel_id) is var replacedPath
		&& JsonContent.Create(audioControl) is var jsonContent
		&& await HttpSendAsync(replacedPath, method, jsonContent, sender) is { Content: var responseContent }
			? await responseContent.ReadFromJsonAsync<Message?>()
			: null;
}
