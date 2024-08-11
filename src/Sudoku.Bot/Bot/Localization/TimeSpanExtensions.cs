namespace Sudoku.Bot.Localization;

/// <summary>
/// 提供关于 <see cref="TimeSpan"/> 类型的扩展方法。
/// </summary>
/// <seealso cref="TimeSpan"/>
public static class TimeSpanExtensions
{
	/// <summary>
	/// 将当前 <see cref="TimeSpan"/> 对象转换为中文字符串。
	/// </summary>
	/// <param name="this">当前对象。</param>
	/// <returns>中文字符串。</returns>
	public static string ToChineseTimeSpanString(this TimeSpan @this)
		=> @this.ToString(
			(int)@this.TotalSeconds switch
			{
				< 60 => "s' 秒'",
				var seconds when seconds % 60 == 0 => "m' 分钟'",
				< 3600 => "m' 分 'ss' 秒'",
				var seconds when seconds % 3600 == 0 => "h' 个小时'",
				< 86400 => "h' 小时 'mm' 分 'ss' 秒'",
				var seconds when seconds % 86400 == 0 => "d' 天'",
				_ => "d' 天 'hh' 小时 'mm' 分 'ss' 秒'"
			}
		);
}
