namespace Sudoku.Bot.Annotations;

/// <summary>
/// 表示绑定指令的特性。
/// </summary>
public abstract class CommandBaseAttribute : AnnotationAttribute
{
	/// <summary>
	/// 表示是否当前指令只用于调试和测试使用。
	/// </summary>
	public bool IsDebugging { get; init; }
}
