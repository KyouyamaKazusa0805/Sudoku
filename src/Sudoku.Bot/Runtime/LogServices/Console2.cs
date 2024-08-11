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
		Console.WriteLine($@"[{DateTime.Now:yyyy/MM/dd HH\:mm\:ss}] {s}");
		if (severity != LogSeverity.None)
		{
			Console.ResetColor();
		}
	}

	/// <summary>
	/// 阻塞控制台，避免控制台在执行异步函数的时候，主线程继续执行后续内容。
	/// </summary>
	/// <param name="exitCharacters">退出使用的字符。</param>
	[SuppressMessage("Style", "IDE0011:Add braces", Justification = "<Pending>")]
	public static void BlockConsole(params ReadOnlySpan<char> exitCharacters)
	{
		var searchValues = SearchValues.Create(exitCharacters);
		while (!searchValues.Contains(Console.ReadLine()?[0] ?? '\0')) ;
	}
}
