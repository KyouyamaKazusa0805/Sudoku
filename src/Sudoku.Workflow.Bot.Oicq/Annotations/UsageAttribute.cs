namespace Sudoku.Workflow.Bot.Oicq.Annotations;

/// <summary>
/// 提供一个特性，标记到指令的类型上，表示该类型的指令使用情况。
/// </summary>
/// <param name="usageText">表示指令的说明字符串。</param>
/// <param name="description">表示指令的解释，为 <paramref name="usageText"/> 的解释文字。</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed partial class UsageAttribute(
	[PrimaryConstructorParameter] string usageText,
	[PrimaryConstructorParameter] string description
) : CommandAnnotationAttribute;
