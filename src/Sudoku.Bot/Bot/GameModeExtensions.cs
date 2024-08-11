namespace Sudoku.Bot;

/// <summary>
/// 给 <see cref="GameMode"/> 字段提供扩展方法。
/// </summary>
/// <seealso cref="GameMode"/>
public static class GameModeExtensions
{
	/// <summary>
	/// 获取指定游戏玩法的名称。
	/// </summary>
	/// <param name="this">当前字段。</param>
	/// <returns>玩法名称。</returns>
	public static string? GetModeName(this GameMode @this)
		=> typeof(GameMode).GetField(@this.ToString())?
			.GetCustomAttribute<GameModeNameAttribute>()?
			.Name;
}
