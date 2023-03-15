namespace Sudoku.Workflow.Bot.Oicq.Annotations;

/// <summary>
/// 提供一个特性，用于标记到指令模块的某个参数上，表示该参数是一个双参数指令。
/// </summary>
/// <param name="name"><inheritdoc cref="ArgumentAttribute(string, int)" path="/param[@name='name']"/></param>
/// <remarks>
/// <b>双参数指令</b>表示一个参数，需要带一个额外参数，拼凑在一起，才是一个整体的参数信息。
/// </remarks>
[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public sealed class DoubleArgumentAttribute(string name) : ArgumentAttribute(name, 1);
