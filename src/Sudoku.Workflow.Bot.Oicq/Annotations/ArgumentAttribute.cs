namespace Sudoku.Workflow.Bot.Oicq.Annotations;

/// <summary>
/// 提供一个特性，用来表示属性作为命令行解析的参数信息。
/// </summary>
public abstract class ArgumentAttribute : CommandLineAnnotationAttribute
{
	/// <summary>
	/// 表示该指令模块的参数需要多少个子参数信息。
	/// </summary>
	public abstract int RequiredValuesCount { get; }
}
