namespace Sudoku.Workflow.Bot.Oicq.Annotations;

/// <summary>
/// 提供一个特性，用于一个指令类型上，表示该指令需要用户达到多少级别的时候才可以使用的指令。
/// </summary>
/// <param name="requiredUserLevel">表示用户的级别。</param>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class RequiredUserLevelAttribute(int requiredUserLevel) : CommandAnnotationAttribute
{
	/// <summary>
	/// 表示用户的级别。
	/// </summary>
	public int RequiredUserLevel { get; } = requiredUserLevel;
}
