namespace Sudoku.Workflow.Bot.Oicq.Annotations;

/// <summary>
/// 表示物品的详细分组的注解。用于一个枚举字段，表示该字段对应的物品的分组。
/// </summary>
/// <param name="group">表示该字段对应物品的组别。</param>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed partial class ItemGroupAttribute([PrimaryConstructorParameter] ItemGroup group) : EnumFieldAttribute;
