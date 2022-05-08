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
		var api = BotApis.音频控制;
		var response = await HttpSendAsync(
			api.Path.Replace("{channel_id}", channel_id),
			api.Method,
			JsonContent.Create(audioControl),
			sender
		);

		return response is null ? null : await response.Content.ReadFromJsonAsync<Message?>();
	}
}
