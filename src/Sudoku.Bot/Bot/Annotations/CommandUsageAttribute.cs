namespace Sudoku.Bot.Annotations;

/// <summary>
/// 表示一个指令的具体用法的特性。
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed partial class CommandUsageAttribute([PrimaryConstructorParameter] string exampleUsage) : AnnotationAttribute
{
	/// <summary>
	/// 表示该陈列的用法是否是语法表现。
	/// </summary>
	public bool IsSyntax { get; init; }
}
