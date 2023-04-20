namespace Sudoku.Workflow.Bot.Oicq.Annotations;

/// <summary>
/// 提供一个特性，用于标注在指令的参数属性上，表示该参数在显示到帮助文字的地方上的时候，参数的显示部分。
/// 默认情况下，参数显示的名称和参数本身的名字一样，比如“<![CDATA[！查询 QQ <QQ>]]>”。
/// </summary>
/// <param name="argumentDisplayer">表示显示的参数内容。</param>
[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public sealed partial class ArgumentDisplayerAttribute([PrimaryConstructorParameter] string argumentDisplayer) : CommandAnnotationAttribute;
