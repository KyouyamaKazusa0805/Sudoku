namespace Sudoku.Workflow.Bot.Oicq.Annotations;

/// <summary>
/// 表示名字的注解。用于一个枚举字段，表示该字段对应的名字（或其他用来显示的名称）的信息。
/// </summary>
/// <param name="name">字段的名称。</param>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed partial class NameAttribute([PrimaryConstructorParameter] string name) : EnumFieldAttribute;
