namespace Sudoku.Workflow.Bot.Oicq.Annotations;

/// <summary>
/// 提供一个特性，标记到指令的类型上，表示该类型的指令使用情况。
/// </summary>
/// <param name="usageText">表示指令的说明字符串。</param>
/// <param name="description">表示指令的解释，为 <paramref name="usageText"/> 的解释文字。</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class UsageAttribute(string usageText, string description) : CommandAnnotationAttribute
{
	/// <summary>
	/// 表示使用指令的字符串。
	/// </summary>
	public string UsageText { get; } = usageText;

	/// <summary>
	/// 表示说明文字，为 <see cref="UsageText"/> 的解释文字。
	/// </summary>
	public string Description { get; } = description;
}
