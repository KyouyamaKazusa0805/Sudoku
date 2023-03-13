namespace Sudoku.Workflow.Bot.Oicq.Annotations;

/// <summary>
/// 表示名字的注解。用于一个枚举字段，表示该字段对应的名字（或其他用来显示的名称）的信息。
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class NameAttribute : EnumFieldAttribute
{
	/// <summary>
	/// 实例化一个 <see cref="NameAttribute"/> 类型的实例，并给出其名称。
	/// </summary>
	/// <param name="name">字段的名称。</param>
	public NameAttribute(string name) => Name = name;


	/// <summary>
	/// 表示该字段的名称。
	/// </summary>
	public string Name { get; }
}
