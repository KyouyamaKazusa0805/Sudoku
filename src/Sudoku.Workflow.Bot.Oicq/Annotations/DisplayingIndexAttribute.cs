namespace Sudoku.Workflow.Bot.Oicq.Annotations;

/// <summary>
/// 提供一个特性，表示某指令里的某个参数的显示顺序。
/// </summary>
/// <param name="index">表示该指令的显示位置。</param>
[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public sealed class DisplayingIndexAttribute(int index) : CommandAnnotationAttribute
{
	/// <summary>
	/// 表示该指令的显示位置。
	/// </summary>
	public int Index { get; } = index;
}
