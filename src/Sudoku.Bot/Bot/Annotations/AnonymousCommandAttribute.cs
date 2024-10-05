namespace Sudoku.Bot.Annotations;

/// <summary>
/// 表示这是一个匿名指令。这种指令只在指定的长指令环境下。
/// </summary>
/// <param name="commandName">表示长指令的名称。</param>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed partial class AnonymousCommandAttribute([Property] string commandName) : CommandBaseAttribute;
