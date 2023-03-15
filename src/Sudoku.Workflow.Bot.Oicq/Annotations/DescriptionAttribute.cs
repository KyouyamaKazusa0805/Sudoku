namespace Sudoku.Workflow.Bot.Oicq.Annotations;

/// <summary>
/// 表示描述信息的注解。用于一个枚举字段，表示该字段对应的描述信息。
/// </summary>
/// <param name="description">字段的描述信息。</param>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class DescriptionAttribute(string description) : EnumFieldAttribute
{
	/// <summary>
	/// 表示该字段的描述信息。
	/// </summary>
	public string Description { get; } = description;
}
