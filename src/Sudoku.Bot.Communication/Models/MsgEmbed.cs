namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Embed消息对象
/// </summary>
public class MsgEmbed : MessageToCreate
{
	/// <summary>
	/// 构建Embed消息
	/// <para>详情查阅QQ机器人文档 <see href="https://bot.q.qq.com/wiki/develop/api/openapi/message/template/embed_message.html">embed消息</see></para>
	/// </summary>
	/// <param name="title">标题</param>
	/// <param name="prompt">提示</param>
	/// <param name="thumbnail">缩略图URL</param>
	/// <param name="embedFields">内容列表</param>
	/// <param name="replyMsgId">要回复的消息id</param>
	public MsgEmbed(
		string? title = null, string? prompt = null, string? thumbnail = null,
		List<MessageEmbedField>? embedFields = null, string? replyMsgId = null)
	{
		Id = replyMsgId;
		EmbedFields = embedFields ?? new();
		EmbedClass = new MessageEmbed { Thumbnail = EmbedThumbnail, Fields = EmbedFields };
		Title = title;
		Prompt = prompt;
		Thumbnail = thumbnail;
		Embed = EmbedClass;
	}


	/// <summary>
	/// 设置要回复的目标消息
	/// </summary>
	/// <param name="msgId">目标消息的Id</param>
	/// <returns></returns>
	public MsgEmbed SetReplyMsgId(string? msgId) { Id = msgId; return this; }

	/// <summary>
	/// 标题
	/// </summary>
	public string? Title { get => EmbedClass.Title; set => EmbedClass.Title = value; }

	/// <summary>
	/// 设置标题
	/// </summary>
	/// <param name="title">标题内容</param>
	/// <returns></returns>
	public MsgEmbed SetTitle(string? title) { Title = title; return this; }

	/// <summary>
	/// 提示
	/// </summary>
	public string? Prompt { get => EmbedClass.Prompt; set => EmbedClass.Prompt = value; }

	/// <summary>
	/// 设置提示
	/// </summary>
	/// <param name="prompt">提示内容</param>
	/// <returns></returns>
	public MsgEmbed SetPrompt(string? prompt) { Prompt = prompt; return this; }

	/// <summary>
	/// 缩略图URL
	/// </summary>
	public string? Thumbnail { get => EmbedThumbnail.Url; set => EmbedThumbnail.Url = value; }

	/// <summary>
	/// 设置缩略图
	/// </summary>
	/// <param name="thumbnail">缩略图URL</param>
	/// <returns></returns>
	public MsgEmbed SetThumbnail(string? thumbnail) { Thumbnail = thumbnail; return this; }

	/// <summary>
	/// 消息列表
	/// </summary>
	public List<MessageEmbedField> EmbedFields { get; set; }

	/// <summary>
	/// 添加一行内容
	/// </summary>
	/// <param name="content">行内容</param>
	/// <returns></returns>
	public MsgEmbed AddLine(string? content) { EmbedFields.Add(new() { Name = content }); return this; }

	private MessageEmbedThumbnail EmbedThumbnail { get; set; } = new();

	private MessageEmbed EmbedClass { get; set; }
}
