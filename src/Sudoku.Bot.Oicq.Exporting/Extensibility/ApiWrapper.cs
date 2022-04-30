namespace Sudoku.Bot.Oicq.Extensibility;

/// <summary>
/// Provides an API wrapper.
/// </summary>
public sealed class ApiWrapper : IApiWrapper
{
	/// <summary>
	/// Indicates the QQ number of the bot.
	/// </summary>
	private string _bot = null!;


	/// <inheritdoc/>
	public void SendGroupMessage(string group, string msg) => Api.SendMessage(_bot, 0, 2, group, string.Empty, msg, 0);

	/// <inheritdoc/>
	public void SendC2cMessage(string qq, string msg) => Api.SendMessage(_bot, 0, 1, string.Empty, qq, msg, 0);

	/// <inheritdoc/>
	public void OutputLog(string message) => Api.Output(message);

	/// <inheritdoc/>
	public void Init(params object[] args)
	{
	}

	/// <inheritdoc/>
	public void SetData(AmiableEventArgs data) => _bot = data.Bot.ToString();

	/// <inheritdoc/>
	public void HandleGroupEvent(int requestType, string qq, string group, string seq, int messageType, string message)
		=> throw new NotImplementedException();

	/// <inheritdoc/>
	public object Clone() => MemberwiseClone();

	/// <inheritdoc/>
	public bool IsOnline(string qq) => throw new NotImplementedException();

	/// <inheritdoc/>
	public bool SignIn(string group, string address, string message) => throw new NotImplementedException();

	/// <inheritdoc/>
	public string GetAppDirectory(string AppName)
	{
		string dir = Path.Combine(Directory.GetCurrentDirectory(), "config", AppName);
		if (!Directory.Exists(dir))
		{
			Directory.CreateDirectory(dir);
		}

		return dir;
	}

	/// <inheritdoc/>
	public string GetFriendsRemark(string qq) => throw new NotImplementedException();

	/// <inheritdoc/>
	public string GetGroupAdmin(string group) => throw new NotImplementedException();

	/// <inheritdoc/>
	public string GetGroupName(string group) => throw new NotImplementedException();

	/// <inheritdoc/>
	public string GetNick(string qq) => throw new NotImplementedException();

	/// <inheritdoc/>
	public string GetPictureLink(int picType, string group, string picGuid) => throw new NotImplementedException();

	/// <inheritdoc/>
	public string GetBotOnlineInfo() => throw new NotImplementedException();

	/// <inheritdoc/>
	public string GetVoiceLink(string message) => throw new NotImplementedException();

	/// <inheritdoc/>
	public string Like(string qq) => throw new NotImplementedException();

	/// <inheritdoc/>
	public string RecallGroupMessage(string group, string messageNumber, string messageId)
		=> throw new NotImplementedException();
}
