namespace Sudoku.Bot.Communication;

/// <summary>
/// 事件订阅权限
/// <para>
/// 基础事件(默认有订阅权限)：GUILDS, GUILD_MEMBERS, AT_MESSAGES, GUILD_MESSAGE_REACTIONS<br/>
/// 详见：<see href="https://bot.q.qq.com/wiki/develop/api/gateway/intents.html">官方文档</see>
/// </para>
/// </summary>
/// <completionlist cref="Intents"/>
[Flags]
public enum Intent
{
	/// <summary>
	/// <para>
	/// GUILD_CREATE - 当机器人加入新guild时 <br/>
	/// GUILD_UPDATE - 当guild资料发生变更时 <br/>
	/// GUILD_DELETE - 当机器人退出guild时 <br/>
	/// CHANNEL_CREATE - 当channel被创建时 <br/>
	/// CHANNEL_UPDATE - 当channel被更新时 <br/>
	/// CHANNEL_DELETE - 当channel被删除时
	/// </para>
	/// </summary>
	GUILDS = 1,

	/// <summary>
	/// <para>
	/// GUILD_MEMBER_ADD - 当成员加入时 <br/>
	/// GUILD_MEMBER_UPDATE - 当成员资料变更时 <br/>
	/// GUILD_MEMBER_REMOVE - 当成员被移除时
	/// </para>
	/// </summary>
	GUILD_MEMBERS = 1 << 1,

	/// <summary>
	/// MESSAGE_CREATE - 频道内发送的所有消息的事件（仅私域可用）
	/// </summary>
	MESSAGE_CREATE = 1 << 9,

	/// <summary>
	/// <para>
	/// MESSAGE_REACTION_ADD - 为消息添加表情表态 <br/>
	/// MESSAGE_REACTION_REMOVE - 为消息删除表情表态
	/// </para>
	/// </summary>
	GUILD_MESSAGE_REACTIONS = 1 << 10,

	/// <summary>
	/// DIRECT_MESSAGE_CREATE - 当收到用户发给机器人的私信消息时
	/// </summary>
	DIRECT_MESSAGE_CREATE = 1 << 12,

	/// <summary>
	/// MESSAGE_AUDIT_PASS - 消息审核通过
	/// MESSAGE_AUDIT_REJECT - 消息审核被拒绝
	/// </summary>
	MESSAGE_AUDIT = 1 << 27,

	/// <summary>
	/// <para>
	/// THREAD_CREATE - 当用户创建主题时 <br/>
	/// THREAD_UPDATE - 当用户更新主题时 <br/>
	/// THREAD_DELETE - 当用户删除主题时 <br/>
	/// POST_CREATE - 当用户创建帖子时 <br/>
	/// POST_DELETE - 当用户删除帖子时 <br/>
	/// REPLY_CREATE - 当用户回复评论时 <br/>
	/// REPLY_DELETE - 当用户删除评论时
	/// </para>
	/// </summary>
	FORUM_EVENT = 1 << 28,

	/// <summary>
	/// <para>
	/// AUDIO_START - 音频播放开始时 <br/>
	/// AUDIO_FINISH - 音频播放结束时 <br/>
	/// AUDIO_ON_MIC - 上麦时 <br/>
	/// AUDIO_OFF_MIC - 下麦时
	/// </para>
	/// </summary>
	AUDIO_ACTION = 1 << 29,

	/// <summary>
	/// AT_MESSAGE_CREATE - 当收到@机器人的消息时
	/// </summary>
	AT_MESSAGE_CREATE = 1 << 30,
}
