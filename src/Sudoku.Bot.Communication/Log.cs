namespace Sudoku.Bot.Communication;

/// <summary>
/// 集中处理日志信息
/// </summary>
public static class Log
{
	/// <summary>
	/// 日志输出队列
	/// </summary>
	private static readonly ConcurrentQueue<LogEntry> LogQueue = new();


	/// <summary>
	/// Indicates whether the bot is now working.
	/// </summary>
	private static bool _isWorking = false;


	/// <summary>
	/// 时间格式化器
	/// <para>定义日志输出的时间戳格式 (默认值 HH:mm:ss.fff)</para>
	/// </summary>
	public static string TimeFormatter { get; set; } = "HH:mm:ss.f";

	/// <summary>
	/// 日志记录级别
	/// </summary>
	public static LogLevel LogLevel { get; set; } = LogLevel.Info;

	/// <summary>
	/// 获取格式化的日期标签
	/// </summary>
	private static string TimeStamp => $"[{DateTime.Now.ToString(TimeFormatter)}]";


	/// <summary>
	/// 自定义日志输出
	/// </summary>
	public static event Action<LogEntry>? LogTo;


	/// <summary>
	/// 打印日志
	/// </summary>
	private static void Print(LogEntry logItem)
	{
		_ = Task.Run(handler);


		void handler()
		{
			LogQueue.Enqueue(logItem);
			if (_isWorking)
			{
				return;
			}

			_isWorking = true;
			while (LogQueue.TryDequeue(out var entry))
			{
				if (LogLevel <= entry.Level)
				{
					Console.ForegroundColor = ConsoleColor.White;
					Console.Write($"{entry.TimeStamp}[{entry.Level.ToString()[0]}]");
					Console.ForegroundColor = entry.Level switch
					{
						LogLevel.Debug => ConsoleColor.Gray,
						LogLevel.Info => ConsoleColor.DarkGreen,
						LogLevel.Warning => ConsoleColor.DarkYellow,
						LogLevel.Error => ConsoleColor.DarkRed,
						_ => ConsoleColor.Magenta,
					};

					Console.WriteLine(Unicoder.Decode(entry.Message));
					Console.ResetColor();
					LogTo?.Invoke(entry);
				}
			}

			_isWorking = false;
		}
	}

	/// <summary>
	/// 打印调试
	/// </summary>
	/// <param name="message"></param>
	public static void Debug(string message) => Print(new(LogLevel.Debug, message, TimeStamp));

	/// <summary>
	/// 打印日志
	/// </summary>
	/// <param name="message"></param>
	public static void Info(string message) => Print(new(LogLevel.Info, message, TimeStamp));

	/// <summary>
	/// 打印警告
	/// </summary>
	/// <param name="message"></param>
	public static void Warn(string message) => Print(new(LogLevel.Warning, message, TimeStamp));

	/// <summary>
	/// 打印错误
	/// </summary>
	/// <param name="message"></param>
	public static void Error(string message) => Print(new(LogLevel.Error, message, TimeStamp));
}
