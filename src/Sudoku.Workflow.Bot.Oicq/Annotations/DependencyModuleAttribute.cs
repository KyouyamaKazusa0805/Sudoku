namespace Sudoku.Workflow.Bot.Oicq.Annotations;

/// <summary>
/// 表示一个特性，用于一个命令模块的类型本身上，表示该命令模块执行需要依赖于某个命令。
/// </summary>
/// <typeparam name="T">具体依赖的命令类型。</typeparam>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class DependencyModuleAttribute<T> : CommandAnnotationAttribute where T : Command, new()
{
}
