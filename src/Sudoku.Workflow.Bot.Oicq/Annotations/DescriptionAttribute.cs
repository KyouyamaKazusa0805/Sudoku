namespace Sudoku.Workflow.Bot.Oicq.Annotations;

/// <summary>
/// 表示描述信息的注解。用于一个枚举字段，表示该字段对应的描述信息。
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class DescriptionAttribute : EnumFieldAttribute
{
	/// <summary>
	/// 实例化一个 <see cref="DescriptionAttribute"/> 类型的实例，并给出其描述信息。
	/// </summary>
	/// <param name="description">字段的描述信息。</param>
	public DescriptionAttribute(string description) => Description = description;


	/// <summary>
	/// 表示该字段的描述信息。
	/// </summary>
	public string Description { get; }
}
