namespace Sudoku.Workflow.Bot.Oicq.ComponentModel;

/// <summary>
/// 为 <see cref="GameMode"/> 实例提供扩展方法。
/// </summary>
/// <seealso cref="GameMode"/>
public static class GameModeExtensions
{
	/// <summary>
	/// 获取指定游戏模式的名称。
	/// </summary>
	public static string GetName(this GameMode @this)
		=> @this.GetType().GetField(@this.ToString())!.GetCustomAttribute<NameAttribute>()!.Name;
}
