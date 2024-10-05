namespace Sudoku.Bot.Annotations;

/// <summary>
/// 表示这是一个指令。这个特性用于控制台反射的方式读取。
/// </summary>
/// <param name="commandName">表示指令的名称。</param>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed partial class CommandAttribute([Property] string commandName) : CommandBaseAttribute;
