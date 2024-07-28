namespace Sudoku.Bot;

/// <summary>
/// <see cref="ChatMessage"/> 的扩展方法。
/// </summary>
public static class ChatMessageExtensions
{
	/// <summary>
	/// 获取参数，不含指令名。
	/// </summary>
	public static string[] GetContentArguments(this ChatMessage @this)
		=> @this.Content.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[1..];
}
