namespace Sudoku.Runtime.LogServices;

/// <summary>
/// <see cref="Console"/> 类型的扩展。
/// </summary>
public static class Console2
{
	/// <summary>
	/// 在控制台上输出一行日志信息。
	/// </summary>
	/// <param name="s">字符串。</param>
	public static void WriteLog(string s) => WriteLog(LogSeverity.None, s);

	/// <summary>
	/// 在控制台上输出一行指定严重程度的日志信息。
	/// </summary>
	/// <param name="severity">严重程度。</param>
	/// <param name="s">字符串。</param>
	public static void WriteLog(LogSeverity severity, string s)
		=> Console.WriteLine($"{severity switch
		{
			LogSeverity.Info => "\e[38;2;19;161;14m",
			LogSeverity.Warning => "\e[38;2;193;156;0m",
			LogSeverity.Error => "\e[38;2;197;15;31m",
			_ => string.Empty
		}}[{DateTime.Now:yyyy/MM/dd HH\\:mm\\:ss}] {s}\e[0m");

	/// <summary>
	/// 阻塞控制台，避免控制台在执行异步函数的时候，主线程继续执行后续内容。
	/// </summary>
	/// <param name="exitCharacters">退出使用的字符。</param>
	public static void BlockConsole(params ReadOnlyCharSequence exitCharacters)
	{
		var searchValues = SearchValues.Create(exitCharacters);
		while (!searchValues.Contains(Console.ReadLine()?[0] ?? '\0')) ;
	}
}
