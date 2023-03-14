namespace Sudoku.Workflow.Bot.Oicq.Annotations;

/// <summary>
/// 表示一个特性，用于一个命令模块的属性上，表示该属性对应的命令参数的提示信息。
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public sealed class HintAttribute : CommandAnnotationAttribute
{
	/// <summary>
	/// 实例化一个 <see cref="HintAttribute"/> 的实例，并给出其提示文字。
	/// </summary>
	/// <param name="hint">表示提示文字。</param>
	public HintAttribute(string hint) => Hint = hint;


	/// <summary>
	/// 表示该命令参数的提示信息。比如用户在输入了“！查询 内容 ？”的时候，提示“内容”参数的详细信息。
	/// </summary>
	public string Hint { get; }
}
