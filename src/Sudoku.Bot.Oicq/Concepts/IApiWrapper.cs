namespace Sudoku.Bot.Oicq.Concepts;

/// <summary>
/// Defines an API wrapper.
/// </summary>
public interface IApiWrapper : ICloneable
{
	/// <summary>
	/// Initializes the current API wrapper instance. The method will be invoked before the create event.
	/// </summary>
	/// <param name="args">The arguments.</param>
	void Init(params object[] args);

	/// <summary>
	/// Sets the raw data. The method is the backing implementation for large varieties of QQ operations.
	/// </summary>
	/// <param name="data">The raw data.</param>
	void SetData(AmiableEventArgs data);

	/// <summary>
	/// Sends the message to the specified QQ group.
	/// </summary>
	/// <param name="group">The group.</param>
	/// <param name="msg">The message content.</param>
	void SendGroupMessage(string group, string msg);

	/// <summary>
	/// Sends the C2C message to the specified friend.
	/// </summary>
	/// <param name="qq">The QQ number of the target friend.</param>
	/// <param name="msg">The message content.</param>
	void SendC2cMessage(string qq, string msg);

	/// <summary>
	/// Sets the requests for a group.
	/// </summary>
	/// <param name="requestType">The request type.</param>
	/// <param name="qq">The person who raises the request.</param>
	/// <param name="group">The group QQ number.</param>
	/// <param name="seq">The sequence.</param>
	/// <param name="messageType">The message type.</param>
	/// <param name="message">The message.</param>
	void HandleGroupEvent(int requestType, string qq, string group, string seq, int messageType, string message);

	/// <summary>
	/// Output the log.
	/// </summary>
	/// <param name="message">The content of the message you want to log.</param>
	void OutputLog(string message);

	/// <summary>
	/// Indicates whether the specified person is online.
	/// </summary>
	/// <param name="qq">The desired QQ number.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	bool IsOnline(string qq);

	/// <summary>
	/// Applies the sign-in-ing in a group.
	/// </summary>
	/// <param name="group">The group QQ number.</param>
	/// <param name="address">The address.</param>
	/// <param name="message">The message.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	bool SignIn(string group, string address, string message);

	/// <summary>
	/// Gets the directory as the playground of the plugin via a <see cref="string"/> value as the app name.
	/// </summary>
	/// <param name="appName">The app name.</param>
	/// <returns>The directory as the playground of the plugin.</returns>
	string GetAppDirectory(string appName);

	/// <summary>
	/// Gets the owner of the specified QQ group.
	/// </summary>
	/// <param name="group">The group number as string representation.</param>
	/// <returns>The QQ number of the owner.</returns>
	string GetGroupAdmin(string group);

	/// <summary>
	/// Gets the nickname of the specified person.
	/// </summary>
	/// <param name="qq">The QQ number of the person you want to get.</param>
	/// <returns>The nickname of the person.</returns>
	string GetNick(string qq);

	/// <summary>
	/// Gets the remarked name of the specified person.
	/// </summary>
	/// <param name="qq">The QQ number of the person you want to get.</param>
	/// <returns>The remarked name of the person.</returns>
	string GetFriendsRemark(string qq);

	/// <summary>
	/// Gets the name of the specified QQ group.
	/// </summary>
	/// <param name="group">The group QQ as the string representation.</param>
	/// <returns>The group name.</returns>
	string GetGroupName(string group);

	/// <summary>
	/// Gets the online info for a bot.
	/// </summary>
	/// <returns>The info.</returns>
	string GetBotOnlineInfo();

	/// <summary>
	/// Gets the link of a picture via the <see cref="Guid"/> of the picture.
	/// </summary>
	/// <param name="pictureType">Indicates the picture type.</param>
	/// <param name="group">The group QQ number.</param>
	/// <param name="pictureGuid">The <see cref="Guid"/> value as string representation of the picture.</param>
	/// <returns>The link of the picture.</returns>
	string GetPictureLink(int pictureType, string group, string pictureGuid);

	/// <summary>
	/// Gets the link of a voice.
	/// </summary>
	/// <param name="message">The message for a voice.</param>
	/// <returns>The voice link.</returns>
	string GetVoiceLink(string message);

	/// <summary>
	/// Recalls a message item in the specified group.
	/// </summary>
	/// <param name="group">The group QQ number.</param>
	/// <param name="messageNumber">The message number.</param>
	/// <param name="messageId">The message ID.</param>
	/// <returns>The return info.</returns>
	string RecallGroupMessage(string group, string messageNumber, string messageId);

	/// <summary>
	/// Likes (i.e. "Dian Zan" in Chinese) the QQ.
	/// </summary>
	/// <param name="qq">The QQ number you want to like.</param>
	/// <returns>The return info.</returns>
	string Like(string qq);
}
