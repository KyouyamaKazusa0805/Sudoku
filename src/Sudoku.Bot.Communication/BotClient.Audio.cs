namespace Sudoku.Bot.Communication;

partial class BotClient
{
	/// <summary>
	/// 音频控制
	/// </summary>
	/// <param name="channel_id">子频道Id</param>
	/// <param name="audioControl">音频对象</param>
	/// <param name="sender"></param>
	/// <returns></returns>
	public async Task<Message?> AudioControlAsync(string channel_id, AudioControl audioControl, Sender? sender = null)
	{
		_ = BotApis.ControlAudioInChannel is { Path: var path, Method: var method };
		var response = await HttpSendAsync(
			path.Replace("{channel_id}", channel_id),
			method,
			JsonContent.Create(audioControl),
			sender
		);

		return response is null ? null : await response.Content.ReadFromJsonAsync<Message?>();
	}
}
