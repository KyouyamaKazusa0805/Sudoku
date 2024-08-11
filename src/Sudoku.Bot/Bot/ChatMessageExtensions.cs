namespace Sudoku.Bot;

/// <summary>
/// <see cref="ChatMessage"/> 的扩展方法。
/// </summary>
public static class ChatMessageExtensions
{
	/// <summary>
	/// 获取消息的指令名部分，包含斜杠。
	/// </summary>
	public static string GetCommandFullName(this ChatMessage @this)
	{
		var content = @this.Content.Trim();
		return content[content.IndexOf(' ') is var i and not -1 ? ..i : ..].Trim();
	}

	/// <summary>
	/// 获取参数部分的全部内容，但不拆分，直接以一整个字符串返回。
	/// </summary>
	public static string GetPlainArguments(this ChatMessage @this)
	{
		var content = @this.Content.Trim();
		return content.IndexOf(' ') is var i and not -1 ? content[(i + 1)..].Trim() : string.Empty;
	}

	/// <summary>
	/// 获取参数，不含指令名。
	/// </summary>
	public static string[] GetContentArguments(this ChatMessage @this)
		=> @this.Content.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[1..];
}
