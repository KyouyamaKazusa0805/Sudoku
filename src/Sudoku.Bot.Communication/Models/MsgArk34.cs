namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 模板消息 id=34
/// <para>大图模板</para>
/// </summary>
public class MsgArk34 : MessageToCreate
{
	private readonly MessageArkKeyValuePair _arkDesc = new() { Key = "#DESC#", Value = null };

	private readonly MessageArkKeyValuePair _arkPrompt = new() { Key = "#PROMPT#", Value = null };

	private readonly MessageArkKeyValuePair _arkMetaTitle = new() { Key = "#METATITLE#", Value = null };

	private readonly MessageArkKeyValuePair _arkMetaDesc = new() { Key = "#METADESC#", Value = null };

	private readonly MessageArkKeyValuePair _arkMetaIcon = new() { Key = "#METAICON#", Value = null };

	private readonly MessageArkKeyValuePair _arkMetaPreview = new() { Key = "#METAPREVIEW#", Value = null };

	private readonly MessageArkKeyValuePair _arkMetaUrl = new() { Key = "#METAURL#", Value = null };


	/// <summary>
	/// 构造模板消息
	/// </summary>
	/// <param name="desc">描述</param>
	/// <param name="prompt">提示</param>
	/// <param name="metaTitle">标题</param>
	/// <param name="metaDesc">详情</param>
	/// <param name="metaIcon">小图标URL</param>
	/// <param name="metaPreview">大图URL</param>
	/// <param name="metaUrl">跳转链接</param>
	/// <param name="replyMsgId">要回复的消息id</param>
	public MsgArk34(
		string? desc = null, string? prompt = null, string? metaTitle = null,
		string? metaDesc = null, string? metaIcon = null, string? metaPreview = null,
		string? metaUrl = null, string? replyMsgId = null)
	{
		Id = replyMsgId;
		Desc = desc;
		Prompt = prompt;
		MetaTitle = metaTitle;
		MetaDesc = metaDesc;
		MetaIcon = metaIcon;
		MetaPreview = metaPreview;
		MetaUrl = metaUrl;
		Ark = new()
		{
			TemplateId = 34,
			Kv = new()
			{
				_arkDesc,
				_arkPrompt,
				_arkMetaTitle,
				_arkMetaDesc,
				_arkMetaIcon,
				_arkMetaPreview,
				_arkMetaUrl
			}
		};
	}

	
	/// <summary>
	/// 设置要回复的目标消息
	/// </summary>
	/// <param name="msgId">目标消息的Id</param>
	/// <returns></returns>
	public MsgArk34 SetReplyMsgId(string? msgId) { Id = msgId; return this; }

	/// <summary>
	/// 描述
	/// </summary>
	public string? Desc { get => _arkDesc.Value; set => _arkDesc.Value = value; }

	/// <summary>
	/// 设置描述
	/// </summary>
	/// <param name="desc">描述内容</param>
	/// <returns></returns>
	public MsgArk34 SetDesc(string? desc) { Desc = desc; return this; }

	/// <summary>
	/// 提示
	/// </summary>
	public string? Prompt { get => _arkPrompt.Value; set => _arkPrompt.Value = value; }

	/// <summary>
	/// 设置提示
	/// </summary>
	/// <param name="prompt">提示内容</param>
	/// <returns></returns>
	public MsgArk34 SetPrompt(string? prompt) { Prompt = prompt; return this; }

	/// <summary>
	/// 标题
	/// </summary>
	public string? MetaTitle { get => _arkMetaTitle.Value; set => _arkMetaTitle.Value = value; }

	/// <summary>
	/// 设置标题
	/// </summary>
	/// <param name="metaTitle">标题内容</param>
	/// <returns></returns>
	public MsgArk34 SetMetaTitle(string? metaTitle) { MetaTitle = metaTitle; return this; }

	/// <summary>
	/// 详情
	/// </summary>
	public string? MetaDesc { get => _arkMetaDesc.Value; set => _arkMetaDesc.Value = value; }

	/// <summary>
	/// 设置详情
	/// </summary>
	/// <param name="metaDesc">详情内容</param>
	/// <returns></returns>
	public MsgArk34 SetMetaDesc(string? metaDesc) { MetaDesc = metaDesc; return this; }

	/// <summary>
	/// 小图标URL
	/// </summary>
	public string? MetaIcon { get => _arkMetaIcon.Value; set => _arkMetaIcon.Value = value; }

	/// <summary>
	/// 设置小图标
	/// </summary>
	/// <param name="iconLink">小图标URL</param>
	/// <returns></returns>
	public MsgArk34 SetMetaIcon(string? iconLink) { MetaIcon = iconLink; return this; }

	/// <summary>
	/// 大图URL
	/// </summary>
	public string? MetaPreview { get => _arkMetaPreview.Value; set => _arkMetaPreview.Value = value; }

	/// <summary>
	/// 设置大图
	/// </summary>
	/// <param name="metaPreview">大图URL</param>
	/// <returns></returns>
	public MsgArk34 SetMetaPreview(string? metaPreview) { MetaPreview = metaPreview; return this; }

	/// <summary>
	/// 跳转链接
	/// </summary>
	public string? MetaUrl { get => _arkMetaUrl.Value; set => _arkMetaUrl.Value = value; }

	/// <summary>
	/// 设置跳转链接
	/// </summary>
	/// <param name="metaUrl">跳转链接</param>
	/// <returns></returns>
	public MsgArk34 SetMetaUrl(string? metaUrl) { MetaUrl = metaUrl; return this; }
}
