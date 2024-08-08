namespace Sudoku.Bot.Logging;

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
	{
		if (severity != LogSeverity.None)
		{
			Console.ForegroundColor = severity switch
			{
				LogSeverity.Info => ConsoleColor.Green,
				LogSeverity.Warning => ConsoleColor.DarkYellow,
				_ => ConsoleColor.Red
			};
		}
		Console.WriteLine($@"[{DateTime.Now:yyyy/MM/dd hh\:mm\:ss}] {s}");
		if (severity != LogSeverity.None)
		{
			Console.ResetColor();
		}
	}

	/// <summary>
	/// 从控制台读取一行文本。
	/// </summary>
	/// <returns>读入的一行文本字符串。</returns>
	public static string? ConsoleReadLine() => Console.ReadLine();

	/// <summary>
	/// 阻塞控制台，避免控制台在执行异步函数的时候，主线程继续执行后续内容。
	/// </summary>
	/// <param name="exitCharacter">退出使用的字符。</param>
	[SuppressMessage("Style", "IDE0011:Add braces", Justification = "<Pending>")]
	public static void BlockConsole(char exitCharacter)
	{
		while ((ConsoleReadLine()?[0] ?? '\0') != exitCharacter) ;
	}
}
