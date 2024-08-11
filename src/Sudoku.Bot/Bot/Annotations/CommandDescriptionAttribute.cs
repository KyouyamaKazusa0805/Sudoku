namespace Sudoku.Bot.Annotations;

/// <summary>
/// 表示一个指令的描述信息。会用在反射里显示给用户。
/// </summary>
/// <param name="description">描述内容。</param>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed partial class CommandDescriptionAttribute([PrimaryConstructorParameter] string description) : CommandAnnotationAttribute;
