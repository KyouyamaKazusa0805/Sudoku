namespace Sudoku.Bot.Annotations;

/// <summary>
/// 表示这是一个指令。这个特性用于控制台反射的方式读取。
/// </summary>
/// <param name="commandName">表示指令的名称。</param>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed partial class CommandAttribute([PrimaryConstructorParameter] string commandName) : CommandAnnotationAttribute
{
	/// <summary>
	/// 表示是否当前指令只用于调试和测试使用。
	/// </summary>
	public bool IsDebugging { get; init; }
}
