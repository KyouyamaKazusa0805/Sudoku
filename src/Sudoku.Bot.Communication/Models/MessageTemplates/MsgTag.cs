namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 消息内嵌标签
/// <para>仅作用于content中</para>
/// </summary>
public static class MsgTag
{
	/// <summary>
	/// 创建 @用户 标签
	/// </summary>
	/// <param name="userId">用户id</param>
	/// <returns></returns>
	public static string User(string userId) => $"<@!{userId}>";

	/// <summary>
	/// 创建 #子频道 标签
	/// </summary>
	/// <param name="channelId">子频道id</param>
	/// <returns></returns>
	public static string Channel(string channelId) => $"<#{channelId}>";

	/// <summary>
	/// 创建 @用户 标签
	/// </summary>
	/// <param name="user">用户对象</param>
	/// <returns></returns>
	public static string Tag(this User user) => User(user.Id);

	/// <summary>
	/// 创建 #子频道 标签
	/// </summary>
	/// <param name="channel">子频道对象</param>
	/// <returns></returns>
	public static string Tag(this Channel channel) => Channel(channel.Id);
}
