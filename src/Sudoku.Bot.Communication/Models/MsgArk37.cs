namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 模板消息 id=37
/// <para>
/// 大图模板<br/>
/// <em>尺寸为 975*540</em>
/// </para>
/// </summary>
public class MsgArk37 : MessageToCreate
{
	private readonly MessageArkKv _arkPrompt = new() { Key = "#PROMPT#", Value = null };

	private readonly MessageArkKv _arkMetaTitle = new() { Key = "#METATITLE#", Value = null };

	private readonly MessageArkKv _arkMetaSubTitle = new() { Key = "#METASUBTITLE#", Value = null };

	private readonly MessageArkKv _arkMetaCover = new() { Key = "#METACOVER#", Value = null };

	private readonly MessageArkKv _arkMetaUrl = new() { Key = "#METAURL#", Value = null };


	/// <summary>
	/// 构造模板消息
	/// </summary>
	/// <param name="prompt">描述</param>
	/// <param name="metaTitle">标题</param>
	/// <param name="metaSubTitle">子标题</param>
	/// <param name="metaCover">大图URL</param>
	/// <param name="metaUrl">跳转链接</param>
	/// <param name="replyMsgId">要回复的消息id</param>
	public MsgArk37(
		string? prompt = null, string? metaTitle = null, string? metaSubTitle = null,
		string? metaCover = null, string? metaUrl = null, string? replyMsgId = null)
	{
		Id = replyMsgId;
		Prompt = prompt;
		MetaTitle = metaTitle;
		MetaSubTitle = metaSubTitle;
		MetaCover = metaCover;
		MetaUrl = metaUrl;
		Ark = new()
		{
			TemplateId = 37,
			Kv = new()
			{
				_arkPrompt,
				_arkMetaTitle,
				_arkMetaSubTitle,
				_arkMetaCover,
				_arkMetaUrl
			}
		};
	}


	/// <summary>
	/// 设置要回复的目标消息
	/// </summary>
	/// <param name="msgId">目标消息的Id</param>
	/// <returns></returns>
	public MsgArk37 SetReplyMsgId(string? msgId) { Id = msgId; return this; }

	/// <summary>
	/// 提示
	/// </summary>
	public string? Prompt { get => _arkPrompt.Value; set => _arkPrompt.Value = value; }

	/// <summary>
	/// 设置提示
	/// </summary>
	/// <param name="prompt">提示内容</param>
	/// <returns></returns>
	public MsgArk37 SetPrompt(string? prompt) { Prompt = prompt; return this; }

	/// <summary>
	/// 标题
	/// </summary>
	public string? MetaTitle { get => _arkMetaTitle.Value; set => _arkMetaTitle.Value = value; }

	/// <summary>
	/// 设置标题
	/// </summary>
	/// <param name="metaTitle">标题内容</param>
	/// <returns></returns>
	public MsgArk37 SetMetaTitle(string? metaTitle) { MetaTitle = metaTitle; return this; }

	/// <summary>
	/// 子标题
	/// </summary>
	public string? MetaSubTitle { get => _arkMetaSubTitle.Value; set => _arkMetaSubTitle.Value = value; }

	/// <summary>
	/// 设置子标题
	/// </summary>
	/// <param name="subTitle">子标题内容</param>
	/// <returns></returns>
	public MsgArk37 SetMetaSubTitle(string? subTitle) { MetaSubTitle = subTitle; return this; }

	/// <summary>
	/// 大图URL
	/// </summary>
	public string? MetaCover { get => _arkMetaCover.Value; set => _arkMetaCover.Value = value; }

	/// <summary>
	/// 设置大图
	/// </summary>
	/// <param name="cover">大图URL</param>
	/// <returns></returns>
	public MsgArk37 SetMetaCover(string? cover) { MetaCover = cover; return this; }

	/// <summary>
	/// 跳转链接
	/// </summary>
	public string? MetaUrl { get => _arkMetaUrl.Value; set => _arkMetaUrl.Value = value; }

	/// <summary>
	/// 设置跳转链接
	/// </summary>
	/// <param name="metaUrl">跳转链接</param>
	/// <returns></returns>
	public MsgArk37 SetMetaUrl(string? metaUrl) { MetaUrl = metaUrl; return this; }
}
